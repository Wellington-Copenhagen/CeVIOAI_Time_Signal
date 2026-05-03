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
        List<TalkUnit> _talkUnits;
        public List<VoiceProperty> Properties = new List<VoiceProperty>();
        public List<uint> Emotions;
        public string Cast;
        public string Content;
        public string AlternativeText;
        SpeakingState2 speakingState;
        Label _label;
        public bool Talking
        {
            get
            {
                if(speakingState == null || speakingState.IsCompleted)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }
        public VoiceOutput()
        {
            _label = new Label();
            _label.Location = new Point(10, 20);
            _label.Size = new Size(600, 300);
            _label.BackColor = Color.White;
            Controls.Add(_label);
            _talkUnits = new List<TalkUnit>();
        }
        public void AddNewTalkUnit()
        {
            GUI_Components.StartHost();

            if (!Talker2.AvailableCasts.Contains(Cast))
            {
                _label.Text = $"{Cast}はライセンスを取得していないキャストです。";
                return;
            }
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
            _talkUnits.Add(new TalkUnit(Content, talker2));
        }
        public void EveryTick()
        {
            for(int i = 0; i < _talkUnits.Count; i++)
            {
                if (_talkUnits[i].IsTimeOut())
                {
                    _talkUnits.RemoveAt(i);
                    i--;
                }
            }
            if(_talkUnits.Count > 0 && !Talking)
            {
                if (ServiceControl2.IsHostStarted)
                {
                    speakingState = _talkUnits[0].Talker.Speak(_talkUnits[0].Content);
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
                    for (int i = 0; i < _talkUnits[0].Talker.Components.Count; i++)
                    {
                        toShow = toShow + " " + _talkUnits[0].Talker.Components[i].Name + "：" + Emotions[i].ToString();
                    }
                    toShow = toShow + "\n\n";
                    toShow = toShow + _talkUnits[0].Content;
                }
                else
                {
                    toShow = AlternativeText;
                }
                _talkUnits.RemoveAt(0);
                _label.Text = toShow;
            }
        }
    }
    internal class TalkUnit
    {
        public TalkUnit(string content, Talker2 talker)
        {
            Content = content;
            Talker = talker;
            GeneratedTime = DateTime.Now;
        }
        public string Content
        {
            private set;
            get;
        }
        public Talker2 Talker
        {
            private set;
            get;
        }
        public DateTime GeneratedTime
        {
            private set;
            get;
        }
        public bool IsTimeOut()
        {
            return (DateTime.Now - GeneratedTime).TotalMinutes > 1;
        }
    }
}
