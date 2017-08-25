using Contacts.Helpers;
using Contacts.Models;
using Contacts.Services;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;
using Plugin.Media;
using Plugin.Media.Abstractions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace Contacts.ViewModel
{
    public class NewContactViewModel : Contact, INotifyPropertyChanged
    {
        #region Events
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region Attributes
        private DialogService dialogService;
        private ApiService apiService;
        private NavegationService navigationService;
        private bool isRunning;
        private bool isEnabled;
        private ImageSource imageSource;//Mostrar la imagen para la visualización
        private MediaFile file;//Donde estará la foto en pantalla
        #endregion

        #region Properties
        public bool IsRunning
        {
            set
            {
                if (isRunning != value)
                {
                    isRunning = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsRunning"));
                }
            }
            get
            {
                return isRunning;
            }
        }

        public bool IsEnabled
        {
            set
            {
                if (isEnabled != value)
                {
                    isEnabled = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsEnabled"));
                }
            }
            get
            {
                return isEnabled;
            }
        }

        public ImageSource ImageSource
        {
            set
            {
                if (imageSource != value)
                {
                    imageSource = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ImageSource"));
                }
            }
            get
            {
                return imageSource;
            }
        }
        #endregion

        #region Consttructors
        public NewContactViewModel() {
            apiService = new ApiService();
            dialogService = new DialogService();
            navigationService = new NavegationService();

            IsEnabled = true;
        }
        #endregion

        #region Command
        //Para tomar la foto
        public ICommand TakePictureCommand { get { return new RelayCommand(TakePicture); } }

        private async void TakePicture()
        {
            await CrossMedia.Current.Initialize();

            if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)//Si la camara esta disponible y si soporta foto
            {
                await dialogService.ShowMessage("No Camera", ":( No camera available.");
            }

            file = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions//Toma la foto
            {
                Directory = "Sample",
                Name = "test.jpg",
                PhotoSize = PhotoSize.Small,
            });

            IsRunning = true;

            if (file != null)//Si tomo la foto
            {
                ImageSource = ImageSource.FromStream(() =>
                {
                    var stream = file.GetStream();
                    return stream;
                });
            }

            IsRunning = false;
        }

        public ICommand NewContactCommand
        {
            get { return new RelayCommand(NewContact); }
        }

        private async void NewContact()
        {
            if (string.IsNullOrEmpty(FirstName))
            {
                await dialogService.ShowMessage("Error", "You must enter a first name");
                return;
            }

            if (string.IsNullOrEmpty(LastName))
            {
                await dialogService.ShowMessage("Error", "You must enter a last name");
                return;
            }

            byte[] imageArray = null;
            if (file != null)
            {
                imageArray = FilesHelper.ReadFully(file.GetStream());
                file.Dispose();
            }

            var contact = new Contact
            {
                EmailAddress = EmailAddress,
                FirstName = FirstName,
                ImageArray = imageArray,
                LastName = LastName,
                PhoneNumber = PhoneNumber,
            };

            //get foto

            IsRunning = true;
            IsEnabled = false;
            var response = await apiService.Post(
                "https://contactsxamarintata.azurewebsites.net", 
                "/api", 
                "/Contacts", 
                this);
            //var response = await apiService.Post("http://contactsbackprep.azurewebsites.net", "/api", "/Contacts", contact);
            IsRunning = false;
            IsEnabled = true;

            if (!response.IsSuccess)
            {
                await dialogService.ShowMessage("Error", response.Message);
                return;
            }

            //Get foto

            //Get foto

            await navigationService.Back();//regresamos a la pagina de la lista
        }
        #endregion

    }
}
