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
    internal class EmotionGauge : UIComponent
    {
        TrackBar _trackBar;
        Label _emotionNameLabel;
        Label _emotionValueLabel;
        public EmotionGauge()
        {
            Size = new Size(45, 90);

            _emotionNameLabel = new Label();
            _emotionNameLabel.Location = new Point(0, 0);
            _emotionNameLabel.Size = new Size(45, 12);
            Controls.Add(_emotionNameLabel);

            _emotionValueLabel = new Label();
            _emotionValueLabel.Location = new Point(0, 15);
            _emotionValueLabel.Size = new Size(45, 12);
            Controls.Add(_emotionValueLabel);

            _trackBar = new TrackBar();
            _trackBar.Location = new Point(0, 30);
            _trackBar.Size = new Size(45, 60);
            _trackBar.Minimum = 0;
            _trackBar.Maximum = 100;
            _trackBar.Scroll += OnGaugeMove;
            _trackBar.Orientation = Orientation.Vertical;
            Controls.Add(_trackBar);

            SetValue(0);
        }
        public void SetName(string name)
        {
            _emotionNameLabel.Text = name;
        }
        public void SetValue(uint value)
        {
            _trackBar.ValueChanged -= OnGaugeMove;
            _trackBar.Value = (int)value;
            _emotionValueLabel.Text = value.ToString();
            _trackBar.ValueChanged += OnGaugeMove;
        }
        public uint GetValue()
        {
            return (uint)_trackBar.Value;
        }
        void OnGaugeMove(object sender, EventArgs e)
        {
            ValueChanged?.Invoke();
        }
        public override void Enable()
        {
            _trackBar.Enabled = true;
            _emotionNameLabel.Enabled = true;
            _emotionValueLabel.Enabled = true;
        }
        public override void Disable()
        {
            _trackBar.Enabled = false;
            _emotionNameLabel.Enabled = false;
            _emotionValueLabel.Enabled = false;
        }
    }
}
