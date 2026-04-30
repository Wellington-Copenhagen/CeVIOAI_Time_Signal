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
        public List<VoiceProperty> Properties = new List<VoiceProperty>();
        public List<uint> Emotions;
        public string Cast;
        public string Content;
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
            ContentParser.Parse(parsed, out string error);
            GUI_Components.voiceOutput.Content = parsed;
            GUI_Components.voiceOutput.AlternativeText = error;

            GUI_Components.voiceOutput.Talk();
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
                throw new Exception();
            }
            _name = element.Element("Name").Value;
            if(element.Element("VoiceProperties") != null)
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
            Properties.Add(new Volume());
            Properties.Add(new Speed());
            Properties.Add(new Tone());
            Properties.Add(new Alpha());
            _name = "New";
            Properties = new List<VoiceProperty>() { new Volume(), new Speed(), new Tone(), new Alpha(), new ToneScale() };
            Emotions = new List<uint>() { 0, 0, 0, 0, 0 };
            Cast = Talker2.AvailableCasts[0];
            Content = "";
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
