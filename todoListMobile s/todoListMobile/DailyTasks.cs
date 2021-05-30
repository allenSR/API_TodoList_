using Android.App;
using Android.Content;
using Android.OS;
using Android.Preferences;
using Android.Runtime;
using Android.Support.V7.RecyclerView.Extensions;
using Android.Views;
using Android.Widget;
using Java.Lang;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using TodoList;
using static Android.Widget.AdapterView;

namespace todoListMobile
{
    
    [Activity(Label = "New activity")]
    public class DailyTask : Activity
    {
        ListView listView;
        TextView textViewDate;
        ImageButton addTaskButton;
        List<Tasks> tasks = new List<Tasks>();


        protected override void OnCreate(Bundle savedInstanceState)

        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.DailyTask);
            listView = FindViewById<ListView>(Resource.Id.tasksListView);
            textViewDate = FindViewById<TextView>(Resource.Id.textViewDate);
            addTaskButton = FindViewById<ImageButton>(Resource.Id.addTaskButton);
            Android.Widget.Toolbar toolbar = FindViewById<Android.Widget.Toolbar>(Resource.Id.toolbar1);
            


            ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(this);
            string date = prefs.GetString("CurrentDate", "");

            toolbar.InflateMenu(Resource.Menu.myMenu);

            toolbar.MenuItemClick += (sender, e) => {
                string title = e.Item.TitleFormatted.ToString();
               if(title == "Обновить" )
                {
                    try
                    {
                        int count = tasks.Count;
                        while (tasks.Count != 0)
                        {
                            tasks.Remove(tasks[count - 1]);
                            count--;
                        }
                        //var url = host + port + "/TasksDay";
                        var url = "http://192.168.0.103:3006/TasksDay";
                        using (var webClient = new WebClient())
                        {
                            var pars = new NameValueCollection();
                            webClient.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                            pars.Add("format", "json");
                            pars.Add("User", prefs.GetInt("ID_User", 0).ToString());
                            pars.Add("Date", date);
                            byte[] responsebytes = webClient.UploadValues(url, pars);
                            string responsebody = Encoding.UTF8.GetString(responsebytes);

                            string time;
                            if (responsebody == "[]")
                            {
                                //Нет задач
                            }
                            var objResponse = JsonConvert.DeserializeObject<List<Tasks>>(responsebody);
                            for (int i = 0; i < objResponse.Count; i++)
                            {
                                var a = objResponse[i];
                                var id = a.ID_Task;
                                var numberOfRows = i + 1;
                                var description = a.Description;
                                DateTime dateCur = (DateTime)a.Date;
                                if (a.DateDeadline.ToString() == "00:00:00")
                                {
                                    time = "---";
                                }
                                else if (a.DateDeadline != null || a.DateDeadline.ToString() != "---")
                                {
                                    DateTime deadline = Convert.ToDateTime(a.DateDeadline);
                                    time = deadline.ToString("t");
                                }
                                else time = "---";
                                var status = a.DealStatus;

                               
                               
                                tasks.Add(new Tasks()
                                {
                                    ID_Task = a.ID_Task,
                                    User = a.User,
                                    NumberOfRows = i + 1,
                                    Description = a.Description,
                                    DateDeadline = time,
                                    DealStatus = status,
                                    Date = dateCur.ToString("O")

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

                        });
                        Dialog dialog = alert.Create();
                        dialog.Show();
                    }

                    Toast.MakeText(this, "Обновлено", ToastLength.Long).Show();
                   
                    TaskAdapter adapter = new TaskAdapter(this, tasks);
                    
                    listView.Adapter = adapter;
                }
               
            };

            addTaskButton.Click += addTaskButton_Button;


           

            DateTime dt = Convert.ToDateTime(date);
            textViewDate.Text += " " + dt.ToString("dddd"); ;


            try
            {
                //var url = host + port + "/TasksDay";
                var url = "http://192.168.0.103:3006/TasksDay";
                using (var webClient = new WebClient())
                {
                    var pars = new NameValueCollection();
                    webClient.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                    pars.Add("format", "json");
                    pars.Add("User", prefs.GetInt("ID_User", 0).ToString());
                    pars.Add("Date", date);
                    byte[] responsebytes = webClient.UploadValues(url, pars);
                    string responsebody = Encoding.UTF8.GetString(responsebytes);

                    string time;
                    if (responsebody == "[]")
                    {
                        //Нет задач
                    }
                    var objResponse = JsonConvert.DeserializeObject<List<Tasks>>(responsebody);
                    for (int i = 0; i < objResponse.Count; i++)
                    {
                        var a = objResponse[i];
                        var id = a.ID_Task;
                        var numberOfRows = i + 1;
                        var description = a.Description;
                        DateTime dateCur = (DateTime)a.Date;
                        if(a.DateDeadline.ToString() == "00:00:00")
                        {
                            time = "---";
                        }
                        else if (a.DateDeadline != null || a.DateDeadline.ToString() != "---")
                        {
                            DateTime deadline = Convert.ToDateTime(a.DateDeadline);
                            time = deadline.ToString("t");
                        }
                        else time = "---";
                        var status = a.DealStatus;

                        tasks.Add(new Tasks()
                        {
                            ID_Task = a.ID_Task,
                            User = a.User,
                            NumberOfRows = i + 1,
                            Description = a.Description,
                            DateDeadline = time,
                            DealStatus = status,
                            Date = dateCur.ToString("O")

                        });

                    }

                }
            }
            catch (System.Exception ex)
            {
                AlertDialog.Builder alert = new AlertDialog.Builder(this, AlertDialog.ThemeHoloDark);
                alert.SetTitle("Ошибка");
                alert.SetMessage(ex.Message);
                alert.SetNegativeButton("ОК", (senderAlert, args) => {

                });
                Dialog dialog = alert.Create();
                dialog.Show();
            }

            listView.ItemLongClick += listview_ItemLongClick;

            TaskAdapter adapter = new TaskAdapter(this, tasks);
            listView.Adapter = adapter;
        }


        public bool onCreateOptionsMenu(Menu menu)
        {
            MenuInflater inflater = new MenuInflater(this);
            inflater.Inflate(Resource.Id.toolbar1, (IMenu)menu);
            return true;
        }
        public bool OnNavigationItemSelected(IMenuItem item)
        {
           /* switch (item.ItemId)
            {
              case Resource.Id.refreshItem:
                    Toast.MakeText(this, "u click on " + Resource.String.title_refresh, ToastLength.Long).Show();
                    return true;
                case Resource.Id.item2:
                    Toast.MakeText(this, "u click on item 2" , ToastLength.Long).Show();
                    return true;
                case Resource.Id.item3:
                    Toast.MakeText(this, "u click on item 3" , ToastLength.Long).Show();
                    return true;
               
                    return true;
            }*/
            return false;
        }
            private void listview_ItemLongClick(object sender, AdapterView.ItemLongClickEventArgs e)
             {
            string selectedID_Task = tasks[e.Position].ID_Task.ToString();
            string selectedUser = tasks[e.Position].User.ToString();
            DateTime time;
            AlertDialog.Builder alert = new AlertDialog.Builder(this, AlertDialog.ThemeDeviceDefaultDark);
            alert.SetTitle("Выберите действие");
            // alert.SetMessage(ex.Message);
            alert.SetNegativeButton("Редактировать", (senderAlert, args) => {
                ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(this);
                string Currentdate = prefs.GetString("CurrentDate", "");

                LayoutInflater inflater = LayoutInflater.From(this);
                View subView = inflater.Inflate(Resource.Layout.AddTaskDialog, null);
                EditText descriptionTaskText = (EditText)subView.FindViewById(Resource.Id.addTaskEditText);
                CheckBox deadlineCheckBox = (CheckBox)subView.FindViewById(Resource.Id.deadlineCheckbox);
                TimePicker deadlineTimePicker = (TimePicker)subView.FindViewById(Resource.Id.deadlineTimePicker);
                deadlineTimePicker.SetIs24HourView((Java.Lang.Boolean)true);


                if (tasks[e.Position].DateDeadline.ToString() != "---")
                {
                    deadlineCheckBox.Checked = true;
                    time = Convert.ToDateTime(tasks[e.Position].DateDeadline);
                    DateTime hours = Convert.ToDateTime(tasks[e.Position].DateDeadline);

                    /// Нужно установить время в пикере из выбранной задачи
                    // deadlineTimePicker.Hour = 
                    //deadlineTimePicker.Minute = time;
                    // deadlineTimePicker.SetCurrentHour(new Integer(10));
                }
                if (tasks[e.Position].DateDeadline == null)
                {
                    deadlineCheckBox.Checked = false;
                }
                descriptionTaskText.Text = tasks[e.Position].Description;


                AlertDialog.Builder builder = new AlertDialog.Builder(this, AlertDialog.ThemeHoloDark);
                // builder.SetTitle("AlertDialog");
                //builder.SetMessage("AlertDialog Message");
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
                        //var url = host + port + "/DailyTaskUpdate";
                        var url = "http://192.168.0.103:3006/DailyTaskUpdate";
                        using (var webClient = new WebClient())
                        {
                            string endDate = Convert.ToDateTime(tasks[e.Position].Date).ToString("O");
                            var pars = new NameValueCollection();
                            webClient.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                            pars.Add("format", "json");

                            pars.Add("User", tasks[e.Position].User.ToString());
                            pars.Add("Date", endDate);
                            pars.Add("Description", descriptionTaskText.Text);
                            pars.Add("ID_Task", tasks[e.Position].ID_Task.ToString());
                            if (deadlineCheckBox.Checked == true)
                            {
                                string hour = (string)deadlineTimePicker.CurrentHour;
                                string minute = (string)deadlineTimePicker.CurrentMinute;
                                time = Convert.ToDateTime(hour + ":" + minute + ":00");
                                pars.Add("DateDeadline", time.ToString("t"));
                            }
                            else if (deadlineCheckBox.Checked == false)
                            {
                                pars.Add("DateDeadline", null);
                            }

                            if (tasks[e.Position].DealStatus == 1)
                            {
                                pars.Add("DealStatus", "1");
                            }
                            else if (tasks[e.Position].DealStatus == 2)
                            {
                                pars.Add("DealStatus", "2");
                            }

                            DateTime dt;
                            string hour1 = (string)deadlineTimePicker.CurrentHour;
                            string minute1 = (string)deadlineTimePicker.CurrentMinute;
                            time = Convert.ToDateTime(hour1 + ":" + minute1 + ":00");

                            var elem = tasks.FirstOrDefault(x => x.ID_Task == tasks[e.Position].ID_Task);
                            if (elem != null)
                            {
                                elem.User = tasks[e.Position].User;
                                elem.NumberOfRows = tasks[e.Position].NumberOfRows;
                                elem.Description = descriptionTaskText.Text;
                                elem.DealStatus = tasks[e.Position].DealStatus;
                                if (deadlineCheckBox.Checked == false)
                                {
                                    elem.DateDeadline = "---";
                                }
                                else if (deadlineCheckBox.Checked == true)
                                    elem.DateDeadline = time.ToString("t");

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
            alert.SetPositiveButton("Удалить", (senderAlert, args) =>
            {
                AlertDialog.Builder alertConfirm = new AlertDialog.Builder(this, AlertDialog.ThemeDeviceDefaultDark);
                alertConfirm.SetTitle("Подтверждение");
                alertConfirm.SetMessage($"Вы уверены, что хотите удалить задачу \"{tasks[e.Position].Description}\" ");
                alertConfirm.SetPositiveButton("Да", (senderAlert, args) => {

                    try
                    {
                        //var url = host + port + "/DailyTaskDelete";
                        var url = "http://192.168.0.103:3006/DailyTaskDelete";
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
            Dialog dialog = alert.Create();
            dialog.Show();
        }


        private void addTaskButton_Button(object sender, EventArgs e)
        {

            listView = FindViewById<ListView>(Resource.Id.tasksListView);

            ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(this);
            string Currentdate = prefs.GetString("CurrentDate", "");
            int ID_User = prefs.GetInt("ID_User", 0);

            LayoutInflater inflater = LayoutInflater.From(this);
            View subView = inflater.Inflate(Resource.Layout.AddTaskDialog, null);
            EditText descriptionTaskText = (EditText)subView.FindViewById(Resource.Id.addTaskEditText);
            CheckBox deadlineCheckBox = (CheckBox)subView.FindViewById(Resource.Id.deadlineCheckbox);
            TimePicker deadlineTimePicker = (TimePicker)subView.FindViewById(Resource.Id.deadlineTimePicker);
            deadlineTimePicker.SetIs24HourView((Java.Lang.Boolean)true);

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
                checkedStatus = deadlineCheckBox.Checked;
                descriptionText = descriptionTaskText.Text;
                string hour = (string)deadlineTimePicker.CurrentHour;
                string minute = (string)deadlineTimePicker.CurrentMinute;
                time = Convert.ToDateTime(hour + ":" + minute + ":00");
                string timeInTasks = "";
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
                        pars.Add("Date", Currentdate);
                        pars.Add("DealStatus", "1");
                        pars.Add("Description", descriptionText);

                        if (checkedStatus == true)
                        {
                            pars.Add("DateDeadline", time.ToString("t"));
                            timeInTasks = time.ToString("t");
                        }
                        else if (checkedStatus == false)
                        {
                            pars.Add("DateDeadline", null);
                            timeInTasks = "---";
                        }
                        byte[] responsebytes = webClient.UploadValues(url, pars);
                        string responsebody = Encoding.UTF8.GetString(responsebytes);
                        Toast.MakeText(this, "Задача успешно добавлена.", ToastLength.Long);

                        tasks.Add(new Tasks()
                        {
                            NumberOfRows = tasks.Count + 1,
                            Description = descriptionText,
                            DateDeadline = timeInTasks,
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
                }            });
            AlertDialog alertDialog = builder.Create();
            alertDialog.Show();

        }

        private void deadlineCheckBox_CheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            LayoutInflater inflater = LayoutInflater.From(this);
            View subView = inflater.Inflate(Resource.Layout.AddTaskDialog, null);
            TimePicker deadlineTimePicker = (TimePicker)subView.FindViewById(Resource.Id.deadlineTimePicker);

            CheckBox deadlineCheckBox = (CheckBox)subView.FindViewById(Resource.Id.deadlineCheckbox);
            if (deadlineCheckBox.Checked == true)
            {
                deadlineTimePicker.Enabled = true;
            }

            if (deadlineCheckBox.Checked == false)
            {
                deadlineTimePicker.Enabled = false;
            }
        }
    }

    public class TaskAdapter : BaseAdapter<Tasks>
    {
        List<Tasks> items;
        Activity context;
        public TaskAdapter(Activity context, List<Tasks> items)
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
        public bool isEnabled(int position)
        {
            return true;
        }
        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var item = items[position];

            View view = convertView;
            if (view == null) // no view to re-use, create new
                view = context.LayoutInflater.Inflate(Resource.Layout._items, null);
            view.FindViewById<TextView>(Resource.Id.nuberTaskText).Text = item.NumberOfRows.ToString();
            view.FindViewById<TextView>(Resource.Id.descriptionTaskText).Text = item.Description;
            if (item.DateDeadline != null)
            {
                view.FindViewById<TextView>(Resource.Id.deadlineText).Text = item.DateDeadline.ToString();
            }
            if (item.DealStatus == 2)
            {
                view.FindViewById<CheckBox>(Resource.Id.dealStatusCheckBox).Checked = true;
            }
            if (item.DealStatus == 1)
            {
                view.FindViewById<CheckBox>(Resource.Id.dealStatusCheckBox).Checked = false;
            }
 
            CheckBox checkBox = view.FindViewById<CheckBox>(Resource.Id.dealStatusCheckBox);
            ListView  list = view.FindViewById<ListView>(Resource.Id.tasksListView);

                checkBox.CheckedChange += (s, e) =>
                {

                    try
                    {
                        //var url = host + port + "/DailyTaskUpdateStatus";
                        var url = "http://192.168.0.103:3006/DailyTaskUpdateStatus";
                        using (var webClient = new WebClient())
                        {
                            var pars = new NameValueCollection();
                            webClient.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                            pars.Add("format", "json");

                            pars.Add("ID_Task", item.ID_Task.ToString());
                            if (checkBox.Checked == true)
                            {
                                pars.Add("DealStatus", "2");
                            }
                            if (checkBox.Checked == false)
                            {
                                pars.Add("DealStatus", "1");
                            }

                            byte[] responsebytes = webClient.UploadValues(url, pars);
                            string responsebody = Encoding.UTF8.GetString(responsebytes);
                        }
                    }
                    catch (System.Exception ex)
                    {
                        Toast.MakeText(context, ex.Message, ToastLength.Long).Show();
                    }
                };     
            
            return view;
        }

    }
}