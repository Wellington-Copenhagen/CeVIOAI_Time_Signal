using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace CeVIO_AI_時報.ProcessUnit
{
    // Default()後ならフィールドはすべて正常な値が入っている
    // フィールドに代入するときも正常な値にする
    // コンストラクションの時点でCeVIOのAPIが呼べる
    internal interface IProcessUnit
    {
        // Default()が呼ばれた後であれば、エラーは出さないこと
        void OnRun();

        // Default()が呼ばれた後であれば、エラーは出さないこと
        XElement StoreToXML();
        
        // エラーは出さない
        // XMLにない要素は無視する
        void LoadFromXML(XElement element);
        
        // エラーは出さないこと
        void Default();
    }
}
