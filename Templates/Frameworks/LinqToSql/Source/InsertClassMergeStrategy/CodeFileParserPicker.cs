using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms.Design;
using System.Windows.Forms;
using System.Drawing.Design;
using System.ComponentModel;
using CodeSmith.Engine;

namespace CodeSmith.Engine
{
    public class CodeFileParserPicker : UITypeEditor
    {
        #region Declarations

        private IWindowsFormsEditorService editorService = null;
        private OpenFileDialog _openFileDialog = null;

        #endregion

        #region Constructor

        public CodeFileParserPicker()
            : base()
        {
        }

        #endregion

        #region UITypeEditor Overrides

        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            if (provider != null)
            {
                editorService = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
                System.Windows.Forms.Control editorControl = editorService as System.Windows.Forms.Control;
                if (editorControl != null)
                {
                    CodeTemplate codeTemplate = context.Instance as CodeTemplate;
                    string fileKey = String.Format("@__{0}_{1}_FileSource", codeTemplate.GetType().Name, context.PropertyDescriptor.Name);

                    if (_openFileDialog == null)
                    {
                        _openFileDialog = new OpenFileDialog();
                        if (codeTemplate != null && codeTemplate.CodeTemplateInfo.DirectoryName.Length > 0)
                        {
                            _openFileDialog.InitialDirectory = codeTemplate.CodeTemplateInfo.DirectoryName;
                        }
                        _openFileDialog.CheckFileExists = true;
                        _openFileDialog.Multiselect = false;

                        _openFileDialog.DefaultExt = ".cs";
                        _openFileDialog.Title = "Select a Code file:";
                        _openFileDialog.Filter = "C# files (*.cs)|*.cs|VB files (*.vb)|*.vb";
                    }

                    DialogResult result = _openFileDialog.ShowDialog(editorControl);
                    if (result == DialogResult.OK)
                    {
                        value = new CodeFileParser(_openFileDialog.FileName, GetParseMethodBodies(context));
                        codeTemplate.ContextData[fileKey] = _openFileDialog.FileName;
                    }
                    else
                    {
                        value = null;
                        codeTemplate.ContextData[fileKey] = String.Empty;
                    }
                }
            }

            // always set the value so that XML is refreshed
            context.PropertyDescriptor.SetValue(context.Instance, value);

            return value;
        }
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }

        #endregion

        #region Methods

        protected bool GetParseMethodBodies(ITypeDescriptorContext context)
        {
            bool result = false;

            CodeTemplate codeTemplate = context.Instance as CodeTemplate;
            string parseMethodBodies = codeTemplate.GetPropertyAttribute(context.PropertyDescriptor.Name, "ParseMethodBodies");

            Boolean.TryParse(parseMethodBodies, out result);

            return result;
        }

        #endregion
    }
}
