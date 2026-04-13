using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CeVIO_AI_時報.UI_Component
{
    // 45x90の大きさ
    internal class EmotionGauge
    {
        TrackBar _trackBar;
        Label _emotionNameLabel;
        Label _emotionValueLabel;
        public Action OnValueChanged;
        public EmotionGauge(Point location)
        {
            _emotionNameLabel = new Label();
            _emotionNameLabel.Location = location;
            _emotionNameLabel.Size = new Size(45, 12);

            _emotionValueLabel = new Label();
            _emotionValueLabel.Location = location + new Size(0, 15);
            _emotionValueLabel.Size = new Size(45, 12);

            _trackBar = new TrackBar();
            _trackBar.Location = location + new Size(0, 30);
            _trackBar.Size = new Size(45, 60);
            _trackBar.Minimum = 0;
            _trackBar.Maximum = 100;
            _trackBar.Scroll += (sender, e) => OnGaugeMove();
            _trackBar.Orientation = Orientation.Vertical;

            SetValue(0, false);
        }
        public void SetName(string name)
        {
            _emotionNameLabel.Text = name;
        }
        public void SetValue(uint value, bool enabled)
        {
            _trackBar.Enabled = enabled;
            _emotionNameLabel.Enabled = enabled;
            _emotionValueLabel.Enabled = enabled;
            if (enabled)
            {
                _trackBar.Value = (int)value;
                _emotionValueLabel.Text = value.ToString();
                OnValueChanged?.Invoke();
            }
        }
        public uint GetValue()
        {
            return (uint)_trackBar.Value;
        }
        void OnGaugeMove()
        {
            OnValueChanged?.Invoke();
        }
    }
}
