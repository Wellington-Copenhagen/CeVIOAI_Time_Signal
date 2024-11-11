using CeVIO.Talk.RemoteService2;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;

namespace CeVIO_AI_時報
{
    public partial class Form1 : Form
    {
        float CPUusage;
        RootLayer rootLayer;
        List<string> Casts;
        static public Dictionary<string, List<string>> EmotionNames;
        static public string WhatIsTalking;
        static public int nextUnitSelectIndex;
        public Form1()
        {
            InitializeComponent();
            CPUusage = 0;
            WhatIsTalking = "";
            nextUnitSelectIndex = 0;
            Form2 form2 = new Form2();
            //TODO CeVIOの方から配列を引いてくる
            ServiceControl2.StartHost(false);
            form2.Close();
            EmotionNames = new Dictionary<string, List<string>>();
            foreach (string castName in Talker2.AvailableCasts)
            {
                EmotionNames[castName] = new List<string>();
                Talker2 talker2 = new Talker2();
                talker2.Cast = castName;
                for (int i = 0; i < talker2.Components.Count; i++)
                {
                    EmotionNames[castName].Add(talker2.Components[i].Name);
                }
            }

            Casts = new List<string>();
            foreach (KeyValuePair<string, List<string>> pair in EmotionNames)
            {
                Casts.Add(pair.Key);
                listBox_Casts.Items.Add(pair.Key);
            }
            rootLayer = new RootLayer();
            rootLayer.Load();
            InitGUI();
        }
        bool ReadFinished = false;

