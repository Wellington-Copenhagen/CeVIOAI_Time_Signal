using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Management;
using System.Diagnostics;
using System.Threading;
using CeVIO.Talk.RemoteService2;

namespace CeVIO_AI_時報
{
    internal class Signal_Execute
    {
        /*欲しい関数
         * 時間制約
         * ランダム制約
         * 読み上げ
         * 
         */
        //キャストの名前と感情パラメーターの名前のリスト
        public XElement ForTalk;
        public bool AbleToTalk;
        public Signal_Execute(XElement loaded)
        {
            AbleToTalk = true;
            ForTalk = loaded;
        }
        public void Root(XElement rootLayer)
        {
            while (!AbleToTalk)
            {
                Thread.Sleep(1000);
            }
            PerformanceCounter performanceCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            float CPUusage = performanceCounter.NextValue();
            int result = IntParse(rootLayer.Element("CPUborder").Value);
            if (CPUusage > result)
            {
                return;
            }
            foreach (XElement element in rootLayer.Elements())
            {
                if (element.Name.ToString()[0] == '_')
                {
                    TimeCheck(element);
                }
            }
        }
        Dictionary<string,string> InContentParseDic = new Dictionary<string,string>();
        public string toShow = "";
        public void Talk(XElement readLayer)
        {
            Talker2 talker2 = new Talker2();
            talker2.Volume = (uint)IntParse(readLayer.Element("Volume").Value);
            talker2.Speed = (uint)IntParse(readLayer.Element("Speed").Value);
            talker2.Tone = (uint)IntParse(readLayer.Element("Tone").Value);
            talker2.Alpha = (uint)IntParse(readLayer.Element("Alpha").Value);
            talker2.ToneScale = (uint)IntParse(readLayer.Element("ToneScale").Value);
            talker2.Cast = readLayer.Element("Cast").Value;
            for(int i = 0; i < talker2.Components.Count; i++)
            {
                talker2.Components[i].Value = (uint)IntParse(readLayer.Element("Emo" + i).Value);
            }
            DateTime now = DateTime.Now;
            InContentParseDic.Clear();
            InContentParseDic["年"] = now.Year.ToString();
            InContentParseDic["月"] = now.Month.ToString();
            InContentParseDic["日"] = now.Day.ToString();
            InContentParseDic["時"] = now.Second.ToString();
            InContentParseDic["分"] = now.Minute.ToString();
            switch (now.DayOfWeek)
            {
                case DayOfWeek.Sunday:
                    InContentParseDic["曜日"] = "日曜日";
                    break;
                case DayOfWeek.Monday:
                    InContentParseDic["曜日"] = "月曜日";
                    break;
                case DayOfWeek.Tuesday:
                    InContentParseDic["曜日"] = "火曜日";
                    break;
                case DayOfWeek.Wednesday:
                    InContentParseDic["曜日"] = "水曜日";
                    break;
                case DayOfWeek.Thursday:
                    InContentParseDic["曜日"] = "木曜日";
                    break;
                case DayOfWeek.Friday:
                    InContentParseDic["曜日"] = "金曜日";
                    break;
                case DayOfWeek.Saturday:
                    InContentParseDic["曜日"] = "土曜日";
                    break;
            }
            string content = readLayer.Element("Content").Value;
            int pointer = 0;
            int startParen = -1;
            string contentError = "";
            while(pointer < content.Length)
            {
                if (content[pointer] == '{')
                {
                    startParen = pointer;
                }
                if (content[pointer] == '}')
                {
                    if(startParen == -1)
                    {
                        contentError = "中括弧の位置関係がおかしいです。";
                        break;
                    }
                    string inParen = content.Substring(startParen+1, pointer);
                    bool find = false;
                    foreach(KeyValuePair<string,string> pair in InContentParseDic)
                    {
                        if (SameString(inParen, pair.Key))
                        {
                            content = content.Remove(startParen,pointer+1);
                            content = content.Insert(startParen,pair.Value);
                            pointer = startParen + pair.Value.Length - 1;
                            find = true;
                            break;
                        }
                    }
                    if (!find)
                    {
                        contentError = contentError + "中括弧の中が変換できない言葉です。";
                    }
                }
                pointer++;
            }
            toShow = "";
            if (contentError.Length == 0)
            {
                talker2.Speak(content);
                toShow = toShow + "キャスト：" + talker2.Cast+"\n";
                toShow = toShow + "パラメータ：　大きさ：" + (talker2.Volume * 0.16 - 8.0).ToString("N2");
                toShow = toShow + "　速さ：" + Math.Pow(5,talker2.Speed * 0.02 - 1.0).ToString("N2");
                toShow = toShow + "　高さ：" + (talker2.Tone * 12 - 600).ToString("N0");
                toShow = toShow + "　声質：" + (talker2.Alpha * 0.02 - 1.0).ToString("N2");
                toShow = toShow + "　抑揚：" + (talker2.ToneScale * 0.02).ToString("N2")+"\n";
                toShow = toShow + "感情：";
                for(int i = 0;i < talker2.Components.Count; i++)
                {
                    toShow = toShow + "　" + talker2.Components[i].Name + "：" + talker2.Components[i].Value;
                }
                toShow = toShow + "\n";
                toShow = toShow + content;
            }
            else
            {
                toShow = contentError;
            }
        }
        public void TimeCheck(XElement timeLayer)
        {
            bool goNext = false;
            DateTime now = DateTime.Now;
            int currentTime = now.Hour * 60 + now.Minute;
            int startTime = IntParse(timeLayer.Element("StartTime").Value);
            int endTime = IntParse(timeLayer.Element("EndTime").Value);
            int interval = IntParse(timeLayer.Element("Interval").Value);
            if (startTime <= endTime)
            {
                if(startTime < currentTime && currentTime < endTime)
                {
                    if (timeLayer.Element("DayOfWeek").Value[(int)now.DayOfWeek] == 'T')
                    {
                        if((currentTime-startTime)%interval == 0)
                        {
                            goNext = true;
                        }
                    }
                }
            }
            else
            {
                if(startTime < currentTime)
                {
                    if (timeLayer.Element("DayOfWeek").Value[(int)now.DayOfWeek] == 'T')
                    {
                        if ((currentTime - startTime) % interval == 0)
                        {
                            goNext = true;
                        }
                    }
                }
                if (currentTime < endTime)
                {
                    int yesterday = (int)now.DayOfWeek - 1;
                    if (yesterday == -1) yesterday = 6;
                    if (timeLayer.Element("DayOfWeek").Value[yesterday] == 'T')
                    {
                        if ((currentTime + 1440 - startTime) % interval == 0)
                        {
                            goNext = true;
                        }
                    }
                }
            }
            if (goNext)
            {
                foreach(XElement element in timeLayer.Elements())
                {
                    if (element.Name.ToString()[0] == '_')
                    {
                        RandomCheck(element);
                    }
                }
            }
        }
        public void RandomCheck(XElement randomLayer)
        {
            int count = 0;
            foreach(XElement element in randomLayer.Elements())
            {
                if (element.Name.ToString()[0] == '_')
                {
                    count++;
                }
            }
            Random random = new Random();
            int randResult = random.Next(0, count);
            Talk(randomLayer.Element("_" + randResult));
        }
        public int IntParse(string from)
        {
            if (!int.TryParse(from, out int result)) throw new Exception();
            return result;
        }
        static public bool SameString(string A, string B)
        {
            if (A.Length != B.Length) return false;
            for (int i = 0; i < A.Length; i++)
            {
                if (A[i] != B[i]) return false;
            }
            return true;
        }
    }
}
