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
        List<TalkContent> _contents;
        public RandomSelect(XElement element)
        {
            LoadFromXML(element);
        }
        public void OnRun()
        {
            Random rand = new Random();
            int index = rand.Next(0, _contents.Count);
            _contents[index].OnRun();
        }
        public XElement StoreToXML()
        {
            XElement element = new XElement("RandomSelect");
            element.Add(new XElement("Selections"));
            foreach (TalkContent content in _contents)
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
            _contents = new List<TalkContent>();
            int i = 0;
            while(element.Element("_" + i.ToString()) == null)
            {
                TalkContent talkContent = new TalkContent(element.Element("_" + i.ToString()));
                _contents.Add(talkContent);
            }
            if(element.Element("Selections") != null)
            {
                foreach (XElement talkContent in element.Element("Selections").Elements())
                {
                    _contents.Add(new TalkContent(talkContent));
                }
            }
        }
        public void OnLoadToGUI()
        {
            GUI_Components.randomSelectPart.OnChanged = null;
            List<IListboxAddDeletableComponent> casted = new List<IListboxAddDeletableComponent>();
            for (int i = 0; i < _contents.Count; i++)
            {
                casted.Add(_contents[i]);
            }
            GUI_Components.randomSelectPart.talkContentListBox.SetComponents(casted);
            GUI_Components.randomSelectPart.OnChanged = OnChanged;
        }
        void OnChanged()
        {
            _contents[GUI_Components.randomSelectPart.talkContentListBox.SelectedIndex()].OnLoadToGUI();
        }
    }
}
