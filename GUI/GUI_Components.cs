using CeVIO_AI_時報.UI_Component;
using CeVIO.Talk.RemoteService2;
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
        static public GUI_RootPart rootPart;
        static public GUI_TimeConditionPart timeConditionPart;
        static public GUI_RandomSelectPart randomSelectPart;
        static public GUI_TalkContentPart talkContentPart;
        static public TextBox notifyTextBox;
        static public Button modeChangeButton;
        static public Button saveButton;
        static public Action OnSaveButtonClicked;
        public enum Mode
        {
            Edit,
            Run
        }
        static public Mode CurrentMode;
        void OnLoad()
        {
        }
        void OnCeVIOLoad()
        {
        }
        void OnModeChangeButtonClicked()
        {
            if(CurrentMode == Mode.Edit)
            {
                CurrentMode = Mode.Run;
            }
        }
        public void Notify(string message)
        {
            if (notifyTextBox != null)
            {
                notifyTextBox.Text = message;
            }
        }
    }
    internal interface GUI_Part
    {
        void Disable();
        void Enable();
        void OnLoad();
        void OnCeVIOLoad();
        Action ChangeEvent();
    }
}
