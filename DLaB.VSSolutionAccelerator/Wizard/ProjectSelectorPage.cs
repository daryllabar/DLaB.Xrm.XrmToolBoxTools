using Source.DLaB.Common;
using System.Windows.Forms;

namespace DLaB.VSSolutionAccelerator.Wizard
{
    public partial class ProjectSelectorPage : UserControl
    {
        public bool Optional => !string.IsNullOrWhiteSpace(Info.NoneText);
        public ProjectSelectorInfo Info { get; set; }
        private string LoadedSolutionPath { get; set; }

        public ProjectSelectorPage()
        {
            InitializeComponent();
        }

        public static ProjectSelectorPage Create(string question, ProjectSelectorInfo info)
        {
            var page = new ProjectSelectorPage
            {
                QuestionLabel = {Text = question},
                DescriptionText = {Text = info.Description},
                Info = info
            };

            return page;
        }

        private TreeNode CreateTreeNode(string projectLine)
        {
            projectLine = projectLine.Replace(" ", "");
            return new TreeNode(projectLine.SubstringByString("=\"", "\","))
            {
                Tag = projectLine.SubstringByString(",\"", "\",")
            };
        }

        private void SolutionView_AfterCheck(object sender, TreeViewEventArgs e)
        {
            if (!e.Node.Checked)
            {
                return;
            }
            UncheckAllNodesButCurrent(e.Node.TreeView.Nodes, e.Node);
        }

        private static void UncheckAllNodesButCurrent(TreeNodeCollection nodes, TreeNode checkedNode)
        {
            foreach (TreeNode node in nodes)
            {
                if (node != checkedNode)
                {
                    node.Checked = false;
                }
                UncheckAllNodesButCurrent(node.Nodes, checkedNode);
            }
        }
        private static TreeNode GetCheckedNode(TreeNodeCollection nodes)
        {
            foreach (TreeNode node in nodes)
            {
                if (node.Checked)
                {
                    return node;
                }
            }

            return null;
        }
    }
}
