using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DLaB.VSSolutionAccelerator.Logic;

namespace DLaB.VSSolutionAccelerator.Wizard
{
    public class ProjectSelectorInfo
    {
        public string Description { get; set; }
        public string NoneText { get; set; }
        public int SavedResultsGenericPageIndex { get; set; }
        public ProjectInfo.ProjectType? ProjectFilter { get; set; }
        public string DefaultProjectWithFile { get; set; }
    }
}
