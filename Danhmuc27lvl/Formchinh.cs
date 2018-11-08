using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using Tulpep.NotificationWindow;
using System.Globalization;
using AnhLuu = Danhmuc27lvl.Properties.Resources;
using Outlook = Microsoft.Office.Interop.Outlook;
using System.Runtime.InteropServices;


namespace Danhmuc27lvl
{
    public partial class Formchinh : Form
    {
        [DllImport("user32.dll")]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);


        [DllImport("USER32.DLL")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("User32.dll")]
        public static extern bool ShowWindow(IntPtr handle, int nCmdShow);


        Thread luongmail;
        Thread newmail;
        Thread xulyanh;
        Thread chenmahang;
        Thread chaynen;
        Thread tudongloadanh;
        Thread luongnenmail;
        Thread filemoi;

        System.Media.SoundPlayer phatduocban = new System.Media.SoundPlayer(Properties.Resources.duocban);
        System.Media.SoundPlayer phatchuaduocban = new System.Media.SoundPlayer(Properties.Resources.chuaban);
        private static bool phatAM = true;
        static bool cofiledmmoi = false;

        public void phatAMTHANH_BAN()
        {
            if (phatAM)
            {
                phatduocban.Play();
            }
        }
        public void phatAMTHANH_KOBAN()
        {
            if (phatAM)
            {
                phatchuaduocban.Play();
            }
        }
        string duongdanfilemoi = Application.StartupPath + @"\filedanhmuc\";
        string duongdanchuaanh = Application.StartupPath + @"\luuanh\";
        static string ngaychonbandau = null;
        static int sodongchon = 0;
        static string thoigiancapnhat = null;
        static bool phathaykhongphat = true;
        List<string> thongtinmailmoi= new List<string>();
        List<string> filedmmoi = new List<string>();

        private ManualResetEvent dieukhienthread = new ManualResetEvent(true);

        public Formchinh()
        {
            InitializeComponent();

            loadkhikhoidong();

            chaynen = new Thread(luongchaynen);
            chaynen.IsBackground = true;
            chaynen.Start();

            tudongloadanh = new Thread(hamtudongloadanh);
            tudongloadanh.IsBackground = true;
            tudongloadanh.Start();


            luongnenmail = new Thread(luongchaynenbaomail);
            luongnenmail.IsBackground = true;
            luongnenmail.Start();

            filemoi = new Thread(hamgoifiledanhmucmoi);
            filemoi.IsBackground = true;
            filemoi.Start();
            
        }
        public async void loadkhikhoidong()
        {
            string h = await xulyFireBase.layngayGannhat();
            xulyFireBase.taobang(h, datag1);
            lbtongma.Text = datag1.Rows.Count.ToString();
           
        }
        void luongchaynen()
        {
            while (true)
            {
                Thread.Sleep(1000);
                newmail = new Thread(hamloadmailmoi);
                newmail.IsBackground = true;
                newmail.Start();


                luongmail = new Thread(hamcapnhat);
                luongmail.IsBackground = true;
                luongmail.Start();

                xulyanh = new Thread(hamxulyanh);
                xulyanh.IsBackground = true;
                xulyanh.Start();

                chenmahang = new Thread(chenma);
                chenmahang.IsBackground = true;
                chenmahang.Start();

                chenmahang.Join();


                Thread.Sleep(300000);
            }
        }
        void hamgoifiledanhmucmoi()
        {
            while (true)
            {
                Thread.Sleep(1000);
                if (filedmmoi != null)
                {
                    pbfiledanhmucmoi.Invoke(new MethodInvoker(delegate ()
                    {
                        pbfiledanhmucmoi.Image = Properties.Resources.deadpool;
                    }));
                    foreach (string item in filedmmoi)
                    {
                        lbfiledanhmucmoi.Invoke(new MethodInvoker(delegate ()
                        {
                            lbfiledanhmucmoi.Text = "File mới:--> "+item;
                        }));
                        Thread.Sleep(2000);
                    }
                }
               
            }
        }
        void luongchaynenbaomail()
        {
            while (true)
            {
                Thread.Sleep(1500);
                if(thongtinmailmoi != null)
                {
                    foreach (string tt in thongtinmailmoi)
                    {
                        lbtrangthai.Invoke(new MethodInvoker(delegate ()
                       {
                           lbtrangthai.Text = tt;
                       }));
                        Thread.Sleep(3000);
                    }
                }
                
            }
            
        }
        
