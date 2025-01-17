using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MAUTmethod
{
    public partial class EditParameterForm : Form
    {
        public string ParameterName { get; private set; }
        public string ParameterType { get; private set; }
        public int ParameterWeight { get; private set; }
        public EditParameterForm(string currentName, string currentType, int currentWeight)
        {
            InitializeComponent(currentName, currentType, currentWeight);
        }
        private void InitializeComponent(string currentName, string currentType, int currentWeight)
        {
            this.Text = "Edit Parameter";
            this.Width = 400;
            this.Height = 250;

            Label nameLabel = new Label() { Left = 10, Top = 20, Text = "Parameter Name:", Width = 100 };
            TextBox nameTextBox = new TextBox() { Left = 120, Top = 20, Width = 250, Text = currentName.Split(' ')[0] };

            Label typeLabel = new Label() { Left = 10, Top = 60, Text = "Parameter Type:", Width = 100 };
            ComboBox typeComboBox = new ComboBox() { Left = 120, Top = 60, Width = 250 };
            typeComboBox.Items.AddRange(new string[] { "Basic", "Composite" });
            typeComboBox.SelectedItem = currentType;

            Label weightLabel = new Label() { Left = 10, Top = 100, Text = "Parameter Weight:", Width = 100 };
            TextBox weightTextBox = new TextBox() { Left = 120, Top = 100, Width = 250, Text = currentWeight.ToString() };

            Button confirmation = new Button() { Text = "Ok", Left = 250, Width = 100, Top = 150 };
            confirmation.Click += (sender, e) =>
            {
                this.ParameterName = nameTextBox.Text;
                this.ParameterType = typeComboBox.SelectedItem?.ToString();
                this.ParameterWeight = int.Parse(weightTextBox.Text);
                this.DialogResult = DialogResult.OK;
                this.Close();
            };

            this.Controls.Add(nameLabel);
            this.Controls.Add(nameTextBox);
            this.Controls.Add(typeLabel);
            this.Controls.Add(typeComboBox);
            this.Controls.Add(weightLabel);
            this.Controls.Add(weightTextBox);
            this.Controls.Add(confirmation);
        }
    }
}