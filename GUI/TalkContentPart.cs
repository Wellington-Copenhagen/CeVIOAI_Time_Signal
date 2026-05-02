using CeVIO.Talk.RemoteService2;
using CeVIO_AI_時報.ProcessUnit;
using CeVIO_AI_時報.UI_Component;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CeVIO_AI_時報.GUI
{
    // 400x400の大きさ
    internal class GUI_TalkContentPart : GUIPart
    {
        TalkContent _talkContent;
        List<EmotionGauge> _emotionGauges = new List<EmotionGauge>();
        List<VoicePropertyGauge> _voicePropertyGauges = new List<VoicePropertyGauge>();
        TextBox _contentTextBox;
        ListBox _castsListBox;
        Button _testTalkButton;
        Label _contentTextboxLabel;
        public GUI_TalkContentPart()
        {
            for(int i = 0;i < 5; i++)
            {
                _voicePropertyGauges.Add(new VoicePropertyGauge());
                _voicePropertyGauges.Last().Location = new Point(10 + 50 * i, 20);
                _voicePropertyGauges.Last().OnValueChanged += () => OnChangedByUser?.Invoke();
                Controls.Add(_voicePropertyGauges.Last());
            }
            for (int i = 0; i < 5; i++)
            {
                _emotionGauges.Add(new EmotionGauge());
                _emotionGauges.Last().Location = new Point(10 + 50 * i, 110);
                Controls.Add(_emotionGauges.Last());
                _emotionGauges.Last().OnValueChanged += () => OnChangedByUser?.Invoke();
            }

            _testTalkButton = new Button();
            _testTalkButton.Text = "試聴";
            _testTalkButton.Location = new Point(260, 20);
            _testTalkButton.Size = new Size(70, 30);
            _testTalkButton.Click += OnTestTalkClicked;
            Controls.Add(_testTalkButton);

            _castsListBox = new ListBox();
            _castsListBox.Location = new Point(260, 50);
            _castsListBox.Size = new Size(150, 150);
            _castsListBox.SelectedIndexChanged += (sender, e) =>
            {
                OnChangedByUser?.Invoke();
            };
            Controls.Add(_castsListBox);

            _contentTextboxLabel = new Label();
            _contentTextboxLabel.Location = new Point(10, 200);
            _contentTextboxLabel.Size = new Size(100, 20);
            _contentTextboxLabel.Text = "話す内容";
            Controls.Add(_contentTextboxLabel);

            _contentTextBox = new TextBox();
            _contentTextBox.Location = new Point(10, 220);
            _contentTextBox.Multiline = true;
            _contentTextBox.Size = new Size(400, 160);
            _contentTextBox.TextChanged += (sender, e) => OnChangedByUser?.Invoke();
            Controls.Add(_contentTextBox);

            Disable();
        }
        public void UpdateEmotionLabel()
        {
            if (ServiceControl2.IsHostStarted)
            {
                Talker2 talker2 = new Talker2(_castsListBox.SelectedItem.ToString());
                for (int i = 0; i < 5; i++)
                {
                    _emotionGauges[i].SetName(talker2.Components[i].Name);
                }
            }
        }
        protected override void UpdateVisual()
        {
            if(_talkContent != null)
            {
                Enable();
                for (int i = 0; i < 5; i++)
                {
                    if (i < _talkContent.Emotions.Count)
                    {
                        _emotionGauges[i].SetValue(_talkContent.Emotions[i], true);
                    }
                    else
                    {
                        _emotionGauges[i].SetValue(0, false);
                    }
                }
                UpdateEmotionLabel();
                for (int i = 0; i < 5; i++)
                {
                    _voicePropertyGauges[i].SetValue(_talkContent.Properties[i]);
                }
                _contentTextBox.Text = _talkContent.Content;
            }
            else
            {
                Disable();
            }
        }
        protected override void UpdateValue()
        {
            if (_talkContent != null)
            {
                for (int i = 0; i < 5; i++)
                {
                    _talkContent.Properties[i].Value = _voicePropertyGauges[i].GetValue();
                }
                for (int i = 0; i < 5; i++)
                {
                    if (i < _talkContent.Emotions.Count)
                    {
                        _talkContent.Emotions[i] = _emotionGauges[i].GetValue();
                    }
                }
                _talkContent.Cast = _castsListBox.SelectedItem.ToString();
                _talkContent.Content = _contentTextBox.Text;
            }
        }
        protected override void InitWithCeVIO()
        {
            List<string> castNames = Talker2.AvailableCasts.ToList();
            foreach (string castName in castNames)
            {
                _castsListBox.Items.Add(castName);
            }
            if (_castsListBox.Items.Count > 0)
            {
                _castsListBox.SelectedIndex = 0;
            }
        }
        public override void Disable()
        {
            Enabled = false;
            for(int i = 0;i < 5;i++)
            {
                _emotionGauges[i].Disable();
            }
            for (int i = 0; i < 5; i++)
            {
                _voicePropertyGauges[i].Disable();
            }
            _contentTextBox.Enabled = false;
            _castsListBox.Enabled = false;
            _testTalkButton.Enabled = false;
        }
        protected override void Enable()
        {
            Enabled = true;
            for (int i = 0; i < 5; i++)
            {
                _emotionGauges[i].Enable();
            }
            for (int i = 0; i < 5; i++)
            {
                _voicePropertyGauges[i].Enable();
            }
            _contentTextBox.Enabled = true;
            _castsListBox.Enabled = true;
            _testTalkButton.Enabled = true;
        }
        public void SetProcessUnit(TalkContent talkContent)
        {
            if(_talkContent == talkContent)
            {
                return;
            }
            _talkContent = talkContent;
        }
        protected override bool HasBindingProcessUnit()
        {
            return _talkContent != null;
        }
        void OnTestTalkClicked(object sender, EventArgs e)
        {
            if (_talkContent != null)
            {
                _talkContent.OnRun();
            }
        }
    }
}
