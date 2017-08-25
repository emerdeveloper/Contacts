using Contacts.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contacts.Services
{
    public class NavegationService
    {
        public async Task Navigate(string pageName)//pagina a la cual se va a navegar
        {
            switch (pageName)
            {
                case "NewContactPage":
                    await App.Current.MainPage.Navigation.PushAsync(new NewContactPage());//agregamos la nueva pagina
                    break;
                case "EditContactPage":
                    await App.Current.MainPage.Navigation.PushAsync(new EditContactPage());
                    break;
                default:
                    break;
            }
        }

        public async Task Back()//retrocedemos a la pagina anterior
        {
            await App.Current.MainPage.Navigation.PopAsync();
        }
    }
}