        private void timer1_Tick(object sender, EventArgs e)
        {
            CPUusage = performanceCounter1.NextValue();
            label_WhatIsTalking.Text = WhatIsTalking;
            if(Mode == mode.Run)
            {
                DateTime now = DateTime.Now;
                if (!ReadFinished && now.Second < 30)
                {
                    rootLayer.Run(); ;
                    ReadFinished = true;
                }
                if (now.Second > 30)
                {
                    ReadFinished = false;
                }
            }
        }
        public bool IsCPUConditionOk()
        {
            if (CPUusage > (float)numericUpDown_CPUusageBorder.Value)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private void trackBar_Scroll(object sender, EventArgs e)
        {
            rootLayer.Set("Emo1", trackBar_Emo1.Value);
            rootLayer.Set("Emo2", trackBar_Emo2.Value);
            rootLayer.Set("Emo3", trackBar_Emo3.Value);
            rootLayer.Set("Emo4", trackBar_Emo4.Value);
            rootLayer.Set("Emo5", trackBar_Emo5.Value);
            label_Emo1_Value.Text = (trackBar_Emo1.Value * 0.01).ToString();
            label_Emo2_Value.Text = (trackBar_Emo2.Value * 0.01).ToString();
            label_Emo3_Value.Text = (trackBar_Emo3.Value * 0.01).ToString();
            label_Emo4_Value.Text = (trackBar_Emo4.Value * 0.01).ToString();
            label_Emo5_Value.Text = (trackBar_Emo5.Value * 0.01).ToString();
        }
        private void button_Volume_Up_Click(object sender, EventArgs e)
        {
            rootLayer.Increment("Volume", 1, 0, 100);
            label_Volume.Text = (rootLayer.GetInt("Volume") * 0.16 - 8.0).ToString("N2");
        }

        private void button_Volume_Down_Click(object sender, EventArgs e)
        {
            rootLayer.Decrement("Volume", 1, 0, 100);
            label_Volume.Text = (rootLayer.GetInt("Volume") * 0.16 - 8.0).ToString("N2");
        }

        private void button_Speed_Up_Click(object sender, EventArgs e)
        {
            rootLayer.Increment("Speed", 1, 0, 100);
            label_Speed.Text = Math.Pow(5,rootLayer.GetInt("Speed") * 0.02 - 1.0).ToString("N2");
        }

        private void button_Speed_Down_Click(object sender, EventArgs e)
        {
            rootLayer.Decrement("Speed", 1, 0, 100);
            label_Speed.Text = Math.Pow(5, rootLayer.GetInt("Speed") * 0.02 - 1.0).ToString("N2");
        }

        private void button_Tone_Up_Click(object sender, EventArgs e)
        {
            rootLayer.Increment("Tone", 1, 0, 100);
            label_Tone.Text = (rootLayer.GetInt("Tone") * 12 - 600).ToString("N0");
        }

        private void button_Tone_Down_Click(object sender, EventArgs e)
        {
            rootLayer.Decrement("Tone", 1, 0, 100);
            label_Tone.Text = (rootLayer.GetInt("Tone") * 12 - 600).ToString("N0");
        }

        private void button_Alpha_Up_Click(object sender, EventArgs e)
        {
            rootLayer.Increment("Alpha", 1, 0, 100);
            label_Alpha.Text = ( rootLayer.GetInt("Alpha") * 0.02 - 1.0).ToString("N2");
        }

        private void button_Alpha_Down_Click(object sender, EventArgs e)
        {
            rootLayer.Decrement("Alpha", 1, 0, 100);
            label_Alpha.Text = (rootLayer.GetInt("Alpha") * 0.02 - 1.0).ToString("N2");
        }

        private void button_ToneScale_Up_Click(object sender, EventArgs e)
        {
            rootLayer.Increment("ToneScale", 1, 0, 100);
            label_ToneScale.Text = ( rootLayer.GetInt("ToneScale") * 0.02).ToString("N2");
        }

        private void button_ToneScale_Down_Click(object sender, EventArgs e)
        {
            rootLayer.Decrement("ToneScale", 1, 0, 100);
            label_ToneScale.Text = (rootLayer.GetInt("ToneScale") * 0.02).ToString("N2");
        }

        private void Content_Changed(object sender, EventArgs e)
        {
            rootLayer.Set("Content",textBox_ContentToSet.Text);
        }

        private void End_Changed(object sender, EventArgs e)
        {
            rootLayer.Set("EndTime",(numericUpDown_EndHour.Value*60+numericUpDown_EndMinute.Value).ToString());
        }

        private void Start_Changed(object sender, EventArgs e)
        {
            rootLayer.Set("StartTime", (numericUpDown_StartHour.Value * 60 + numericUpDown_StartMinute.Value).ToString());
        }

        private void Interval_Changed(object sender, EventArgs e)
        {
            rootLayer.Set("Interval", numericUpDown_Interval.Value.ToString());
        }

        private void DayOfWeekChanged(object sender, EventArgs e)
        {
            string toAdd = "";
            if(checkBox_Sunday.CheckState == CheckState.Checked)
            {
                toAdd = toAdd + 'T';
            }
            else
            {
                toAdd = toAdd + 'F';
            }
            if (checkBox_Monday.CheckState == CheckState.Checked)
            {
                toAdd = toAdd + 'T';
            }
            else
            {
                toAdd = toAdd + 'F';
            }
            if (checkBox_Tuesday.CheckState == CheckState.Checked)
            {
                toAdd = toAdd + 'T';
            }
            else
            {
                toAdd = toAdd + 'F';
            }
            if (checkBox_Wednesday.CheckState == CheckState.Checked)
            {
                toAdd = toAdd + 'T';
            }
            else
            {
                toAdd = toAdd + 'F';
            }
            if (checkBox_Thursday.CheckState == CheckState.Checked)
            {
                toAdd = toAdd + 'T';
            }
            else
            {
                toAdd = toAdd + 'F';
            }
            if (checkBox_Friday.CheckState == CheckState.Checked)
            {
                toAdd = toAdd + 'T';
            }
            else
            {
                toAdd = toAdd + 'F';
            }
            if (checkBox_Saturday.CheckState == CheckState.Checked)
            {
                toAdd = toAdd + 'T';
            }
            else
            {
                toAdd = toAdd + 'F';
            }
            rootLayer.Set("DayOfWeek", toAdd);
        }

        private void Cast_Changed(object sender, EventArgs e)
        {
            rootLayer.Set("Cast", Casts[listBox_Casts.SelectedIndex]);
            InitReadLayer();
        }

        private void button_Return_Click(object sender, EventArgs e)
        {
            rootLayer.GoRoot();
            InitGUI();
        }

        private void button_Enter_Click(object sender, EventArgs e)
        {
            if (listBox_Units.SelectedIndex >= 0)
            {
                rootLayer.GoLeaf(listBox_Units.SelectedIndex);

                InitGUI();
            }
        }

        private void button_Delete_Click(object sender, EventArgs e)
        {
            if (listBox_Units.SelectedIndex >= 0)
            {
                var result = System.Windows.Forms.MessageBox.Show("削除しますか？","確認",MessageBoxButtons.YesNo,MessageBoxIcon.Question);
                if(result == DialogResult.Yes)
                {
                    rootLayer.DeleteLeaf(listBox_Units.SelectedIndex);
                    InitGUI();
                }
            }
        }

        private void button_New_Click(object sender, EventArgs e)
        {
            rootLayer.NewLeaf(textBox_Name.Text);
            InitGUI();
        }

        private void button_NameChange_Click(object sender, EventArgs e)
        {
            if (listBox_Units.SelectedIndex >= 0)
            {
                rootLayer.ChangeName(textBox_Name.Text, listBox_Units.SelectedIndex);
                InitGUI();
            }
        }

        private void button_Save_Click(object sender, EventArgs e)
        {
            label_SavePosition.Text = "保存中…";
            Enabled = false;
            rootLayer.Save();
            Enabled = true;
            label_SavePosition.Text = "保存完了";
        }

        private void button_TestTalk_Click(object sender, EventArgs e)
        {
            rootLayer.TestRun();
        }
        //描画中の値の初期化用
        private void tabControl_Changed(object sender, EventArgs e)
        {
            InitGUI();
        }

        private void InitGUI()
        {
            InitListBox_Units();
            switch ((LayerI)tabControl1.SelectedIndex)
            {
                case LayerI.Root:
                    InitRootLayer();
                    break;
                case LayerI.Time:
                    InitTimeLayer();
                    break;
                case LayerI.Random:
                    InitRandomLayer();
                    break;
                case LayerI.Read:
                    InitReadLayer();
                    break;
            }
        }
        static bool RootInitAble = true;
        private void InitRootLayer()
        {
            if (RootInitAble)
            {
                RootInitAble = false;
                numericUpDown_CPUusageBorder.Value = rootLayer.GetInt("CPUborder");
                RootInitAble = true;
            }
        }
        static bool TimeInitAble = true;
        private void InitTimeLayer()
        {
            if (TimeInitAble)
            {
                TimeInitAble = false;
                int startTime = rootLayer.GetInt("StartTime");
                numericUpDown_StartHour.Value = (startTime - startTime % 60) / 60;
                numericUpDown_StartMinute.Value = startTime % 60;
                int endTime = rootLayer.GetInt("EndTime");
                numericUpDown_EndHour.Value = (endTime - endTime % 60) / 60;
                numericUpDown_EndMinute.Value = endTime % 60;
                numericUpDown_Interval.Value = rootLayer.GetInt("Interval");
                string temp = rootLayer.GetStr("DayOfWeek");
                char[] dayOfWeek = new char[7];
                for (int i = 0; i < 7; i++)
                {
                    dayOfWeek[i] = temp[i];
                }
                if (dayOfWeek[0] == 'T')
                {
                    checkBox_Sunday.CheckState = CheckState.Checked;
                }
                else
                {
                    checkBox_Sunday.CheckState = CheckState.Unchecked;
                }
                if (dayOfWeek[1] == 'T')
                {
                    checkBox_Monday.CheckState = CheckState.Checked;
                }
                else
                {
                    checkBox_Monday.CheckState = CheckState.Unchecked;
                }
                if (dayOfWeek[2] == 'T')
                {
                    checkBox_Tuesday.CheckState = CheckState.Checked;
                }
                else
                {
                    checkBox_Tuesday.CheckState = CheckState.Unchecked;
                }
                if (dayOfWeek[3] == 'T')
                {
                    checkBox_Wednesday.CheckState = CheckState.Checked;
                }
                else
                {
                    checkBox_Wednesday.CheckState = CheckState.Unchecked;
                }
                if (dayOfWeek[4] == 'T')
                {
                    checkBox_Thursday.CheckState = CheckState.Checked;
                }
                else
                {
                    checkBox_Thursday.CheckState = CheckState.Unchecked;
                }
                if (dayOfWeek[5] == 'T')
                {
                    checkBox_Friday.CheckState = CheckState.Checked;
                }
                else
                {
                    checkBox_Friday.CheckState = CheckState.Unchecked;
                }
                if (dayOfWeek[6] == 'T')
                {
                    checkBox_Saturday.CheckState = CheckState.Checked;
                }
                else
                {
                    checkBox_Saturday.CheckState = CheckState.Unchecked;
                }
                TimeInitAble = true;
            }
        }
        static bool RandomInitAble = true;
        private void InitRandomLayer()
        {
            if (RandomInitAble)
            {
                RandomInitAble = false;

                RandomInitAble = true;
            }
        }
        static bool ReadInitAble = true;
        private void InitReadLayer()
        {
            if (ReadInitAble)
            {
                ReadInitAble=false;
                textBox_ContentToSet.Text = rootLayer.GetStr("Content");
                for (int i = 0; i < Casts.Count; i++)
                {
                    if (SameString(Casts[i], rootLayer.GetStr("Cast")))
                    {
                        listBox_Casts.SelectedIndex = i;
                        break;
                    }
                }
                //感情の名前
                {
                    if (EmotionNames[Casts[listBox_Casts.SelectedIndex]].Count >= 1)
                    {
                        label_Emo1_Name.Visible = true;
                        label_Emo1_Value.Visible = true;
                        trackBar_Emo1.Visible = true;
                        label_Emo1_Name.Text = EmotionNames[Casts[listBox_Casts.SelectedIndex]][0];
                    }
                    else
                    {
                        label_Emo1_Name.Visible = false;
                        label_Emo1_Value.Visible = false;
                        trackBar_Emo1.Visible = false;
                    }
                    if (EmotionNames[Casts[listBox_Casts.SelectedIndex]].Count >= 2)
                    {
                        label_Emo2_Name.Visible = true;
                        label_Emo2_Value.Visible = true;
                        trackBar_Emo2.Visible = true;
                        label_Emo2_Name.Text = EmotionNames[Casts[listBox_Casts.SelectedIndex]][1];
                    }
                    else
                    {
                        label_Emo2_Name.Visible = false;
                        label_Emo2_Value.Visible = false;
                        trackBar_Emo2.Visible = false;
                    }
                    if (EmotionNames[Casts[listBox_Casts.SelectedIndex]].Count >= 3)
                    {
                        label_Emo3_Name.Visible = true;
                        label_Emo3_Value.Visible = true;
                        trackBar_Emo3.Visible = true;
                        label_Emo3_Name.Text = EmotionNames[Casts[listBox_Casts.SelectedIndex]][2];
                    }
                    else
                    {
                        label_Emo3_Name.Visible = false;
                        label_Emo3_Value.Visible = false;
                        trackBar_Emo3.Visible = false;
                    }
                    if (EmotionNames[Casts[listBox_Casts.SelectedIndex]].Count >= 4)
                    {
                        label_Emo4_Name.Visible = true;
                        label_Emo4_Value.Visible = true;
                        trackBar_Emo4.Visible = true;
                        label_Emo4_Name.Text = EmotionNames[Casts[listBox_Casts.SelectedIndex]][3];
                    }
                    else
                    {
                        label_Emo4_Name.Visible = false;
                        label_Emo4_Value.Visible = false;
                        trackBar_Emo4.Visible = false;
                    }
                    if (EmotionNames[Casts[listBox_Casts.SelectedIndex]].Count >= 5)
                    {
                        label_Emo5_Name.Visible = true;
                        label_Emo5_Value.Visible = true;
                        trackBar_Emo5.Visible = true;
                        label_Emo5_Name.Text = EmotionNames[Casts[listBox_Casts.SelectedIndex]][4];
                    }
                    else
                    {
                        label_Emo5_Name.Visible = false;
                        label_Emo5_Value.Visible = false;
                        trackBar_Emo5.Visible = false;
                    }
                }
                //感情の値
                {
                    trackBar_Emo1.Value = rootLayer.GetInt("Emo1");
                    trackBar_Emo2.Value = rootLayer.GetInt("Emo2");
                    trackBar_Emo3.Value = rootLayer.GetInt("Emo3");
                    trackBar_Emo4.Value = rootLayer.GetInt("Emo4");
                    trackBar_Emo5.Value = rootLayer.GetInt("Emo5");
                    label_Emo1_Value.Text = (trackBar_Emo1.Value * 0.01).ToString();
                    label_Emo2_Value.Text = (trackBar_Emo2.Value * 0.01).ToString();
                    label_Emo3_Value.Text = (trackBar_Emo3.Value * 0.01).ToString();
                    label_Emo4_Value.Text = (trackBar_Emo4.Value * 0.01).ToString();
                    label_Emo5_Value.Text = (trackBar_Emo5.Value * 0.01).ToString();
                }
                //パラメーター
                {
                    label_Volume.Text = (rootLayer.GetInt("Volume") * 0.16 - 8.0).ToString("N2");
                    label_Speed.Text = Math.Pow(5, rootLayer.GetInt("Speed") * 0.02 - 1.0).ToString("N2");
                    label_Tone.Text = (rootLayer.GetInt("Tone") * 12 - 600).ToString("N0");
                    label_Alpha.Text = (rootLayer.GetInt("Alpha") * 0.02 - 1.0).ToString("N2");
                    label_ToneScale.Text = (rootLayer.GetInt("ToneScale") * 0.02).ToString("N2");
                }
                ReadInitAble |= true;
            }
        }
        static bool LBInitAble = true;
        private void InitListBox_Units()
        {
            if(LBInitAble)
            {
                LBInitAble = false;
                tabControl1.SelectedIndex = rootLayer.CurrentTree.Count;
                listBox_Units.Items.Clear();
                foreach (string s in rootLayer.GetLeafDictionary())
                {
                    listBox_Units.Items.Add(s);
                }
                label_Breadclumb.Text = rootLayer.GetBreadclumb();
                if(listBox_Units.SelectedIndex == -1 && listBox_Units.Items.Count > 0)
                {
                    if (nextUnitSelectIndex >= 0 && nextUnitSelectIndex < listBox_Units.Items.Count)
                    {
                        listBox_Units.SelectedIndex = nextUnitSelectIndex;
                        nextUnitSelectIndex = 0;
                    }
                    else
                    {
                        listBox_Units.SelectedIndex = 0;
                    }
                }
                LBInitAble = true;
            }
        }
        public int IntParse(string from)
        {
            if (!int.TryParse(from, out int result)) throw new Exception();
            return result;
        }
        static public bool SameString(string A, string B)
        {
            if (A.Length != B.Length) return false;
            for (int i = 0; i < A.Length; i++)
            {
                if (A[i] != B[i]) return false;
            }
            return true;
        }
        enum mode
        {
            Edit=0,
            Run=1
        }
        mode Mode=mode.Run;
        private void button_ModeChange_Click(object sender, EventArgs e)
        {
            if(Mode == mode.Run)
            {
                Mode = mode.Edit;
                button_ModeChange.Text = "編集モード";
                tabControl1.Enabled = true;
                button_Delete.Enabled = true;
                button_New.Enabled = true;
                button_Enter.Enabled = true;
                button_Return.Enabled = true;
                button_NameChange.Enabled = true;
                listBox_Units.Enabled = true;
                button_GoUp.Enabled = true;
                button_GoDown.Enabled = true;
                textBox_Name.Enabled = true;
            }
            else if(Mode == mode.Edit)
            {
                Mode = mode.Run;
                button_ModeChange.Text = "実行モード";
                tabControl1.Enabled = false;
                button_Delete.Enabled = false;
                button_New.Enabled = false;
                button_Enter.Enabled = false;
                button_Return.Enabled = false;
                button_NameChange.Enabled = false;
                listBox_Units.Enabled = false;
                button_GoUp.Enabled = false;
                button_GoDown.Enabled = false;
                textBox_Name.Enabled = false;
            }
        }

        private void button_GoDown_Click(object sender, EventArgs e)
        {
            rootLayer.LeafGoDown(listBox_Units.SelectedIndex);
            InitGUI();
        }

        private void button_GoUp_Click(object sender, EventArgs e)
        {
            rootLayer.LeafGoUp(listBox_Units.SelectedIndex);
            InitGUI();
        }

        private void When_Close(object sender, FormClosingEventArgs e)
        {
            var result = System.Windows.Forms.MessageBox.Show("保存しますか？", "確認", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if(result == DialogResult.Yes)
            {
                rootLayer.Save();
            }
        }
    }
}
