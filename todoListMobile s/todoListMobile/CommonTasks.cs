using Android.App;
using Android.Content;
using Android.OS;
using Android.Preferences;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;
using TodoList;

namespace todoListMobile
{[Activity(Label ="Общие задачи")]

    class CommonTasks : Activity
    {
        List<Tasks> tasks = new List<Tasks>();
        ListView listView;
        ImageButton addTaskButton_Common;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.CommonTasks);
            listView = FindViewById<ListView>(Resource.Id.tasksListViewCommon);
            addTaskButton_Common = FindViewById<ImageButton>(Resource.Id.addTaskButton_Common);
            Android.Widget.Toolbar toolbarCommonTasks = FindViewById<Android.Widget.Toolbar>(Resource.Id.toolbarCommonTasks);
            ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(this);
            string date = prefs.GetString("CurrentDate", "");

            toolbarCommonTasks.InflateMenu(Resource.Menu.myMenu_CommonTasks);

            toolbarCommonTasks.MenuItemClick += (sender, e) => {
                string title = e.Item.TitleFormatted.ToString();
                if (title == "Обновить")
                {

                    try
                    {
                        int count = tasks.Count;
                        while (tasks.Count != 0)
                        {
                            tasks.Remove(tasks[count - 1]);
                            count--;
                        }

                        string url = "http://" + prefs.GetString("addres", "") + ":" + prefs.GetString("port", "") + "/CommonTasks";
                        using (var webClient = new WebClient())
                        {
                            var pars = new NameValueCollection();
                            webClient.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                            pars.Add("format", "json");
                            pars.Add("User", prefs.GetInt("ID_User", 0).ToString());
                            byte[] responsebytes = webClient.UploadValues(url, pars);
                            string responsebody = Encoding.UTF8.GetString(responsebytes);

                            var objResponse = JsonConvert.DeserializeObject<List<Tasks>>(responsebody);
                            for (int i = 0; i < objResponse.Count; i++)
                            {
                                var a = objResponse[i];
                                var id = a.ID_Task;
                                var task = a.Description;
                                var status = a.DealStatus;

                                tasks.Add(new Tasks()
                                {
                                    ID_Task = a.ID_Task,
                                    User = a.User,
                                    NumberOfRows = i + 1,
                                    Description = a.Description,
                                    DealStatus = status,
                                });
                            }
                        }
                    }
                    catch (System.Exception ex)
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

                    Toast.MakeText(this, "Обновлено", ToastLength.Long).Show();

                    TaskAdapter adapter = new TaskAdapter(this, tasks);
                    listView.Adapter = adapter;
                }

            };

            try
            {
                //var url = " http://192.168.0.103:3006/CommonTasks";
                string url = "http://" + prefs.GetString("addres", "") + ":" + prefs.GetString("port", "") + "/CommonTasks";
                //var url = host + port + "/CommonTasks";
                using (var webClient = new WebClient())
                {
                    var pars = new NameValueCollection();
                    webClient.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                    pars.Add("format", "json");
                    pars.Add("User", prefs.GetInt("ID_User", 0).ToString());
                    byte[] responsebytes = webClient.UploadValues(url, pars);
                    string responsebody = Encoding.UTF8.GetString(responsebytes);

                    var objResponse = JsonConvert.DeserializeObject<List<Tasks>>(responsebody);
                    for (int i = 0; i < objResponse.Count; i++)
                    {
                        var a = objResponse[i];
                        var id = a.ID_Task;
                        var task = a.Description;
                        var status = a.DealStatus;
                        var user = prefs.GetInt("ID_User", 0);

                        tasks.Add(new Tasks()
                        {
                            ID_Task = a.ID_Task,
                            User = user,
                            NumberOfRows = i + 1,
                            Description = a.Description,
                            DealStatus = status,

                        });
                    }
                }
            }
            catch (System.Exception ex)
            {
                AlertDialog.Builder alert = new AlertDialog.Builder(this, AlertDialog.ThemeHoloDark);
                alert.SetTitle("Ошибка");
                alert.SetMessage(ex.Message);
                alert.SetNegativeButton("ОК", (senderAlert, args) =>
                {
                    return;
                });
                Dialog dialog = alert.Create();
                dialog.Show();
                return;
            }

