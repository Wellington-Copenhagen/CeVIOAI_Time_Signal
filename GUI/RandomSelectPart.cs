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
            _talkContentListBox = new ListBoxAddDeletable<TalkContent>(new Point(10, 20), this);
            _talkContentListBox.OnChanged = () => OnChangedByUser?.Invoke();

            Disable();
        }
        protected override void UpdateVisual()
        {
            _talkContentListBox.SetComponents(_randomSelect.Contents);
            if (_randomSelect.Contents.Count != 0 && _talkContentListBox.SelectedIndex >= 0)
            {
                GUI_Components.talkContentPart.SetProcessUnit(_randomSelect.Contents[_talkContentListBox.SelectedIndex]);
            }
            else
            {
                GUI_Components.talkContentPart.SetProcessUnit(null);
            }
        }
        protected override void UpdateValue()
        {
            if(_randomSelect != null)
            {
                _talkContentListBox.SetComponents(_randomSelect.Contents);
            }
        }
        protected override void InitWithCeVIO()
        {

        }
        public override void Disable()
        {
            Enabled = false;
            _talkContentListBox?.Disable();
            _talkContentListBox.SetComponents(new List<TalkContent>());
        }
        protected override void Enable()
        {
            Enabled = true;
            _talkContentListBox.Enable();
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
        }
        protected override bool HasBindingProcessUnit()
        {
            return _randomSelect != null;
        }
    }
}
