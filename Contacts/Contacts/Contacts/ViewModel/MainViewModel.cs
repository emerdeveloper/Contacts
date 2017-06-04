using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contacts.ViewModel
{
    public class MainViewModel
    {
        public ContactsViewModel Contacts { get; set; }

        #region Constructor
        //Solo instanciar los que arrancan con la aplicación
        public MainViewModel()
        {
            Contacts = new ContactsViewModel();
        }
        #endregion

    }
}
