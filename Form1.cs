using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using OxyPlot;
using OxyPlot.Series;
using OxyPlot.WindowsForms;

namespace MAUTmethod
{
    public partial class Form1 : Form
    {
        private Label debugLabel;
        private Label parameterNamesLabel;
        private Label utilityValuesLabel;

        private TreeView treeViewParameters;
        private DataGridView dataGridViewAlternatives;
        private Button buttonAddParameter;
        private Button buttonEditParameter;
        private Button buttonDeleteParameter;
        private Button buttonCalculate;
        private Button buttonSetUtilityFunction;
        private ComboBox comboBoxUtilityFunction;

        private Button buttonAddAlternative;
        private Button buttonEditAlternative;

        private Dictionary<string, UtilityFunction> utilityFunctions;

        public Form1()
        {
            InitializeComponent();
            utilityFunctions = new Dictionary<string, UtilityFunction>();

            // Initialize TreeView
            this.treeViewParameters = new TreeView();
            this.treeViewParameters.Location = new System.Drawing.Point(12, 12);
            this.treeViewParameters.Size = new System.Drawing.Size(200, 400);
            this.treeViewParameters.AfterSelect += new TreeViewEventHandler(this.treeViewParameters_AfterSelect);
            this.Controls.Add(this.treeViewParameters);

            // Initialize DataGridView
            this.dataGridViewAlternatives = new DataGridView();
            this.dataGridViewAlternatives.Location = new System.Drawing.Point(220, 12);
            this.dataGridViewAlternatives.Size = new System.Drawing.Size(600, 200);
            this.dataGridViewAlternatives.AllowUserToAddRows = false;
            this.Controls.Add(this.dataGridViewAlternatives);

            // Initialize ComboBox for Utility Functions
            this.comboBoxUtilityFunction = new ComboBox();
            this.comboBoxUtilityFunction.Items.AddRange(new string[] { "Linear", "Exponential", "Logarithmic", "Convex", "Concave" });
            this.comboBoxUtilityFunction.Location = new System.Drawing.Point(220, 220);
            this.comboBoxUtilityFunction.Size = new System.Drawing.Size(200, 21);
            this.Controls.Add(this.comboBoxUtilityFunction);

            // Initialize Buttons
            this.buttonAddParameter = new Button();
            this.buttonAddParameter.Text = "Add Parameter";
            this.buttonAddParameter.Location = new System.Drawing.Point(12, 420);
            this.buttonAddParameter.Click += new EventHandler(this.buttonAddParameter_Click);
            this.Controls.Add(this.buttonAddParameter);

            this.buttonEditParameter = new Button();
            this.buttonEditParameter.Text = "Edit Parameter";
            this.buttonEditParameter.Location = new System.Drawing.Point(120, 420);
            this.buttonEditParameter.Click += new EventHandler(this.buttonEditParameter_Click);
            this.Controls.Add(this.buttonEditParameter);

            this.buttonDeleteParameter = new Button();
            this.buttonDeleteParameter.Text = "Delete Parameter";
            this.buttonDeleteParameter.Location = new System.Drawing.Point(230, 420);
            this.buttonDeleteParameter.Click += new EventHandler(this.buttonDeleteParameter_Click);
            this.Controls.Add(this.buttonDeleteParameter);

            this.buttonSetUtilityFunction = new Button();
            this.buttonSetUtilityFunction.Text = "Set Utility Function";
            this.buttonSetUtilityFunction.Location = new System.Drawing.Point(340, 420);
            this.buttonSetUtilityFunction.Click += new EventHandler(this.buttonSetUtilityFunction_Click);
            this.Controls.Add(this.buttonSetUtilityFunction);

            this.buttonCalculate = new Button();
            this.buttonCalculate.Text = "Calculate";
            this.buttonCalculate.Location = new System.Drawing.Point(450, 420);
            this.buttonCalculate.Click += new EventHandler(this.buttonCalculate_Click);
            this.Controls.Add(this.buttonCalculate);

            // Initialize Alternative Management Buttons
            this.buttonAddAlternative = new Button();
            this.buttonAddAlternative.Text = "Add Alternative";
            this.buttonAddAlternative.Location = new System.Drawing.Point(560, 420);
            this.buttonAddAlternative.Size = new System.Drawing.Size(150, 24);
            this.buttonAddAlternative.Click += new EventHandler(this.buttonAddAlternative_Click);
            this.Controls.Add(this.buttonAddAlternative);

            this.buttonEditAlternative = new Button();
            this.buttonEditAlternative.Text = "Edit Alternative";
            this.buttonEditAlternative.Size = new System.Drawing.Size(150, 24);
            this.buttonEditAlternative.Location = new System.Drawing.Point(740, 420);
            this.buttonEditAlternative.Click += new EventHandler(this.buttonEditAlternative_Click);
            this.Controls.Add(this.buttonEditAlternative);

            // Initialize debug labels
            debugLabel = new Label();
            debugLabel.Location = new System.Drawing.Point(220, 250);
            debugLabel.Size = new System.Drawing.Size(400, 20);
            debugLabel.Text = "Debug info will be shown here.";
            this.Controls.Add(debugLabel);

            parameterNamesLabel = new Label();
            parameterNamesLabel.Location = new System.Drawing.Point(220, 280);
            parameterNamesLabel.Size = new System.Drawing.Size(400, 20);
            parameterNamesLabel.Text = "Parameter names will be shown here.";
            this.Controls.Add(parameterNamesLabel);

            utilityValuesLabel = new Label();
            utilityValuesLabel.Location = new System.Drawing.Point(220, 310);
            utilityValuesLabel.Size = new System.Drawing.Size(400, 20);
            utilityValuesLabel.Text = "Utility values will be shown here.";
            this.Controls.Add(utilityValuesLabel);

            InitializeAlternativeGrid();
        }

