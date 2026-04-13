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
    internal class RootLevel : IProcessUnit
    {
        List<TimeCondition> _timeConditions;
        public RootLevel(XElement element)
        {
            LoadFromXML(element);
        }
        public void OnRun()
        {
            foreach (var timeCondition in _timeConditions)
            {
                timeCondition.OnRun();
            }
        }
        public XElement StoreToXML()
        {
            XElement element = new XElement("RootLecel");
            element.Add(new XElement("TimeConditions"));
            foreach (TimeCondition timeCondition in _timeConditions)
            {
                element.Element("TimeConditions").Add(timeCondition.StoreToXML());
            }
            return element;
        }
        public void LoadFromXML(XElement element)
        {
            if (element.Element("Type") == null || element.Element("Type").Value != "Root")
            {
                throw new Exception();
            }
            _timeConditions = new List<TimeCondition>();
            int i = 0;
            while (element.Element("_" + i.ToString()) == null)
            {
                TimeCondition timeCondition = new TimeCondition(element.Element("_" + i.ToString()));
                _timeConditions.Add(timeCondition);
            }
            if (element.Element("TimeConditions") != null)
            {
                foreach (XElement timeCondition in element.Element("TimeConditions").Elements())
                {
                    _timeConditions.Add(new TimeCondition(timeCondition));
                }
            }
        }
        public void OnLoadToGUI()
        {
            GUI_Components.rootPart.OnChanged = null;
            List<IListboxAddDeletableComponent> casted = new List<IListboxAddDeletableComponent>();
            for (int i = 0; i < _timeConditions.Count; i++)
            {
                casted.Add(_timeConditions[i]);
            }
            GUI_Components.rootPart.listBox.SetComponents(casted);
            GUI_Components.rootPart.OnChanged = OnChanged;
        }
        void OnChanged()
        {
            _timeConditions[GUI_Components.rootPart.listBox.SelectedIndex()].OnLoadToGUI();
        }
    }
}
