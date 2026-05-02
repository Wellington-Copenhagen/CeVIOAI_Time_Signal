using CeVIO_AI_時報.ProcessUnit;
using CeVIO_AI_時報.UI_Component;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CeVIO_AI_時報.GUI
{
    internal class GUI_TimeConditionPart : GUIPart
    {
        TimeCondition _timeCondition;
        List<CheckBox> _dayOfWeekCheckBoxes;
        TimeSelector _timeSelectorStart;
        TimeSelector _timeSelectorEnd;
        NumericUpDown _intervalNumericUpDown;
        Label _intervalLabel;
        public GUI_TimeConditionPart()
        {
            _dayOfWeekCheckBoxes = new List<CheckBox>();
            List<string> dayOfWeekNames = new List<string>() { "月曜日", "火曜日", "水曜日", "木曜日", "金曜日", "土曜日", "日曜日" };
            for (int i = 0; i < 7; i++)
            {
                _dayOfWeekCheckBoxes.Add(new CheckBox());
                _dayOfWeekCheckBoxes.Last().Location = new Point(10, 20 + 20 * i);
                _dayOfWeekCheckBoxes.Last().Size = new Size(60, 16);
                _dayOfWeekCheckBoxes.Last().Text = dayOfWeekNames[i];
                _dayOfWeekCheckBoxes.Last().CheckedChanged += (sender, e) => OnChangedByUser?.Invoke();
                Controls.Add(_dayOfWeekCheckBoxes.Last());
            }
            _timeSelectorStart = new TimeSelector("から");
            _timeSelectorStart.Location = new Point(10, 160);
            _timeSelectorStart.OnValueChanged += () => OnChangedByUser?.Invoke();
            Controls.Add(_timeSelectorStart);

            _timeSelectorEnd = new TimeSelector("まで");
            _timeSelectorEnd.Location = new Point(10, 200);
            _timeSelectorEnd.OnValueChanged += () => OnChangedByUser?.Invoke();
            Controls.Add( _timeSelectorEnd);

            _intervalNumericUpDown = new NumericUpDown();
            _intervalNumericUpDown.Location = new Point(10, 240);
            _intervalNumericUpDown.Size = new Size(60, 20);
            _intervalNumericUpDown.Minimum = 1;
            _intervalNumericUpDown.Maximum = 1440;
            _intervalNumericUpDown.ValueChanged += (sender, e) => OnChangedByUser?.Invoke();
            Controls.Add(_intervalNumericUpDown);

            _intervalLabel = new Label();
            _intervalLabel.Location = new Point(70, 240);
            _intervalLabel.Size = new Size(60, 20);
            _intervalLabel.Text = "分おき";
            Controls.Add(_intervalLabel);

            Disable();
        }
        public void SetDayOfWeeks(List<bool> dayOfWeeks)
        {
            for (int i = 0; i < 7; i++)
            {
                _dayOfWeekCheckBoxes[i].Checked = dayOfWeeks[i];
            }
        }
        public List<bool> GetDayOfWeeks()
        {
            List<bool> dayOfWeeks = new List<bool>();
            for (int i = 0; i < 7; i++)
            {
                dayOfWeeks.Add(_dayOfWeekCheckBoxes[i].Checked);
            }
            return dayOfWeeks;
        }
        protected override void UpdateVisual()
        {
            SetDayOfWeeks(_timeCondition.DayOfWeeks);
            _timeSelectorStart.SetTime(_timeCondition.StartTime);
            _timeSelectorEnd.SetTime(_timeCondition.EndTime);
            _intervalNumericUpDown.Value = _timeCondition.Interval;
            GUI_Components.randomSelectPart.SetProcessUnit(_timeCondition.BindingRandomSelect);
        }
        protected override void UpdateValue()
        {
            if(_timeCondition != null)
            {
                _timeCondition.DayOfWeeks = GetDayOfWeeks();
                _timeCondition.StartTime = _timeSelectorStart.GetTime();
                _timeCondition.EndTime = _timeSelectorEnd.GetTime();
                _timeCondition.Interval = (int)_intervalNumericUpDown.Value;
            }
        }
        protected override void InitWithCeVIO()
        {

        }
        public override void Disable()
        {
            Enabled = false;
            foreach (CheckBox checkBox in _dayOfWeekCheckBoxes)
            {
                checkBox.Enabled = false;
            }
            _timeSelectorStart?.Disable();
            _timeSelectorEnd?.Disable();
            if (_intervalNumericUpDown != null) _intervalNumericUpDown.Enabled = false;
            if (_intervalLabel != null) _intervalLabel.Enabled = false;
        }
        protected override void Enable()
        {
            Enabled = true;
            foreach (CheckBox checkBox in _dayOfWeekCheckBoxes)
            {
                checkBox.Enabled = true;
            }
            _timeSelectorStart.Enable();
            _timeSelectorEnd.Enable();
            _intervalNumericUpDown.Enabled = true;
            _intervalLabel.Enabled = true;
        }
        public void SetProcessUnit(TimeCondition timeCondition)
        {
            if(_timeCondition == timeCondition)
            {
                return;
            }
            _timeCondition = timeCondition;
        }
        protected override bool HasBindingProcessUnit()
        {
            return _timeCondition != null;
        }
    }
}
