﻿using System;
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

namespace SWDApp
{
    [Activity(Label = "Activity1")]
    public class CriterionSelectActivity : Activity
    {
        AHP ahp;

        SeekBar seekBar1;
        TextView criterionText;
        TextView criterionTextValue;
        TextView textView1;

        RadioGroup radioGroup;
        RadioButton checkedCriterion;
        RadioButton criterion1;
        RadioButton criterion2;
        RadioButton criterionEqual;

        Button next;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            //test if all criterions selected
            SetContentView(Resource.Layout.CriterionSelect);
            //else go next
            //StartActivity(typeof(DecisionSelectActivity));



            //get data from previous activity, deserializing it to object
            ahp = MyApp.ahp;

            seekBar1 = FindViewById<SeekBar>(Resource.Id.seekBar1);
            criterionText = FindViewById<TextView>(Resource.Id.criterionText);
            criterionTextValue = FindViewById<TextView>(Resource.Id.criterionTextValue);
            textView1 = FindViewById<TextView>(Resource.Id.textView1);

            radioGroup = FindViewById<RadioGroup>(Resource.Id.criterionGroup);
            checkedCriterion = FindViewById<RadioButton>(radioGroup.CheckedRadioButtonId);
            criterion1 = FindViewById<RadioButton>(Resource.Id.criterion1);
            criterion2 = FindViewById<RadioButton>(Resource.Id.criterion2);
            criterionEqual = FindViewById<RadioButton>(Resource.Id.criterionEqual);

            next = FindViewById<Button>(Resource.Id.next);

            seekBar1.Enabled = false;

            criterion1.Text = string.Format("{0} {1}", ahp.firstCriterion(), ahp.getC());
            criterionEqual.Text = "Jednakowa ważność obu kryteriów";
            criterion2.Text = ahp.secondCriterion();


            textView1.Text = string.Format(
    "{0:0.00}   {1:0.00}   {2:0.00}   {3:0.00}\n" +
    "{4:0.00}   {5:0.00}   {6:0.00}   {7:0.00}\n" +
    "{8:0.00}   {9:0.00}   {10:0.00}   {11:0.00}\n" +
    "{12:0.00}   {13:0.00}   {14:0.00}   {15:0.00}\n",
    ahp.criterionRank(0, 0),
    ahp.criterionRank(0, 1),
    ahp.criterionRank(0, 2),
    ahp.criterionRank(0, 3),
    ahp.criterionRank(1, 0),
    ahp.criterionRank(1, 1),
    ahp.criterionRank(1, 2),
    ahp.criterionRank(1, 3),
    ahp.criterionRank(2, 0),
    ahp.criterionRank(2, 1),
    ahp.criterionRank(2, 2),
    ahp.criterionRank(2, 3),
    ahp.criterionRank(3, 0),
    ahp.criterionRank(3, 1),
    ahp.criterionRank(3, 2),
    ahp.criterionRank(3, 3)

    );


            radioGroup.CheckedChange += (object sender, RadioGroup.CheckedChangeEventArgs e) =>
            {
                if (criterionEqual.Checked)
                {
                    seekBar1.Enabled = false;
                    criterionText.Enabled = false;
                    criterionTextValue.Enabled = false;
                    criterionTextValue.Text = "";
                    criterionText.Text = "";
                }
                else
                {
                    criterionText.Enabled = true;
                    criterionTextValue.Enabled = true;
                    seekBar1.Enabled = true;

                    if (criterion1.Checked)
                        criterionText.Text = string.Format("\"{0}\" względem \"{1}\" jest", ahp.firstCriterion(), ahp.secondCriterion());
                    else
                        criterionText.Text = string.Format("\"{0}\" względem \"{1}\" jest", ahp.secondCriterion(), ahp.firstCriterion());

                    criterionTextAssign(seekBar1.Progress);
                }
            };

            seekBar1.ProgressChanged += (object sender, SeekBar.ProgressChangedEventArgs e) =>
                criterionTextAssign(e.Progress);

            next.Click += (object sender, EventArgs e) =>
            {
                if (criterionEqual.Checked)
                    ahp.saveCriterion(1);
                else
                {
                    switch (seekBar1.Progress)
                    {
                        case 0: //nieznacznie bardziej preferowane 
                            ahp.saveCriterion(criterion1.Checked ? 3 : (decimal)1 / 3);
                            break;
                        case 1: //silniej preferowane
                            ahp.saveCriterion(criterion1.Checked ? 5 : (decimal)1 / 5);
                            break;
                        case 2: //bardzo silnie preferowane
                            ahp.saveCriterion(criterion1.Checked ? 5 : (decimal)1 / 5);
                            break;
                        case 3: //wyłącznie preferowane
                            ahp.saveCriterion(criterion1.Checked ? 9 : (decimal)1 / 9);
                            break;
                        default:
                            break;
                    }
                }

                if (ahp.nextCriterion())
                    StartActivity(typeof(CriterionSelectActivity));
                else
                {
                    textView1.Text = string.Format(
"sum  {0:0.00}   {1:0.00}   {2:0.00}   {3:0.00}\n" +
"avg {4:0.00}   {5:0.00}   {6:0.00}   {7:0.00}\n" +
"ci {8:0.00}   ",

ahp.sum(0), ahp.sum(1), ahp.sum(2), ahp.sum(3),
ahp.avg(0), ahp.avg(1), ahp.avg(2), ahp.avg(3),
ahp.ci

);

                    Toast.MakeText(this, "Go to decision comparision matrix", ToastLength.Short).Show();
                    //StartActivity(typeof(CriterionSelectActivity));
                }
            };
        }

        void criterionTextAssign(int progress)
        {
            switch (progress)
            {
                case 0:
                    criterionTextValue.Text = string.Format("nieznacznie bardziej preferowane {0} 0", progress);
                    break;
                case 1:
                    criterionTextValue.Text = string.Format("silniej preferowane {0} 1", progress);
                    break;
                case 2:
                    criterionTextValue.Text = string.Format("bardzo silnie preferowane {0} 2", progress);
                    break;
                case 3:
                    criterionTextValue.Text = string.Format("wyłącznie preferowane {0} 3", progress);
                    break;
                default:
                    criterionTextValue.Text = "";
                    break;
            }
        }
    }
}