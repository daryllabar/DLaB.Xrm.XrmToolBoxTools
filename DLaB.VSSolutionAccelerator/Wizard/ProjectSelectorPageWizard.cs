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
            var defaultFound = false;
            foreach (var project in solutionParser.Projects
                .Where(l => !Info.ProjectFilter.HasValue || l.Contains(ProjectInfo.GetTypeId(Info.ProjectFilter.Value)))
                .Select(CreateTreeNode).OrderBy(n => n.Text))
            {
                if (Info.DefaultProjectWithFile != null && !defaultFound)
                {
                    var path = project.Tag.ToString();
                    var setAsDefault = IsDefaultNode(path);

                    if (setAsDefault)
                    {
                        defaultFound = true;
                        project.Checked = true;
                    }
                }
                SolutionView.Nodes.Add(project);
            }
            SolutionView.EndUpdate();
        }

        private bool IsDefaultNode(string path)
        {
            var setAsDefault = false;
            if (path.EndsWith(".shproj"))
            {
                setAsDefault = File.ReadAllLines(path.Substring(0, path.Length - "shproj".Length) + "projitems")
                    .Any(l => l.Contains(Info.DefaultProjectWithFile + "\""));
            }
            else
            {
                var parser = new ProjectFileParser(File.ReadAllLines(path));
                if (parser.ItemGroups.TryGetValue(ProjectFileParser.ItemGroupTypes.Compile, out var compileItems))
                {
                    setAsDefault = ContainsDefaultProjectFile(compileItems);
                }

                if (!setAsDefault && parser.ItemGroups.TryGetValue(ProjectFileParser.ItemGroupTypes.None, out var noneItems))
                {
                    setAsDefault = ContainsDefaultProjectFile(noneItems);
                }

                if (!setAsDefault && parser.ItemGroups.TryGetValue(ProjectFileParser.ItemGroupTypes.Content, out var contentItems))
                {
                    setAsDefault = ContainsDefaultProjectFile(contentItems);
                }
            }

            return setAsDefault;
        }

        private bool ContainsDefaultProjectFile(List<string> items)
        {
            return items.Any(i => i.Contains($@"""{Info.DefaultProjectWithFile}"""));
        }

        bool IWizardPage.IsRequired(object[] saveResults)
        {
            return true;
        }
    }
}