            listView.ItemLongClick += ListView_ItemLongClick;
            addTaskButton_Common.Click += addTaskButton_Common_Click;
            TaskAdapter_Common adapter = new TaskAdapter_Common(this, tasks);
            listView.Adapter = adapter;
        }

        private void ListView_ItemLongClick(object sender, AdapterView.ItemLongClickEventArgs e)
        {
            ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(this);
            string Currentdate = prefs.GetString("CurrentDate", "");
            string selectedID_Task = tasks[e.Position].ID_Task.ToString();
            string selectedUser = tasks[e.Position].User.ToString();
            DateTime time;
            AlertDialog.Builder alertMain = new AlertDialog.Builder(this, AlertDialog.ThemeDeviceDefaultDark);
            alertMain.SetTitle("Выберите действие");
            alertMain.SetPositiveButton("Редактировать", (senderAlert, args) => {

               

                LayoutInflater inflater = LayoutInflater.From(this);
                View subView = inflater.Inflate(Resource.Layout.AddCommonTaskDialog, null);
                EditText descriptionTaskText = (EditText)subView.FindViewById(Resource.Id.addTaskEditText_Common);
                descriptionTaskText.Text = tasks[e.Position].Description;

                AlertDialog.Builder builder = new AlertDialog.Builder(this, AlertDialog.ThemeHoloDark);
                builder.SetView(subView);
                builder.SetPositiveButton("ОК", (senderAlert, args) =>
                {
                    if (string.IsNullOrEmpty(descriptionTaskText.Text))
                    {
                        AlertDialog.Builder alertError = new AlertDialog.Builder(this);
                        alertError.SetTitle("Ошибка");
                        alertError.SetMessage("Введите описание задачи");
                        alertError.SetNegativeButton("ОК", (senderAlert, args) =>
                        {
                            return;
                        });
                        Dialog dialogError = alertError.Create();
                        dialogError.Show();
                        return;
                    }

                    try
                    {
                        string url = "http://" + prefs.GetString("addres", "") + ":" + prefs.GetString("port", "") + "/CommonTaskUpdate";
                        using (var webClient = new WebClient())
                        {
                            var pars = new NameValueCollection();
                            webClient.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                            pars.Add("format", "json");

                            pars.Add("User", tasks[e.Position].User.ToString());
                            pars.Add("Description", descriptionTaskText.Text);
                            pars.Add("ID_Task", tasks[e.Position].ID_Task.ToString());
                            if (tasks[e.Position].DealStatus == 1)
                            {
                                pars.Add("DealStatus", "1");
                            }
                            else if (tasks[e.Position].DealStatus == 2)
                            {
                                pars.Add("DealStatus", "2");
                            }

                            var elem = tasks.FirstOrDefault(x => x.ID_Task == tasks[e.Position].ID_Task);
                            if (elem != null)
                            {
                                elem.User = tasks[e.Position].User;
                                elem.NumberOfRows = tasks[e.Position].NumberOfRows;
                                elem.Description = descriptionTaskText.Text;
                                elem.DealStatus = tasks[e.Position].DealStatus;
                            }
                            byte[] responsebytes = webClient.UploadValues(url, pars);
                            string responsebody = Encoding.UTF8.GetString(responsebytes);
                        }
                        Toast.MakeText(this, "Задача успешно изменена", ToastLength.Long).Show();
                        listView.Adapter = new TaskAdapter(this, tasks);
                    }
                    catch (System.Exception ex)
                    {
                        Toast.MakeText(this, "Произошла ошибка " + ex.Message, ToastLength.Long).Show();
                    }
                });
                Dialog dialogEdit = builder.Create();
                dialogEdit.Show();
            });
            alertMain.SetNegativeButton("Удалить", (senderAlert, args) =>
            {
                AlertDialog.Builder alertConfirm = new AlertDialog.Builder(this, AlertDialog.ThemeDeviceDefaultDark);
                alertConfirm.SetTitle("Подтверждение");
                alertConfirm.SetMessage($"Вы уверены, что хотите удалить задачу \"{tasks[e.Position].Description}\" ");
                alertConfirm.SetPositiveButton("Да", (senderAlert, args) => {

                    try
                    {
                        //var url = host + port + "/DailyTaskDelete";
                        string url = "http://" + prefs.GetString("addres", "") + ":" + prefs.GetString("port", "") + "/CommonTaskDelete";
                        using (var webClient = new WebClient())
                        {
                            var pars = new NameValueCollection();
                            webClient.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                            pars.Add("format", "json");
                            pars.Add("ID_Task", tasks[e.Position].ID_Task.ToString());
                            byte[] responsebytes = webClient.UploadValues(url, pars);
                            string responsebody = Encoding.UTF8.GetString(responsebytes);
                        }
                        Toast.MakeText(this, "Задача успешно удалена.", ToastLength.Long).Show();
                        tasks.Remove(tasks[e.Position]);

                        for (int i = 0; i < tasks.Count; i++)
                        {
                            tasks[i].NumberOfRows = i + 1;
                        }
                        listView.Adapter = new TaskAdapter(this, tasks);

                    }
                    catch (System.Exception ex)
                    {
                        Toast.MakeText(this, "Произошла ошибка " + ex.Message, ToastLength.Long).Show();
                    }
                });
                alertConfirm.SetNegativeButton("Нет", (senderAlert, args) =>
                {
                    return;
                });
                Dialog dialogConfirm = alertConfirm.Create();
                dialogConfirm.Show();
            });
            Dialog dialog = alertMain.Create();
            dialog.Show();
        }

        private void addTaskButton_Common_Click(object sender, EventArgs e)
        {
            ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(this);
            string Currentdate = prefs.GetString("CurrentDate", "");
            int ID_User = prefs.GetInt("ID_User", 0);

            LayoutInflater inflater = LayoutInflater.From(this);
            View subView = inflater.Inflate(Resource.Layout.AddCommonTaskDialog, null);
         

            EditText descriptionTaskText = (EditText)subView.FindViewById(Resource.Id.addTaskEditText_Common);

            bool checkedStatus = false;
            string descriptionText = "";
            DateTime time;

            AlertDialog.Builder builder = new AlertDialog.Builder(this, AlertDialog.ThemeHoloDark);
            builder.SetView(subView);
            builder.SetPositiveButton("ОК", (senderAlert, args) =>
            {
                if (string.IsNullOrEmpty(descriptionTaskText.Text))
                {
                    AlertDialog.Builder alert = new AlertDialog.Builder(this);
                    alert.SetTitle("Ошибка");
                    alert.SetMessage("Введите описание задачи");
                    alert.SetNegativeButton("ОК", (senderAlert, args) => {
                        return;
                    });
                    Dialog dialog = alert.Create();
                    dialog.Show();
                    return;
                }
                descriptionText = descriptionTaskText.Text;
                try
                {
                    var url = " http://192.168.0.103:3006/DailyTaskAdd";
                    //var url = host + port + "/DailyTaskAdd";
                    using (var webClient = new WebClient())
                    {
                        var pars = new NameValueCollection();
                        webClient.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                        pars.Add("format", "json");
                        pars.Add("User", ID_User.ToString());
                        pars.Add("DealStatus", "1");
                        pars.Add("Description", descriptionText);

                        byte[] responsebytes = webClient.UploadValues(url, pars);
                        string responsebody = Encoding.UTF8.GetString(responsebytes);
                        Toast.MakeText(this, "Задача успешно добавлена.", ToastLength.Long);

                        tasks.Add(new Tasks()
                        {
                            NumberOfRows = tasks.Count + 1,
                            Description = descriptionText,
                            DealStatus = 1,


                        });
                        listView.Adapter = new TaskAdapter(this, tasks);

                    }
                }
                catch (System.Exception ex)
                {
                    AlertDialog.Builder alert = new AlertDialog.Builder(this);
                    alert.SetTitle("Ошибка");
                    alert.SetMessage(ex.Message);
                    alert.SetNegativeButton("ОК", (senderAlert, args) => {
                        return;
                    });
                    Dialog dialog = alert.Create();
                    dialog.Show();
                }
            });
            AlertDialog alertDialog = builder.Create();
            alertDialog.Show();

        }

        private void listview_ItemLongClick(object sender, AdapterView.ItemLongClickEventArgs e)
        {
           
            
        }
    }

    public class TaskAdapter_Common : BaseAdapter<Tasks>
    {
        List<Tasks> items;
        Activity context;
        public TaskAdapter_Common(Activity context, List<Tasks> items)
            : base()
        {
            this.context = context;
            this.items = items;
        }
        public override long GetItemId(int position)
        {
            return position;
        }
        public override Tasks this[int position]
        {
            get { return items[position]; }
        }
        public override int Count
        {
            get { return items.Count; }
        }
 
        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var item = items[position];
            View view = convertView;
            if (view == null) // no view to re-use, create new
                view = context.LayoutInflater.Inflate(Resource.Layout._itemsCommonTask, null);
            view.FindViewById<TextView>(Resource.Id.nuberTaskText_Common).Text = item.NumberOfRows.ToString();
            view.FindViewById<TextView>(Resource.Id.descriptionTaskText_Common).Text = item.Description;
           /* if (item.DateDeadline != null)
            {
                view.FindViewById<TextView>(Resource.Id.).Text = item.DateDeadline.ToString();
            }*/
            if (item.DealStatus == 2)
            {
                view.FindViewById<CheckBox>(Resource.Id.dealStatusCheckBox_Common).Checked = true;
            }
            if (item.DealStatus == 1)
            {
                view.FindViewById<CheckBox>(Resource.Id.dealStatusCheckBox_Common).Checked = false;
            }

            CheckBox checkBox = view.FindViewById<CheckBox>(Resource.Id.dealStatusCheckBox_Common);
            ListView list = view.FindViewById<ListView>(Resource.Id.tasksListViewCommon);

            checkBox.CheckedChange += (s, e) =>
            {

               
            };

            return view;
        }

    }
}