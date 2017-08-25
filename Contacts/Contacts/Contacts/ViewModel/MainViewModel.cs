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
    public class MainViewModel
    {
        #region Atributos
        public NavegationService navigationService;
        #endregion

        #region Properties
        public ContactsViewModel Contacts { get; set; }
        public NewContactViewModel NewContact { get; set; } 
        public EditContactViewModel EditContact { get; set; }
        #endregion

        #region Constructor
        //Solo instanciar los que arrancan con la aplicación
        public MainViewModel()
        {
            instance = this;
            Contacts = new ContactsViewModel();//Por aca inicia
             // NewContact = new NewContactViewModel();
            navigationService = new NavegationService();
        }
        #endregion

        #region Singleton
        static MainViewModel instance;

        public static MainViewModel GetInstance()
        {
            if (instance == null)
            {
                instance = new MainViewModel();
            }

            return instance;
        }
        #endregion

        #region Command
        //esste comando lo definio la region en el COntactsPage.xaml
        public ICommand AddContactCommand
        {
            get
            { return new RelayCommand(AddContact); }
        }

        private async void AddContact()
        {
            NewContact = new NewContactViewModel();
            await navigationService.Navigate("NewContactPage");
        }
        #endregion

    }
}
