﻿
using DevExpress.Data.TreeList;
using QLHS_DR.EOfficeServiceReference;
using System.Collections.Generic;

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
        public PermissionType Permissions { get; set; }
        public List<Permission> ListPermissions { get; set; }
        public QLHS_DR.EOfficeServiceReference.User CurrentUser { get; set; }
        internal string Token { get; set; }
    
    }
}
