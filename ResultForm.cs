using System.Text;

namespace MAUTmethod
{
    public partial class ResultForm : Form
    {
        private TreeView treeViewParameters;
        private Label debugLabel;

        public ResultForm(Dictionary<string, Dictionary<string, double>> alternativeUtilities, TreeView treeViewParameters)
        {
            this.treeViewParameters = treeViewParameters;
            InitializeComponent(alternativeUtilities);
        }

        private void InitializeComponent(Dictionary<string, Dictionary<string, double>> alternativeUtilities)
        {
            this.Text = "Results";
            this.Width = 1000;
            this.Height = 600;

            // Create and set up the DataGridView
            DataGridView resultDataGridView = new DataGridView
            {
                Left = 10,
                Top = 10,
                Width = 960,
                Height = 500,
                AllowUserToAddRows = false,
                ReadOnly = true
            };

            resultDataGridView.Columns.Add("Parameter", "Parameter");
            resultDataGridView.Columns.Add("Weight", "Weight");

            foreach (var alt in alternativeUtilities.Keys)
            {
                resultDataGridView.Columns.Add(alt, alt);
            }

            var allParameters = GetAllParameters(treeViewParameters.Nodes);
            var weights = GetWeights(treeViewParameters.Nodes);

            // Calculate and add the utility for each parameter
            foreach (var param in allParameters)
            {
                DataGridViewRow row = new DataGridViewRow();
                row.CreateCells(resultDataGridView);
                row.Cells[0].Value = param;

                if (weights.ContainsKey(param))
                {
                    row.Cells[1].Value = param == "Root" ? 1.0 : weights[param] / 100.0; // Adjust weight to decimal format, set root weight to 1.0
                }
                else
                {
                    row.Cells[1].Value = "N/A";
                }

                int colIndex = 2; // Adjusted for weight column
                foreach (var alt in alternativeUtilities.Keys)
                {
                    double utility = CalculateUtility(param, alt, alternativeUtilities, weights);
                    row.Cells[colIndex].Value = utility == -1 ? "N/A" : utility.ToString();
                    colIndex++;
                }
                resultDataGridView.Rows.Add(row);
            }

            this.Controls.Add(resultDataGridView);

            // Create and set up the debug label
            debugLabel = new Label
            {
                Left = 10,
                Top = 520,
                Width = 960,
                Height = 60,
                AutoSize = true
            };
            this.Controls.Add(debugLabel);

            // Calculate and update the utility for the root node
            UpdateRootUtilities(resultDataGridView, alternativeUtilities, weights);

            // Rebuild tree hierarchy
            TreeView resultTreeView = new TreeView
            {
                Left = 10,
                Top = 580,
                Width = 960,
                Height = 200
            };
            CloneTreeNodes(treeViewParameters.Nodes, resultTreeView.Nodes);
            this.Controls.Add(resultTreeView);
        }

        private void UpdateRootUtilities(DataGridView resultDataGridView, Dictionary<string, Dictionary<string, double>> alternativeUtilities, Dictionary<string, double> weights)
        {
            DataGridViewRow rootRow = null;
            foreach (DataGridViewRow row in resultDataGridView.Rows)
            {
                if (row.Cells[0].Value != null && row.Cells[0].Value.ToString() == "Root")
                {
                    rootRow = row;
                    break;
                }
            }

            if (rootRow == null) return;

            // Set root weight to 1.0
            rootRow.Cells[1].Value = 1.0;

            // Collect debug information
            StringBuilder debugInfo = new StringBuilder();
            debugInfo.AppendLine("Root Node Children:");

            int rootColIndex = 2; // Adjusted for weight column
            foreach (var alt in alternativeUtilities.Keys)
            {
                double rootUtility = CalculateRootUtility(resultDataGridView, alternativeUtilities, weights, alt, debugInfo);
                rootRow.Cells[rootColIndex].Value = rootUtility == -1 ? "N/A" : rootUtility.ToString();
                rootColIndex++;
            }

            // Update the debug label with collected information
            debugLabel.Text = debugInfo.ToString();
        }

        private double CalculateRootUtility(DataGridView resultDataGridView, Dictionary<string, Dictionary<string, double>> alternativeUtilities, Dictionary<string, double> weights, string alt, StringBuilder debugInfo)
        {
            TreeNode rootNode = treeViewParameters.Nodes[0]; // Assuming the root node is the first node
            double utility = 0.0;

            debugInfo.AppendLine($"Root Node: {rootNode.Text}");

            foreach (TreeNode childNode in rootNode.Nodes)
            {
                string childParam = childNode.Text.Split(' ')[0];

                double childUtility = GetUtilityFromDataGridView(resultDataGridView, childParam, alt);
                double childWeight = GetWeightFromDataGridView(resultDataGridView, childParam);

                if (childUtility != -1 && childWeight != -1)
                {
                    utility += childUtility * childWeight;

                    // Append debug information
                    debugInfo.AppendLine($"Child: {childParam}, Utility: {childUtility}, Weight: {childWeight}");
                }
                else
                {
                    // Append debug information for missing values
                    debugInfo.AppendLine($"Child: {childParam}, Missing Utility or Weight");
                }
            }
            return utility;
        }

