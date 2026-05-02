using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CeVIO_AI_時報.UI_Component
{
    // 200x20の大きさ
    internal class TimeSelector : UserControl
    {
        NumericUpDown _hourSelector;
        Label _hourLabel;
        NumericUpDown _minuteSelector;
        Label _minuteLabel;
        public Action OnValueChanged;
        public TimeSelector(string suffix)
        {
            Size = new Size(200, 20);

            _hourSelector = new NumericUpDown();
            _hourSelector.Location = new Point(0, 0);
            _hourSelector.Size = new Size(60, 20);
            _hourSelector.Minimum = 0;
            _hourSelector.Maximum = 23;
            _hourSelector.ValueChanged += (sender, e) => OnValueChanged?.Invoke();
            Controls.Add(_hourSelector);

            _hourLabel = new Label();
            _hourLabel.Location = new Point(60, 0);
            _hourLabel.Size = new Size(20, 20);
            _hourLabel.Text = "時";
            Controls.Add(_hourLabel);

            _minuteSelector = new NumericUpDown();
            _minuteSelector.Location = new Point(80, 0);
            _minuteSelector.Size = new Size(60, 20);
            _minuteSelector.Minimum = 0;
            _minuteSelector.Maximum = 59;
            _minuteSelector.ValueChanged += (sender, e) => OnValueChanged?.Invoke();
            Controls.Add(_minuteSelector);

            _minuteLabel = new Label();
            _minuteLabel.Location = new Point(140, 0);
            _minuteLabel.Size = new Size(60, 20);
            _minuteLabel.Text = "分" + suffix;
            Controls.Add(_minuteLabel);
        }
        public DateTime GetTime()
        {
            return new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, (int)_hourSelector.Value, (int)_minuteSelector.Value, 0);

        }
        public void SetTime(DateTime time)
        {
            _hourSelector.Value = time.Hour;
            _minuteSelector.Value = time.Minute;
        }
        public void Disable()
        {
            _hourSelector.Enabled = false;
            _minuteSelector.Enabled = false;
        }
        public void Enable()
        {
            _hourSelector.Enabled = true;
            _minuteSelector.Enabled = true;
        }
    }
}
