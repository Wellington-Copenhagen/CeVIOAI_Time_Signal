using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.IO;

namespace CeVIO_AI_時報
{
    public enum LayerI
    {
        Root = 0,
        Time = 1,
        Random = 2,
        Read = 3
    }
    internal class XMLLoader
    {
        public XElement Loaded;
        //無い層を補完するために使う
        //内側の辞書は要素の名前と初期値
        static public Dictionary<LayerI, Dictionary<string, string>> ElementAndInitial;
        static public Dictionary<LayerI,string> LayerS = new Dictionary<LayerI, string>()
        {
            {LayerI.Root,"Root" },
            {LayerI.Time,"Time" },
            {LayerI.Random,"Random" },
            {LayerI.Read,"Read" },
        };
        static public int LayerCount = 4;
        /*
         * 構造の認識
         * 各タイプごとに存在しない場合にどのように補完するか(おそらく全部分岐なしで層生成？)
         */
        public XMLLoader()
        {
            Loaded = new XElement("Root");
            ElementAndInitial = new Dictionary<LayerI, Dictionary<string, string>>();
            {
                Dictionary<string, string> root = new Dictionary<string, string>();
                root["CPUborder"] = "50";
                root["Type"] = LayerS[LayerI.Root];
                root["Name"] = "新しい";
                ElementAndInitial[LayerI.Root] = root;
            }
            {
                Dictionary<string, string> time = new Dictionary<string, string>();
                time["StartTime"] = "0";
                time["EndTime"] = "1439";
                time["DayOfWeek"] = "TTTTTTT";
                time["Interval"] = "30";
                time["Type"] = LayerS[LayerI.Time];
                time["Name"] = "新しい";
                ElementAndInitial[LayerI.Time] = time;
            }
            {
                Dictionary<string, string> random = new Dictionary<string, string>();
                random["Type"] = LayerS[LayerI.Random];
                random["Name"] = "新しい";
                ElementAndInitial[LayerI.Random] = random;
            }
            {
                Dictionary<string, string> read = new Dictionary<string, string>();
                read["Volume"] = "50";
                read["Speed"] = "50";
                read["Tone"] = "50";
                read["Alpha"] = "50";
                read["ToneScale"] = "50";
                read["Emo1"] = "100";
                read["Emo2"] = "0";
                read["Emo3"] = "0";
                read["Emo4"] = "0";
                read["Emo5"] = "0";
                read["Cast"] = Form1.EmotionNames.First().Key;
                read["Content"] = "";
                read["Type"] = LayerS[LayerI.Read];
                read["Name"] = "新しい";
                ElementAndInitial[LayerI.Read] = read;
            }
            Load();
        }
        /*
         * <A>
         *  <C1>
         *  </C1>
         *  <C2>
         *  </C2>
         * </A>
         * を
         * <A>
         *  <B>
         *  <Bのパラメーター>
         *   <C1>
         *   </C1>
         *   <C2>
         *   </C2>
         *  </B>
         * </A>
         * としたい場合
         * <A>のレベルのXElementとBを入力する
         */
        public XElement InsertLayer(XElement current,LayerI toInsert)
        {
            XElement gathered = new XElement("_0");
            List<XName> shouldBeDeleted = new List<XName>();
            foreach(XElement element in current.Elements())
            {
                if (element.Name.ToString()[0] == '_')
                {
                    gathered.Add(element);
                    shouldBeDeleted.Add(element.Name);
                }
            }
            foreach(KeyValuePair<string,string> pair in ElementAndInitial[toInsert])
            {
                gathered.Add(new XElement(pair.Key, pair.Value));
            }
            foreach(XName name in shouldBeDeleted)
            {
                current.Element(name).Remove();
            }
            current.Add(gathered);
            return current;
        }
        public void Load()
        {
            if (File.Exists("config.txt"))
            {
                XElement temp = XElement.Load("config.txt");
                Loaded = LoadOneLayer(temp);
            }
            else
            {
                Loaded = CreateLayer(LayerI.Root);
            }
            
        }
        public XElement CreateLayer(LayerI layer)
        {
            XElement output = new XElement("_0");
            if((int)layer < LayerCount - 1)
            {
                output.Add(CreateLayer(layer + 1));
            }
            foreach(KeyValuePair<string,string> pair in ElementAndInitial[layer])
            {
                output.Add(new XElement(pair.Key, pair.Value));
            }
            return output;
        }
        //Rootでは<_0>とRootの1つ下にあるべき層を指定する
        //再帰的にreadまで呼んでいって
        //Dより先をまず処理させて
        //AD->ACD->ABCD
        //とする
        public XElement LoadOneLayer(XElement loaded)
        {
            LayerI thisLayer = LayerI.Root;
            bool initialized = false;
            for(int i = 0;i < LayerCount; i++)
            {
                if (SameString(loaded.Element("Type").Value, LayerS[(LayerI)i]))
                {
                    thisLayer = (LayerI)i;
                    initialized = true;
                    break;
                }
            }
            if(!initialized)
            {
                throw new Exception();
            }
            foreach(XElement element in loaded.Elements())
            {
                if (element.Name.ToString()[0] == '_')
                {
                    XElement processing = LoadOneLayer(element);
                    LayerI nextLayer = LayerI.Root;
                    initialized = false;
                    for (int i = 0; i < LayerCount; i++)
                    {
                        if (SameString(loaded.Element("Type").Value, LayerS[(LayerI)i]))
                        {
                            nextLayer = (LayerI)i;
                            initialized = true;
                            break;
                        }
                    }
                    if (!initialized)
                    {
                        throw new Exception();
                    }
                    while ((int)thisLayer < (int)nextLayer - 1)
                    {
                        nextLayer--;
                        processing = InsertLayer(processing, nextLayer);
                    }
                    XName name = processing.Name;
                    loaded.Element(name).Remove();
                    loaded.Add(processing);
                }
            }
            return loaded;
        }
        static public bool SameString(string A,string B)
        {
            if(A.Length !=  B.Length) return false;
            for (int i = 0; i < A.Length; i++)
            {
                if( A[i] != B[i]) return false;
            }
            return true;
        }
    }
}
