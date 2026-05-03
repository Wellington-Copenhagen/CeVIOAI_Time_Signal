using CeVIO.Talk.RemoteService2;
using CeVIO_AI_時報.UI_Component;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CeVIO_AI_時報.GUI
{
    internal class GUI_Components
    {
        static public Panel TopBar;
        static public Panel EditingArea;
        static public Panel BottomArea;

        static public Button modeChangeButton;
        static public Button saveButton;
        static public Button loadButton;

        static public GUI_RootPart rootPart;
        static public GUI_TimeConditionPart timeConditionPart;
        static public GUI_RandomSelectPart randomSelectPart;
        static public GUI_TalkContentPart talkContentPart;
        static public VoiceOutput voiceOutput;

        static public bool UnderOnValueChanged = false;
        static Form1.Mode CurrentMode = Form1.Mode.Run;
        static bool CeVIOLoaded = false;
        static bool DataLoaded = false;
        static public void PlaceComponents(Control parent)
        {
            BottomArea = new Panel();
            BottomArea.Dock = DockStyle.Top;
            BottomArea.Height = 370;
            parent.Controls.Add(BottomArea);

            EditingArea = new Panel();
            EditingArea.Dock = DockStyle.Top;
            EditingArea.Height = 400;
            parent.Controls.Add(EditingArea);

            TopBar = new Panel();
            TopBar.Dock = DockStyle.Top;
            TopBar.Height = 30;
            parent.Controls.Add(TopBar);

            talkContentPart = new GUI_TalkContentPart();
            talkContentPart.Dock = DockStyle.Left;
            talkContentPart.Text = "話す内容";
            talkContentPart.Width = 500;
            EditingArea.Controls.Add(talkContentPart);

            randomSelectPart = new GUI_RandomSelectPart();
            randomSelectPart.Dock = DockStyle.Left;
            randomSelectPart.Text = "ランダム選択";
            randomSelectPart.Width = 150;
            EditingArea.Controls.Add(randomSelectPart);

            timeConditionPart = new GUI_TimeConditionPart();
            timeConditionPart.Dock = DockStyle.Left;
            timeConditionPart.Text = "時間の条件";
            timeConditionPart.Width = 200;
            EditingArea.Controls.Add(timeConditionPart);

            rootPart = new GUI_RootPart();
            rootPart.Dock = DockStyle.Left;
            rootPart.Width = 150;
            rootPart.Text = "全内容";
            EditingArea.Controls.Add(rootPart);
            
            voiceOutput = new VoiceOutput();
            voiceOutput.Dock = DockStyle.Fill;
            voiceOutput.Text = "読み上げた内容";
            voiceOutput.Height = 370;
            BottomArea.Controls.Add(voiceOutput);

            saveButton = new Button();
            saveButton.Text = "保存する";
            saveButton.Location = new Point(5, 5);
            saveButton.Size = new Size(80, 20);
            TopBar.Controls.Add(saveButton);

            modeChangeButton = new Button();
            modeChangeButton.Text = "実行モード";
            modeChangeButton.Location = new Point(90, 5);
            modeChangeButton.Size = new Size(80, 20);
            TopBar.Controls.Add(modeChangeButton);

            loadButton = new Button();
            loadButton.Text = "読み込む";
            loadButton.Location = new Point(175, 5);
            loadButton.Size = new Size(80, 20);
            TopBar.Controls.Add(loadButton);

            //rootPart.ValueChanged = () => UpdateVisual();
            //timeConditionPart.ValueChanged= () => UpdateVisual();
            //randomSelectPart.ValueChanged = () => UpdateVisual();
            //talkContentPart.ValueChanged = () => UpdateVisual();
        }
        static public void OnCeVIOLoaded()
        {
            CeVIOLoaded = true;
            rootPart.InitWithCeVIO();
            timeConditionPart.InitWithCeVIO();
            randomSelectPart.InitWithCeVIO();
            talkContentPart.InitWithCeVIO();

            UpdateVisual();
        }
        static public void OnDataLoaded()
        {
            DataLoaded = true;

            UpdateVisual();
        }
        static public void UpdateVisual()
        {
            rootPart.UpdateUI(CeVIOLoaded && DataLoaded && CurrentMode == Form1.Mode.Edit);
            timeConditionPart.UpdateUI(CeVIOLoaded && DataLoaded && CurrentMode == Form1.Mode.Edit);
            randomSelectPart.UpdateUI(CeVIOLoaded && DataLoaded && CurrentMode == Form1.Mode.Edit);
            talkContentPart.UpdateUI(CeVIOLoaded && DataLoaded && CurrentMode == Form1.Mode.Edit);
        }
        static public void OnModeChanged(Form1.Mode mode)
        {
            CurrentMode = mode;
            UpdateVisual();
            if(CurrentMode == Form1.Mode.Edit)
            {
                saveButton.Enabled = true;
                loadButton.Enabled = true;
            }
            else
            {
                saveButton.Enabled = false;
                loadButton.Enabled = false;
            }
        }
        static public void StartHost()
        {
            if (!ServiceControl2.IsHostStarted)
            {
                Form popup = new Form();
                popup.Size = new Size(600,0);
                popup.Text = "CeVIO AIトークエディターを起動中です。";

                popup.Show();
                HostStartResult startResult = ServiceControl2.StartHost(false);
                popup.Close();
                if (startResult != HostStartResult.Succeeded)
                {
                    MessageBox.Show("CeVIOトークエディタが起動できませんでした。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Environment.Exit(0);
                }
                if (Talker2.AvailableCasts.Count() == 0)
                {
                    MessageBox.Show("利用可能なキャストがいませんでした。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Environment.Exit(0);
                }
            }
        }
    }
}
