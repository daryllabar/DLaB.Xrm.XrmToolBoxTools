using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using DLaB.VSSolutionAccelerator.Logic;

namespace DLaB.VSSolutionAccelerator.Wizard
{
    public partial class ProjectSelectorPage : IWizardPage
    {
        public UserControl Content => this;

        public bool PageValid
        {
            get
            {
                if (Optional)
                {
                    return true;
                }

                return GetCheckedNode(SolutionView.Nodes) != null;
            }
        }

        public string ValidationMessage => "Please select a project!";

        public void Cancel()
        {
        }

        public object Save()
        {
            return GetCheckedNode(SolutionView.Nodes).Tag.ToString();
        }

        void IWizardPage.Load(object[] saveResults)
        {
            // Called when the Page is loaded in the wizard.
            // Helpful if additional logic is required before loading

            var solutionPath = saveResults[Info.SavedResultsGenericPageIndex].ToString();
            if (LoadedSolutionPath == solutionPath)
            {
                // No Changes required
                return;
            }

            LoadedSolutionPath = solutionPath;
            SolutionView.BeginUpdate();
            SolutionView.Nodes.Clear();

            if (Optional)
            {
                SolutionView.Nodes.Add(new TreeNode(Info.NoneText) { Tag = string.Empty });
            }

            var solutionParser = new SolutionFileParser(File.ReadAllLines(solutionPath));
            foreach (var project in solutionParser.Projects
                .Where(l => !Info.ProjectFilter.HasValue || l.Contains(ProjectInfo.GetTypeId(Info.ProjectFilter.Value)))
                .Select(CreateTreeNode).OrderBy(n => n.Text))
            {
                SolutionView.Nodes.Add(project);
            }
            SolutionView.EndUpdate();
        }

        bool IWizardPage.IsRequired(object[] saveResults)
        {
            return true;
        }
    }
}
