using CeVIO.Talk.RemoteService2;
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
    // 400x400の大きさ
    internal class GUI_TalkContentPart : GUI_Part
    {
        public List<EmotionGauge> emotionGauges;
        public VoicePropertyGauge volumeGauge;
        public VoicePropertyGauge speedGauge;
        public VoicePropertyGauge toneGauge;
        public VoicePropertyGauge alphaGauge;
        public VoicePropertyGauge toneScaleGauge;
        public TextBox contentTextBox;
        public ListBox castsListBox;
        public Action OnChanged;
        public GUI_TalkContentPart(Point location)
        {
            volumeGauge = new VoicePropertyGauge(location + new Size(0, 0), new Volume());
            volumeGauge.OnValueChanged += () => OnChanged?.Invoke();
            speedGauge = new VoicePropertyGauge(location + new Size(50, 0), new Speed());
            speedGauge.OnValueChanged += () => OnChanged?.Invoke();
            toneGauge = new VoicePropertyGauge(location + new Size(100, 0), new Tone());
            toneGauge.OnValueChanged += () => OnChanged?.Invoke();
            alphaGauge = new VoicePropertyGauge(location + new Size(150, 0), new Alpha());
            alphaGauge.OnValueChanged += () => OnChanged?.Invoke();
            toneScaleGauge = new VoicePropertyGauge(location + new Size(200, 0), new ToneScale());
            toneScaleGauge.OnValueChanged += () => OnChanged?.Invoke();
            for (int i = 0; i < 5; i++)
            {
                emotionGauges.Add(new EmotionGauge(location + new Size(50 * i, 90)));
                emotionGauges.Last().OnValueChanged += () => OnChanged?.Invoke();
            }

            castsListBox = new ListBox();
            castsListBox.Location = location + new Size(250, 0);
            castsListBox.Size = new Size(150, 180);
            castsListBox.SelectedIndexChanged += (sender, e) =>
            {
                OnChanged?.Invoke();
                OnCastChanged();
            };


            contentTextBox = new TextBox();
            contentTextBox.Location = location + new Size(0, 180);
            contentTextBox.Size = new Size(400, 220);
            contentTextBox.TextChanged += (sender, e) => OnChanged?.Invoke();
        }
        public void OnCeVIOLoad()
        {
            List<string> castNames = Talker2.AvailableCasts.ToList();
            foreach (string castName in castNames)
            {
                castsListBox.Items.Add(castName);
            }
            UpdateEmotionLabel();
        }
        public void UpdateEmotionLabel()
        {
            if (ServiceControl2.IsHostStarted)
            {
                Talker2 talker2 = new Talker2(castsListBox.SelectedItem.ToString());
                for (int i = 0; i < 5; i++)
                {
                    emotionGauges[i].SetName(talker2.Components[i].Name);
                }
            }
        }
        public void OnCastChanged()
        {
            UpdateEmotionLabel();
        }
    }
}