        private double GetUtilityFromDataGridView(DataGridView dgv, string param, string alt)
        {
            foreach (DataGridViewRow row in dgv.Rows)
            {
                if (row.Cells[0].Value != null && row.Cells[0].Value.ToString() == param)
                {
                    foreach (DataGridViewCell cell in row.Cells)
                    {
                        if (dgv.Columns[cell.ColumnIndex].HeaderText == alt)
                        {
                            if (double.TryParse(cell.Value.ToString(), out double utility))
                            {
                                return utility;
                            }
                        }
                    }
                }
            }
            return -1;
        }

        private double GetWeightFromDataGridView(DataGridView dgv, string param)
        {
            foreach (DataGridViewRow row in dgv.Rows)
            {
                if (row.Cells[0].Value != null && row.Cells[0].Value.ToString() == param)
                {
                    if (double.TryParse(row.Cells[1].Value.ToString(), out double weight))
                    {
                        return weight;
                    }
                }
            }
            return -1;
        }

        private double CalculateUtility(string param, string alt, Dictionary<string, Dictionary<string, double>> alternativeUtilities, Dictionary<string, double> weights)
        {
            // First check if it's a basic parameter
            if (alternativeUtilities[alt].ContainsKey(param))
            {
                return alternativeUtilities[alt][param];
            }

            // Otherwise, calculate it as a composite parameter
            TreeNode node = FindNodeByParameter(treeViewParameters.Nodes, param);
            if (node == null || node.Nodes.Count == 0)
                return -1;

            double utility = 0.0;
            foreach (TreeNode childNode in node.Nodes)
            {
                string childParam = childNode.Text.Split(' ')[0];
                if (alternativeUtilities[alt].ContainsKey(childParam) && weights.ContainsKey(childParam))
                {
                    utility += alternativeUtilities[alt][childParam] * (weights[childParam] / 100.0);
                }
            }
            return utility;
        }

        private TreeNode FindNodeByParameter(TreeNodeCollection nodes, string param)
        {
            foreach (TreeNode node in nodes)
            {
                if (node.Text.StartsWith(param + " "))
                    return node;
                if (node.Nodes.Count > 0)
                {
                    TreeNode foundNode = FindNodeByParameter(node.Nodes, param);
                    if (foundNode != null)
                        return foundNode;
                }
            }
            return null;
        }

        private Dictionary<string, double> GetWeights(TreeNodeCollection nodes)
        {
            Dictionary<string, double> weights = new Dictionary<string, double>();

            foreach (TreeNode node in nodes)
            {
                string parameterName = node.Text.Split(' ')[0];
                double weight = 1.0;

                if (node.Text.Contains('('))
                {
                    string weightStr = node.Text.Split('(')[1].Split(')')[0];
                    weight = double.Parse(weightStr);
                }

                weights[parameterName] = weight;

                if (node.Nodes.Count > 0)
                {
                    Dictionary<string, double> childWeights = GetWeights(node.Nodes);
                    foreach (var kvp in childWeights)
                    {
                        weights[kvp.Key] = kvp.Value;
                    }
                }
            }

            return weights;
        }

        private List<string> GetAllParameters(TreeNodeCollection nodes)
        {
            List<string> parameters = new List<string>();
            foreach (TreeNode node in nodes)
            {
                if (node.Nodes.Count == 0)
                {
                    // Basic parameter
                    parameters.Add(node.Text.Split(' ')[0]); // Extract parameter name without weight or utility info
                }
                else
                {
                    // Composite parameter
                    parameters.Add(node.Text.Split(' ')[0]);
                    parameters.AddRange(GetAllParameters(node.Nodes));
                }
            }
            return parameters;
        }

        private void CloneTreeNodes(TreeNodeCollection sourceNodes, TreeNodeCollection targetNodes)
        {
            foreach (TreeNode node in sourceNodes)
            {
                TreeNode newNode = new TreeNode(node.Text)
                {
                    Tag = node.Tag
                };
                targetNodes.Add(newNode);
                CloneTreeNodes(node.Nodes, newNode.Nodes);
            }
        }
    }
}
