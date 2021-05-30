using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;

namespace todoListMobile
{
    [Activity(Label = "New activity")]
    class Register: MainActivity
    {
        Button returnButton;
        protected override void OnCreate(Bundle savedInstanceState)

        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.RegistrationForms);

            EditText loginText = FindViewById<EditText>(Resource.Id.loginRText);
            EditText passText = FindViewById<EditText>(Resource.Id.passwordRText);
            EditText repeatPassText = FindViewById<EditText>(Resource.Id.passwordRepeatText);
            returnButton = FindViewById<Button>(Resource.Id.returnButton);
            Button registrationButton = FindViewById<Button>(Resource.Id.registrationRButton);
            registrationButton.Click += registrationButton_Click;
            returnButton.Click += returnButton_Click;
        }

        private void returnButton_Click(object sender, EventArgs e)
        {
            Intent intent = new Intent(this, typeof(login));
            StartActivity(intent);
        }

        private void registrationButton_Click(object sender, EventArgs e)
        {
            EditText loginText = FindViewById<EditText>(Resource.Id.loginRText);
            EditText passText = FindViewById<EditText>(Resource.Id.passwordRText);
            EditText repeatPassText = FindViewById<EditText>(Resource.Id.passwordRepeatText);

            StringBuilder errors = new StringBuilder();
            if (string.IsNullOrWhiteSpace(loginText.Text)) errors.AppendLine("Придумайте логин.");
            if (string.IsNullOrWhiteSpace(passText.Text)) errors.AppendLine("Придумайте пароль.");
            if(string.IsNullOrWhiteSpace(repeatPassText.Text)) errors.AppendLine("Повторите пароль");
            if (errors.Length > 0)
            {
                // MessageBox.Show(errors.ToString(), "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);

                AlertDialog.Builder alert = new AlertDialog.Builder(this);
                alert.SetTitle("Ошибка");
                alert.SetMessage(errors.ToString());
                alert.SetNegativeButton("ОК", (senderAlert, args) => {
                    return;
                });
                Dialog dialog = alert.Create();
                dialog.Show();
                return;
            }

            if(passText.Text != repeatPassText.Text)
            {
                AlertDialog.Builder alert = new AlertDialog.Builder(this);
                alert.SetTitle("Ошибка");
                alert.SetMessage("Введенные пароли не совпадают");
                alert.SetNegativeButton("ОК", (senderAlert, args) => {
                    return;
                });
                Dialog dialog = alert.Create();
                dialog.Show();
                return;
            }


            try
            {
                string url = "http://192.168.0.103:3006/userCheck";
                //string url = host + port + "/userCheck";

                using (var webClient = new WebClient())
                {
                    var pars = new NameValueCollection();
                    webClient.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                    pars.Add("format", "json");
                    pars.Add("Login", loginText.Text);
                    byte[] responsebytes = webClient.UploadValues(url, pars);
                    string responsebody = Encoding.UTF8.GetString(responsebytes);
                    if (responsebody != "[]")
                    {
                        AlertDialog.Builder alert = new AlertDialog.Builder(this);
                        alert.SetTitle("Ошибка");
                        alert.SetMessage("Пользователь с таким логином уже существует.");
                        alert.SetNegativeButton("ОК", (senderAlert, args) => {
                            return;
                        });
                        Dialog dialog = alert.Create();
                        dialog.Show();
                        return;
                    }

                 using (var Webclient2 = new WebClient())
                 {
                        string url2 = "http://192.168.0.103:3006/UserAdd";
                        var pars2 = new NameValueCollection();
                        Webclient2.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                        pars2.Add("format", "json");
                        pars2.Add("Login", loginText.Text);
                        pars2.Add("Password", passText.Text);
                        byte[] responsebytes2 = Webclient2.UploadValues(url2, pars2);
                        string responsebody2 = Encoding.UTF8.GetString(responsebytes2);

                        AlertDialog.Builder alert = new AlertDialog.Builder(this);
                        alert.SetTitle("Уведомление");
                        alert.SetMessage("Пользователь успешно создан.");
                        alert.SetNegativeButton("ОК", (senderAlert, args) => {
                            return;
                        });
                        Dialog dialog = alert.Create();
                        dialog.Show();
                        return;
                    }
                }

            }
            catch(Exception ex)
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