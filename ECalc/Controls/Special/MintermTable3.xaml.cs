﻿using ECalc.Classes;
using ECalc.Engineering;
using System.Collections.Generic;
using System.Windows.Controls;

namespace ECalc.Controls.Special
{
    /// <summary>
    /// Interaction logic for MintermTable3.xaml
    /// </summary>
    public partial class MintermTable3 : UserControl, IMintermTable
    {
        public MintermTable3()
        {
            InitializeComponent();
        }

        public List<LogicItem> Selected
        {
            get
            {
                List<LogicItem> ret = new List<LogicItem>();
                foreach (var item in Functions.GetMintermTableValues(Minterm3x))
                {
                    ret.Add(new LogicItem() { Index = item.Key, Checked = item.Value, BinaryValue = LogicItem.GetBinaryValue(item.Key, 3) });
                }
                return ret;
            }
            set { Functions.SetMintermTableValues(Minterm3x, value); }
        }


        public void ClearInput()
        {
            Functions.ClearMintermtable(Minterm3x);
        }

        public void SwapVarnames()
        {
            foreach (var tb in Helpers.FindChildren<TextBlock>(this))
            {
                switch (tb.Text)
                {
                    case "A":
                        tb.Text = "C";
                        break;
                    case "B":
                        tb.Text = "B";
                        break;
                    case "C":
                        tb.Text = "A";
                        break;
                }
            }
        }
    }
}
