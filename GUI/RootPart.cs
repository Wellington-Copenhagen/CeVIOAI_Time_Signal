using CeVIO_AI_時報.ProcessUnit;
using CeVIO_AI_時報.UI_Component;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Forms;

namespace CeVIO_AI_時報.GUI
{
    internal class GUI_RootPart : GUIPart
    {
        RootLevel _rootLevel;
        ListBoxAddDeletable<TimeCondition> listBox;
        public GUI_RootPart()
        {
            listBox = new ListBoxAddDeletable<TimeCondition>(new Point(10,20), this);
            listBox.OnChanged = () => OnChangedByUser?.Invoke();

            Disable();
        }
        protected override void UpdateVisual()
        {
            listBox.SetComponents(_rootLevel.TimeConditions);
            if (listBox.GetComponents().Count != 0 && listBox.SelectedIndex >= 0)
            {
                GUI_Components.timeConditionPart.SetProcessUnit(_rootLevel.TimeConditions[listBox.SelectedIndex]);
            }
            else
            {
                GUI_Components.timeConditionPart.SetProcessUnit(null);
            }
        }
        protected override void UpdateValue()
        {
            _rootLevel.TimeConditions = listBox.GetComponents();
        }
        protected override void InitWithCeVIO()
        {
        }
        public override void Disable()
        {
            listBox?.Disable();
            Enabled = false;
        }
        protected override void Enable()
        {
            listBox.Enable();
            Enabled = true;
        }
        public void SetProcessUnit(RootLevel rootLevel)
        {
            if (rootLevel == _rootLevel)
            {
                return;
            }
            _rootLevel = rootLevel;
            if(_rootLevel != null && _rootLevel.TimeConditions.Count > 0)
            {
                GUI_Components.timeConditionPart.SetProcessUnit(_rootLevel.TimeConditions[0]);
                listBox.SelectedIndex = 0;
            }
            else
            {
                GUI_Components.timeConditionPart.SetProcessUnit(null);
                listBox.SelectedIndex = -1;
            }
        }
        protected override bool HasBindingProcessUnit()
        {
            return _rootLevel != null;
        }
    }
}
