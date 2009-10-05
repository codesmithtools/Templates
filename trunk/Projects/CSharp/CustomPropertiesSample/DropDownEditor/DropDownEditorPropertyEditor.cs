//------------------------------------------------------------------------------
//
// Copyright (c) 2002-2009 CodeSmith Tools, LLC.  All rights reserved.
// 
// The terms of use for this software are contained in the file
// named sourcelicense.txt, which can be found in the root of this distribution.
// By using this software in any fashion, you are agreeing to be bound by the
// terms of this license.
// 
// You must not remove this notice, or any other, from this software.
//
//------------------------------------------------------------------------------

using System;
using System.ComponentModel;
using System.Windows.Forms.Design;
using System.Drawing.Design;

namespace CodeSmith.Samples
{
	public class DropDownEditorPropertyEditor : UITypeEditor
	{
		private IWindowsFormsEditorService editorService;
		private DropDownEditorPropertyEditorControl _dropDownEditorPropertyEditorControl;
		
		public DropDownEditorPropertyEditor(): base()
		{
		}
		
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value) 
		{
			if (provider != null) 
			{
				editorService = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
				if (editorService != null) 
				{
					if (_dropDownEditorPropertyEditorControl == null) _dropDownEditorPropertyEditorControl = new DropDownEditorPropertyEditorControl();
					_dropDownEditorPropertyEditorControl.Start(editorService, value);
					editorService.DropDownControl(_dropDownEditorPropertyEditorControl);
					
					return new DropDownEditorProperty(_dropDownEditorPropertyEditorControl.SampleStringTextBox.Text, _dropDownEditorPropertyEditorControl.SampleBooleanCheckBox.Checked);
				}
			}
			
			return value;
		}
		
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) 
		{
			return UITypeEditorEditStyle.DropDown;
		}
	}
}
