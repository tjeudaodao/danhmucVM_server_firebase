using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Danhmuc27lvl
{
    class taikhoan
    {
        public trunghang cnf_27lvl { get; set; }
        public trunghang cnf_121cb { get; set; }
        public trunghang cnf_171tp { get; set; }
        public trunghang cnf_181gv { get; set; }
        public trunghang cnf_25ldh { get; set; }
        public trunghang cnf_335cg { get; set; }
        public trunghang cnf_554nvc { get; set; }
        public trunghang cnf_aeon { get; set; }
        public trunghang cnf_royal { get; set; }
        public trunghang cnf_timecity { get; set; }
        public trunghang cnf_bigc { get; set; }
        public trunghang cnf_185thd { get; set; }
        
        public trunghang layten (string ten)
        {
            trunghang kq = null;
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
            else if (ten == "cnf_185thd")
            {
                kq = cnf_185thd;
            }
            return kq;
        }
    }
    class trunghang
    {
        public string trangthaitrung { get; set; }
    }
}
