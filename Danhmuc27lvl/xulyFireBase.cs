using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;
using System.Data;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace Danhmuc27lvl
{
    class xulyFireBase
    {
        public static EventStreamResponse trunghangListener;
        public static void huyListener()
        {
            trunghangListener.Cancel();
        }
        public static string tencuahang;
        public static string setTenCuaHang
        {
            get { return tencuahang; }
            set { tencuahang = value; }
        }
        public static IFirebaseClient clientFirebase;
        public static IFirebaseConfig configFirebase = new FirebaseConfig
        {
            AuthSecret = "w2evy6pLiTOlWdsl3ZJ40eJ1qvCkCrFGUecs2kou",
            BasePath = "https://danhmucvm-cnf.firebaseio.com/"
        };
        class thongso
        {
            public ngaymoinhat ngaymoinhat { get; set; }
            public filemoi filemoi { get; set; }
            
        }
        class filemoi
        {
            public string tenfile { get; set; }
        }
        class ngaymoinhat
        {
            public string tenngay { get; set; }
        }
        // cac thao tac voi co so du lieu firebase
        public static async Task<string> layngayGannhat()
        { 
            clientFirebase = new FireSharp.FirebaseClient(configFirebase);
            FirebaseResponse layngay = await clientFirebase.GetAsync("thongso/ngaymoinhat");
            ngaymoinhat kq = layngay.ResultAs<ngaymoinhat>();
            return kq.tenngay;
        }
        public static async void taobang(string ngaychon,DataGridView dtv)
        {
            clientFirebase = new FireSharp.FirebaseClient(configFirebase);
            DataTable dt = new DataTable();
            dt.Columns.Add("Mã tổng");
            dt.Columns.Add("Mô tả");
            dt.Columns.Add("Chủ đề");
            dt.Columns.Add("Ghi chú");
            dt.Columns.Add("Ngày được bán");
            dt.Columns.Add("Tình trạng trưng");
            dt.AcceptChanges();
            try
            {
                FirebaseResponse laytheongay = await clientFirebase.GetAsync("ngayduocban/" + ngaychon);
                Dictionary<string, dulieu> kq = laytheongay.ResultAs<Dictionary<string, dulieu>>();
                foreach (KeyValuePair<string, dulieu> item in kq)
                {
                    DataRow row = dt.NewRow();
                    row[0] = item.Key;
                    row[1] = item.Value.mota;
                    row[2] = item.Value.chude;
                    row[3] = item.Value.ghichu;
                    row[4] = item.Value.ngayduocban;
                    row[5] = item.Value.taikhoancnf.layten(tencuahang).trangthaitrung;
                    dt.Rows.Add(row);
                }
            }
            catch (Exception)
            {
                dtv.DataSource = null;
                return;
            }
            dtv.DataSource = dt;
        }
        public static async void chenFilemoi(string tenfile)
        {
            clientFirebase = new FireSharp.FirebaseClient(configFirebase);
            FirebaseResponse chen = await clientFirebase.UpdateAsync("thongso/filemoi", new { tenfile = tenfile});
        }
        public static async void langngheLoadbang(DataGridView dtv)
        {
            clientFirebase = new FireSharp.FirebaseClient(configFirebase);
            EventStreamResponse response = await clientFirebase.OnAsync("thongso/ngaymoinhat",
                changed:
                (sender, args, context) => {
                    dtv.Invoke(new MethodInvoker(delegate ()
                    {
                        taobang(args.Data, dtv);
                    }));
                });
        }
        public static async void langngheTrungHang(DataGridView dtv)
        {
            clientFirebase = new FireSharp.FirebaseClient(configFirebase);
            trunghangListener = await clientFirebase.OnAsync("ngayduocban",
                changed:
                (sender, args, context) =>
                {

                    var con = ketnoisqlite.khoitao();
                    if (con.docClick() == "0")
                    {
                        string path = args.Path;
                        string patt = @"/(\d{8})/(\d{1}\w{2}\d{2}\w\d{3})/";
                        var match = Regex.Match(path, patt);
                        string ngaydangso = match.Groups[1].ToString();
                        string mahang = match.Groups[2].ToString();
                        con.updatetrunghang(mahang, args.Data);
                        dtv.Invoke(new MethodInvoker(delegate ()
                        {
                            taobang(ngaydangso, dtv);
                        }));
                    }
                    
                });
        }
        public static async void updateTrunghangFB(string ngaydangso, string matong, string tentrangthai)
        {
            clientFirebase = new FireSharp.FirebaseClient(configFirebase);
            FirebaseResponse chen = await clientFirebase.UpdateAsync("ngayduocban/" + ngaydangso + "/" + matong + "/taikhoancnf/" + tencuahang , new { trangthaitrung = tentrangthai });
        }
    }
}
