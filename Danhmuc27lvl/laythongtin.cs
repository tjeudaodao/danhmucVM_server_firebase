using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Danhmuc27lvl
{
    class laythongtin
    {
        private string ngayduocban;
        private string maduocban;
        private string motamaban;
        private string chudemaban;
        private string ghichu;
        private string ngaydangso;
        public string Ngayduocban
        {
            get { return ngayduocban; }
            set { ngayduocban = value; }
        }
        public string Maduocban
        {
            get { return maduocban; }
            set { maduocban = value; }
        }
        public string Motamaban
        {
            get { return motamaban; }
            set { motamaban = value; }
        }
        public string Chudemaban
        {
            get { return chudemaban; }
            set { chudemaban = value; }
        }
        public string Ghichu
        {
            get { return ghichu; }
            set { ghichu = value; }
        }
        public string Ngaydangso
        {
            get {return ngaydangso; }
            set { ngaydangso = value; }
        }
        public laythongtin(string ngayduocban,string maduocban,string motamaban,string chudemaban,string ghichu,string ngaydangso)
        {
            this.ngayduocban = ngayduocban;
            this.maduocban = maduocban;
            this.motamaban = motamaban;
            this.chudemaban = chudemaban;
            this.ghichu = ghichu;
            this.ngaydangso = ngaydangso;
        }
    }
}
