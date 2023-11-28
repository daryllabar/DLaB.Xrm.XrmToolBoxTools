using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace DLaB.XrmToolBoxCommon.Editors
{
    public class PathEditor : UITypeEditor
    {
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            if (context == null || context.Instance == null)
                return base.GetEditStyle(context);
            return UITypeEditorEditStyle.Modal;
        }

        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {

            if (context?.Instance == null || provider == null || context.PropertyDescriptor == null)
            {
                return value;
            }

            var info = (PathEditorAttribute) context.PropertyDescriptor.Attributes.Cast<Attribute>().FirstOrDefault(a => a is PathEditorAttribute)
                       ?? new PathEditorAttribute();

            var isDirectory = info.GetDirectoryFlag(context);
            var fileName = info.GetDefaultFileName(context, (string)value);

            if (isDirectory)
            {
                var folderDialog = new FolderBrowserDialog
                {
                    SelectedPath = fileName == null ? null : Path.GetFullPath(fileName),
                };

                using (folderDialog)
                {
                    DialogResult res = folderDialog.ShowDialog();
                    if (res == DialogResult.OK)
                    {
                        return info.GetPath(folderDialog.SelectedPath) + @"\";
                    }
                }
            }
            else
            {
                var fileDialog = new OpenFileDialog
                {
                    CheckFileExists = info.CheckFileExists,
                    CheckPathExists = info.CheckPathExists,
                    DefaultExt = info.DefaultExt,
                    Filter = info.Filter,
                    FileName = fileName,
                };

                using (fileDialog)
                {
                    DialogResult res = fileDialog.ShowDialog();
                    if (res == DialogResult.OK)
                    {
                        return info.GetPath(fileDialog.FileName);
                    }
                }
            }

            return value;
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class PathEditorAttribute: Attribute
    {
        public string Filter { get; set; }
        public bool CheckFileExists { get; set; }

        public bool CheckPathExists { get; set; }
        public string DefaultExt { get; set; }

        public bool IsDirectory { get; set; }

        public PathEditorAttribute(string filter = "All Files (*.*)|*.*", string defaultExt = "", bool checkFileExists = true, bool checkPathExists = true, bool isDirectory = false)
        {
            CheckFileExists = checkFileExists;
            CheckPathExists = checkPathExists;
            DefaultExt = defaultExt;
            Filter = filter;
            IsDirectory = isDirectory;
        }

        public virtual string GetDefaultFileName(ITypeDescriptorContext context, string currentPath)
        {
            if (currentPath == null)
            {
                return null;
            }
            return File.Exists(currentPath)
                    || Directory.Exists(currentPath)
                ? currentPath
                : null;
        }

        public virtual string GetPath(string absolutePath)
        {
            return absolutePath;
        }

        public virtual bool GetDirectoryFlag(ITypeDescriptorContext context)
        {
            return IsDirectory;
        }
    }

    public class RelativePathEditorAttribute : PathEditorAttribute
    {
        public string BasePath { get; set; }

        public RelativePathEditorAttribute(string basePath, string filter = "All Files (*.*)|*.*", string defaultExt = "", bool checkFileExists = true, bool checkPathExists = true, bool isDirectory = false) : base(filter, defaultExt, checkFileExists, checkPathExists, isDirectory)
        {
            BasePath = basePath;
        }

        public override string GetDefaultFileName(ITypeDescriptorContext context, string currentPath)
        {
            if (File.Exists(BasePath))
            {
                BasePath = Path.GetDirectoryName(BasePath);
            }
            var absolutePath = Path.Combine(BasePath?? "NULL", currentPath ?? string.Empty);
            return base.GetDefaultFileName(context, absolutePath);
        }

        public override string GetPath(string absolutePath)
        {

            var relativeDirectory = File.Exists(BasePath)
                ? Path.GetDirectoryName(BasePath)
                : BasePath;

            return GetPath(absolutePath, relativeDirectory);
        }

        private string GetPath(string absolutePath, string relativeDirectory)
        {
            if (absolutePath == null)
            {
                return base.GetPath(null);
            }
            if (string.IsNullOrWhiteSpace(relativeDirectory))
            {
                return absolutePath;
            }

            if (!relativeDirectory.EndsWith("\\"))
            {
                relativeDirectory += "\\";
            }

            var relativePath = Uri.UnescapeDataString(new Uri(relativeDirectory).MakeRelativeUri(new Uri(absolutePath)).ToString());
            return relativePath.Replace('/', '\\');
        }
    }

    public class DynamicRelativePathEditorAttribute : RelativePathEditorAttribute
    {
        public string RelativePathPropertyName { get; set; }
        public string DirectoryFlagPropertyName { get; set; }

        /// <summary>
        ///
        /// </summary>
        /// <param name="relativePathPropertyName">Specifies the name of a property of the context that contains the relative path</param>
        /// <param name="directoryFlagPropertyName">Specifies the name of a property of the context that determines if the relative path is a folder</param>
        /// <param name="filter"></param>
        /// <param name="defaultExt"></param>
        /// <param name="checkFileExists"></param>
        /// <param name="checkPathExists"></param>
        public DynamicRelativePathEditorAttribute(string relativePathPropertyName, string directoryFlagPropertyName, string filter = "All Files (*.*)|*.*", string defaultExt = "", bool checkFileExists = true, bool checkPathExists = true) : base(null, filter, defaultExt, checkFileExists, checkPathExists)
        {
            RelativePathPropertyName = relativePathPropertyName;
            DirectoryFlagPropertyName = directoryFlagPropertyName;
        }

        public DynamicRelativePathEditorAttribute(string relativePathPropertyName, bool isDirectory, string filter = "All Files (*.*)|*.*", string defaultExt = "", bool checkFileExists = true, bool checkPathExists = true) : base(null, filter, defaultExt, checkFileExists, checkPathExists, isDirectory)
        {
            RelativePathPropertyName = relativePathPropertyName;
        }

        public override string GetDefaultFileName(ITypeDescriptorContext context, string currentPath)
        {
            var prop = context.Instance.GetType().GetProperty(RelativePathPropertyName, BindingFlags.Public | BindingFlags.Instance);
            if (prop == null)
            {
                throw new NullReferenceException(RelativePathPropertyName + " is not a property of " + context.Instance.GetType().FullName);
            }
            BasePath = (string)prop.GetValue(context.Instance);
            return base.GetDefaultFileName(context, currentPath);
        }

        public override bool GetDirectoryFlag(ITypeDescriptorContext context)
        {
            if (DirectoryFlagPropertyName == null)
            {
                return base.GetDirectoryFlag(context);
            }

            var prop = context.Instance.GetType().GetProperty(DirectoryFlagPropertyName, BindingFlags.Public | BindingFlags.Instance);
            if (prop == null)
            {
                throw new NullReferenceException(DirectoryFlagPropertyName + " is not a property of " + context.Instance.GetType().FullName);
            }
            return IsDirectory = (bool)prop.GetValue(context.Instance);
        }
    }
}
