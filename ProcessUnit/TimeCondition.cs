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
        public List<bool> DayOfWeeks;
        public DateTime StartTime;
        public DateTime EndTime;
        public int Interval;
        public DateTime LastTimeSpan;
        string _name;
        public RandomSelect BindingRandomSelect;
        public TimeCondition()
        {
            Default();
        }
        public TimeCondition(XElement element)
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
            DateTime now = DateTime.Now;
            if ((now - LastTimeSpan).TotalMinutes < 1)
            {
                return;
            }
            if (SatisfyIntervalCondition() && SatisfyTimeRangeCondition())
            {
                BindingRandomSelect.OnRun();
            }
            LastTimeSpan = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day,
                DateTime.Now.Hour, DateTime.Now.Minute, 0);
        }

        public XElement StoreToXML()
        {
            XElement element = new XElement("TimeCondition");
            element.Add(new XElement("Name", _name));
            element.Add(new XElement("Type", "Time"));
            XElement dayOfWeek = new XElement("DayOfWeeks");
            for(int i = 0; i < 7; i++)
            {
                dayOfWeek.Add(new XElement("DayOfWeek", DayOfWeeks[i]));
            }
            element.Add(dayOfWeek);
            element.Add(new XElement("StartTime", StartTime.Ticks));
            element.Add(new XElement("EndTime", EndTime.Ticks));
            element.Add(new XElement("Interval", Interval));
            element.Add(BindingRandomSelect.StoreToXML());
            return element;
        }
        public void LoadFromXML(XElement element)
        {
            if(element.Element("Type") == null || element.Element("Type").Value != "Time")
            {
                throw new Exception();
            }
            _name = element.Element("Name").Value;
            if (element.Element("DayOfWeeks") != null)
            {
                DayOfWeeks = element.Element("DayOfWeeks").Elements("DayOfWeek").Select(x => bool.Parse(x.Value)).ToList();
            }
            if(element.Element("StartTime") != null)
            {
                StartTime = new DateTime(long.Parse(element.Element("StartTime").Value));
            }
            if(element.Element("EndTime") != null)
            {
                EndTime = new DateTime(long.Parse(element.Element("EndTime").Value));
            }
            if(element.Element("Interval") != null)
            {
                Interval = int.Parse(element.Element("Interval").Value);
            }
            if (element.Element("RandomSelect") != null)
            {
                BindingRandomSelect = new RandomSelect(element.Element("RandomSelect"));
            }
        }
        public void Default()
        {
            _name = "New";
            DayOfWeeks = new List<bool>() { true, true, true, true, true, true, true };
            StartTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);
            EndTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59);
            Interval = 60;
            LastTimeSpan = DateTime.MinValue;
            BindingRandomSelect = new RandomSelect();
        }

        public bool TodayApproved()
        {
            DateTime now = DateTime.Now;
            return DayOfWeeks[(int)now.DayOfWeek];
        }
        public bool TomorrowApproved()
        {
            DateTime now = DateTime.Now;
            int dayOfWeek = (int)now.DayOfWeek + 1;
            if (dayOfWeek == 7) dayOfWeek = 0;
            return DayOfWeeks[dayOfWeek];
        }
        public bool SatisfyTimeRangeCondition()
        {
            DateTime now = DateTime.Now;
            if (StartTime <= EndTime)
            {
                if (TodayApproved() && now.TimeOfDay >= StartTime.TimeOfDay && now.TimeOfDay <= EndTime.TimeOfDay)
                {
                    return true;
                }
            }
            else
            {
                if ((TodayApproved() && now.TimeOfDay >= StartTime.TimeOfDay) ||
                    (TomorrowApproved() && now.TimeOfDay <= EndTime.TimeOfDay))
                {
                    return true;
                }
            }
            return false;
        }
        public bool SatisfyIntervalCondition()
        {
            DateTime now = DateTime.Now;
            if ((int)(now - StartTime).TotalMinutes % Interval < 1)
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
                    DayOfWeeks[i] = true;
                }
                else
                {
                    DayOfWeeks[i] = false;
                }
            }
    }
}
}
