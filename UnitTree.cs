using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace CeVIO_AI_時報
{
    internal class RootLayer
    {
        List<string> 
        List<RandomLayer> Layers;
        public RootLayer()
        {

        }
        public ref RandomLayer CurrentRandomLayer()
        {

        }

        public XElement ParseToXElement()
        {

        }
    }
    internal class RandomLayer
    {
        public Dictionary<DayOfWeek, bool> DayOfWeekAble;
        public int StartHour;
        public int EndHour;
        public int StartSec;
        public int EndSec;
        public string Name;
        List<ReadLayer> Layers;
        public RandomLayer(string name)
        {
            StartHour = 0;
            StartSec = 0;
            EndHour = 23;
            EndSec = 59;
            Name = name;
            DayOfWeekAble = new Dictionary<DayOfWeek, bool>
            {
                { DayOfWeek.Sunday, true },
                { DayOfWeek.Monday, true },
                { DayOfWeek.Tuesday, true },
                { DayOfWeek.Wednesday, true },
                { DayOfWeek.Thursday, true },
                { DayOfWeek.Friday, true },
                { DayOfWeek.Saturday, true },
            };
        }
        public RandomLayer(RandomLayer from)
        {
            Layers = new List<ReadLayer>();
            DayOfWeekAble = new Dictionary<DayOfWeek, bool>();
            for (int i = 0; i < from.Layers.Count; i++)
            {
                Layers.Add(from.Layers[i]);
            }
            for (int i = 0; i < from.DayOfWeekAble.Count; i++)
            {
                DayOfWeekAble.Add((DayOfWeek)i,from.DayOfWeekAble[(DayOfWeek)i]);
            }
            StartHour = from.StartHour;
            StartSec = from.StartSec;
            EndHour = from.EndHour;
            EndSec = from.EndSec;
            Name = from.Name;
        }
        public XElement ParseToXElement()
        {
            XElement output = new XElement(Name);
            output.Add 
            for(int i = 0; i < Layers.Count; i++)
            {
                XElement talkLayer = Layers[i].ParseToXElement();
                output.Add(talkLayer);
            }
        }
        public bool IsAbleToRead()
        {
            DateTime dateTime = DateTime.Now;
            int currentTime = dateTime.Hour * 60 + dateTime.Second;
            int startTime = StartHour * 60 + StartSec;
            int endTime = EndHour * 60 + EndSec;
            if(startTime < endTime)
            {
                //スタートより終わりがあとになってる場合、つまりは日を跨がない時間指定
                if (DayOfWeekAble[dateTime.DayOfWeek])
                {
                    if(startTime <= currentTime && currentTime <= endTime)
                    {
                        return true;
                    }
                    else
                    {
                        //時間指定外
                        return false;
                    }
                }
                else
                {
                    //曜日指定外
                    return false;
                }
            }
            else
            {
                //スタートより終わりが前になってる場合、つまりは日を跨ぐ時間指定
                if(currentTime <= endTime)
                {
                    if (YesterdayIsAble())
                    {
                        return true;
                    }
                    else
                    {
                        //曜日指定外
                        return false;
                    }
                }
                else if(startTime <= currentTime)
                {
                    if (DayOfWeekAble[dateTime.DayOfWeek])
                    {
                        return true;
                    }
                    else
                    {
                        //曜日指定外
                        return false;
                    }
                }
                else
                {
                    //時間指定外
                    return false;
                }
            }
        }
        public bool YesterdayIsAble()
        {
            DateTime today = DateTime.Now;
            DayOfWeek yesterday = today.DayOfWeek;
            if(yesterday != DayOfWeek.Sunday)
            {
                yesterday = yesterday - 1;
            }
            else
            {
                yesterday = DayOfWeek.Saturday;
            }
            return DayOfWeekAble[yesterday];
        }
    }
    internal class ReadLayer
    {
        //setter
        //XElementへのパーサー
        //読み上げ
        //コピーコンストラクタ
        //コンストラクタ
        public List<int> Params
        {
            get;
            private set;
        }
        public string CastName;
        public string Content;
        static List<string> ParamNameS = new List<string> {"Volume","Speed","Tone","Alpha","ToneScale","Emo1", "Emo2", "Emo3", "Emo4", "Emo5" };
        string Name;
        public enum ParamNameE
        {
            Volume = 0,
            Speed = 1,
            Tone = 2,
            Alpha = 3,
            ToneScale = 4,
            Emo1 = 5,
            Emo2 = 6,
            Emo3 = 7,
            Emo4 = 8,
            Emo5 = 9,
        }
        public ReadLayer(string name)
        {
            Params = new List<int>
            {
                50,50,50,50,50,100,0,0,0,0
            };
            CastName = "";
            Content = "";
            Name = name;
        }
        public ReadLayer(ReadLayer from)
        {
            Params = new List<int>();
            for(int i = 0;i < from.Params.Count; i++)
            {
                Params.Add(from.Params[i]);
            }
            CastName = from.CastName;
            Content = from.Content;
            Name = from.Name;
        }
        public void ParamSet(int num,ParamNameE name)
        {
            Params[(int)name] = num;
        }
        public void ParamIncrement(ParamNameE name)
        {
            Params[(int)name] = Params[(int)name] + 1;
            if (Params[(int)name] > 100)
            {
                Params[(int)name] = 100;
            }
        }
        public void ParamDecrement(ParamNameE name)
        {
            Params[(int)name] = Params[(int)name] - 1;
            if (Params[(int)name] < 0)
            {
                Params[(int)name] = 0;
            }
        }
        public string getParamShowValue(ParamNameE name)
        {
            switch (name)
            {
                case ParamNameE.Volume:
                    double d = Params[0] * 0.16 - 8.0;
                    return d.ToString("N2");
                case ParamNameE.Speed:
                    d = Math.Pow(5, Params[1] / 50.0 - 1.0);
                    return d.ToString("N2");
                case ParamNameE.Tone:
                    d = Params[2] * 12 - 600;
                    return d.ToString("N0");
                case ParamNameE.Alpha:
                    d = Params[3] / 50.0 - 1.0;
                    return d.ToString("N2");
                case ParamNameE.ToneScale:
                    d = Params[4] / 50.0;
                    return d.ToString("N2");
                case ParamNameE.Emo1:
                    d = Params[5] / 100.0;
                    return d.ToString("N2");
                case ParamNameE.Emo2:
                    d = Params[6] / 100.0;
                    return d.ToString("N2");
                case ParamNameE.Emo3:
                    d = Params[7] / 100.0;
                    return d.ToString("N2");
                case ParamNameE.Emo4:
                    d = Params[8] / 100.0;
                    return d.ToString("N2");
                case ParamNameE.Emo5:
                    d = Params[9] / 100.0;
                    return d.ToString("N2");
                default:
                    throw new Exception();
            }
        }
        /* <Name>
         *  <Volume>50</Volume>
         *  ...
         *  <Emo5>0<Emo5>
         *  <Cast>すずきつづみ<Cast>
         *  <Content>こんにちはすずきつづみです！<Content>
         * </Name>
         */
        public XElement ParseToXElement()
        {
            XElement output = new XElement(Name);
            for (int i = 0; i < ParamNameS.Count; i++)
            {
                XElement temp = new XElement(ParamNameS[i], Params[i]);
                output.Add(temp);
            }
            XElement tempCast = new XElement("Cast", CastName);
            output.Add(tempCast);
            XElement tempContent = new XElement("Content", Content);
            output.Add(tempContent);
            return output;
        }
        public void Talk()
        {

        }
    }
}
