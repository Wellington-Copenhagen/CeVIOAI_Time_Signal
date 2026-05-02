using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CeVIO_AI_時報.UI_Component
{
    internal abstract class UIComponent : UserControl
    {
        // ユーザーによって値が変更された
        public Action ValueChanged;
        // 使うことができるという見た目にする
        public abstract void Enable();
        // 使うことができないという見た目にする
        public abstract void Disable();
        // 値を変更する系のメソッドは即座に見た目が更新される
    }
}
