using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace CeVIO_AI_時報.UI_Component
{
    // 120x340の大きさ
    internal class ListBoxAddDeletable<T> : UIComponent
        where T : IListboxAddDeletableComponent , new()
    {
        ListBox _listbox;
        Button _deleteButton;
        Button _addButton;
        Button _renameButton;
        TextBox _nameBox;
        Button _moveUpButton;
        Button _moveDownButton;
        List<T> _components = new List<T>();
        public int SelectedIndex
        {
            get
            {
                return _listbox.SelectedIndex;
            }
            set
            {
                _listbox.SelectedIndexChanged -= OnSelectedIndexChanged;
                int previous = SelectedIndex;
                if (_listbox.Items.Count == 0)
                {
                    _listbox.SelectedIndex = -1;
                }
                else if (value >= _listbox.Items.Count)
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
                if(previous != SelectedIndex) 
                {
                    if (SelectedIndex != -1)
                    {
                        _nameBox.Text = SelectedItem().GetName();
                    }
                    else
                    {
                        _nameBox.Text = "";
                    }
                }
                _listbox.SelectedIndexChanged += OnSelectedIndexChanged;
            }
        }
        public List<T> Components
        {
            set
            {
                _components = value;
                UpdateAppearance(SelectedIndex);
            }
            get
            {
                return _components;
            }
        }
        public ListBoxAddDeletable()
        {
            Size = new Size(120, 340);

            _listbox = new ListBox();
            _listbox.Location = new Point(0, 0);
            _listbox.Size = new Size(120, 240);
            _listbox.SelectedIndexChanged += OnSelectedIndexChanged;
            Controls.Add( _listbox );
            _deleteButton = new Button();
            _deleteButton.Location = new Point(5, 245);
            _deleteButton.Size = new Size(50, 20);
            _deleteButton.Text = "削除";
            _deleteButton.Click += (sender, e) => Delete();
            Controls.Add( _deleteButton );

            _addButton = new Button();
            _addButton.Location = new Point(65, 245);
            _addButton.Size = new Size(50, 20);
            _addButton.Text = "追加";
            _addButton.Click += (sender, e) => Add();
            Controls.Add( _addButton );

            _renameButton = new Button();
            _renameButton.Location = new Point(5, 270);
            _renameButton.Size = new Size(110, 20);
            _renameButton.Text = "名前を変更";
            _renameButton.Click += (sender, e) => Rename();
            Controls.Add( _renameButton );

            _nameBox = new TextBox();
            _nameBox.Location = new Point(5, 295);
            _nameBox.Size = new Size(110, 20);
            Controls.Add( _nameBox );

            _moveUpButton = new Button();
            _moveUpButton.Location = new Point(5, 320);
            _moveUpButton.Size = new Size(50, 20);
            _moveUpButton.Text = "上へ";
            _moveUpButton.Click += (sender, e) => MoveUp();
            Controls.Add( _moveUpButton );

            _moveDownButton = new Button();
            _moveDownButton.Location = new Point(65, 320);
            _moveDownButton.Size = new Size(50, 20);
            _moveDownButton.Text = "下へ";
            _moveDownButton.Click += (sender, e) => MoveDown();
            Controls.Add( _moveDownButton );

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
            }
            else
            {
                _listbox.Items.Clear();
                SelectedIndex = -1;
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
        void Delete()
        {
            if(SelectedIndex != -1)
            {
                _components.RemoveAt(SelectedIndex);
                UpdateAppearance(0);
                ValueChanged?.Invoke();
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
                    ValueChanged?.Invoke();
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
                ValueChanged?.Invoke();
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
                    ValueChanged?.Invoke();
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
                    ValueChanged?.Invoke();
                }
            }
        }
        public override void Disable()
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
        public override void Enable()
        {
            _listbox.Enabled = true;
            _deleteButton.Enabled = true;
            _addButton.Enabled = true;
            _renameButton.Enabled = true;
            _nameBox.Enabled = true;
            _moveUpButton.Enabled = true;
            _moveDownButton.Enabled = true;
        }
        void OnSelectedIndexChanged(object sender, EventArgs e)
        {
            ValueChanged?.Invoke();
        }
    }
    public interface IListboxAddDeletableComponent
    {
        string GetName();
        void SetName(string newName);
    }
}
