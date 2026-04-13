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
    internal class TimeCondition : IProcessUnit, IListboxAddDeletableComponent
    {
        List<bool> _dayOfWeeks;
        DateTime _startTime;
        DateTime _endTime;
        int _interval;
        DateTime _lastTimeSpan;
        string _name;
        RandomSelect _bindingRandomSelect;
        public TimeCondition(XElement element)
        {
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
            DateTime now = DateTime.Now;
            if ((now - _lastTimeSpan).TotalMinutes == 0)
            {
                return;
            }
            if (SatisfyIntervalCondition() && SatisfyTimeRangeCondition())
            {
                _bindingRandomSelect.OnRun();
            }
            _lastTimeSpan = now;
        }
        public void OnLoadToGUI()
        {
            GUI_Components.timeConditionPart.OnChanged = null;
            GUI_Components.timeConditionPart.SetDayOfWeeks(_dayOfWeeks);
            GUI_Components.timeConditionPart.timeSelectorStart.SetTime(_startTime);
            GUI_Components.timeConditionPart.timeSelectorEnd.SetTime(_endTime);
            GUI_Components.timeConditionPart.intervalNumericUpDown.Value = _interval;
            GUI_Components.timeConditionPart.OnChanged = OnChanged;
        }
        public void OnChanged()
        {
            _dayOfWeeks = GUI_Components.timeConditionPart.GetDayOfWeeks();
            _startTime = GUI_Components.timeConditionPart.timeSelectorStart.GetTime();
            _endTime = GUI_Components.timeConditionPart.timeSelectorEnd.GetTime();
            _interval = (int)GUI_Components.timeConditionPart.intervalNumericUpDown.Value;
        }

        public XElement StoreToXML()
        {
            XElement element = new XElement("TimeCondition");
            element.Add(new XElement("Name", _name));
            XElement dayOfWeek = new XElement("DayOfWeeks");
            for(int i = 0; i < 7; i++)
            {
                dayOfWeek.Add(new XElement("DayOfWeek", _dayOfWeeks[i]));
            }
            element.Add(dayOfWeek);
            element.Add(new XElement("StartTime", _startTime));
            element.Add(new XElement("EndTime", _endTime));
            element.Add(new XElement("Interval", _interval));
            element.Add(_bindingRandomSelect.StoreToXML());
            return element;
        }
        public void LoadFromXML(XElement element)
        {
            if(element.Element("Type") == null || element.Element("Type").Value != "Time")
            {
                throw new Exception();
            }
            _name = element.Attribute("Name").Value;
            if(element.Attribute("DayOfWeek") != null)
            {
                _dayOfWeeks = element.Element("DayOfWeeks").Elements("DayOfWeek").Select(x => bool.Parse(x.Value)).ToList();
            }
            if(element.Element("DayOfWeeks") != null)
            {
                ParseTFDayOfWeek(element.Element("DayOfWeeks").Value);
            }
            _startTime = DateTime.Parse(element.Element("StartTime").Value);
            _endTime = DateTime.Parse(element.Element("EndTime").Value);
            _interval = int.Parse(element.Element("Interval").Value);
            if(element.Element("_0") == null)
            {
                _bindingRandomSelect = new RandomSelect(element.Element("_0"));
            }
            if (element.Element("RandomSelect") == null)
            {
                _bindingRandomSelect = new RandomSelect(element.Element("RandomSelect"));
            }
        }

        public bool TodayApproved()
        {
            DateTime now = DateTime.Now;
            return _dayOfWeeks[(int)now.DayOfWeek];
        }
        public bool TomorrowApproved()
        {
            DateTime now = DateTime.Now;
            int dayOfWeek = (int)now.DayOfWeek + 1;
            if (dayOfWeek == 7) dayOfWeek = 0;
            return _dayOfWeeks[dayOfWeek];
        }
        public bool SatisfyTimeRangeCondition()
        {
            DateTime now = DateTime.Now;
            if (_startTime <= _endTime)
            {
                if (TodayApproved() && now.TimeOfDay >= _startTime.TimeOfDay && now.TimeOfDay <= _endTime.TimeOfDay)
                {
                    return true;
                }
            }
            else
            {
                if ((TodayApproved() && now.TimeOfDay >= _startTime.TimeOfDay) ||
                    (TomorrowApproved() && now.TimeOfDay <= _endTime.TimeOfDay))
                {
                    return true;
                }
            }
            return false;
        }
        public bool SatisfyIntervalCondition()
        {
            DateTime now = DateTime.Now;
            if ((now - _startTime).TotalMinutes % _interval == 0)
            {
                return true;
            }
            return false;
        }
        void ParseTFDayOfWeek(string dayOfWeekString)
        {
            for(int i = 0; i < 7; i++)
            {
                if (dayOfWeekString[i] == 'T')
                {
                    _dayOfWeeks[i] = true;
                }
                else
                {
                    _dayOfWeeks[i] = false;
                }
            }
    }
}
}
