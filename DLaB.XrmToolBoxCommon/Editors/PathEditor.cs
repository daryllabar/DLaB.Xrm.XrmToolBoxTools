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

            var dlg = new OpenFileDialog
            {
                CheckFileExists = info.CheckFileExists,
                CheckPathExists = info.CheckPathExists,
                DefaultExt = info.DefaultExt,
                Filter = info.Filter,
                FileName = info.GetDefaultFileName(context, (string)value)
            };

            using (dlg)
            {
                DialogResult res = dlg.ShowDialog();
                if (res == DialogResult.OK)
                {
                    return info.GetPath(dlg.FileName);
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

        public PathEditorAttribute(string filter = "All Files (*.*)|*.*", string defaultExt = "", bool checkFileExists = true, bool checkPathExists = true)
        {
            CheckFileExists = checkFileExists;
            CheckPathExists = checkPathExists;
            DefaultExt = defaultExt;
            Filter = filter;
        }

        public virtual string GetDefaultFileName(ITypeDescriptorContext context, string currentPath)
        {
            return File.Exists(currentPath)
                    || Directory.Exists(currentPath)
                ? currentPath
                : null;
        }

        public virtual string GetPath(string absolutePath)
        {
            return absolutePath;
        }
    }

    public class RelativePathEditorAttribute : PathEditorAttribute
    {
        public string BasePath { get; set; }

        public RelativePathEditorAttribute(string basePath, string filter = "All Files (*.*)|*.*", string defaultExt = "", bool checkFileExists = true, bool checkPathExists = true) : base(filter, defaultExt, checkFileExists, checkPathExists)
        {
            BasePath = basePath;
        }

        public override string GetDefaultFileName(ITypeDescriptorContext context, string currentPath)
        {
            if (File.Exists(BasePath))
            {
                BasePath = Path.GetDirectoryName(BasePath);
            }
            var absolutePath = Path.Combine(BasePath, currentPath);
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
                return base.GetPath(absolutePath);
            }
            if (string.IsNullOrWhiteSpace(relativeDirectory))
            {
                return absolutePath;
            }

            if (!relativeDirectory.EndsWith("\\"))
            {
                relativeDirectory += "\\";
            }

            var relativePath = new Uri(relativeDirectory).MakeRelativeUri(new Uri(absolutePath));
            return relativePath.ToString().Replace('/', '\\');
        }
    }

    public class DynamicRelativePathEditorAttribute : RelativePathEditorAttribute
    {
        public string RelativePathPropertyName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="relativePathPropertyName">Specifies the name of a property of the context that contains the relative path</param>
        /// <param name="filter"></param>
        /// <param name="checkFileExists"></param>
        /// <param name="checkPathExists"></param>
        public DynamicRelativePathEditorAttribute(string relativePathPropertyName, string filter = "All Files (*.*)|*.*", string defaultExt = "", bool checkFileExists = true, bool checkPathExists = true) : base(null, filter, defaultExt, checkFileExists, checkPathExists)
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
    }
}