        void hamloadmailmoi() // ham nay chay dau tien 1
        {
            try
            {
                var xulymail = layfileoutlook.Instance();
                thongtinmailmoi = xulymail.loadmailmoi();
                
                pbmail.Invoke(new MethodInvoker(delegate ()
                {
                    if (thongtinmailmoi != null)
                    {
                        pbmail.Image = Properties.Resources.newmail;
                    }
                    else
                    {
                        pbmail.Image = Properties.Resources.mail;
                    }
                }));
                lbbaomail.Invoke(new MethodInvoker(delegate ()
                {
                    if (thongtinmailmoi != null)
                    {
                        lbbaomail.Text = "Có Mail mới";
                    }
                    else
                    {
                        lbbaomail.Text = "Thông báo:";
                    }
                }));

                lbthongbaocapnhat.Invoke(new MethodInvoker(delegate ()
                {
                    lbthongbaocapnhat.Text = "Đang cập nhật";// cho load tung file save trong mail
                    lbthongbaocapnhat.ForeColor = System.Drawing.Color.Crimson;
                }));
                pbtrangthaicapnhat.Invoke(new MethodInvoker(delegate ()
                {
                    pbtrangthaicapnhat.Image = Properties.Resources.loading;
                }));
                this.Invoke(new Action(delegate ()
                {
                    IntPtr hWnd = FindWindow(null, "Internet Security Warning"); // Window Titel
                    if (hWnd != IntPtr.Zero)
                    {
                        ShowWindow(hWnd, 9);
                        //The bring the application to focus
                        SetForegroundWindow(hWnd);
                        SendKeys.Send("{TAB}");
                        SendKeys.Send("{ENTER}");
                    }

                }));
            }
            catch (Exception ex)
            {
                ghiloi.WriteLogError(ex);
                return;
            }
            
            
            
        }
        void hamcapnhat() // chay thu 2m*zv,
        {
            newmail.Join(); // ham cap nhat se doi ham newmail xu ly xong moi chay
            try
            {
                var xulyoutlook = layfileoutlook.Instance();
                var ham = hamtao.Khoitao();
                filedmmoi = xulyoutlook.luufilemoi();

                ham.luudanhmuchangmoi();
                this.Invoke(new Action(delegate ()
                {
                    IntPtr hWnd = FindWindow(null, "Internet Security Warning"); // Window Titel
                    if (hWnd != IntPtr.Zero)
                    {
                        ShowWindow(hWnd, 9);
                        //The bring the application to focus
                        SetForegroundWindow(hWnd);
                        SendKeys.Send("{TAB}");
                        SendKeys.Send("{ENTER}");
                    }

                }));
                if (filedmmoi != null)
                {
                    foreach (string item in filedmmoi)
                    {
                        xulyFireBase.chenFilemoi(filedmmoi[0]);
                    }
                }

            }
                catch (Exception)
                {

                    throw;
                }



        }
        void hamxulyanh() // chay thu 3
        {
            luongmail.Join(); // ham xulyanh se doi cho thread luonggmail chay xong moi bat day chay
            var ham = hamtao.Khoitao();
            //ham.xulyanh();
            if (ham.xulyanh())
            {
                cofiledmmoi = true;
            }
            else cofiledmmoi = false;
        }

