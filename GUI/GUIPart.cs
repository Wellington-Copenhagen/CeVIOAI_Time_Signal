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
        protected bool valueChanging = false;
        protected GUIPart()
        {
            ValueChanged = () =>
            {
                if(GUI_Components.UnderOnValueChanged)
                {
                    return;
                }
            };
        }
        public Action ValueChanged;
        // データの値をもとに、UIの状態を更新する。
        // UIの状態をもとに、データの値を更新する。
        public abstract void UpdateValue();
        // CeVIOがロードされたときに実行される。CeVIOの情報をもとにUIを初期化する
        public virtual void InitWithCeVIO()
        {

        }
        // UIを操作できなくする。
        public abstract void Disable();
        // UIを操作できるようにする。
        public abstract void Enable();
        public abstract bool HasBindingProcessUnit();
        public void OnValueChanged(object sender, EventArgs e)
        {
            if (valueChanging)
            {
                return;
            }
            UpdateValue();
            UpdateUI(true);
            ValueChanged?.Invoke();
        }
        public void OnValueChanged()
        {
            UpdateValue();
            if (valueChanging)
            {
                return;
            }
            UpdateUI(true);
            ValueChanged?.Invoke();
        }
        public void UpdateUI(bool enable)
        {
            valueChanging = true;
            if (HasBindingProcessUnit() && enable)
            {
                Enable();
            }
            else
            {
                Disable();
            }
            valueChanging = false;
        }
    }
}
