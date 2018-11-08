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

namespace Danhmuc27lvl
{
    class xulyFireBase
    {
        public static string tencuahang = "cnf_27lvl";
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
            dt.Columns.Add("Chủ đề");
            dt.Columns.Add("Mo tả");
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
                    row[1] = item.Value.chude;
                    row[2] = item.Value.mota;
                    row[3] = item.Value.ghichu;
                    row[4] = item.Value.ngayduocban;
                    row[5] = item.Value.taikhoancnf.layten(tencuahang);
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
    }
}
