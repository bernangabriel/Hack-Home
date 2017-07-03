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

namespace HackAtHome.CustomAdapters
{
    public class EvidencesAdapter : BaseAdapter<Evidence>
    {

        List<Evidence> items;
        Activity Context;
        int ItemLayoutTemplate;
        int EvidenceTitleViewID;
        int EvidenceStatusViewID;


        public EvidencesAdapter(Activity context, List<Evidence> evidences,
            int itemLayoutTemplate, int evidenceTitleViewID, int evidenceStatusViewID)
        {
            this.Context = context;
            this.items = evidences;
            this.ItemLayoutTemplate = itemLayoutTemplate;
            this.EvidenceTitleViewID = evidenceTitleViewID;
            this.EvidenceStatusViewID = evidenceStatusViewID;
        }


        public override Evidence this[int position]
        {
            get
            {
                return items[position];
            }
        }

        public override int Count
        {
            get
            {
                return items.Count;
            }
        }

        public override long GetItemId(int position)
        {
            return items[position].EvidenceID;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var item = this.items[position];
            View ItemView = null;

            if (convertView == null)
                ItemView = this.Context.LayoutInflater.Inflate(ItemLayoutTemplate, null);
            else
                ItemView = convertView;

            ItemView.FindViewById<TextView>(EvidenceTitleViewID).Text = item.Title;
            ItemView.FindViewById<TextView>(EvidenceStatusViewID).Text = item.Status;

            return ItemView;
        }
    }
}