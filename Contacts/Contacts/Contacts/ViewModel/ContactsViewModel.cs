using Contacts.Models;
using Contacts.Services;
using GalaSoft.MvvmLight.Command;
using Plugin.Connectivity;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Contacts.ViewModel
{
    public class ContactsViewModel
    {
        #region Events
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region Attributes
        private ApiService apiService;
        public DialogService dialogService;
        private bool isRefreshing;
        #endregion

        #region Properties
        public ObservableCollection<ContactItemViewModel> MyContacts { get; set; }

        public bool IsRefreshing
        {
            set
            {
                if (isRefreshing != value)
                {
                    isRefreshing = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsRefreshing"));
                }
            }
            get
            {
                return isRefreshing;
            }
        }

        public bool VisibleFlag;

        #endregion

        #region Constructors
        public ContactsViewModel()
        {
            apiService = new ApiService();
            dialogService = new DialogService();

            MyContacts = new ObservableCollection<ContactItemViewModel>();

            LoadContacts();
        }
        #endregion

        #region Methods
        public async void LoadContacts()
        {
           if (!CrossConnectivity.Current.IsConnected)
            {
                await dialogService.ShowMessage("Error", "Verifica tu conección a internet");//change Display for ForeingExchangePage
                VisibleFlag = true;
                return;
            }

            //Check get access to internet
            var isRechable = await CrossConnectivity.Current.IsRemoteReachable("www.google.com");
            if (isRechable)
            {
                await dialogService.ShowMessage("Error", "No hay acceso a internet");//change Display for ForeingExchangePage
                VisibleFlag = true;
                return;
            }
            IsRefreshing = true;//show waiting
            VisibleFlag = false;
            var response = await apiService.Get<Contact>(
                "https://contactsxamarintata.azurewebsites.net",
                "/api",
                "/Contacts");
            IsRefreshing = false;//Hide Waiting
            if (!response.IsSuccess)
            {
                await dialogService.ShowMessage("Error", response.Message);
                IsRefreshing = false;//Hide Waiting
                return;
            }

            var Contacts = (List<Contact>)response.Result;
            ReloadContacts(Contacts);
        }

        private void ReloadContacts(List<Contact> contacts)
        {
            MyContacts.Clear();
            foreach (var contact in contacts.OrderBy(c => c.FirstName).ThenBy(c => c.LastName))
            {
                MyContacts.Add(new ContactItemViewModel
                {
                    ContactId = contact.ContactId,
                    EmailAddress = contact.EmailAddress,
                    FirstName = contact.FirstName,
                    LastName = contact.LastName,
                    PhoneNumber = contact.PhoneNumber,
                    Image = contact.Image,
                });
            }
        }

        private void Refresh()
        {
            IsRefreshing = true;
            LoadContacts();
            IsRefreshing = false;
        }
        #endregion

        #region Commands
        public ICommand RefreshCommand
        {
            get { return new RelayCommand(Refresh); }
        }
        #endregion

        #region Singleton
        private static ContactsViewModel instance;

        public static ContactsViewModel GetInstance()
        {
            if (instance == null)
            {
                instance = new ContactsViewModel();
            }

            return instance;
        }
        #endregion
    }
}