        public async void chenma() // chay thu 4
        {
            xulyanh.Join(); //ham chenma(thread chenmahang) se doi cho ham xulyanh chay xong moi chay
            var ham = hamtao.Khoitao();
            ham.xulymahang();
            thoigiancapnhat = DateTime.Now.ToString("HH:mm:ss");
            lbthongbaocapnhat.Invoke(new MethodInvoker(delegate ()
            {
                lbthongbaocapnhat.Text = "Đã cập nhật xong -" + " Lúc: " + thoigiancapnhat;
                lbthongbaocapnhat.ForeColor = System.Drawing.Color.Navy;
            }));
            pbtrangthaicapnhat.Invoke(new MethodInvoker(delegate ()
            {
                pbtrangthaicapnhat.Image = Properties.Resources.ok;
            }));
            ngaychonbandau = await xulyFireBase.layngayGannhat();
            datag1.Invoke(new MethodInvoker(delegate ()
            {
                xulyFireBase.taobang(ngaychonbandau,datag1);
                lbtongma.Invoke(new MethodInvoker(delegate ()
                {
                    lbtongma.Text = datag1.Rows.Count.ToString();
                }));
            }));
            if (cofiledmmoi)
            {
                this.Invoke(new Action(delegate ()
                {
                    NotificationHts("Đã cập nhật xong - " + " Lúc: " + thoigiancapnhat);
                }));
            }
            
            this.Invoke(new Action(delegate ()
            {
                IntPtr hWnd = FindWindow(null, "Internet Security Warning"); // Window Titel
                if (hWnd != IntPtr.Zero)
                {
                    ShowWindow(hWnd, 9);
                    //The bring the application to focus
                    SetForegroundWindow(hWnd);
                    SendKeys.Send("{TAB}");
                    SendKeys.Send("{ENTER}");
                }

            }));
        }
        void hamtudongloadanh()
        {
            while (true)
            {
                Thread.Sleep(2000);
                string[] tonghopanh = Directory.GetFiles(Application.StartupPath + @"\luuanh\");
                for (int i = 0; i < tonghopanh.Length; i++)
                {
                    pbanhsanpham.Invoke(new MethodInvoker(delegate ()
                    {
                        pbanhsanpham.ImageLocation = tonghopanh[i];
                    }));
                    lbmahang.Invoke(new MethodInvoker(delegate ()
                    {
                        lbmahang.Text = Path.GetFileNameWithoutExtension(tonghopanh[i]);
                    }));

                    Thread.Sleep(2000);

                    dieukhienthread.WaitOne(Timeout.Infinite);
                }
            }
            
        }
        /// <summary>
        /// 
        /// cac ham phuc vu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Formchinh_Load(object sender, EventArgs e)
        {
            
        }
        void laythongtinvaolabel(string mahang)
        {
            if (mahang == null)
            {
                lbmahang.Text= "Mã hàng";
                lbmotasanpham.Text = "Mô tả sản phẩm";
                lbngayban.Text = "Ngày bán";
                lbduocbanhaychua.Text = "Chưa được bán";
                lbdatrunghaychua.Text = "";
                phatchuaduocban.Play();
                return;
            }
            var conlite = ketnoi.Instance();
            var ham = hamtao.Khoitao();
            List<laythongtin> laytt = new List<laythongtin>();
            laytt = conlite.loclaythongtin1ma(mahang);
            string kiemtra = conlite.Kiemtra("matong", "hangduocban", mahang);
            if (kiemtra != null)
            {
                foreach (laythongtin tt in laytt)
                {
                    lbmotasanpham.Text = tt.Motamaban + " - " + tt.Chudemaban + " - " + tt.Ghichu;
                    lbngayban.Text = tt.Ngayduocban;
                    DateTime dt1 = DateTime.ParseExact(tt.Ngayduocban, "dd/MM/yyyy", null);
                    if (dt1 <= DateTime.Now)
                    {
                        lbduocbanhaychua.Text = "Được bán";
                        phatAMTHANH_BAN();
                    }
                    else
                    {
                        lbdatrunghaychua.Text = "Chưa được bán";
                        phatAMTHANH_KOBAN();
                    }
                    string trunghang = conlite.laythongtintrunghang(mahang);
                    if (trunghang == null)
                    {
                        lbdatrunghaychua.Text = "Chưa trưng bán";
                    }
                    else
                    {
                        lbdatrunghaychua.Text = trunghang;
                    }
                    loadanh(mahang);
                }
            }
            else if (kiemtra == null)
            {
                pbThemvaoduocban.Enabled = true;
                lbdatrunghaychua.Text = "Chưa trưng bán";
                lbduocbanhaychua.Text = "Chưa được bán";
                phatAMTHANH_KOBAN();
                loadanh(mahang);
            }

        }
        void loadanh(string tenanh)
        {
            if (File.Exists(duongdanchuaanh + tenanh + ".png"))
            {
                dungphatanh();
                pbanhsanpham.ImageLocation = duongdanchuaanh + tenanh + ".png";
                lbmahang.Text = tenanh;
                
            }
            else
            {
                pbanhsanpham.Image = Properties.Resources.bombs;
                lbmahang.Text = "Mã hàng";
            }
        }
        
        void updatetrunghangthanhdatrung()
        {
            try
            {

                var con = ketnoi.Instance();

                if (datag1.SelectedRows.Count > 0)
                {
                    string matong = null;
                    foreach (DataGridViewRow row in datag1.SelectedRows)
                    {
                        matong = row.Cells[0].Value.ToString();
                        con.updatedatrunghangthanhdatrung(matong);
                    }
                    sodongchon = datag1.SelectedRows.Count;
                    if (ngaychonbandau == null)
                    {
                        ngaychonbandau = con.layngayganhat();
                    }
                    if (nuthts_trung.Checked)
                    {
                        datag1.DataSource = con.laydanhsachCHUATRUNG();
                    }
                    else datag1.DataSource = con.laythongtinkhichonngay("ngaydangso",ngaychonbandau);
                    updatesoluongtrenbang();
                    NotificationHts("Vừa cập nhật : " + sodongchon.ToString() + " mã hàng");
                }
            }
            catch (Exception ex)
            {
                NotificationHts("Có vấn đề");
                lbtrangthai.Text = ex.ToString();
            }


        }
        void updatetrunghangthanhchuatrung()
        {
            try
            {
                var con = ketnoi.Instance();
                if (datag1.SelectedRows.Count > 0)
                {
                    string matong = null;
                    foreach (DataGridViewRow row in datag1.SelectedRows)
                    {
                        matong = row.Cells[0].Value.ToString();
                        con.updatetrunghangthanhchuatrung(matong);

                    }
                    sodongchon = datag1.SelectedRows.Count;
                    NotificationHts("Vừa cập nhật : " + sodongchon.ToString() + " mã hàng");
                    if (ngaychonbandau == null)
                    {
                        ngaychonbandau = con.layngayganhat();
                    }
                    if (nuthts_trung.Checked)
                    {
                        datag1.DataSource = con.laydanhsachCHUATRUNG();
                    }
                    else datag1.DataSource = con.laythongtinkhichonngay("ngaydangso", ngaychonbandau);
                    updatesoluongtrenbang();
                }
            }
            catch (Exception ex)
            {
                NotificationHts("Có vấn đề");
                lbtrangthai.Text = ex.ToString();
            }


        }
        void NotificationHts(string noidung)
        {
            PopupNotifier pop = new PopupNotifier();
            pop.TitleText = "Thông báo - hts";
            pop.ContentText = "\" "+noidung + " \"";
            pop.Image = Properties.Resources.totoro1;
            pop.IsRightToLeft = false;
            pop.TitleColor = System.Drawing.Color.Lime;
            pop.TitleFont = new System.Drawing.Font("Comic Sans MS", 10, System.Drawing.FontStyle.Underline);
            pop.BodyColor = System.Drawing.Color.DimGray;
            pop.Size = new System.Drawing.Size(380, 130);
            pop.ImageSize = new System.Drawing.Size(100, 100);
            pop.ImagePadding = new Padding(15);
            pop.ContentColor = System.Drawing.Color.White;
            pop.ContentFont = new System.Drawing.Font("Comic Sans MS", 12, System.Drawing.FontStyle.Bold);
            pop.Delay = 3500;
            pop.BorderColor = System.Drawing.Color.DimGray;
            pop.HeaderHeight = 1;
            pop.Popup();
        }
      
        void updatesoluongtrenbang()
        {
            lbtongma.Text = datag1.Rows.Count.ToString();
        }
        #region Thao tac xu kien
        private void txtbarcode_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (!string.IsNullOrEmpty(txtbarcode.Text))
                    {
                        var consql = ketnoi.Instance();
                        string masp = consql.laymasp(txtbarcode.Text);
                        laythongtinvaolabel(masp);

                        //lbmahang.Text = masp;
                        loadanh(masp);
                        txtbarcode.Clear();
                        txtbarcode.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                txtbarcode.Clear();
                txtbarcode.Focus();
                lbmahang.Text = "Mã hàng";
                lbmotasanpham.Text = "Mô tả sản phẩm";
                lbngayban.Text = "Ngày bán";
                lbduocbanhaychua.Text = "Chưa được bán";
                lbdatrunghaychua.Text = "";
                //phatchuaduocban.Play();
                lbtrangthai.Text=ex.ToString();
                NotificationHts("Có vấn đề Barcode rồi \nXem lại.");
            }
           
        }

