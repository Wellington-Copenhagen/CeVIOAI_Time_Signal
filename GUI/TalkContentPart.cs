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
        List<string> _castNames;
        Dictionary<string, List<string>> _emotionNames;
        public GUI_TalkContentPart()
        {
            for(int i = 0;i < 5; i++)
            {
                _voicePropertyGauges.Add(new VoicePropertyGauge());
                _voicePropertyGauges.Last().Location = new Point(10 + 50 * i, 20);
                _voicePropertyGauges.Last().ValueChanged += OnValueChanged;
                Controls.Add(_voicePropertyGauges.Last());
            }
            for (int i = 0; i < 5; i++)
            {
                _emotionGauges.Add(new EmotionGauge());
                _emotionGauges.Last().Location = new Point(10 + 50 * i, 110);
                Controls.Add(_emotionGauges.Last());
                _emotionGauges.Last().ValueChanged += OnValueChanged;
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
            _castsListBox.SelectedIndexChanged += OnValueChanged;
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
            _contentTextBox.TextChanged += OnValueChanged;
            Controls.Add(_contentTextBox);

            Disable();
        }
        public void UpdateEmotionLabel()
        {
            _castsListBox.Items.Clear();
            foreach (string castName in _castNames)
            {
                _castsListBox.Items.Add(castName);
            }
            if (!_castsListBox.Items.Contains(_talkContent.Cast))
            {
                _castsListBox.Items.Add(_talkContent.Cast);
            }
            if (_castsListBox.Items.Count > 0)
            {
                _castsListBox.SelectedIndex = _castsListBox.Items.IndexOf(_talkContent.Cast);
            }

            if (_emotionNames.ContainsKey(_talkContent.Cast))
            {
                for (int i = 0; i < _emotionNames[_talkContent.Cast].Count; i++)
                {
                    _emotionGauges[i].SetName(_emotionNames[_talkContent.Cast][i]);
                }
                for (int i = _emotionNames[_talkContent.Cast].Count; i < 5; i++)
                {
                    _emotionGauges[i].SetName("");
                }
            }
        }
        public override void UpdateValue()
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
        public override void InitWithCeVIO()
        {
            _castNames = Talker2.AvailableCasts.ToList();
            _emotionNames = new Dictionary<string, List<string>>();
            foreach (string castName in _castNames)
            {
                Talker2 talker = new Talker2();
                talker.Cast = castName;
                _emotionNames[castName] = talker.Components.Select(
                    (component) => component.Name
                    ).ToList();
            }

            foreach (string castName in _castNames)
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

            _castsListBox.Enabled = false;
            _castsListBox.Items.Clear();

            for (int i = 0;i < 5;i++)
            {
                _emotionGauges[i].Disable();
            }
            for (int i = 0; i < 5; i++)
            {
                _voicePropertyGauges[i].Disable();
            }
            _contentTextBox.Enabled = false;
            _contentTextBox.Text = "";

            _testTalkButton.Enabled = false;
        }
        public override void Enable()
        {
            Enabled = true;

            _castsListBox.Enabled = true;
            _castsListBox.Items.Clear();
            foreach (string castName in _castNames)
            {
                _castsListBox.Items.Add(castName);
            }
            if (!_castsListBox.Items.Contains(_talkContent.Cast))
            {
                _castsListBox.Items.Add(_talkContent.Cast);
                MessageBox.Show($"{_talkContent.Cast}はライセンスを取得していないキャストです。");
            }
            if (_castsListBox.Items.Count > 0)
            {
                _castsListBox.SelectedIndex = _castsListBox.Items.IndexOf(_talkContent.Cast);
            }

            if (_emotionNames.ContainsKey(_talkContent.Cast))
            {
                for (int i = 0; i < _emotionNames[_talkContent.Cast].Count; i++)
                {
                    _emotionGauges[i].SetName(_emotionNames[_talkContent.Cast][i]);
                    _emotionGauges[i].SetValue(_talkContent.Emotions[i]);
                }
                for (int i = _emotionNames[_talkContent.Cast].Count; i < 5; i++)
                {
                    _emotionGauges[i].Disable();
                }
            }
            else
            {
                for (int i = 0; i < 5; i++)
                {
                    _emotionGauges[i].Disable();
                }
            }

            for (int i = 0; i < 5; i++)
            {
                _voicePropertyGauges[i].SetValue(_talkContent.Properties[i]);
            }

            _testTalkButton.Enabled = true;

            _contentTextBox.Enabled = true;
            _contentTextBox.Text = _talkContent.Content;
        }
        public void SetProcessUnit(TalkContent talkContent)
        {
            if(_talkContent == talkContent)
            {
                return;
            }
            _talkContent = talkContent;
            UpdateUI(true);
        }
        public override bool HasBindingProcessUnit()
        {
            return _talkContent != null;
        }
        void OnTestTalkClicked(object sender, EventArgs e)
        {
            if (_talkContent != null && !GUI_Components.voiceOutput.Talking)
            {
                _talkContent.OnRun();
            }
        }
    }
}
