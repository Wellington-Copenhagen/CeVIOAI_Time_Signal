using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CeVIO_AI_時報.UI_Component
{
    // 40x70の大きさ
    internal class VoicePropertyGauge
    {
        Label _nameLabel;
        Button _upButton;
        Label _valueLabel;
        Button _downButton;
        public Action OnValueChanged;
        VoiceProperty _value;
        public VoicePropertyGauge(Point location, VoiceProperty voiceProperty)
        {
            _nameLabel = new Label();
            _nameLabel.Location = location;
            _nameLabel.Size = new Size(40, 12);

            _upButton = new Button();
            _upButton.Location = location + new Size(0, 15);
            _upButton.Size = new Size(40, 18);
            _upButton.Text = "∧";
            _upButton.Click += (sender, e) => OnUpButtonClick();

            _valueLabel = new Label();
            _valueLabel.Location = location + new Size(0, 35);
            _valueLabel.Size = new Size(40, 12);

            _downButton = new Button();
            _downButton.Location = location + new Size(0, 50);
            _downButton.Size = new Size(40, 18);
            _downButton.Text = "∨";
            _downButton.Click += (sender, e) => OnDownButtonClick();

            SetValue(voiceProperty);
        }
        public void SetValue(VoiceProperty value)
        {
            if (value.Value == _value.Value) return;
            _value = value;
            _valueLabel.Text = _value.ValueString();
            OnValueChanged?.Invoke();
        }
        public uint GetValue()
        {
            return _value.Value;
        }
        void OnUpButtonClick()
        {
            _value.Value++;
            SetValue(_value);
        }
        void OnDownButtonClick()
        {
            _value.Value--;
            SetValue(_value);
        }
    }
}
