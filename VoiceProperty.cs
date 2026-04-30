using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CeVIO_AI_時報
{
    internal abstract class VoiceProperty
    {
        protected uint _value = 50;
        public uint Value
        {
            get { return _value; }
            set
            {
                _value = Math.Max(0, Math.Min(100, value));
            }
        }
        public abstract string ValueString();
        public abstract string NameJa();
        public abstract string NameEn();
        public override string ToString()
        {
            return NameJa() + "：" + ValueString();
        }
    }
    internal class Volume : VoiceProperty
    {
        public override string ValueString()
        {
            return ((float)_value * 0.16 - 8.0).ToString("N2");
        }
        public override string NameJa()
        {
            return "音量";
        }
        public override string NameEn()
        {
            return "Volume";
        }
    }
    internal class Speed : VoiceProperty
    {
        public override string ValueString()
        {
            return Math.Pow(5, (float)_value * 0.02 - 1.0).ToString("N2");
        }
        public override string NameJa()
        {
            return "速度";
        }
        public override string NameEn()
        {
            return "Speed";
        }
    }
    internal class Tone : VoiceProperty
    {
        public override string ValueString()
        {
            return ((float)_value * 12 - 600).ToString("N0");
        }
        public override string NameJa()
        {
            return "高さ";
        }
        public override string NameEn()
        {
            return "Tone";
        }
    }
    internal class Alpha : VoiceProperty
    {
        public override string ValueString()
        {
            return ((float)_value * 0.02 - 1.0).ToString("N2");
        }
        public override string NameJa()
        {
            return "声質";
        }
        public override string NameEn()
        {
            return "Alpha";
        }
    }
    internal class ToneScale : VoiceProperty
    {
        public override string ValueString()
        {
            return ((float)_value * 0.02).ToString("N2");
        }
        public override string NameJa()
        {
            return "抑揚";
        }
        public override string NameEn()
        {
            return "ToneScale";
        }
    }
}
