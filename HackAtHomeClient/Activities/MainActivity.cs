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
using HackAtHome.CustomAdapters;
using Android.Util;

namespace HackAtHomeClient.Activities
{
    [Activity(Label = "@string/ApplicationName")]
    public class MainActivity : Activity
    {
        public static string fullNameValue = string.Empty;
        public static string tokenValue = string.Empty;
        public static List<Evidence> EvidenceListValue = null;

        MicrosoftServiceClient microsoftServiceClient = null;
        public MainActivity()
        {
            microsoftServiceClient = new MicrosoftServiceClient();
        }

        TextView fullName = null;
        ListView lvEvidences = null;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Main);

            //Initialize all controls
            fullName = FindViewById<TextView>(Resource.Id.fullName);
            lvEvidences = FindViewById<ListView>(Resource.Id.lvEvidences);

            if (bundle != null)
            {
                EvidenceListValue = JsonConvert
                    .DeserializeObject<List<Evidence>>(bundle.GetString("EvidencesData"));

                if (EvidenceListValue?.Count > 0)
                {
                    LoadAllEvidences(EvidenceListValue);
                }
            }
            else
            {
                LoadAllEvidences(null);
            }

            //Set event handlers
            lvEvidences.ItemClick += LvEvidences_ItemClick;
        }

        protected override void OnSaveInstanceState(Bundle outState)
        {
            if (EvidenceListValue?.Count > 0)
            {
                outState.PutString("EvidencesData", JsonConvert.SerializeObject(EvidenceListValue));
            }
            base.OnSaveInstanceState(outState);
        }


        private void LvEvidences_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            int evidenceID = e.Position;
            Intent intent = new Intent(this, typeof(EvidenceDetailsActivity));
            intent.PutExtra("evidenceID", evidenceID);
            intent.PutExtra("evidence", JsonConvert.SerializeObject(EvidenceListValue[evidenceID]));
            intent.AddFlags(ActivityFlags.ClearTop);

            StartActivity(intent);
        }

        /// <summary>
        /// Load all evidences
        /// </summary>
        private async void LoadAllEvidences(List<Evidence> SavedEvidences)
        {
            string token = this.Intent.GetStringExtra("token");
            string userFullName = this.Intent.GetStringExtra("fullName");

            if (SavedEvidences == null)
            {
                if (!string.IsNullOrEmpty(token))
                {
                    //show loading indicator
                    ProgressDialog progress = new ProgressDialog(this);
                    progress.Indeterminate = true;
                    progress.SetProgressStyle(ProgressDialogStyle.Spinner);
                    progress.SetMessage(Resources.GetString(Resource.String.Cargando));
                    progress.SetCancelable(false);
                    progress.Show();

                    List<Evidence> Evidences =
                        await microsoftServiceClient.GetEvidencesAsync(token);

                    if (Evidences?.Count > 0)
                    {
                        fullName.Text = userFullName;

                        //set static values
                        EvidenceListValue = Evidences;
                        fullNameValue = userFullName;
                        tokenValue = token;

                        //fill ListView
                        lvEvidences.Adapter = new EvidencesAdapter(
                            this,
                            Evidences,
                            Resource.Layout.EvidenceItem,
                            Resource.Id.evidenceTitle,
                            Resource.Id.evidenceStatus);

                        progress.Dismiss();
                    }
                    else
                    {
                        progress.Dismiss();
                    }
                }
            }
            else
            {
                fullName.Text = userFullName;

                //fill ListView
                lvEvidences.Adapter = new EvidencesAdapter(
                    this,
                    SavedEvidences,
                    Resource.Layout.EvidenceItem,
                    Resource.Id.evidenceTitle,
                    Resource.Id.evidenceStatus);
            }
        }
    }
}