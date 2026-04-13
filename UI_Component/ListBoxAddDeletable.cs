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
    public class ListBoxAddDeletable
    {
        ListBox _listbox;
        Button _deleteButton;
        Button _addButton;
        Button _renameButton;
        Label _label_name;
        TextBox _nameBox;
        Button _moveUpButton;
        Button _moveDownButton;
        List<IListboxAddDeletableComponent> _components;
        IListboxAddDeletableComponent _defaultComponent;
        public Action OnChanged;
        public ListBoxAddDeletable(Point position)
        {
            _listbox = new ListBox();
            _listbox.Location = position;
            _listbox.Size = new Size(120, 240);
            _listbox.SelectedIndexChanged += (sender, e) => UpdateAppearance();

            _deleteButton = new Button();
            _deleteButton.Location = position + new Size(5, 245);
            _deleteButton.Size = new Size(50, 20);
            _deleteButton.Text = "削除";
            _deleteButton.Click += (sender, e) => Delete();

            _addButton = new Button();
            _addButton.Location = position + new Size(65, 245);
            _addButton.Size = new Size(50, 20);
            _addButton.Text = "追加";
            _addButton.Click += (sender, e) => Add();

            _renameButton = new Button();
            _renameButton.Location = position + new Size(5, 270);
            _renameButton.Size = new Size(50, 20);
            _renameButton.Text = "変更";
            _renameButton.Click += (sender, e) => Rename();

            _nameBox = new TextBox();
            _nameBox.Location = position + new Size(5, 295);
            _nameBox.Size = new Size(110, 20);

            _moveUpButton = new Button();
            _moveUpButton.Location = position + new Size(5, 320);
            _moveUpButton.Size = new Size(50, 20);
            _moveUpButton.Text = "上へ";
            _moveUpButton.Click += (sender, e) => MoveUp();

            _moveDownButton = new Button();
            _moveDownButton.Location = position + new Size(65, 320);
            _moveDownButton.Size = new Size(50, 20);
            _moveDownButton.Text = "下へ";
            _moveDownButton.Click += (sender, e) => MoveDown();

            UpdateAppearance();
        }
        void UpdateAppearance()
        {
            int selectedIndex = _listbox.SelectedIndex;
            _listbox.Items.Clear();
            foreach (IListboxAddDeletableComponent component in _components)
            {
                _listbox.Items.Add(component.GetName());
            }
            _listbox.SelectedIndex = selectedIndex;
            _nameBox.Text = _components[selectedIndex].GetName();
            OnChanged?.Invoke();
        }
        public int SelectedIndex()
        {
            return _listbox.SelectedIndex;
        }
        public void SetComponents(List<IListboxAddDeletableComponent> components)
        {
            _components = components;
            UpdateAppearance();
        }
        void Delete()
        {
            _components.RemoveAt(_listbox.SelectedIndex);
            UpdateAppearance();
        }
        void Add()
        {
            _components.Add(_defaultComponent);
            _listbox.SelectedIndex = _components.Count - 1;
            UpdateAppearance();
        }
        void Rename()
        {
            _components[_listbox.SelectedIndex].SetName(_nameBox.Text);
            UpdateAppearance();
        }
        void MoveUp()
        {
            int index = _listbox.SelectedIndex;
            if (index > 0)
            {
                var temp = _components[index - 1];
                _components[index - 1] = _components[index];
                _components[index] = temp;
                _listbox.SelectedIndex = index - 1;
                UpdateAppearance();
            }
        }
        void MoveDown()
        {
            int index = _listbox.SelectedIndex;
            if (index < _components.Count - 1)
            {
                var temp = _components[index + 1];
                _components[index + 1] = _components[index];
                _components[index] = temp;
                _listbox.SelectedIndex = index + 1;
                UpdateAppearance();
            }
        }
    }
    public interface IListboxAddDeletableComponent
    {
        string GetName();
        void SetName(string newName);
    }
}
