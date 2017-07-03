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
using Newtonsoft.Json;
using HackAtHome.Entities;
using HackAtHome.SAL;
using Android.Webkit;
using Android.Graphics;

namespace HackAtHomeClient.Activities
{
    [Activity(Label = "@string/ApplicationName")]
    public class EvidenceDetailsActivity : Activity
    {
        MicrosoftServiceClient microsoftServiceClient = null;
        public EvidenceDetailsActivity()
        {
            microsoftServiceClient = new MicrosoftServiceClient();
        }

        TextView fullName = null;
        TextView evidenceTitle = null;
        TextView evidenceStatus = null;
        WebView evidenceDescription = null;
        ImageView evidenceImage = null;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.EvidenceItemDetails);

            ActionBar.SetHomeButtonEnabled(true);
            ActionBar.SetDisplayHomeAsUpEnabled(true);

            //Initialize controls
            fullName = FindViewById<TextView>(Resource.Id.fullName);
            evidenceTitle = FindViewById<TextView>(Resource.Id.evidenceTitle);
            evidenceStatus = FindViewById<TextView>(Resource.Id.evidenceStatus);
            evidenceDescription = FindViewById<WebView>(Resource.Id.evidenceDescription);
            evidenceImage = FindViewById<ImageView>(Resource.Id.evidenceImage);

            evidenceImage.SetBackgroundColor(Android.Graphics.Color.Transparent);

            //load evidence details
            LoadEvidenceDetails();
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    Finish();
                    return true;

                default:
                    return base.OnOptionsItemSelected(item);
            }
        }

        /// <summary>
        /// Load Evidence Details
        /// </summary>
        private async void LoadEvidenceDetails()
        {
            string tokenValue = MainActivity.tokenValue;
            string fullNameValue = MainActivity.fullNameValue;
            Evidence evidence = JsonConvert.DeserializeObject<Evidence>(
                this.Intent.GetStringExtra("evidence"));

            if (evidence != null && !string.IsNullOrEmpty(tokenValue))
            {
                //show loading indicator
                ProgressDialog progress = new ProgressDialog(this);
                progress.Indeterminate = true;
                progress.SetProgressStyle(ProgressDialogStyle.Spinner);
                progress.SetMessage(Resources.GetString(Resource.String.Cargando));
                progress.SetCancelable(false);
                progress.Show();

                EvidenceDetail evidenceDetails =
                    await microsoftServiceClient.GetEvidenceByIDAsync(tokenValue, evidence.EvidenceID.ToString());

                if (evidenceDetails != null)
                {
                    fullName.Text = fullNameValue;
                    evidenceTitle.Text = evidence.Title;
                    evidenceStatus.Text = evidence.Status;

                    evidenceDescription.LoadDataWithBaseURL(null, $"<font color=\"white\">{evidenceDetails.Description.Replace("<br/><br/>", "")}</font>", "text/html", "uft-8", null);
                    evidenceDescription.SetBackgroundColor(Color.Transparent);
                    Koush.UrlImageViewHelper.SetUrlDrawable(evidenceImage, evidenceDetails.Url);

                    progress.Dismiss();
                }
                else
                {
                    progress.Dismiss();
                }
            }
        }
    }
}