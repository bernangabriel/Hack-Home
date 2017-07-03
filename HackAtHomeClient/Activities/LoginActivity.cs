using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using HackAtHome.Entities;
using HackAtHome.SAL;

namespace HackAtHomeClient.Activities
{
    [Activity(Label = "@string/ApplicationName", MainLauncher = true, Icon = "@drawable/Icon")]
    public class LoginActivity : Activity
    {
        MicrosoftServiceClient microsoftServiceClient = null;

        EditText txtEmail = null;
        EditText txtPassword = null;
        Button btnValidate = null;

        public LoginActivity()
        {
            microsoftServiceClient = new MicrosoftServiceClient();
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Login);

            //Initialize controls
            txtEmail = FindViewById<EditText>(Resource.Id.Correo);
            txtPassword = FindViewById<EditText>(Resource.Id.Contrasena);
            btnValidate = FindViewById<Button>(Resource.Id.btnValidar);

            //Set event handlers
            btnValidate.Click += BtnValidate_Click;
        }


        /// <summary>
        /// On Validate event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void BtnValidate_Click(object sender, EventArgs e)
        {
            string email = txtEmail.Text;
            string password = txtPassword.Text;

            if (!string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(password))
            {
                //show loading indicator
                ProgressDialog progress = new ProgressDialog(this);
                progress.Indeterminate = true;
                progress.SetProgressStyle(ProgressDialogStyle.Spinner);
                progress.SetMessage(Resources.GetString(Resource.String.Cargando));
                progress.SetCancelable(false);
                progress.Show();

                ResultInfo resultInfo =
                     await microsoftServiceClient.AutenticateAsync(email, password);

                if (resultInfo.Status == Status.Success)
                {
                    progress.Dismiss();

                    Intent intent = new Intent(this, typeof(MainActivity));
                    intent.PutExtra("fullName", resultInfo.FullName);
                    intent.PutExtra("token", resultInfo.Token);

                    StartActivity(intent);
                }
                else
                {
                    progress.Dismiss();
                    AlertDialog.Builder builder = new AlertDialog.Builder(this);
                    AlertDialog alert = builder.Create();
                    alert.SetTitle(Resources.GetString(Resource.String.DatosIncorrectosTitle));
                    alert.SetMessage(Resources.GetString(Resource.String.DatosIncorrectosMessage));
                    alert.SetButton("Ok", (c, ev) => { });
                    alert.Show();

                    txtPassword.Text = string.Empty;
                }
            }
        }
    }
}