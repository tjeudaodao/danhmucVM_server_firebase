using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Danhmuc27lvl
{
    class taikhoan
    {
        public string cnf_27lvl { get; set; }
        public string cnf_121cb { get; set; }
        public string cnf_171tp { get; set; }
        public string cnf_181gv { get; set; }
        public string cnf_25ldh { get; set; }
        public string cnf_335cg { get; set; }
        public string cnf_554nvc { get; set; }
        public string cnf_aeon { get; set; }
        public string cnf_royal { get; set; }
        public string cnf_timecity { get; set; }
        public string cnf_bigc { get; set; }
        
        public string layten (string ten)
        {
            string kq = null;
            if (ten == "cnf_27lvl")
            {
                kq = cnf_27lvl;
            }
            else if (ten == "cnf_121cb")
            {
                kq = cnf_121cb;
            }
            else if (ten == "cnf_171tp")
            {
                kq = cnf_171tp;
            }
            else if (ten == "cnf_181gv")
            {
                kq = cnf_181gv;
            }
            else if (ten == "cnf_25ldh")
            {
                kq = cnf_25ldh;
            }
            else if (ten == "cnf_335cg")
            {
                kq = cnf_335cg;
            }
            else if (ten == "cnf_554nvc")
            {
                kq = cnf_554nvc;
            }
            else if (ten == "cnf_aeon")
            {
                kq = cnf_aeon;
            }
            else if (ten == "cnf_royal")
            {
                kq = cnf_royal;
            }
            else if (ten == "cnf_timecity")
            {
                kq = cnf_timecity;
            }
            else if (ten == "cnf_bigc")
            {
                kq = cnf_bigc;
            }
            return kq;
        }
    }
}
