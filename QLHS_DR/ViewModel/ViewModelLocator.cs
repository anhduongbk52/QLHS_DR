

using EofficeClient.ViewModel;
using EofficeClient.ViewModel.DocumentViewModel;

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
       
    }
}
