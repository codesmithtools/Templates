//------------------------------------------------------------------------------
//
// Copyright (c) 2002-2011 CodeSmith Tools, LLC.  All rights reserved.
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
using System.Windows.Forms.Design;

namespace CodeSmith.Samples
{
	/// <summary>
	/// Summary description for DropDownEditorPropertyEditorControl.
	/// </summary>
	public class DropDownEditorPropertyEditorControl : System.Windows.Forms.UserControl
	{
		public System.Windows.Forms.TextBox SampleStringTextBox;
		public System.Windows.Forms.CheckBox SampleBooleanCheckBox;
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public DropDownEditorPropertyEditorControl()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			// TODO: Add any initialization after the InitializeComponent call

		}
		
		public void Start(IWindowsFormsEditorService editorService, object value)
		{
			if (value is DropDownEditorProperty)
			{
				SampleStringTextBox.Text = ((DropDownEditorProperty)value).SampleString;
				SampleBooleanCheckBox.Checked = ((DropDownEditorProperty)value).SampleBoolean;
			}
		}
		
		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.SampleStringTextBox = new System.Windows.Forms.TextBox();
			this.SampleBooleanCheckBox = new System.Windows.Forms.CheckBox();
			this.SuspendLayout();
			// 
			// SampleStringTextBox
			// 
			this.SampleStringTextBox.Location = new System.Drawing.Point(8, 8);
			this.SampleStringTextBox.Name = "SampleStringTextBox";
			this.SampleStringTextBox.TabIndex = 0;
			this.SampleStringTextBox.Text = "Hello World!";
			// 
			// SampleBooleanCheckBox
			// 
			this.SampleBooleanCheckBox.Location = new System.Drawing.Point(8, 32);
			this.SampleBooleanCheckBox.Name = "SampleBooleanCheckBox";
			this.SampleBooleanCheckBox.Size = new System.Drawing.Size(112, 24);
			this.SampleBooleanCheckBox.TabIndex = 1;
			this.SampleBooleanCheckBox.Text = "Sample Boolean";
			// 
			// DropDownEditorPropertyEditorControl
			// 
			this.Controls.Add(this.SampleBooleanCheckBox);
			this.Controls.Add(this.SampleStringTextBox);
			this.Name = "DropDownEditorPropertyEditorControl";
			this.Size = new System.Drawing.Size(112, 56);
			this.ResumeLayout(false);

		}
		#endregion
	}
}
