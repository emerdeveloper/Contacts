using Contacts.Models;
using Contacts.Services;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Contacts.ViewModel
{
    public class ContactItemViewModel : Contact
    {
        #region Attribute
        private NavegationService navegationServce;
        #endregion

        #region Constructors
        public ContactItemViewModel() {
            navegationServce = new NavegationService();
        }
        #endregion

        #region Commands
        public ICommand EditContactCommand {
            get { return new RelayCommand(EditContact); }
        }

        private async void EditContact()
        {
            var mainViewModel = MainViewModel.GetInstance();
            mainViewModel.EditContact = new EditContactViewModel(this);
            await navegationServce.Navigate("EditContactPage");
        }
        #endregion

    }
}
