using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace MAUTmethod
{
    public partial class AlternativeForm : Form
    {
        public string AlternativeName { get; private set; }
        public List<double> ParameterValues { get; private set; }

        private TextBox nameTextBox;
        private List<TextBox> parameterTextBoxes;

        public AlternativeForm(List<string> parameterNames)
        {
            InitializeComponent(parameterNames);
        }

        public AlternativeForm(List<string> parameterNames, string alternativeName, List<double> parameterValues) : this(parameterNames)
        {
            this.AlternativeName = alternativeName;
            this.ParameterValues = parameterValues;
            InitializeFields();
        }

        private void InitializeComponent(List<string> parameterNames)
        {
            this.Text = "Alternative";
            this.Width = 400;
            this.Height = 400;

            Label nameLabel = new Label() { Left = 10, Top = 20, Text = "Alternative Name:", Width = 100 };
            nameTextBox = new TextBox() { Left = 120, Top = 20, Width = 250 };
            this.Controls.Add(nameLabel);
            this.Controls.Add(nameTextBox);

            int top = 60;
            parameterTextBoxes = new List<TextBox>();

            foreach (var paramName in parameterNames)
            {
                Label paramLabel = new Label() { Left = 10, Top = top, Text = paramName + ":", Width = 100 };
                TextBox paramTextBox = new TextBox() { Left = 120, Top = top, Width = 250 };
                paramTextBox.Name = paramName;
                this.Controls.Add(paramLabel);
                this.Controls.Add(paramTextBox);
                parameterTextBoxes.Add(paramTextBox);

                top += 40;
            }

            Button confirmation = new Button() { Text = "Ok", Left = 270, Width = 100, Top = top };
            confirmation.Click += (sender, e) =>
            {
                this.AlternativeName = nameTextBox.Text;
                this.ParameterValues = new List<double>();
                foreach (var paramTextBox in parameterTextBoxes)
                {
                    this.ParameterValues.Add(double.Parse(paramTextBox.Text));
                }
                this.DialogResult = DialogResult.OK;
                this.Close();
            };

            this.Controls.Add(confirmation);
        }

        private void InitializeFields()
        {
            nameTextBox.Text = this.AlternativeName;
            for (int i = 0; i < this.ParameterValues.Count; i++)
            {
                parameterTextBoxes[i].Text = this.ParameterValues[i].ToString();
            }
        }
    }
}