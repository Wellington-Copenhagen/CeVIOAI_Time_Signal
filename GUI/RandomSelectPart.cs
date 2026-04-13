using CeVIO_AI_時報.UI_Component;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CeVIO_AI_時報.GUI
{
    // 120x340の大きさ
    internal class GUI_RandomSelectPart : GUI_Part
    {
        public ListBoxAddDeletable talkContentListBox;
        public Action OnChanged;
        public GUI_RandomSelectPart(Point location)
        {
            talkContentListBox = new ListBoxAddDeletable(location);
            talkContentListBox.OnChanged = () => OnChanged?.Invoke();
        }
    }
}
