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
    internal class RandomSelect : IProcessUnit
    {
        public List<TalkContent> Contents = new List<TalkContent>();
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
                throw new Exception();
            }
            if(element.Element("Selections") != null)
            {
                Contents = new List<TalkContent>();
                foreach (XElement talkContent in element.Element("Selections").Elements())
                {
                    Contents.Add(new TalkContent(talkContent));
                }
            }
        }
        public void Default()
        {
        }
    }
}
