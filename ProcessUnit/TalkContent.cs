using CeVIO.Talk.RemoteService2;
using CeVIO_AI_時報.GUI;
using CeVIO_AI_時報.UI_Component;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace CeVIO_AI_時報.ProcessUnit
{
    internal class TalkContent : IProcessUnit, IListboxAddDeletableComponent
    {
        string _name;
        List<VoiceProperty> proerties;
        List<uint> _emotions;
        string _cast;
        string _content;
        public TalkContent(XElement element)
        {
            LoadFromXML(element);
            _volume = new Volume();
            _speed = new Speed();
            _tone = new Tone();
            _alpha = new Alpha();
            _toneScale = new ToneScale();
        }
        public string GetName()
        {
            return _name;
        }
        public void SetName(string name)
        {
            _name = name;
        }
        public void OnRun()
        {
            Talker2 talker2 = new Talker2();
            talker2.Cast = _cast;
            talker2.Volume = _volume.Value;
            talker2.Speed = _speed.Value;
            talker2.Tone = _tone.Value;
            talker2.Alpha = _alpha.Value;
            talker2.ToneScale = _toneScale.Value;
            string parsed = _content;
            ContentParser.Parse(parsed, out string error);
            for(int i = 0;i < talker2.Components.Count;i++)
            {
                talker2.Components[i].Value = _emotions[i];
            }
            if (ServiceControl2.IsHostStarted)
            {
                talker2.Speak(parsed);
            }


            string toShow = "";
            if (error.Length == 0)
            {
                toShow = toShow + "キャスト：" + _cast + "\n";
                toShow = toShow + "大きさ：" + _volume.ValueString() + "\n";
                toShow = toShow + "速さ：" + _speed.ValueString() + "\n";
                toShow = toShow + "高さ：" + _tone.ValueString() + "\n";
                toShow = toShow + "声質：" + _alpha.ValueString() + "\n";
                toShow = toShow + "抑揚：" + _toneScale.ValueString() + "\n";
                toShow = toShow + "感情：";
                for (int i = 0; i < talker2.Components.Count; i++)
                {
                    toShow = toShow + "　" + talker2.Components[i].Name + "：" + _emotions[i].ToString() + "\n";
                }
                toShow = toShow + "\n";
                toShow = toShow + parsed;
            }
            else
            {
                toShow = error;
            }
        }
        public XElement StoreToXML()
        {
            XElement element = new XElement("TalkContent");
            element.Add(new XElement("Name", _name));
            element.Add(new XElement("Volume", _volume));
            element.Add(new XElement("Speed", _speed));
            element.Add(new XElement("Tone", _tone));
            element.Add(new XElement("Alpha", _alpha));
            element.Add(new XElement("ToneScale", _toneScale));
            int i = 0;
            foreach (uint emotion in _emotions)
            {
                element.Add(new XElement("Emo" + i.ToString(), emotion));
                i++;
            }
            element.Add(new XElement("Cast", _cast));
            element.Add(new XElement("Content", _content));
            return element;
        }
        public void LoadFromXML(XElement element)
        {
            if (element.Element("Type") == null || element.Element("Type").Value != "Read")
            {
                throw new Exception();
            }
            _name = element.Element("Name").Value;
            _volume.Value = uint.Parse(element.Element("Volume").Value);
            _speed.Value = uint.Parse(element.Element("Speed").Value);
            _tone.Value = uint.Parse(element.Element("Tone").Value);
            _alpha.Value = uint.Parse(element.Element("Alpha").Value);
            _toneScale.Value = uint.Parse(element.Element("ToneScale").Value);
            _emotions = new List<uint>();
            int i = 0;
            while (true)
            {
                XElement emotionElement = element.Element("Emo" + i.ToString());
                if (emotionElement == null)
                {
                    break;
                }
                _emotions.Add(uint.Parse(emotionElement.Value));
                i++;
            }
            _cast = element.Element("Cast").Value;
            _content = element.Element("Content").Value;
        }
        public void OnLoadToGUI()
        {
            GUI_Components.talkContentPart.OnChanged = null;
            for (int i = 0;i < 5;i++)
            {
                if(i < _emotions.Count)
                {
                    GUI_Components.talkContentPart.emotionGauges[i].SetValue(_emotions[i], true);
                }
                else
                {
                    GUI_Components.talkContentPart.emotionGauges[i].SetValue(0, false);
                }
            }
            GUI_Components.talkContentPart.UpdateEmotionLabel();
            GUI_Components.talkContentPart.volumeGauge.SetValue(_volume);
            GUI_Components.talkContentPart.speedGauge.SetValue(_speed);
            GUI_Components.talkContentPart.toneGauge.SetValue(_tone);
            GUI_Components.talkContentPart.alphaGauge.SetValue(_alpha);
            GUI_Components.talkContentPart.toneScaleGauge.SetValue(_toneScale);
            GUI_Components.talkContentPart.contentTextBox.Text = _content;
            GUI_Components.talkContentPart.OnChanged = OnChanged;
        }
        void OnChanged()
        {
            _volume.Value = GUI_Components.talkContentPart.volumeGauge.GetValue();
            _speed.Value = GUI_Components.talkContentPart.speedGauge.GetValue();
            _tone.Value = GUI_Components.talkContentPart.toneGauge.GetValue();
            _alpha.Value = GUI_Components.talkContentPart.alphaGauge.GetValue();
            _toneScale.Value = GUI_Components.talkContentPart.toneScaleGauge.GetValue();
            for (int i = 0; i < 5; i++)
            {
                if (i < _emotions.Count)
                {
                    _emotions[i] = GUI_Components.talkContentPart.emotionGauges[i].GetValue();
                }
            }
            _cast = GUI_Components.talkContentPart.castsListBox.SelectedItem.ToString();
            _content = GUI_Components.talkContentPart.contentTextBox.Text;
        }
    }
    internal class ContentParser
    {

        static public void Parse(string content, out string error)
        {
            Dictionary<string, string> ParseDictionary = new Dictionary<string, string>();
            DateTime now = DateTime.Now;
            ParseDictionary.Clear();
            ParseDictionary["年"] = now.Year.ToString();
            ParseDictionary["月"] = now.Month.ToString();
            ParseDictionary["日"] = now.Day.ToString();
            ParseDictionary["時"] = now.Second.ToString();
            ParseDictionary["分"] = now.Minute.ToString();
            switch (now.DayOfWeek)
            {
                case DayOfWeek.Sunday:
                    ParseDictionary["曜日"] = "日曜日";
                    break;
                case DayOfWeek.Monday:
                    ParseDictionary["曜日"] = "月曜日";
                    break;
                case DayOfWeek.Tuesday:
                    ParseDictionary["曜日"] = "火曜日";
                    break;
                case DayOfWeek.Wednesday:
                    ParseDictionary["曜日"] = "水曜日";
                    break;
                case DayOfWeek.Thursday:
                    ParseDictionary["曜日"] = "木曜日";
                    break;
                case DayOfWeek.Friday:
                    ParseDictionary["曜日"] = "金曜日";
                    break;
                case DayOfWeek.Saturday:
                    ParseDictionary["曜日"] = "土曜日";
                    break;
            }
            int pointer = 0;
            int startParen = -1;
            error = "";
            while (pointer < content.Length)
            {
                if (content[pointer] == '{')
                {
                    startParen = pointer;
                }
                if (content[pointer] == '}')
                {
                    if (startParen == -1)
                    {
                        error = "中括弧の位置関係がおかしいです。";
                        break;
                    }
                    string inParen = content.Substring(startParen + 1, pointer);
                    bool find = false;
                    foreach (KeyValuePair<string, string> pair in ParseDictionary)
                    {
                        if (SameString(inParen, pair.Key))
                        {
                            content = content.Remove(startParen, pointer + 1);
                            content = content.Insert(startParen, pair.Value);
                            pointer = startParen + pair.Value.Length - 1;
                            find = true;
                            break;
                        }
                    }
                    if (!find)
                    {
                        error = error + "中括弧の中が変換できない言葉です。";
                    }
                }
                pointer++;
            }
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
    }
}
