
using EofficeClient.ViewModel;
using QLHS_DR.ViewModel.DocumentViewModel;
using QLHS_DR.ViewModel.LsxViewModel;
using QLHS_DR.ViewModel.Products;
using QLHS_DR.ViewModel.ProductViewModel;

namespace QLHS_DR.ViewModel
{
    class ViewModelLocator
    {
        public MainViewModel MainViewModel
        {
            get { return new MainViewModel(); }
        }
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
        
    }
}