        private void InitializeAlternativeGrid()
        {
            // Add the columns to the DataGridView
            this.dataGridViewAlternatives.Columns.Add("AlternativeName", "Alternative Name");

            // Add columns for each parameter and their utility values
            AddParameterColumns(treeViewParameters.Nodes);
        }

        private void AddParameterColumns(TreeNodeCollection nodes)
        {
            foreach (TreeNode node in nodes)
            {
                ParameterInfo parameterInfo = node.Tag as ParameterInfo;
                if (parameterInfo != null && parameterInfo.Type == "Basic")
                {
                    string parameterName = node.Text.Split(' ')[0];
                    this.dataGridViewAlternatives.Columns.Add(parameterName, parameterName);
                    this.dataGridViewAlternatives.Columns.Add($"{parameterName}_Utility", $"{parameterName} Utility");
                }
                AddParameterColumns(node.Nodes);
            }
        }

        // Add Alternative
        private void buttonAddAlternative_Click(object sender, EventArgs e)
        {
            AlternativeForm alternativeForm = new AlternativeForm(GetParameterNames());
            if (alternativeForm.ShowDialog() == DialogResult.OK)
            {
                DataGridViewRow row = new DataGridViewRow();
                row.CreateCells(this.dataGridViewAlternatives);
                row.Cells[0].Value = alternativeForm.AlternativeName;
                for (int i = 0; i < alternativeForm.ParameterValues.Count; i++)
                {
                    row.Cells[2 * i + 1].Value = alternativeForm.ParameterValues[i];
                }
                this.dataGridViewAlternatives.Rows.Add(row);
            }
        }