        private void txtmatong_TextChanged(object sender, EventArgs e)
        {
            try
            {
                var consqlite = ketnoi.Instance();
                datag1.DataSource = consqlite.loctheotenmatong(txtmatong.Text);
                string mau = @"\d{1}\w{2}\d{2}[SWAC]\d{3}";
                if (Regex.IsMatch(txtmatong.Text, mau))
                {
                    laythongtinvaolabel(txtmatong.Text);
                }
                updatesoluongtrenbang();
            }
            catch (Exception ex)
            {

                lbtrangthai.Text = ex.ToString();
            }
        }
        private void pbxoamatong_Click(object sender, EventArgs e)
        {
            txtmatong.Clear();
            txtmatong.Focus();
            updatesoluongtrenbang();
        }
        private void monthCalendar1_DateSelected(object sender, DateRangeEventArgs e)
        {
            try
            {
                var month = sender as MonthCalendar;
                DateTime ngaychon = month.SelectionStart;
                ngaychonbandau = month.SelectionStart.ToString("yyyyMMdd", CultureInfo.InvariantCulture);
                Console.WriteLine(ngaychonbandau);
                xulyFireBase.taobang(ngaychonbandau, datag1);
                updatesoluongtrenbang();
                dateTimePicker1.Value = ngaychon;
                dateTimePicker2.Value = ngaychon;
            }
            catch (Exception ex)
            {

                lbtrangthai.Text = ex.ToString();
            }
            
        }

