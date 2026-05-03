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
            listBox = new ListBoxAddDeletable<TimeCondition>();
            listBox.Location = new Point(10, 20);
            listBox.ValueChanged = OnValueChanged;
            Controls.Add(listBox);

            Disable();
        }
        public override void UpdateValue()
        {
            _rootLevel.TimeConditions = listBox.Components;
        }
        public override void Disable()
        {
            listBox.Disable();
            Enabled = false;

            GUI_Components.timeConditionPart.SetProcessUnit(null);
        }
        public override void Enable()
        {
            listBox.Enable();
            Enabled = true;

            listBox.Components = _rootLevel.TimeConditions;
            if (listBox.Components.Count != 0 && listBox.SelectedIndex >= 0)
            {
                GUI_Components.timeConditionPart.SetProcessUnit(_rootLevel.TimeConditions[listBox.SelectedIndex]);
            }
            else
            {
                GUI_Components.timeConditionPart.SetProcessUnit(null);
            }
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
            UpdateUI(true);
        }
        public override bool HasBindingProcessUnit()
        {
            return _rootLevel != null;
        }
    }
}
