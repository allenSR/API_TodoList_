using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace todoListMobile.models
{
    class Users
    {
        public int numberOfRows { get; set; }
        public int ID_User { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
    }
}