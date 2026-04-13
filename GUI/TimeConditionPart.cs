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
    internal class GUI_TimeConditionPart : GUI_Part
    {
        public List<CheckBox> dayOfWeekCheckBoxes;
        public TimeSelector timeSelectorStart;
        public TimeSelector timeSelectorEnd;
        public NumericUpDown intervalNumericUpDown;
        Label _intervalLabel;
        public Action OnChanged;
        public GUI_TimeConditionPart(Point location)
        {
            dayOfWeekCheckBoxes = new List<CheckBox>();
            List<string> dayOfWeekNames = new List<string>() { "月曜日", "火曜日", "水曜日", "木曜日", "金曜日", "土曜日", "日曜日" };
            for (int i = 0; i < 7; i++)
            {
                dayOfWeekCheckBoxes.Add(new CheckBox());
                dayOfWeekCheckBoxes.Last().Location = location + new Size(20 * i, 0);
                dayOfWeekCheckBoxes.Last().Size = new Size(60, 16);
                dayOfWeekCheckBoxes.Last().Text = dayOfWeekNames[i];
                dayOfWeekCheckBoxes.Last().CheckedChanged += (sender, e) => OnChanged?.Invoke();
            }
            timeSelectorStart = new TimeSelector(location + new Size(80, 0), "から");
            timeSelectorStart.OnValueChanged += () => OnChanged?.Invoke();
            timeSelectorEnd = new TimeSelector(location + new Size(80, 40), "まで");
            timeSelectorEnd.OnValueChanged += () => OnChanged?.Invoke();

            intervalNumericUpDown = new NumericUpDown();
            intervalNumericUpDown.Location = location + new Size(80, 80);
            intervalNumericUpDown.Size = new Size(60, 20);
            intervalNumericUpDown.Minimum = 1;
            intervalNumericUpDown.Maximum = 1440;
            intervalNumericUpDown.ValueChanged += (sender, e) => OnChanged?.Invoke();

            _intervalLabel = new Label();
            _intervalLabel.Location = location + new Size(140, 80);
            _intervalLabel.Size = new Size(60, 60);
            _intervalLabel.Text = "分おき";
        }
        public void SetDayOfWeeks(List<bool> dayOfWeeks)
        {
            for (int i = 0; i < 7; i++)
            {
                dayOfWeekCheckBoxes[i].Checked = dayOfWeeks[i];
            }
        }
        public List<bool> GetDayOfWeeks()
        {
            List<bool> dayOfWeeks = new List<bool>();
            for (int i = 0; i < 7; i++)
            {
                dayOfWeeks.Add(dayOfWeekCheckBoxes[i].Checked);
            }
            return dayOfWeeks;
        }
    }
}
