using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CeVIO_AI_時報.GUI
{
    internal abstract class GUIPart : GroupBox
    {
        protected GUIPart()
        {
            OnChangedByUser = () =>
            {
                if(GUI_Components.UnderOnValueChanged)
                {
                    return;
                }
                UpdateValue();
                OnChanged?.Invoke();
            };
        }
        public Action OnChanged;
        protected Action OnChangedByUser;
        // データの値をもとに、UIの状態を更新する。
        protected abstract void UpdateVisual();
        // UIの状態をもとに、データの値を更新する。
        protected abstract void UpdateValue();
        // CeVIOがロードされたときに実行される。CeVIOの情報をもとにUIを初期化する
        protected abstract void InitWithCeVIO();
        // UIを操作できなくする。
        public abstract void Disable();
        // UIを操作できるようにする。
        protected abstract void Enable();
        protected abstract bool HasBindingProcessUnit();
        public void OnCeVIOLoaded()
        {
            InitWithCeVIO();
        }
        public void UpdateUI()
        {
            if (HasBindingProcessUnit())
            {
                Enable();
                UpdateVisual();
            }
            else
            {
                Disable();
            }
        }
    }
}
