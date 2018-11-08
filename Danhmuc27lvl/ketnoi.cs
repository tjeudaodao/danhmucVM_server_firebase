using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MySql.Data.MySqlClient;

namespace Danhmuc27lvl
{
    class ketnoi
    {
        #region khoitao
        private ketnoi()
        {
            string connstring = string.Format("Server=27.72.29.28;port=3306; database=cnf; User Id=kho; password=1234");
            // string connstring = string.Format("Server=localhost;port=3306; database=cnf; User Id=hts; password=1211");
            connection = new MySqlConnection(connstring);
        }
        
        private MySqlConnection connection = null;
       
        private static ketnoi _instance = null;
        public static ketnoi Instance()
        {
            if (_instance == null)
                _instance = new ketnoi();
            return _instance;
        }
        public void Open()
        {
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }
        }
      
        public void Close()
        {
            if (connection.State != ConnectionState.Closed)
            {
                connection.Close();
            }
        }
        #endregion
        #region thao tac tren csdl mysql
        string ngaychen = DateTime.Now.ToString("dd/MM/yyyy");
        string cottrunghang = "trunghang";
        //kiem tra xem ma hang day co trong bang mota chua
        public string Kiemtra(string mahang)
        {
            string sql = @"SELECT matong1 FROM mota WHERE matong1='" + mahang + "'";
            MySqlCommand cmd = new MySqlCommand(sql, connection);
            string hh = null;
            Open();
            MySqlDataReader dtr = cmd.ExecuteReader();
            while (dtr.Read())
            {
                hh = dtr["matong1"].ToString();
            }
            Close();
            return hh;
        }
        public string Kiemtra(string cotcankiem, string tenbangkiem, string giatricantim)
        {
            string sql = string.Format("select {0} from {1} where {0}='{2}'", cotcankiem, tenbangkiem, giatricantim);
            string giatri = null;
            Open();
            MySqlCommand cmd = new MySqlCommand(sql, connection);
            MySqlDataReader dtr = cmd.ExecuteReader();

            while (dtr.Read())
            {
                giatri = dtr[cotcankiem].ToString();
            }
            Close();
            return giatri;
        }
        public string Kiemtra(string laygiatri, string tubang, string noigiatri, string bang)
        {
            string sql = string.Format("select {0} from {1} where {2}='{3}'", laygiatri, tubang, noigiatri, bang);
            string giatri = null;
            Open();
            MySqlCommand cmd = new MySqlCommand(sql, connection);
            MySqlDataReader dtr = cmd.ExecuteReader();

            while (dtr.Read())
            {
                giatri = dtr[laygiatri].ToString();
            }
            Close();
            return giatri;
        }

        // lay masp tu barcode
        public string laymasp(string barcode)
        {
            string sql = string.Format("SELECT masp FROM data WHERE barcode='{0}'", barcode);
            string h = null;
            MySqlCommand cmd = new MySqlCommand(sql, connection);
            Open();
            MySqlDataReader dtr = cmd.ExecuteReader();
            while (dtr.Read())
            {
                h = dtr["masp"].ToString();
            }
            Close();
            int vitri = h.IndexOf("-");
            h = h.Substring(0, vitri);
            return h;
        }

        public void Chenvaobanghangduocban(string maduocban, string ngayduocban, string ghichu, string ngaydangso, string mota, string chude)
        {
            //string sqlchen = @"INSERT INTO hangduocban(matong,ngayban,ghichu,ngaydangso,mota,chude) VALUES('"+maduocban+"','"+ngayduocban+"','"+ghichu+"','"+ngaydangso+",'"+mota+"','"+chude+"')";
            string sqlchen = "insert into hangduocban(matong,ngayban,ghichu,ngaydangso,mota,chude) VALUES(@1,@2,@3,@4,@5,@6)";
            Open();
            //MySqlCommand cmd = new MySqlCommand(sqlchen, connec);
            MySqlCommand cmd = connection.CreateCommand();
            cmd.CommandText = sqlchen;
            cmd.Parameters.AddWithValue("@1", maduocban);
            cmd.Parameters.AddWithValue("@2", ngayduocban);
            cmd.Parameters.AddWithValue("@3", ghichu);
            cmd.Parameters.AddWithValue("@4", ngaydangso);
            cmd.Parameters.AddWithValue("@5", mota);
            cmd.Parameters.AddWithValue("@6", chude);
            cmd.ExecuteNonQuery();
            Close();
        }
        #endregion

