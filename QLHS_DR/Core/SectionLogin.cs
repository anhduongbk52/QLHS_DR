using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QLHS_DR.Core
{
    internal class SectionLogin //Luu tru cac bien cua phien dang nhap
    {
        private static SectionLogin _ins;
        public static SectionLogin Ins
        {
            get
            {
                if (_ins == null)
                    _ins = new SectionLogin();
                return _ins;
            }
            set
            {
                _ins = value;
            }
        }
        public ServiceReference1.User CurrentUser { get; set; }
    }
}
