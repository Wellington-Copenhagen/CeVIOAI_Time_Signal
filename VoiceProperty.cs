using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CeVIO_AI_時報
{
    internal abstract class VoiceProperty
    {
        protected uint _value;
        public uint Value
        {
            get { return _value; }
            set
            {
                _value = Math.Max(0, Math.Min(100, value));
            }
        }
        public abstract string ValueString();
        public abstract string Name();
    }
    internal class Volume : VoiceProperty
    {
        public override string ValueString()
        {
            return (_value * 0.16 - 8.0).ToString("N2");
        }
        public override string Name()
        {
            return "音量";
        }
    }
    internal class Speed : VoiceProperty
    {
        public override string ValueString()
        {
            return Math.Pow(5, _value * 0.02 - 1.0).ToString("N2");
        }
        public override string Name()
        {
            return "速度";
        }
    }
    internal class Tone : VoiceProperty
    {
        public override string ValueString()
        {
            return (_value * 12 - 600).ToString("N0");
        }
        public override string Name()
        {
            return "高さ";
        }
    }
    internal class Alpha : VoiceProperty
    {
        public override string ValueString()
        {
            return (_value * 0.02 - 1.0).ToString("N2");
        }
        public override string Name()
        {
            return "声質";
        }
    }
    internal class ToneScale : VoiceProperty
    {
        public override string ValueString()
        {
            return (_value * 0.02).ToString("N2");
        }
        public override string Name()
        {
            return "抑揚";
        }
    }
}
