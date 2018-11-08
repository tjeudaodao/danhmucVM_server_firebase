using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Text.RegularExpressions;
using OfficeOpenXml;
using OfficeOpenXml.Drawing;
using Spire.Xls;
using System.Drawing.Imaging;
using Microsoft.Office.Interop;
using excel = Microsoft.Office.Interop.Excel;



namespace nhaphts
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        private void pastevaodtg(DataGridView myDataGridView)
        {
            DataObject o = (DataObject)Clipboard.GetDataObject();
            if (o.GetDataPresent(DataFormats.Text))
            {
                if (myDataGridView.RowCount > 0)
                    myDataGridView.Rows.Clear();

                if (myDataGridView.ColumnCount > 0)
                    myDataGridView.Columns.Clear();

                bool columnsAdded = false;
                string[] pastedRows = Regex.Split(o.GetData(DataFormats.Text).ToString().TrimEnd("\r\n".ToCharArray()), "\r\n");
                foreach (string pastedRow in pastedRows)
                {
                    string[] pastedRowCells = pastedRow.Split(new char[] { '\t' });

                    if (!columnsAdded)
                    {
                        for (int i = 0; i < pastedRowCells.Length; i++)
                            myDataGridView.Columns.Add("col" + i, pastedRowCells[i]);

                        columnsAdded = true;
                        continue;
                    }

                    myDataGridView.Rows.Add();
                    int myRowIndex = myDataGridView.Rows.Count - 2;

                    using (DataGridViewRow myDataGridViewRow = myDataGridView.Rows[myRowIndex])
                    {
                        for (int i = 0; i < pastedRowCells.Length; i++)
                            myDataGridViewRow.Cells[i].Value = pastedRowCells[i];
                    }
                }
            }
        }
        public DataTable copyvungchontuexcel()
        {
            DataTable dt = new DataTable();
            DataObject o = (DataObject)Clipboard.GetDataObject();
            if (o.GetDataPresent(DataFormats.Text))
            {

                dt.Columns.Add("Mã sản phẩm");
                dt.Columns.Add("SL");
                dt.AcceptChanges();

                string[] pastedRows = Regex.Split(o.GetData(DataFormats.Text).ToString().TrimEnd("\r\n".ToCharArray()), "\r\n");
                txtnhap.Text = pastedRows.Length.ToString();
                foreach (string pastedRow in pastedRows)
                {
                    string[] pastedRowCells = pastedRow.Split(new char[] { '\t' });

                    DataRow rowadd = dt.NewRow();
                    for (int i = 0; i < pastedRowCells.Length; i++)
                    {

                        rowadd[i] = pastedRowCells[i];

                    }
                    dt.Rows.Add(rowadd);
                }
            }
            return dt;
        }
        public DataTable copynhap()
        {
            DataTable dt = new DataTable();
            DataObject o = (DataObject)Clipboard.GetDataObject();
            if (o.GetDataPresent(DataFormats.Text))
            {

                dt.Columns.Add("Mã sản phẩm");
                dt.Columns.Add("SL");
                dt.AcceptChanges();
                string goc = o.GetData(DataFormats.Text).ToString().TrimEnd("\r\n".ToCharArray());
                string mau = @"\d\w{2}\d{2}[SWAC]\d{3}-\w{2}\d{3}-\w+\s+\d";
                string mau2 = @"\s+";
                
                MatchCollection matchhts = Regex.Matches(goc, mau);

                txtnhap.Text = o.GetData(DataFormats.Text).ToString();
               
                foreach (Match h in matchhts)
                {
                    
                    string[] hang = Regex.Split(h.Value.ToString(), mau2);
                    txtnhap2.Text = h.Value;
                    DataRow rowadd = dt.NewRow();
                    for (int i = 0; i < hang.Length; i++)
                    {

                        rowadd[i] = hang[i];

                    }
                    dt.Rows.Add(rowadd);
                }
            }
            return dt;
        }
        public void taofileexcel()
        {
            ExcelPackage ExcelPkg = new ExcelPackage();
            ExcelWorksheet wsSheet1 = ExcelPkg.Workbook.Worksheets.Add("Sheet1");
            using (ExcelRange Rng = wsSheet1.Cells[2, 2, 2, 2])
            {
                Rng.Value = "tao la tao";
                Rng.Style.Font.Size = 16;
                Rng.Style.Font.Bold = true;
                Rng.Style.Font.Italic = true;
            }
            wsSheet1.Protection.IsProtected = false;
            wsSheet1.Protection.AllowSelectLockedCells = false;
            ExcelPkg.SaveAs(new FileInfo("hts.xlsx"));
        }

        /// <summary>
        /// nhan phim
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void button2_Click(object sender, EventArgs e)
        {

            OpenFileDialog chonfile = new OpenFileDialog();
            chonfile.Filter = "Mời các anh chọn file excel (*.xls)|*.xls";
            if (chonfile.ShowDialog() == DialogResult.OK)
            {
                var excelApp = new excel.Application();
                var wb = excelApp.Workbooks.Open(chonfile.FileName);
                var ws = (excel.Worksheet)wb.Worksheets[2];
               
                int hangbatdau = 0;
               
                List<string> tenanh =new List<string>();
                foreach (var pic in ws.Pictures())
                {
                     hangbatdau = pic.TopLeftCell.Row;
                   
                    tenanh.Add(ws.Cells[hangbatdau, 5].value);
                }
              
                string[] manganh = tenanh.ToArray();
                excelApp.Quit();

                Workbook workbook = new Workbook();
                workbook.LoadFromFile(chonfile.FileName);

                Worksheet sheet = workbook.Worksheets[1];
                
                
                for (int i = 1; i < manganh.Length; i++)
                {
                    Spire.Xls.ExcelPicture picture = sheet.Pictures[i];
                    picture.Picture.Save(manganh[i]+".png", ImageFormat.Png);
                }
            }
             
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog chonfile = new OpenFileDialog();
            chonfile.Filter = "Mời các anh chọn file excel (*.xlsx)|*.xlsx";

            if (chonfile.ShowDialog() == DialogResult.OK)
            {
                //dung thu vien epplus
                 ExcelPackage filechon = new ExcelPackage(new FileInfo(chonfile.FileName));
                 ExcelWorksheet ws = filechon.Workbook.Worksheets[1];
               
                ExcelDrawing hinh1 = ws.Drawings[0];
               // var hinh2 = ws.Drawings[1];
                int cotdau = hinh1.From.Column;
                int dongdau = hinh1.From.Row;
                int cotcuoi = hinh1.To.Column;
                int dongcuoi = hinh1.To.Row;
                txtnhap.Text = cotdau.ToString() + " " + cotcuoi.ToString() + " " + dongdau.ToString() + " " + dongcuoi.ToString();
                // dung thu vien spire
                Workbook workbook = new Workbook();

                workbook.LoadFromFile(chonfile.FileName);

                Worksheet sheet = workbook.Worksheets[1];
                int soanh = sheet.Pictures.Count();
                Spire.Xls.ExcelPicture picture = sheet.Pictures[1];
                foreach (var anh in sheet.Pictures)
                {

                }
            picture.Picture.Save(@"image.png", ImageFormat.Png);
                lbthongbao.Text = "ok so anh la:"+soanh.ToString();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (File.Exists("hts.xlsx"))
            {
                File.Delete("hts.xlsx");
  
            }            taofileexcel();
        }
    }
}
