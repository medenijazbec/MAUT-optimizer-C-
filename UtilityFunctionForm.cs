using System;
using System.Windows.Forms;

namespace MAUTmethod
{
    public partial class UtilityFunctionForm : Form
    {
        public string FunctionType { get; private set; }
        public double MinValue { get; private set; }
        public double MaxValue { get; private set; }

        public UtilityFunctionForm(double minValue, double maxValue)
        {
            InitializeComponent();
            this.Text = "Set Utility Function";
            this.Width = 400;
            this.Height = 250;

            Label functionLabel = new Label() { Left = 10, Top = 20, Text = "Function Type:", Width = 100 };
            ComboBox functionComboBox = new ComboBox() { Left = 120, Top = 20, Width = 250 };
            functionComboBox.Items.AddRange(new string[] { "Linear", "Exponential", "Logarithmic", "Concave", "Convex" });

            Label minValueLabel = new Label() { Left = 10, Top = 60, Text = "Min Value:", Width = 100 };
            TextBox minValueTextBox = new TextBox() { Left = 120, Top = 60, Width = 250, Text = minValue.ToString() };

            Label maxValueLabel = new Label() { Left = 10, Top = 100, Text = "Max Value:", Width = 100 };
            TextBox maxValueTextBox = new TextBox() { Left = 120, Top = 100, Width = 250, Text = maxValue.ToString() };

            Button confirmation = new Button() { Text = "Ok", Left = 250, Width = 100, Top = 150 };
            confirmation.Click += (sender, e) =>
            {
                this.FunctionType = functionComboBox.SelectedItem?.ToString();
                this.MinValue = double.Parse(minValueTextBox.Text);
                this.MaxValue = double.Parse(maxValueTextBox.Text);
                this.DialogResult = DialogResult.OK;
                this.Close();
            };

            this.Controls.Add(functionLabel);
            this.Controls.Add(functionComboBox);
            this.Controls.Add(minValueLabel);
            this.Controls.Add(minValueTextBox);
            this.Controls.Add(maxValueLabel);
            this.Controls.Add(maxValueTextBox);
            this.Controls.Add(confirmation);
        }
    }
}
