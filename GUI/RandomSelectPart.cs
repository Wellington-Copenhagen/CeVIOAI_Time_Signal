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
    // 120x340の大きさ
    internal class GUI_RandomSelectPart : GUIPart
    {
        RandomSelect _randomSelect;
        ListBoxAddDeletable<TalkContent> _talkContentListBox;
        public GUI_RandomSelectPart()
        {
            _talkContentListBox = new ListBoxAddDeletable<TalkContent>();
            _talkContentListBox.Location = new Point(10, 20);
            Controls.Add(_talkContentListBox);

            _talkContentListBox.ValueChanged = OnValueChanged;

            Disable();
        }
        public override void UpdateValue()
        {
            if(_randomSelect != null)
            {
                _talkContentListBox.Components = _randomSelect.Contents;
            }
        }
        public override void Disable()
        {
            Enabled = false;
            _talkContentListBox?.Disable();
            _talkContentListBox.Components = new List<TalkContent>();
        }
        public override void Enable()
        {
            Enabled = true;
            _talkContentListBox.Enable();

            _talkContentListBox.Components = _randomSelect.Contents;
            if (_randomSelect.Contents.Count != 0 && _talkContentListBox.SelectedIndex >= 0)
            {
                GUI_Components.talkContentPart.SetProcessUnit(_randomSelect.Contents[_talkContentListBox.SelectedIndex]);
            }
            else
            {
                GUI_Components.talkContentPart.SetProcessUnit(null);
            }
        }
        public void SetProcessUnit(RandomSelect randomSelect)
        {
            if(_randomSelect == randomSelect)
            {
                return;
            }
            _randomSelect = randomSelect;
            if (_randomSelect != null && _randomSelect.Contents.Count > 0)
            {
                GUI_Components.talkContentPart.SetProcessUnit(_randomSelect.Contents[0]);
                _talkContentListBox.SelectedIndex = 0;
            }
            else
            {
                GUI_Components.talkContentPart.SetProcessUnit(null);
                _talkContentListBox.SelectedIndex = -1;
            }
            UpdateUI(true);
        }
        public override bool HasBindingProcessUnit()
        {
            return _randomSelect != null;
        }
    }
}