        // Edit Alternative
        private void buttonEditAlternative_Click(object sender, EventArgs e)
        {
            if (this.dataGridViewAlternatives.SelectedRows.Count > 0)
            {
                DataGridViewRow selectedRow = this.dataGridViewAlternatives.SelectedRows[0];
                string alternativeName = selectedRow.Cells[0].Value.ToString();
                List<double> parameterValues = new List<double>();
                for (int i = 1; i < selectedRow.Cells.Count; i += 2)
                {
                    parameterValues.Add(Convert.ToDouble(selectedRow.Cells[i].Value));
                }

                AlternativeForm alternativeForm = new AlternativeForm(GetParameterNames(), alternativeName, parameterValues);
                if (alternativeForm.ShowDialog() == DialogResult.OK)
                {
                    selectedRow.Cells[0].Value = alternativeForm.AlternativeName;
                    for (int i = 0; i < alternativeForm.ParameterValues.Count; i++)
                    {
                        selectedRow.Cells[2 * i + 1].Value = alternativeForm.ParameterValues[i];
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select an alternative to edit.");
            }
        }

        private List<string> GetParameterNames()
        {
            List<string> parameterNames = new List<string>();
            foreach (DataGridViewColumn column in this.dataGridViewAlternatives.Columns)
            {
                if (column.Index == 0 || column.HeaderText.EndsWith("Utility")) continue; // Skip the first column and utility columns
                parameterNames.Add(column.HeaderText);
            }
            return parameterNames;
        }

        private void buttonSetUtilityFunction_Click(object sender, EventArgs e)
        {
            TreeNode selectedNode = treeViewParameters.SelectedNode;
            if (selectedNode != null)
            {
                ParameterInfo parameterInfo = selectedNode.Tag as ParameterInfo;
                if (parameterInfo != null && parameterInfo.Type == "Basic")
                {
                    string parameterName = selectedNode.Text.Split(' ')[0]; // Extract parameter name without weight
                    var minMaxValues = GetMinMaxValues(parameterName);
                    UtilityFunctionForm utilityFunctionForm = new UtilityFunctionForm(minMaxValues.min, minMaxValues.max);
                    if (utilityFunctionForm.ShowDialog() == DialogResult.OK)
                    {
                        string functionType = utilityFunctionForm.FunctionType;
                        double minValue = utilityFunctionForm.MinValue;
                        double maxValue = utilityFunctionForm.MaxValue;

                        utilityFunctions[parameterName] = new UtilityFunction
                        {
                            FunctionType = functionType,
                            MinValue = minValue,
                            MaxValue = maxValue
                        };

                        selectedNode.Text = $"{parameterName} ({parameterInfo.Weight}) [Utility: {functionType}]";

                        // Recalculate utilities for the updated parameter
                        RecalculateUtilitiesForParameter(parameterName);
                    }
                }
                else
                {
                    MessageBox.Show("Utility functions can only be set for basic parameters.");
                }
            }
            else
            {
                MessageBox.Show("Please select a parameter to set a utility function.");
            }
        }


        private void RecalculateUtilitiesForParameter(string parameterName)
        {
            foreach (DataGridViewRow row in dataGridViewAlternatives.Rows)
            {
                if (row.IsNewRow) continue;
                double value = Convert.ToDouble(row.Cells[parameterName].Value);
                double utility = utilityFunctions[parameterName].CalculateUtility(value);
                row.Cells[$"{parameterName}_Utility"].Value = utility;
            }
        }


        private (double min, double max) GetMinMaxValues(string parameterName)
        {
            double minValue = double.MaxValue;
            double maxValue = double.MinValue;

            foreach (DataGridViewRow row in dataGridViewAlternatives.Rows)
            {
                if (row.IsNewRow) continue;
                double value = Convert.ToDouble(row.Cells[parameterName].Value);
                if (value < minValue)
                {
                    minValue = value;
                }
                if (value > maxValue)
                {
                    maxValue = value;
                }
            }

            return (minValue, maxValue);
        }

        private void buttonAddParameter_Click(object sender, EventArgs e)
        {
            TreeNode selectedNode = treeViewParameters.SelectedNode;

            // If no node is selected and tree is empty, create a root node
            if (selectedNode == null && treeViewParameters.Nodes.Count == 0)
            {
                selectedNode = new TreeNode("Root");
                treeViewParameters.Nodes.Add(selectedNode);
            }

            if (selectedNode != null)
            {
                AddParameterForm addParameterForm = new AddParameterForm();
                if (addParameterForm.ShowDialog() == DialogResult.OK)
                {
                    string parameterName = addParameterForm.ParameterName;
                    string parameterType = addParameterForm.ParameterType;
                    int parameterWeight = addParameterForm.ParameterWeight;

                    if (!string.IsNullOrEmpty(parameterName) && ValidateWeight(selectedNode, parameterWeight))
                    {
                        TreeNode newNode = new TreeNode($"{parameterName} ({parameterWeight})");
                        newNode.Tag = new ParameterInfo { Type = parameterType, Weight = parameterWeight };
                        selectedNode.Nodes.Add(newNode);
                        selectedNode.Expand();

                        // Add a new column to the DataGridView for the new basic parameter
                        if (parameterType == "Basic")
                        {
                            this.dataGridViewAlternatives.Columns.Add(parameterName, parameterName);
                            this.dataGridViewAlternatives.Columns.Add($"{parameterName}_Utility", $"{parameterName} Utility");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Invalid parameter name or weight.");
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select a parent node to add a parameter.");
            }
        }

        private void buttonEditParameter_Click(object sender, EventArgs e)
        {
            TreeNode selectedNode = treeViewParameters.SelectedNode;
            if (selectedNode != null)
            {
                // Prevent editing the root node
                if (selectedNode.Text == "Root")
                {
                    MessageBox.Show("Root node cannot be edited.");
                    return;
                }

                ParameterInfo parameterInfo = selectedNode.Tag as ParameterInfo;
                EditParameterForm editParameterForm = new EditParameterForm(selectedNode.Text, parameterInfo.Type, parameterInfo.Weight);
                if (editParameterForm.ShowDialog() == DialogResult.OK)
                {
                    if (ValidateWeight(selectedNode.Parent, editParameterForm.ParameterWeight - parameterInfo.Weight))
                    {
                        selectedNode.Text = $"{editParameterForm.ParameterName} ({editParameterForm.ParameterWeight})";
                        selectedNode.Tag = new ParameterInfo { Type = editParameterForm.ParameterType, Weight = editParameterForm.ParameterWeight };
                    }
                    else
                    {
                        MessageBox.Show("Invalid parameter weight.");
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select a parameter to edit.");
            }
        }

        private bool ValidateWeight(TreeNode parentNode, int newWeight)
        {
            int totalWeight = newWeight;
            foreach (TreeNode childNode in parentNode.Nodes)
            {
                if (childNode != treeViewParameters.SelectedNode)
                {
                    ParameterInfo parameterInfo = childNode.Tag as ParameterInfo;
                    totalWeight += parameterInfo.Weight;
                }
            }
            return totalWeight <= 100;
        }

        public class ParameterInfo
        {
            public string Type { get; set; }
            public int Weight { get; set; }
        }

        private void buttonCalculate_Click(object sender, EventArgs e)
        {
            var alternativeUtilities = new Dictionary<string, Dictionary<string, double>>();

            foreach (DataGridViewRow row in dataGridViewAlternatives.Rows)
            {
                if (row.IsNewRow) continue;

                string alternativeName = row.Cells[0].Value.ToString();
                var utilities = new Dictionary<string, double>();

                for (int i = 1; i < row.Cells.Count; i += 2)
                {
                    string parameterName = dataGridViewAlternatives.Columns[i].HeaderText;
                    double utilityValue = Convert.ToDouble(row.Cells[i + 1].Value);
                    utilities[parameterName] = utilityValue;
                }

                alternativeUtilities[alternativeName] = utilities;
            }

            ResultForm resultForm = new ResultForm(alternativeUtilities, treeViewParameters);
            resultForm.Show();
        }









       

        private void buttonDeleteParameter_Click(object sender, EventArgs e)
        {
            TreeNode selectedNode = treeViewParameters.SelectedNode;
            if (selectedNode != null)
            {
                if (MessageBox.Show("Are you sure you want to delete this parameter?", "Delete Parameter", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    treeViewParameters.Nodes.Remove(selectedNode);
                }
            }
            else
            {
                MessageBox.Show("Please select a parameter to delete.");
            }
        }

        // Event Handlers (to be implemented)
        private void treeViewParameters_AfterSelect(object sender, TreeViewEventArgs e) { }

        // Utility Function class
        public class UtilityFunction
        {
            public string FunctionType { get; set; }
            public double MinValue { get; set; }
            public double MaxValue { get; set; }

            public double CalculateUtility(double value)
            {
                switch (FunctionType)
                {
                    case "Linear":
                        return (value - MinValue) / (MaxValue - MinValue);
                    case "Exponential":
                        return Math.Exp(value - MinValue) / Math.Exp(MaxValue - MinValue);
                    case "Logarithmic":
                        return Math.Log(value - MinValue + 1) / Math.Log(MaxValue - MinValue + 1);
                    case "Concave":
                        return Math.Pow((value - MinValue) / (MaxValue - MinValue), 2);
                    case "Convex":
                        return Math.Sqrt((value - MinValue) / (MaxValue - MinValue));
                    default:
                        return 0;
                }
            }
        }
    }
}
