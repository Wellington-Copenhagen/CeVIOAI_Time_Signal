using CeVIO.Talk.RemoteService2;
using CeVIO_AI_時報.GUI;
using CeVIO_AI_時報.UI_Component;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace CeVIO_AI_時報.ProcessUnit
{
    internal class TalkContent : IProcessUnit, IListboxAddDeletableComponent
    {
        string _name;
        List<VoiceProperty> _properties;
        public List<VoiceProperty> Properties
        {
            get
            {
                Debug.Assert(_properties != null);
                return _properties;
            }
            set
            {
                Debug.Assert(value != null);
                Debug.Assert(value.Count == 5);
                for(int i = 0; i < 5; i++)
                {
                    Debug.Assert(value[i] != null);
                }
                Debug.Assert(value[0] is Volume);
                Debug.Assert(value[1] is Speed);
                Debug.Assert(value[2] is Tone);
                Debug.Assert(value[3] is Alpha);
                Debug.Assert(value[4] is ToneScale);
                _properties = value;
            }
        }
        List<uint> _emotions;
        public List<uint> Emotions
        {
            get
            {
                Debug.Assert(_emotions != null);
                return _emotions;
            }
            set
            {
                Debug.Assert(value != null);
                Debug.Assert(value.Count == 5);
                for (int i = 0;i < 5; i++)
                {
                    Debug.Assert(value[i] <= 100);
                }
                _emotions = value;
            }
        }

        // 現在実行できないキャストでもよい
        // ほかのユーザーの作ったデータでも読めるようにするため
        string _cast;
        public string Cast
        {
            get
            {
                Debug.Assert(_cast != null);
                return _cast;
            }
            set
            {
                Debug.Assert(value != null);
                _cast = value;
            }
        }
        string _content;
        public string Content
        {
            get
            {
                Debug.Assert(_content != null);
                return _content;
            }
            set
            {
                Debug.Assert(value != null);
                _content = value;
            }
        }
        public TalkContent()
        {
            Default();
        }
        public TalkContent(XElement element)
        {
            Default();
            LoadFromXML(element);
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
            GUI_Components.voiceOutput.Cast = Cast;
            GUI_Components.voiceOutput.Properties = Properties;
            GUI_Components.voiceOutput.Emotions = Emotions;
            GUI_Components.voiceOutput.Content = Content;
            string parsed = Content;
            ContentParser.Parse(ref parsed, out string error);
            GUI_Components.voiceOutput.Content = parsed;
            GUI_Components.voiceOutput.AlternativeText = error;

            GUI_Components.voiceOutput.AddNewTalkUnit();
        }
        public XElement StoreToXML()
        {
            XElement element = new XElement("TalkContent");
            element.Add(new XElement("Name", _name));
            element.Add(new XElement("Type", "Read"));
            element.Add(new XElement("VoiceProperties"));
            foreach (VoiceProperty property in Properties)
            {
                element.Element("VoiceProperties").Add(new XElement(property.NameEn(), property.Value));
            }
            element.Add(new XElement("Emotions"));
            int i = 0;
            foreach (uint emotion in Emotions)
            {
                element.Element("Emotions").Add(new XElement("Emotion" + i.ToString(), emotion));
                i++;
            }
            element.Add(new XElement("Cast", Cast));
            element.Add(new XElement("Content", Content));
            return element;
        }
        public void LoadFromXML(XElement element)
        {
            if (element.Element("Type") == null || element.Element("Type").Value != "Read")
            {
                return;
            }
            if (element.Element("Name") != null)
            {
                _name = element.Element("Name").Value;
            }
            if (element.Element("VoiceProperties") != null)
            {
                for (int i = 0; i < 5; i++)
                {
                    if (element.Element("VoiceProperties").Element(Properties[i].NameEn()) == null)
                    {
                        Properties[i].Value = 0;
                    }
                    else
                    {
                        XElement propertyElement = element.Element("VoiceProperties").Element(Properties[i].NameEn());
                        if (propertyElement == null)
                        {
                            Properties[i].Value = 0;
                        }
                        else if(!uint.TryParse(propertyElement.Value, out uint result))
                        {
                            Properties[i].Value = 0;
                        }
                        else
                        {
                            Properties[i].Value = result;
                        }
                    }
                }
            }
            if (element.Element("Emotions") != null)
            {
                for (int i = 0; i < 5; i++)
                {
                    XElement emotionElement = element.Element("Emotions").Element("Emotion" + i.ToString());
                    if (emotionElement == null)
                    {
                        Emotions[i] = 0;
                    }
                    else if(!uint.TryParse(emotionElement.Value, out uint emotionValue))
                    {
                        Emotions[i] = 0;
                    }
                    else
                    {
                        Emotions[i] = emotionValue;
                    }
                }
            }
            if(element.Element("Cast") != null)
            {
                Cast = element.Element("Cast").Value;
            }
            if(element.Element("Content") != null)
            {
                Content = element.Element("Content").Value;
            }
        }
        public void Default()
        {
            _name = "New";
            Properties = new List<VoiceProperty>() { new Volume(), new Speed(), new Tone(), new Alpha(), new ToneScale() };
            Emotions = new List<uint>() { 0, 0, 0, 0, 0 };
            Cast = Talker2.AvailableCasts[0];
            Content = "こんにちは！今は{月}月{日}日{時}時{分}分です。";
        }
    }
    internal class ContentParser
    {
        // エラーは出さない
        // errorで変換できないことを出力する。
        static public void Parse(ref string content, out string error)
        {
            Dictionary<string, string> ParseDictionary = new Dictionary<string, string>();
            DateTime now = DateTime.Now;
            ParseDictionary.Clear();
            ParseDictionary["年"] = now.Year.ToString();
            ParseDictionary["月"] = now.Month.ToString();
            ParseDictionary["日"] = now.Day.ToString();
            ParseDictionary["時"] = now.Hour.ToString();
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
            error = "";
            int parenCount = content.Count((c) => { return c == '{'; });
            if(parenCount != content.Count((c) => { return c == '}'; }))
            {
                error = "中括弧の数が合っていません。\n";
                return;
            }
            for (int i = 0;i < parenCount; i++)
            {
                int start = content.IndexOf('{');
                int end = content.IndexOf('}');
                string inParen = content.Substring(start + 1, end - start - 1);
                if (ParseDictionary.ContainsKey(inParen))
                {
                    content = content.Remove(start, end - start + 1);
                    content = content.Insert(start, ParseDictionary[inParen]);
                }
                else
                {
                    string surrounding = content.Substring(Math.Max(0, start - 10), Math.Min(end + 10, content.Length));
                    error = error + $"「{inParen}」は変換できない言葉です。\n周りの文: {surrounding}\n";
                    return;
                }
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
