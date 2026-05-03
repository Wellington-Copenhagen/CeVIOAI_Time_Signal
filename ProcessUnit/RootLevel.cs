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
    internal class RootLevel : IProcessUnit
    {
        List<TimeCondition> _timeConditions;
        public List<TimeCondition> TimeConditions
        {
            get
            {
                Debug.Assert(_timeConditions != null);
                return _timeConditions;
            }
            set
            {
                Debug.Assert(value != null);
                _timeConditions = value;
            }
        }
        public RootLevel()
        {
            Default();
        }
        public RootLevel(XElement element)
        {
            Default();
            LoadFromXML(element);
        }
        public void OnRun()
        {
            foreach (var timeCondition in TimeConditions)
            {
                timeCondition.OnRun();
            }
        }
        public XElement StoreToXML()
        {
            XElement element = new XElement("Root");
            element.Add(new XElement("Type", "Root"));
            element.Add(new XElement("TimeConditions"));
            foreach (TimeCondition timeCondition in TimeConditions)
            {
                element.Element("TimeConditions").Add(timeCondition.StoreToXML());
            }
            return element;
        }
        public void LoadFromXML(XElement element)
        {
            if (element.Element("Type") == null || element.Element("Type").Value != "Root")
            {
                return;
            }
            TimeConditions.Clear();
            if (element.Element("TimeConditions") != null)
            {
                foreach (XElement timeCondition in element.Element("TimeConditions").Elements())
                {
                    TimeConditions.Add(new TimeCondition(timeCondition));
                }
            }
        }
        public void Default()
        {
            TimeConditions = new List<TimeCondition>();
            TimeConditions.Add(new TimeCondition());
        }
    }
}
