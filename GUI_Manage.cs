using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace CeVIO_AI_時報
{
    internal class GUI_Manage
    {
        public XElement UnderEdit;
        public List<int> CurrentTree;
        public XElement WholeTree;
        public GUI_Manage(XElement loaded)
        {
            CurrentTree = new List<int>();
            WholeTree = loaded;
            UnderEdit = loaded;
            //TODO 感情のリストの生成
        }
        //個別のデータの出し入れ
        public void Set(string paramName,string value)
        {
            UnderEdit.Element(paramName).SetValue(value);
        }
        public string Get(string paramName)
        {
            return UnderEdit.Element(paramName).Value;
        }
        public double GetNum(string paramName)
        {
            if(!double.TryParse(UnderEdit.Element(paramName).Value,out double result))
            {
                throw new Exception();
            }
            return result;
        }
        public void Increment(string paramName, int step,int min,int max)
        {
            int value = IntParse(UnderEdit.Element(paramName).Value);
            value = value + step;
            if(value > max)
            {
                value = max;
            }
            UnderEdit.Element(paramName).SetValue(value);
        }
        public void Decrement(string paramName, int step, int min, int max)
        {
            int value = IntParse(UnderEdit.Element(paramName).Value);
            value = value - step;
            if (value < min)
            {
                value = min;
            }
            UnderEdit.Element(paramName).SetValue(value);
        }
        //ツリー内への書き込み
        //0,wholeTreeで指定すれば良い
        public XElement GetFromWhole(LayerI layerLebel,XElement getFrom)
        {
            if((int)layerLebel == CurrentTree.Count)
            {
                return getFrom;
            }
            else
            {
                XElement chosen = getFrom.Element("_" + CurrentTree[(int)layerLebel]);
                return GetFromWhole(layerLebel + 1, chosen);
            }
        }
        public XElement SetToWhole(LayerI layerLebel, XElement setTo,XElement toSet)
        {
            if ((int)layerLebel == CurrentTree.Count)
            {
                return toSet;
            }
            else
            {
                XElement chosen = setTo.Element("_" + CurrentTree[(int)layerLebel]);
                XElement setted = SetToWhole(layerLebel + 1, chosen,toSet);
                setTo.Element("_"+CurrentTree[(int)layerLebel]).Remove();
                setTo.Add(setted);
                if (layerLebel == 0)
                {
                    WholeTree = setTo;
                }
                return setTo;
            }
        }
        public void TreeGoRoot()
        {
            if(CurrentTree.Count > 0)
            {
                SetToWhole(0, WholeTree, UnderEdit);
                CurrentTree.RemoveAt(CurrentTree.Count - 1);
                UnderEdit = GetFromWhole(0, WholeTree);
            }
        }
        public void TreeGoLeaf(int next)
        {
            if (CurrentTree.Count < XMLLoader.LayerCount - 1)
            {
                SetToWhole(0, WholeTree, UnderEdit);
                CurrentTree.Add(SelectedIndexToLayerIndex[next]);
                UnderEdit = GetFromWhole(0, WholeTree);
            }
        }
        public void AddBlock()
        {
            LayerI current = LayerI.Read;
            for(LayerI i = 0;(int)i < XMLLoader.LayerCount; i++)
            {
                if (XMLLoader.SameString(UnderEdit.Element("Type").Value, XMLLoader.LayerS[i]))
                {
                    current = i; break;
                }
            }
            if((int)current < XMLLoader.LayerCount - 1)
            {
                int ableElement = 0;
                while (true)
                {
                    if (UnderEdit.Elements("_" + ableElement).Count() != 0)
                    {
                        break;
                    }
                    ableElement++;
                }
                XElement toAdd = new XElement("_" + ableElement);
                foreach(KeyValuePair<string,string> pair in XMLLoader.ElementAndInitial[current+1])
                {
                    toAdd.Add(new XElement(pair.Key, pair.Value));
                }
                UnderEdit.Add(toAdd);
            }
        }
        public void DeleteBlock(int toDelete)
        {
            UnderEdit.Element("_"+SelectedIndexToLayerIndex[toDelete]).Remove();
        }
        public void ChangeBlockName(int toChenge,string newName)
        {
            UnderEdit.Element("_" + SelectedIndexToLayerIndex[toChenge]).Element("Name").Value = newName;
        }
        public XElement OrganizeLayer(XElement input)
        {

            List<int> used = new List<int>();
            foreach(XElement element in input.Elements())
            {
                if (element.Name.ToString()[0] == '_')
                {
                    XElement toAdd = OrganizeLayer(element);
                    input.Element(element.Name).Remove();
                    input.Add(toAdd);
                    string nameNum = element.Name.ToString();
                    nameNum = nameNum.Substring(1);
                    int result = IntParse(nameNum);
                    used.Add(result);
                }
            }
            if(used.Count == 0)
            {
                return input;
            }
            used.Sort();
            for(int i = 0;i < used.Count;i++)
            {
                XElement toAdd = new XElement("_" + i, input.Element("_" + used[i]).Elements());
                input.Element("_" + used[i]).Remove();
                input.Add(toAdd);
            }
            return input;
        }
        public void Save()
        {
            WholeTree.Save("config.txt");
        }
        public string GetBreadClumb(XElement element,List<int> tree)
        {
            if (tree.Count == 0)
            {
                return element.Element("Name").Value;
            }
            else
            {
                List<int> popped = new List<int>();
                for(int i = 1; i < tree.Count; i++)
                {
                    popped.Add(tree[i]);
                }
                string output = element.Element("Name").Value + ">";
                output = output + GetBreadClumb(element.Element("_" + tree[0]), popped);
                return output;
            }
        }


        public List<string> NameList = new List<string>();
        Dictionary<int,int> SelectedIndexToLayerIndex = new Dictionary<int,int>();
        public void InitializeUnitList()
        {
            SelectedIndexToLayerIndex.Clear();
            NameList.Clear();
            int counter = 0;
            foreach(XElement element in UnderEdit.Elements())
            {
                if (element.Name.ToString()[0] == '_')
                {
                    NameList.Add(element.Element("Name").Value);
                    int index = IntParse(element.Name.ToString().Substring(1));
                    SelectedIndexToLayerIndex[counter] = counter;
                    counter++;
                }
            }
        }
        public int IntParse(string from)
        {
            if (!int.TryParse(from, out int result)) throw new Exception();
            return result;
        }
        //欲しい関数　編集編
        /*
         * パラメーター類のインクリメントとデクリメント
         * キャストと内容
         * 時間の設定
         * 曜日の設定(各曜日ずつ)
         * 
         * ツリーそれ自体への操作
         * ツリー上の名前
         * 新規生成
         * 名前の衝突の検出
         * 削除
         * 
         * ツリー内での現在位置を戻す関数と進める関数
         * 
         * 保存
         */
    }
}
