using CeVIO_AI_時報.UI_Component;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CeVIO_AI_時報.GUI
{
    internal class GUI_RootPart : GUI_Part
    {
        public ListBoxAddDeletable listBox;
        public Action OnChanged;
        public GUI_RootPart(Point location)
        {
            listBox = new ListBoxAddDeletable(location);
            listBox.OnChanged += OnChanged;
        }
    }
}
