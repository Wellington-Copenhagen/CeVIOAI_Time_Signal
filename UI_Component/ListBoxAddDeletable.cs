using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace CeVIO_AI_時報.UI_Component
{
    // 120x340の大きさ
    public class ListBoxAddDeletable<T> where T : IListboxAddDeletableComponent , new()
    {
        ListBox _listbox;
        Button _deleteButton;
        Button _addButton;
        Button _renameButton;
        Label _label_name;
        TextBox _nameBox;
        Button _moveUpButton;
        Button _moveDownButton;
        List<T> _components = new List<T>();
        public Action OnChanged;
        public ListBoxAddDeletable(Point position, Control panel)
        {
            _listbox = new ListBox();
            _listbox.Location = position;
            _listbox.Size = new Size(120, 240);
            _listbox.SelectedIndexChanged += OnSelectedIndexChanged;
            panel.Controls.Add( _listbox );

            _deleteButton = new Button();
            _deleteButton.Location = position + new Size(5, 245);
            _deleteButton.Size = new Size(50, 20);
            _deleteButton.Text = "削除";
            _deleteButton.Click += (sender, e) => Delete();
            panel.Controls.Add( _deleteButton );

            _addButton = new Button();
            _addButton.Location = position + new Size(65, 245);
            _addButton.Size = new Size(50, 20);
            _addButton.Text = "追加";
            _addButton.Click += (sender, e) => Add();
            panel.Controls.Add( _addButton );

            _renameButton = new Button();
            _renameButton.Location = position + new Size(5, 270);
            _renameButton.Size = new Size(110, 20);
            _renameButton.Text = "名前を変更";
            _renameButton.Click += (sender, e) => Rename();
            panel.Controls.Add( _renameButton );

            _nameBox = new TextBox();
            _nameBox.Location = position + new Size(5, 295);
            _nameBox.Size = new Size(110, 20);
            panel.Controls.Add( _nameBox );

            _moveUpButton = new Button();
            _moveUpButton.Location = position + new Size(5, 320);
            _moveUpButton.Size = new Size(50, 20);
            _moveUpButton.Text = "上へ";
            _moveUpButton.Click += (sender, e) => MoveUp();
            panel.Controls.Add( _moveUpButton );

            _moveDownButton = new Button();
            _moveDownButton.Location = position + new Size(65, 320);
            _moveDownButton.Size = new Size(50, 20);
            _moveDownButton.Text = "下へ";
            _moveDownButton.Click += (sender, e) => MoveDown();
            panel.Controls.Add( _moveDownButton );

            UpdateAppearance(-1);
        }
        void UpdateAppearance(int selectedIndex)
        {
            if(_components.Count != 0)
            {
                _listbox.Items.Clear();
                foreach (IListboxAddDeletableComponent component in _components)
                {
                    _listbox.Items.Add(component.GetName());
                }
                SelectedIndex = selectedIndex;
                _nameBox.Text = _components[SelectedIndex].GetName();
            }
            else
            {
                _listbox.Items.Clear();
                SelectedIndex = -1;
                _nameBox.Text = "";
            }
        }
        public int SelectedIndex
        {
            get
            {
                return _listbox.SelectedIndex;
            }
            set
            {
                _listbox.SelectedIndexChanged -= OnSelectedIndexChanged;
                if (_listbox.Items.Count == 0)
                {
                    _listbox.SelectedIndex = -1;
                }
                else if(value >= _listbox.Items.Count)
                {
                    _listbox.SelectedIndex = _listbox.Items.Count - 1;
                }
                else if (value < 0)
                {
                    _listbox.SelectedIndex = 0;
                }
                else
                {
                    _listbox.SelectedIndex = value;
                }
                _listbox.SelectedIndexChanged += OnSelectedIndexChanged;
            }
        }
        public T SelectedItem()
        {
            if(_listbox.SelectedIndex == -1 || _components.Count == 0)
            {
                return default(T);
            }
            else
            {
                return (T)_listbox.SelectedItem;
            }
        }
        public void SetComponents(List<T> components)
        {
            _components = components;
            UpdateAppearance(SelectedIndex);
        }
        public List<T> GetComponents()
        {
            return _components;
        }
        void Delete()
        {
            if(SelectedIndex != -1)
            {
                _components.RemoveAt(SelectedIndex);
                UpdateAppearance(0);
                OnChanged?.Invoke();
            }
        }
        void Add()
        {
            int index = 0;
            while (true)
            {
                if(_components.Where(c => c.GetName() == "New(" + index + ")").Count() != 0)
                {
                    index++;
                }
                else
                {
                    _components.Add(new T());
                    _components.Last().SetName("New(" + index + ")");
                    UpdateAppearance(_components.Count - 1);
                    OnChanged?.Invoke();
                    break;
                }
            }
        }
        void Rename()
        {
            if (SelectedIndex != -1)
            {
                _components[SelectedIndex].SetName(_nameBox.Text);
                UpdateAppearance(SelectedIndex);
                OnChanged?.Invoke();
            }
        }
        void MoveUp()
        {
            if (SelectedIndex != -1)
            {
                int index = SelectedIndex;
                if (index > 0)
                {
                    var temp = _components[index - 1];
                    _components[index - 1] = _components[index];
                    _components[index] = temp;
                    UpdateAppearance(index - 1);
                    OnChanged?.Invoke();
                }
            }
        }
        void MoveDown()
        {
            if (SelectedIndex != -1)
            {
                int index = SelectedIndex;
                if (index < _components.Count - 1)
                {
                    var temp = _components[index + 1];
                    _components[index + 1] = _components[index];
                    _components[index] = temp;
                    UpdateAppearance(index + 1);
                    OnChanged?.Invoke();
                }
            }
        }
        public void Disable()
        {
            _listbox.Items.Clear();
            _listbox.Enabled = false;
            _deleteButton.Enabled = false;
            _addButton.Enabled = false;
            _renameButton.Enabled = false;
            _nameBox.Enabled = false;
            _moveUpButton.Enabled = false;
            _moveDownButton.Enabled = false;
        }
        public void Enable()
        {
            _listbox.Enabled = true;
            _deleteButton.Enabled = true;
            _addButton.Enabled = true;
            _renameButton.Enabled = true;
            _nameBox.Enabled = true;
            _moveUpButton.Enabled = true;
            _moveDownButton.Enabled = true;
        }
        bool UnderChanging = false;
        void OnSelectedIndexChanged(object sender, EventArgs e)
        {
            OnChanged?.Invoke();
        }
    }
    public interface IListboxAddDeletableComponent
    {
        string GetName();
        void SetName(string newName);
    }
}
