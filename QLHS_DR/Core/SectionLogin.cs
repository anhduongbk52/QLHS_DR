using QLHS_DR.ChatAppServiceReference;
using System.Collections.Generic;
using System.Linq;

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
        private List<Permission> _ListPermissions;
        public List<Permission> ListPermissions
        {
            get { return _ListPermissions; }
            set
            {
                if (_ListPermissions != value)
                {
                    _ListPermissions = value;
                    if (_ListPermissions != null)
                    {
                        _CanRemoveProduct = _ListPermissions.Any(x => x.Code == "productRemoveProduct");
                        _CanOpenProduct = _ListPermissions.Any(x => x.Code == "productOpenPowerTransformer");
                        _CanLockProduct = _ListPermissions.Any(x => x.Code == "productLockProduct");
                        _CanOpenPowerTransformer = _ListPermissions.Any(x => x.Code == "productOpenPowerTransformer");
                        _CanOpenDistributionTransformer = _ListPermissions.Any(x => x.Code == "productOpenDistributionTransformer");
                        _CanOpenInstrumentTransformer = _ListPermissions.Any(x => x.Code == "productOpenInstrumentTransformer");
                        _CanNewProduct = _ListPermissions.Any(x => x.Code == "productNewProduct");
                        _CanChangeTDOfProduct = _ListPermissions.Any(x => x.Code == "productChangeProductInfo");
                        _CanOpenContract = _ListPermissions.Any(x => x.Code == "productOpenContractFile");
                        _CanRemoveContract = _ListPermissions.Any(x => x.Code == "productRemoveContract");
                        _CanRemoveOwnContract = _ListPermissions.Any(x => x.Code == "productRemoveContractOfOwner");
                        _CanUploadContract = _ListPermissions.Any(x => x.Code == "productUploadContract");
                        _CanViewApprovalDocumentProduct = _ListPermissions.Any(x => x.Code == "productViewApprovalDocumentProduct");
                        _CanRestoreApprovalDocumentProduct = _ListPermissions.Any(x => x.Code == "productRestoreApprovalDocumentProduct");
                        _CanRemoveApprovalDocumentProduct = _ListPermissions.Any(x => x.Code == "productRemoveApprovalDocumentProduct");
                        _CanUpdateApprovalDocumentProduct = _ListPermissions.Any(x => x.Code == "productUpdateApprovalDocumentProduct");
                        _CanConfirmDisposedPrintedDocument = _ListPermissions.Any(x => x.Code == "taskConfirmDisposedPrintedDocument");
                        _CanViewLogs=_ListPermissions.Any(x => x.Code == "userViewLog");
                        _CanViewListGroup= _ListPermissions.Any(x => x.Code == "userViewListGroup");
                        _CanPermissionManager = _ListPermissions.Any(x => x.Code == "userPermissionManager");
                        _CanNewRole = _ListPermissions.Any(x => x.Code == "userNewRole");
                        _CanNewUser = _ListPermissions.Any(x => x.Code == "userNewUser");
                        _CanViewListUsers=_ListPermissions.Any(x => x.Code == "userViewListUsers");
                        _CanUploadTransformerManualFile=_ListPermissions.Any(x => x.Code == "productUploadTransformerManualFile");
                        _CanViewListProduct = _ListPermissions.Any(x => x.Code == "productViewListProduct");
                        _CanNewEmployee = _ListPermissions.Any(x => x.Code == "employeeNewEmployee");
                    }
                }
            }
        }
        public User CurrentUser { get; set; }
        internal string Token { get; set; }

        private bool _CanRemoveProduct;
        public bool CanRemoveProduct { get { return _CanRemoveProduct; } }
        private bool _CanChangeTDOfProduct;
        public bool CanChangeTDOfProduct { get { return _CanChangeTDOfProduct; } }
        private bool _CanOpenProduct;
        public bool CanOpenProduct { get { return _CanOpenProduct; } }
        private bool _CanLockProduct;
        public bool CanLockProduct { get { return _CanLockProduct; } }
        private bool _CanOpenPowerTransformer;
        public bool CanOpenPowerTransformer { get { return _CanOpenPowerTransformer; } }
        private bool _CanOpenDistributionTransformer;
        public bool CanOpenDistributionTransformer { get { return _CanOpenDistributionTransformer; } }
        private bool _CanOpenInstrumentTransformer;
        public bool CanOpenInstrumentTransformer { get { return _CanOpenInstrumentTransformer; } }
        private bool _CanNewProduct;
        public bool CanNewProduct { get { return _CanNewProduct; } }
        private bool _CanOpenContract;
        public bool CanOpenContract { get { return _CanOpenContract; } }
        private bool _CanRemoveContract;
        public bool CanRemoveContract { get { return _CanRemoveContract; } }
        private bool _CanRemoveOwnContract;
        public bool CanRemoveOwnContract { get { return _CanRemoveOwnContract; } }
        private bool _CanUploadContract;
        public bool CanUploadContract { get { return _CanUploadContract; } }
        private bool _CanViewApprovalDocumentProduct;
        public bool CanViewApprovalDocumentProduct { get { return _CanViewApprovalDocumentProduct; } }
        private bool _CanRestoreApprovalDocumentProduct;
        public bool CanRestoreApprovalDocumentProduct { get { return _CanRestoreApprovalDocumentProduct; } }
        private bool _CanRemoveApprovalDocumentProduct;
        public bool CanRemoveApprovalDocumentProduct { get { return _CanRemoveApprovalDocumentProduct; } }
        private bool _CanUpdateApprovalDocumentProduct;
        public bool CanUpdateApprovalDocumentProduct { get { return _CanUpdateApprovalDocumentProduct; } }
        private bool _CanConfirmDisposedPrintedDocument;
        public bool CanConfirmDisposedPrintedDocument { get { return _CanConfirmDisposedPrintedDocument; } }
        private bool _CanViewLogs;
        public bool CanViewLogs { get { return _CanViewLogs; } }
        private bool _CanViewListGroup;
        public bool CanViewListGroup { get { return _CanViewListGroup; } }
        private bool _CanPermissionManager;
        public bool CanPermissionManager { get { return _CanPermissionManager; } }
        private bool _CanNewRole;
        public bool CanNewRole { get { return _CanNewRole; } }
        private bool _CanNewUser;
        public bool CanNewUser { get { return _CanNewUser; } }
        private bool _CanViewListUsers;
        public bool CanViewListUsers { get { return _CanViewListUsers; } }
        private bool _CanUploadTransformerManualFile;
        public bool CanUploadTransformerManualFile { get { return _CanUploadTransformerManualFile; } }
        private bool _CanViewListProduct;
        public bool CanViewListProduct { get { return _CanViewListProduct; } }
        private bool _CanNewEmployee;
        public bool CanNewEmployee { get { return _CanNewEmployee; } }

    }
}
