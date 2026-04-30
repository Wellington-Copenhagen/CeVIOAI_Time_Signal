using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace CeVIO_AI_時報.ProcessUnit
{
    internal interface IProcessUnit
    {
        void OnRun();
        XElement StoreToXML();
        void LoadFromXML(XElement element);
        void Default();
    }
}
