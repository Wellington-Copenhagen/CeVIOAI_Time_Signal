using CeVIO.Talk.RemoteService2;
using CeVIO_AI_時報.ProcessUnit;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CeVIO_AI_時報.GUI
{
    internal class VoiceOutput : GroupBox
    {
        public List<VoiceProperty> Properties = new List<VoiceProperty>();
        public List<uint> Emotions;
        public string Cast;
        public string Content;
        public string AlternativeText;
        Label _label;
        public VoiceOutput()
        {
            _label = new Label();
            _label.Location = new Point(10, 20);
            _label.Size = new Size(600, 300);
            _label.BackColor = Color.White;
            Controls.Add(_label);
        }
        public void Talk()
        {
            Talker2 talker2 = new Talker2();
            talker2.Cast = Cast;
            talker2.Volume = Properties[0].Value;
            talker2.Speed = Properties[1].Value;
            talker2.Tone = Properties[2].Value;
            talker2.Alpha = Properties[3].Value;
            talker2.ToneScale = Properties[4].Value;
            for (int i = 0; i < talker2.Components.Count; i++)
            {
                talker2.Components[i].Value = Emotions[i];
            }
            if (ServiceControl2.IsHostStarted)
            {
                talker2.Speak(Content);
            }


            string toShow = "";
            if (AlternativeText.Length == 0)
            {
                toShow = toShow + DateTime.Now.Month + "月" + DateTime.Now.Day + "日"
                    + DateTime.Now.Hour + "時" + DateTime.Now.Minute + "分" + DateTime.Now.Second + "秒\n";
                toShow = toShow + "キャスト：" + Cast + "\n";
                foreach (VoiceProperty property in Properties)
                {
                    toShow = toShow + property + " ";
                }
                toShow = toShow + "\n感情：";
                for (int i = 0; i < talker2.Components.Count; i++)
                {
                    toShow = toShow + " " + talker2.Components[i].Name + "：" + Emotions[i].ToString();
                }
                toShow = toShow + "\n\n";
                toShow = toShow + Content;
            }
            else
            {
                toShow = AlternativeText;
            }
            _label.Text = toShow;
        }
    }
}
