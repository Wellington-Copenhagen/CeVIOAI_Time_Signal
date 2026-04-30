using CeVIO.Talk.RemoteService2;
using CeVIO_AI_時報.GUI;
using CeVIO_AI_時報.ProcessUnit;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using static CeVIO_AI_時報.GUI.GUI_Components;

namespace CeVIO_AI_時報
{
    public partial class Form1 : Form
    {
        RootLevel rootLevel;
        Timer timer;
        public enum Mode
        {
            Edit,
            Run
        }
        Mode CurrentMode = Mode.Run;
        public Form1()
        {
            InitializeComponent();
            HostStartResult startResult = ServiceControl2.StartHost(false);
            if(startResult != HostStartResult.Succeeded)
            {
                MessageBox.Show("CeVIOトークエディタが起動できませんでした。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.Exit(0);
            }
            if(Talker2.AvailableCasts.Count() == 0)
            {
                MessageBox.Show("利用可能なキャストがいませんでした。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.Exit(0);
            }
            Size = new Size(1000, 800);
            GUI_Components.PlaceComponents(this);

            CurrentMode = Mode.Run;
            GUI_Components.OnModeChanged(CurrentMode);

            GUI_Components.OnCeVIOLoaded();

            if (File.Exists("config.txt"))
            {
                try
                {
                    rootLevel = new RootLevel(XElement.Load("config.txt"));
                }
                catch (XmlException)
                {
                    rootLevel = new RootLevel();
                }
            }
            else
            {
                rootLevel = new RootLevel();
            }

            GUI_Components.saveButton.Click += (sender, e) => SaveToFile();
            GUI_Components.modeChangeButton.Click += (sender, e) => OnModeChangeButtonClicked();

            GUI_Components.rootPart.SetProcessUnit(rootLevel);
            GUI_Components.OnDataLoaded();
        }
        void TimerTick(object sender, EventArgs e)
        {
            if(CurrentMode == Mode.Run)
            {
                rootLevel.OnRun();
            }
        }

        void OnModeChangeButtonClicked()
        {
            if (CurrentMode == Mode.Edit)
            {
                CurrentMode = Mode.Run;
                modeChangeButton.Text = "実行モード";
                AskSave();
            }
            else
            {
                CurrentMode = Mode.Edit;
                modeChangeButton.Text = "編集モード";
            }
            GUI_Components.OnModeChanged(CurrentMode);
        }
        void SaveToFile()
        {
            XElement element = rootLevel.StoreToXML();
            element.Save("config.txt");
        }
        void AskSave()
        {
            if (MessageBox.Show("保存しますか？", "保存", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                SaveToFile();
            }
        }
        void WhenClose(object sender, FormClosingEventArgs e)
        {
            AskSave();
        }
    }
}
