using System.Collections.Generic;

namespace DLaB.ModelBuilderExtensions.Entity
{
    /// <summary>
    /// Caches the distinct ProcessStage stage names retrieved from Dataverse,
    /// so they can be used by <see cref="ProcessStageNameConstantsGenerator"/> during code generation.
    /// </summary>
    public static class ProcessStageNameCache
    {
        private static List<string> _stageNames;

        /// <summary>
        /// The distinct stage names loaded from Dataverse.  Null if not yet loaded.
        /// </summary>
        public static List<string> StageNames
        {
            get => _stageNames;
            set => _stageNames = value;
        }

        /// <summary>
        /// Clears the cached stage names.
        /// </summary>
        public static void Clear()
        {
            _stageNames = null;
        }
    }
}
