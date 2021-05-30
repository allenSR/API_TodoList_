using Android.App;
using Android.Content;
using Android.OS;
using Android.Preferences;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Microsoft.PowerBI.Api.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;
using todoListMobile.models;
using Xamarin.Essentials;


namespace todoListMobile
{
    [Activity(Label = "Список дел: вход", MainLauncher = true)]
    class login: Activity
    {
        public static  string APP_PREFERENCES = "mysettings";
        public static  string APP_PREFERENCES_LOGIN = ""; 
        public static string APP_PREFERENCES_PASSWORD = "";
        public static bool APP_PREFERENCES_PassChecked = false;

        CheckBox checkBox;
        EditText loginText;
        EditText passwordText;
        protected override void OnCreate(Bundle savedInstanceState)

        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.login);


            loginText = FindViewById<EditText>(Resource.Id.loginText);
            passwordText = FindViewById<EditText>(Resource.Id.passwordText);
            checkBox = FindViewById<CheckBox>(Resource.Id.checkBoxPass);

            Button loginButton = FindViewById<Button>(Resource.Id.loginButton);
            Button registrationButton = FindViewById<Button>(Resource.Id.registrarionButton);
            Button connectionSettinsButton = FindViewById<Button>(Resource.Id.connectionSettingsButton);

            loginButton.Click += LoginButton_Click;
            registrationButton.Click += registrationButton_Click;
            connectionSettinsButton.Click += connectionSettingsButton_Click;


            ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(this);
            loginText.Text = prefs.GetString("login", "");
            bool checkStatus = prefs.GetBoolean("SavePassCheck", false);
            if(checkStatus == true)
            {
                passwordText.Text = prefs.GetString("password", "");
                checkBox.Checked = true;
            }

            registrationButton.Click += delegate
            {
                Intent intent = new Intent(this, typeof(Register));
                StartActivity(intent);
            };
        }

        
        private void connectionSettingsButton_Click(object sender, EventArgs e)
        {
            ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(this);
            ISharedPreferencesEditor editor = prefs.Edit();

            LayoutInflater inflater = LayoutInflater.From(this);
            View subView = inflater.Inflate(Resource.Layout.ServerAddres, null);
            EditText serverAddresText = (EditText)subView.FindViewById(Resource.Id.serverAddresText);
            EditText serverPortText = (EditText)subView.FindViewById(Resource.Id.serverPortText);
            TextView addres1Text = (TextView)subView.FindViewById(Resource.Id.addres1Text);
            TextView port1Text = (TextView)subView.FindViewById(Resource.Id.port1Text);

            serverAddresText.Text = prefs.GetString("addres", "");
            serverPortText.Text = prefs.GetString("port", "");
            AlertDialog.Builder builder = new AlertDialog.Builder(this, AlertDialog.ThemeDeviceDefaultDark);
            builder.SetView(subView);
            builder.SetPositiveButton("ОК", (senderAlert, args) =>
            {
                if(string.IsNullOrEmpty(serverAddresText.Text))
                {
                    AlertDialog.Builder alert = new AlertDialog.Builder(this, AlertDialog.ThemeDeviceDefaultDark);
                    alert.SetTitle("Ошибка");
                    alert.SetMessage("Укажите адрес сервера.");
                    alert.SetNegativeButton("ОК", (senderAlert, args) => {
                        return;
                    });
                    Dialog dialog = alert.Create();
                    dialog.Show();
                    return;
                }
                if (string.IsNullOrEmpty(serverPortText.Text))
                {
                    AlertDialog.Builder alert = new AlertDialog.Builder(this, AlertDialog.ThemeDeviceDefaultDark);
                    alert.SetTitle("Ошибка");
                    alert.SetMessage("Укажите порт сервера.");
                    alert.SetNegativeButton("ОК", (senderAlert, args) => {
                        return;
                    });
                    Dialog dialog = alert.Create();
                    dialog.Show();
                    return;
                }



                editor.PutString( "addres",  serverAddresText.Text);
                editor.PutString("port",  serverPortText.Text);
                editor.Apply();
                Toast.MakeText(this, "Строка подключения изменена.", ToastLength.Long).Show();
            });
            AlertDialog dialogSettings = builder.Create();
            dialogSettings.Show();
        }

        private void registrationButton_Click(object sender, EventArgs e)
        {
            
        }

  

        private void LoginButton_Click(object sender, EventArgs e)
        {
                     
            ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(this);
            ISharedPreferencesEditor editor = prefs.Edit();


            StringBuilder errors = new StringBuilder();
            if (string.IsNullOrWhiteSpace(loginText.Text)) errors.AppendLine("Введите логин.");
            if (string.IsNullOrWhiteSpace(passwordText.Text)) errors.AppendLine("Введите пароль.");
            if (errors.Length > 0)
            {
                AlertDialog.Builder alert = new AlertDialog.Builder(this);
                alert.SetTitle("Ошибка");
                alert.SetMessage(errors.ToString());
                alert.SetNegativeButton("ОК", (senderAlert, args) => {
                   
                });
                Dialog dialog = alert.Create();
                dialog.Show();
                return;
            }
            
            try
            {
                string url = "http://192.168.0.103:3006/userLogin";
                //string url = host + port + "/userLogin";
                using (var webClient = new WebClient())
                {
                    var pars = new NameValueCollection();
                    webClient.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                    pars.Add("format", "json");
                    pars.Add("Login", loginText.Text);
                    pars.Add("Password", passwordText.Text);
                    byte[] responsebytes = webClient.UploadValues(url, pars);
                    string responsebody = Encoding.UTF8.GetString(responsebytes);
                    if (responsebody == "")
                    {
                        AlertDialog.Builder alert = new AlertDialog.Builder(this);
                        alert.SetTitle("Ошибка");
                        alert.SetMessage("Неверный логин или пароль.");
                        alert.SetNegativeButton("ОК", (senderAlert, args) =>
                        {
                            return;
                        });
                        Dialog dialog = alert.Create();
                        dialog.Show();
                        return;
                    }
                    else editor.PutString("ID_User", responsebody);

                   
                } 
                editor.PutString("login", loginText.Text);
                editor.PutString("password", passwordText.Text);
                editor.PutBoolean("SavePassCheck", checkBox.Checked);
                editor.Apply();    


               Intent intent = new Intent(this, typeof(MainActivity));
                StartActivity(intent);
            }
            catch (Exception ex)
            {
                AlertDialog.Builder alert = new AlertDialog.Builder(this);
                alert.SetTitle("Ошибка");
                alert.SetMessage(ex.Message);
                alert.SetNegativeButton("ОК", (senderAlert, args) => {
                    return;
                });
                Dialog dialog = alert.Create();
                dialog.Show();
                return;
            }
        }
    }
}