        private void pbThemvaoduocban_Click(object sender, EventArgs e)
        {
            try
            {
                var con = ketnoi.Instance();
                if (con.Kiemtra("matong", "hangduocban", lbmahang.Text) == null && lbmahang.Text != "Mã hàng")
                {
                    DialogResult hoi = MessageBox.Show("Thêm mã " + lbmahang.Text + " vào danh sách được bán", "Thông báo", MessageBoxButtons.YesNo);
                    if (hoi == DialogResult.Yes)
                    {
                        // update ma hang vao danh sach duoc ban
                        con.themmamoivaodanhsachduocban(lbmahang.Text);
                        NotificationHts("Vừa thêm mã " + lbmahang.Text + " vào danh sách được bán");
                    }
                }
                else { MessageBox.Show("Mã đấy đã có trong danh sách được bán"); }
            }
            catch (Exception ex)
            {

                lbtrangthai.Text = ex.ToString();
            }
            
        }

        private void btndatrunghang_Click(object sender, EventArgs e)
        {
            try
            {
                updatetrunghangthanhdatrung();
            }
            catch (Exception ex)
            {

                lbtrangthai.Text = ex.ToString();
            }
            
        }

        private void btnchuatrunghang_Click(object sender, EventArgs e)
        {
            try
            {

                updatetrunghangthanhchuatrung();
            }
            catch (Exception ex)
            {

                lbtrangthai.Text = ex.ToString();
            }
        }
        private void datag1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                DataGridViewRow row = datag1.Rows[e.RowIndex];
                lbmahang.Text = row.Cells[0].Value.ToString();
                lbmotasanpham.Text = row.Cells[1].Value.ToString() + " - " + row.Cells[2].Value.ToString() + " - " + row.Cells[3].Value.ToString();
                lbngayban.Text = row.Cells[4].Value.ToString();
                lbdatrunghaychua.Text = row.Cells[5].Value.ToString();
                if (lbdatrunghaychua.Text == null || lbdatrunghaychua.Text=="")
                {
                    lbdatrunghaychua.Text = "Chưa trưng bán";
                }
                
