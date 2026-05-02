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
    internal class VoicePropertyGauge : UIComponent
    {
        Label _nameLabel;
        Button _upButton;
        Label _valueLabel;
        Button _downButton;
        VoiceProperty _value;
        public VoicePropertyGauge()
        {
            Size = new Size(40, 68);

            _nameLabel = new Label();
            _nameLabel.Location = new Point(0, 0);
            _nameLabel.Size = new Size(40, 12);
            Controls.Add(_nameLabel);

            _upButton = new Button();
            _upButton.Location = new Point(0, 15);
            _upButton.Size = new Size(40, 18);
            _upButton.Text = "∧";
            _upButton.Click += (sender, e) => OnUpButtonClick();
            Controls.Add(_upButton);

            _valueLabel = new Label();
            _valueLabel.Location = new Point(0, 35);
            _valueLabel.Size = new Size(40, 12);
            Controls.Add(_valueLabel);

            _downButton = new Button();
            _downButton.Location = new Point(0, 50);
            _downButton.Size = new Size(40, 18);
            _downButton.Text = "∨";
            _downButton.Click += (sender, e) => OnDownButtonClick();
            Controls.Add(_downButton);
        }
        public void SetValue(VoiceProperty value)
        {
            _value = value;
            _valueLabel.Text = _value.ValueString();
            _nameLabel.Text = _value.NameJa();
        }
        public uint GetValue()
        {
            return _value.Value;
        }
        void OnUpButtonClick()
        {
            _value.Value++;
            SetValue(_value);
            ValueChanged?.Invoke();
        }
        void OnDownButtonClick()
        {
            _value.Value--;
            SetValue(_value);
            ValueChanged?.Invoke();
        }
        public override void Enable()
        {
            _upButton.Enabled = true;
            _downButton.Enabled = true;
            _nameLabel.Enabled = true;
            _valueLabel.Enabled = true;
        }
        public override void Disable()
        {
            _upButton.Enabled = false;
            _downButton.Enabled = false;
            _nameLabel.Enabled = false;
            _valueLabel.Enabled = false;
        }
    }
}
