
using EofficeClient.ViewModel;
using QLHS_DR.ViewModel.DepartmentViewModel;
using QLHS_DR.ViewModel.DocumentViewModel;
using QLHS_DR.ViewModel.LsxViewModel;
using QLHS_DR.ViewModel.PhanQuyen;
using QLHS_DR.ViewModel.Products;
using QLHS_DR.ViewModel.ProductViewModel;
using QLHS_DR.ViewModel.UserViewModel;

namespace QLHS_DR.ViewModel
{
    class ViewModelLocator
    {       
        public LoginViewModel LoginViewModel
        {
            get { return new LoginViewModel(); }
        }
        public NewProductViewModel NewProductViewModel
        {
            get { return new NewProductViewModel(); }
        }
        public NewLsxViewModel NewLsxViewModel
        {
            get { return new NewLsxViewModel(); }
        }

        public ProductManagerViewModel ProductManagerViewModel
        {
            get { return new ProductManagerViewModel(); }
        }
        public SignPdfViewModel SignPdfViewModel
        {
            get { return new SignPdfViewModel(); }
        }
        public ListUserViewModel ListUserViewModel
        {
            get { return new ListUserViewModel(); }
        }
        public AddNewUserViewModel AddNewUserViewModel
        {
            get { return new AddNewUserViewModel(); }
        }
        public ListGroupViewModel ListGroupViewModel
        {
            get { return new ListGroupViewModel(); }
        }
        public LoginManagerViewModel LoginManagerViewModel
        {
            get { return new LoginManagerViewModel(); }
        }       
        public DepartmentManagerViewModel DepartmentManagerViewModel
        {
            get { return new DepartmentManagerViewModel(); }
        }
        public NewGroupsUserViewModel NewGroupsUserViewModel
        {
            get { return new NewGroupsUserViewModel(); }
        }
        public FunctionsManagerViewModel FunctionsManagerViewModel
        {
            get { return new FunctionsManagerViewModel(); }
        }
        
    }
}
