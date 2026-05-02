using CeVIO_AI_時報.GUI;
using CeVIO_AI_時報.UI_Component;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace CeVIO_AI_時報.ProcessUnit
{
    internal class RandomSelect : IProcessUnit
    {
        List<TalkContent> _contents;
        public List<TalkContent> Contents
        {
            get
            {
                Debug.Assert(_contents != null);
                return _contents;
            }
            set
            {
                Debug.Assert(value != null);
                _contents = value;
            }
        }
        public RandomSelect(XElement element)
        {
            Default();
            LoadFromXML(element);
        }
        public RandomSelect()
        {
            Default();
        }
        public void OnRun()
        {
            Random rand = new Random();
            int index = rand.Next(0, Contents.Count);
            Contents[index].OnRun();
        }
        public XElement StoreToXML()
        {
            XElement element = new XElement("RandomSelect");
            element.Add(new XElement("Type", "Random"));
            element.Add(new XElement("Selections"));
            foreach (TalkContent content in Contents)
            {
                element.Element("Selections").Add(content.StoreToXML());
            }
            return element;
        }
        public void LoadFromXML(XElement element)
        {
            if (element.Element("Type") == null || element.Element("Type").Value != "Random")
            {
                return;
            }
            if(element.Element("Selections") != null)
            {
                foreach (XElement talkContent in element.Element("Selections").Elements())
                {
                    Contents.Add(new TalkContent(talkContent));
                }
            }
        }
        public void Default()
        {
            Contents = new List<TalkContent>();
        }
    }
}