        #region xu ly tren form
        public DataTable loctheotenmatong(string matong)
        {
            string sql = string.Format("SELECT matong as 'Mã tổng',mota as 'Mô tả',chude as 'Chủ đề',ghichu as 'Ghi chú',ngayban as 'Ngày bán',trunghang as 'Trưng hàng' FROM hangduocban where matong like '{0}%' Group by matong", matong);
            DataTable dt = new DataTable();
            Open();
            MySqlDataAdapter dta = new MySqlDataAdapter(sql, connection);
            dta.Fill(dt);
            Close();
            return dt;
        }
        // laythong tin gan vao list<>
        public List<laythongtin> loclaythongtin1ma(string matong)
        {
            string sql = string.Format("SELECT matong as 'Mã tổng',mota as 'Mô tả',chude as 'Chủ đề',ghichu as 'Ghi chú',ngayban as 'Ngày bán' FROM hangduocban where matong = '{0}'", matong);
            List<laythongtin> laytt = new List<laythongtin>();
            MySqlCommand cmd = new MySqlCommand(sql, connection);
            Open();
            MySqlDataReader dtr = cmd.ExecuteReader();
            while (dtr.Read())
            {
                laytt.Add(new laythongtin(dtr[4].ToString(), dtr[0].ToString(), dtr[1].ToString(), dtr[2].ToString(), dtr[3].ToString(), null));
            }
            Close();
            return laytt;
        }
        // lay thong tin trung hang
        public string laythongtintrunghang(string matong)
        {
            string sql = string.Format("select trunghang from hangduocban where matong='{0}'", matong);
            string h = null;
            Open();
            MySqlCommand cmd = new MySqlCommand(sql, connection);
            MySqlDataReader dtr = cmd.ExecuteReader();
            while (dtr.Read())
            {
                h = dtr[0].ToString();
            }
            Close();
            return h;
        }
        //laytong so ma trong ngay chon
        public string tongmatrongkhoangngaychon(string ngaydau,string ngaycuoi)
        {
            string sql = string.Format(@"select count(matong) from hangduocban where ngaydangso >= '{0}' and ngaydangso <= '{1}'", ngaydau,ngaycuoi);
            string h = null;
            Open();
            MySqlCommand cmd = new MySqlCommand(sql, connection);
            MySqlDataReader dtr = cmd.ExecuteReader();
            while (dtr.Read())
            {
                h = dtr[0].ToString();
            }
            Close();
            return h;
        }
        public string tongmatrongkhoangngaychon_chuatrung(string ngaydau, string ngaycuoi)
        {
            string sql = string.Format(@"select count(matong) 
                                        from hangduocban 
                                        where ngaydangso >= '{0}' and ngaydangso <= '{1}'  and (trunghang is null or trunghang = 'Chưa trưng bán' or trunghang = '')", ngaydau, ngaycuoi);
            string h = null;
            Open();
            MySqlCommand cmd = new MySqlCommand(sql, connection);
            MySqlDataReader dtr = cmd.ExecuteReader();
            while (dtr.Read())
            {
                h = dtr[0].ToString();
            }
            Close();
            return h;
        }
        // lay ngay gan nhat trong bang hang duoc ban
        public string layngayganhat()
        {
            string sql = "select max(ngaydangso) from hangduocban";
            MySqlCommand cmd = new MySqlCommand(sql, connection);
            string hh = null;
            Open();
            MySqlDataReader dtr = cmd.ExecuteReader();
            while (dtr.Read())
            {
                hh = dtr[0].ToString();
            }
            Close();
            return hh;
        }
        public DataTable laythongtinngayganhat(string ngaygannhat)
        {
            string sql = string.Format("SELECT matong as 'Mã tổng',mota as 'Mô tả',chude as 'Chủ đề',ghichu as 'Ghi chú',ngayban as 'Ngày bán',trunghang as 'Trưng hàng' FROM hangduocban where ngaydangso = '{0}' order by matong", ngaygannhat);
            DataTable dt = new DataTable();
            Open();
            MySqlDataAdapter dta = new MySqlDataAdapter(sql, connection);
            dta.Fill(dt);
            Close();
            return dt;
        }
        // lay thong tin khi kich chon ngay
        public DataTable laythongtinkhichonngay(string cotngay, string ngaychon)
        {
            string sql = string.Format("SELECT matong as 'Mã tổng',mota as 'Mô tả',chude as 'Chủ đề',ghichu as 'Ghi chú',ngayban as 'Ngày bán',trunghang as 'Trưng hàng' FROM hangduocban where {1} = '{0}' order by matong", ngaychon,cotngay);
            DataTable dt = new DataTable();
            Open();
            MySqlDataAdapter dta = new MySqlDataAdapter(sql, connection);
            dta.Fill(dt);
            Close();
            return dt;
        }
        // them ma moi vao danh sach hang duoc ban
        public void themmamoivaodanhsachduocban(string mahang)
        {
            string sql = string.Format("INSERT INTO hangduocban(matong,ngayban,ghichu) VALUES('{0}','{1}','{2}')", mahang, ngaychen, "Thêm mã thủ công");
            MySqlCommand cmd = new MySqlCommand(sql, connection);
            Open();
            cmd.ExecuteNonQuery();
            Close();
        }
        // xuat bang khi chon khoang ngay cho viec xuat excel va in
        public DataTable laythongtinkhoangngay(string ngaybatday, string ngayketthuc)
        {
            string sql = string.Format(@"SELECT matong as 'Mã tổng',mota as 'Mô tả',chude as 'Chủ đề',ghichu as 'Ghi chú',ngayban as 'Ngày bán',trunghang as 'Trưng hàng' 
                                        FROM hangduocban 
                                        where ngaydangso >= '{0}' and ngaydangso <= '{1}' 
                                        order by matong", ngaybatday, ngayketthuc);
            DataTable dt = new DataTable();
            Open();
            MySqlDataAdapter dta = new MySqlDataAdapter(sql, connection);
            dta.Fill(dt);
            Close();
            return dt;
        }
        // xuatbang cho viec in chi lay 3 cot matong bst ngayban
        public DataTable laythongtinIn(string ngaybatdau, string ngayketthuc)
        {
            string sql = string.Format(@"SELECT matong as 'Mã tổng',chude as 'Chủ đề',trunghang as 'Trưng hàng' 
                                        FROM hangduocban 
                                        where ngaydangso >= '{0}' and ngaydangso <= '{1}' and (trunghang is null or trunghang = 'Chưa trưng bán' or trunghang = '') 
                                        order by matong", ngaybatdau, ngayketthuc);
            DataTable dt = new DataTable();
            Open();
            MySqlDataAdapter dta = new MySqlDataAdapter(sql, connection);
            dta.Fill(dt);
            Close();
            return dt;
        }
        // update gia tri cot trung hang thanh " da trung hang"
        public void updatedatrunghangthanhdatrung(string matong)
        {
            string ch = Kiemtra("trunghang", "hangduocban", "matong", matong);
            if (ch == null || ch == "Chưa trưng bán" || ch == "")
            {
                string sql = string.Format("UPDATE hangduocban SET trunghang='{0}' WHERE matong='{1}'", "Đã Trưng Bán", matong);
                MySqlCommand cmd = new MySqlCommand(sql, connection);
                Open();
                cmd.ExecuteNonQuery();
                Close();
            }

        }
        // update thanh chua trung hang
        public void updatetrunghangthanhchuatrung(string matong)
        {
            string ch = Kiemtra("trunghang", "hangduocban", "matong", matong);
            if (ch == "Đã Trưng Bán" || ch == null || ch == "")
            {
                string sql = string.Format("UPDATE hangduocban SET trunghang='{0}' WHERE matong='{1}'", "Chưa trưng bán", matong);
                MySqlCommand cmd = new MySqlCommand(sql, connection);
                Open();
                cmd.ExecuteNonQuery();
                Close();
            }
        }
        public void chenvaoFiledanhmuc(string tenFile)
        {
            string sql = "update filedanhmucmoi set tenfile = '" + tenFile + "' , gio = '" + DateTime.Now.ToString("dd-MM-yyyy HH-mm") + "'";
            Open();
            MySqlCommand cmd = new MySqlCommand(sql, connection);
            cmd.ExecuteNonQuery();
            Close();
        }

        public DataTable laydanhsachCHUATRUNG()
        {
            string sql = string.Format(@"SELECT matong as 'Mã tổng',mota as 'Mô tả',chude as 'Chủ đề',ghichu as 'Ghi chú',ngayban as 'Ngày bán',{0} as 'Trưng hàng' 
                                        FROM hangduocban 
                                        WHERE ({0} = 'Chưa trưng bán' or {0} is null or {0} = '') order by ngaydangso", cottrunghang);
            DataTable dt = new DataTable();
            Open();
            MySqlDataAdapter dta = new MySqlDataAdapter(sql, connection);
            dta.Fill(dt);
            Close();
            return dt;
        }

        #endregion
    }
}