                DateTime dt1 = DateTime.ParseExact(lbngayban.Text, "dd/MM/yyyy", null);
                if (dt1 <= DateTime.Now)
                {
                    lbduocbanhaychua.Text = "Được bán";
                    phatAMTHANH_BAN();
                }
                else
                {
                    lbduocbanhaychua.Text = "Chưa được bán";
                    phatAMTHANH_KOBAN();
                }
                loadanh(lbmahang.Text);
            }
            catch (Exception ex)
            {

                lbtrangthai.Text = ex.ToString();
            }
            
        }
        private void btnXuatIn_Click(object sender, EventArgs e)
        {
            try
            {
                string ngaybatdau = dateTimePicker1.Value.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                string ngayketthuc = dateTimePicker2.Value.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                var ham = hamtao.Khoitao();
                ngaybatdau = ham.chuyendoingayvedangso(ngaybatdau);
                ngayketthuc = ham.chuyendoingayvedangso(ngayketthuc);
                var con = ketnoi.Instance();
                DataTable dt = new DataTable();
                dt = con.laythongtinkhoangngay(ngaybatdau, ngayketthuc);
                string tongsoma = con.tongmatrongkhoangngaychon(ngaybatdau, ngayketthuc);
                string tongmachutrung = con.tongmatrongkhoangngaychon_chuatrung(ngaybatdau, ngayketthuc);

                DialogResult dlog = MessageBox.Show("Có muốn lưu file excel không hay in luôn. \nNhấn 'YES' sẽ lưu file và 'NO' sẽ in luôn", "IN LUÔN ?", MessageBoxButtons.YesNo);
                if (dlog == DialogResult.Yes)
                {
                    ham.Xuatfileexcel(dt, ngaybatdau, ngayketthuc, tongsoma);
                    ham.taovainfileexcel(con.laythongtinIn(ngaybatdau, ngayketthuc), tongmachutrung, ngaybatdau, ngayketthuc);

                    PopupNotifier popexcel = new PopupNotifier();
                    popexcel.TitleText = "Thông báo";
                    popexcel.ContentText = "Vừa xuất file excel \nClick vào đây để mở file";
                    popexcel.IsRightToLeft = false;
                    popexcel.Image = Properties.Resources.excel;
                    popexcel.TitleColor = System.Drawing.Color.Lime;
                    popexcel.TitleFont = new System.Drawing.Font("Comic Sans MS", 11, System.Drawing.FontStyle.Underline);
                    popexcel.BodyColor = System.Drawing.Color.DimGray;
                    popexcel.Size = new System.Drawing.Size(380, 130);
                    popexcel.ImageSize = new System.Drawing.Size(100, 100);
                    popexcel.ImagePadding = new Padding(15);
                    popexcel.ContentColor = System.Drawing.Color.White;
                    popexcel.ContentFont = new System.Drawing.Font("Comic Sans MS", 12, System.Drawing.FontStyle.Bold);
                    popexcel.Delay = 3500;
                    popexcel.BorderColor = System.Drawing.Color.DimGray;
                    popexcel.HeaderHeight = 1;
                    popexcel.Click += Popexcel_Click;
                    popexcel.Popup();
                }else
                {
                    ham.taovainfileexcel(con.laythongtinIn(ngaybatdau, ngayketthuc), tongmachutrung, ngaybatdau, ngayketthuc);
                }
                
                
            }
            catch (Exception ex)
            {

                lbtrangthai.Text = ex.ToString();
            }
            
        }

        private void Popexcel_Click(object sender, EventArgs e)
        {
            var ham = hamtao.Khoitao();
            ham.mofileexcelvualuu();
        }

        private void pbphatanh_Click(object sender, EventArgs e)
        {
            if (phathaykhongphat)
            {
                pbphatanh.Image = Properties.Resources.play;
                phathaykhongphat = !phathaykhongphat;
                pbanhsanpham.Image = Properties.Resources.totoro1;
                lbmahang.Text = "Totoro";
                dieukhienthread.Reset();
            }
            else
            {
                pbphatanh.Image = Properties.Resources.pause;
                phathaykhongphat = !phathaykhongphat;
                dieukhienthread.Set();
            }
        }
        void dungphatanh()
        {
            if (phathaykhongphat)
            {
                pbphatanh.Image = Properties.Resources.play;
                phathaykhongphat = !phathaykhongphat;
                dieukhienthread.Reset();
            }
        }


        private void lbfiledanhmucmoi_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(lbfiledanhmucmoi.Text))
            {
                string mau = @"File mới:-->\s(?<tenfile>.*)";
                string get = null;
                MatchCollection matchs = Regex.Matches(lbfiledanhmucmoi.Text, mau);
                foreach (Match mm in matchs)
                {
                    get = mm.Groups["tenfile"].Value.ToString();
                }
                System.Diagnostics.Process.Start(duongdanfilemoi + get);
            }
        }



        #endregion

        private void pbAMTHANH_Click(object sender, EventArgs e)
        {
            phatAM = !phatAM;
            if (phatAM)
            {
                pbAMTHANH.Image = Properties.Resources.speaker;
            }
            else
            {
                pbAMTHANH.Image = Properties.Resources.mute;
            }
        }

        private void nuthts_trung_CheckedChanged(object sender, EventArgs e)
        {
            var con = ketnoi.Instance();
            if (nuthts_trung.Checked)
            {
                datag1.DataSource = con.laydanhsachCHUATRUNG();
                updatesoluongtrenbang();
            }
            else
            {
                ngaychonbandau = con.layngayganhat();
                datag1.DataSource = con.laythongtinngayganhat(ngaychonbandau);
                updatesoluongtrenbang();
            }
        }
    }
}
