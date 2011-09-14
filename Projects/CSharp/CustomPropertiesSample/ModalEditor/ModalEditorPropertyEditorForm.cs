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
	/// Summary description for ModalEditorPropertyEditorForm.
	/// </summary>
	public class ModalEditorPropertyEditorForm : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Button button1;
		public System.Windows.Forms.CheckBox SampleBooleanCheckBox;
		public System.Windows.Forms.TextBox SampleStringTextBox;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components;

		public ModalEditorPropertyEditorForm()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
		}
		
		public void Start(IWindowsFormsEditorService editorService, object value)
		{
			if (value is ModalEditorProperty)
			{
				this.SampleStringTextBox.Text = ((ModalEditorProperty)value).SampleString;
				this.SampleBooleanCheckBox.Checked = ((ModalEditorProperty)value).SampleBoolean;
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

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.button1 = new System.Windows.Forms.Button();
			this.SampleBooleanCheckBox = new System.Windows.Forms.CheckBox();
			this.SampleStringTextBox = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			// 
			// button1
			// 
			this.button1.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.button1.Location = new System.Drawing.Point(32, 64);
			this.button1.Name = "button1";
			this.button1.TabIndex = 5;
			this.button1.Text = "OK";
			// 
			// SampleBooleanCheckBox
			// 
			this.SampleBooleanCheckBox.Location = new System.Drawing.Point(8, 32);
			this.SampleBooleanCheckBox.Name = "SampleBooleanCheckBox";
			this.SampleBooleanCheckBox.Size = new System.Drawing.Size(112, 24);
			this.SampleBooleanCheckBox.TabIndex = 4;
			this.SampleBooleanCheckBox.Text = "Sample Boolean";
			// 
			// SampleStringTextBox
			// 
			this.SampleStringTextBox.Location = new System.Drawing.Point(8, 8);
			this.SampleStringTextBox.Name = "SampleStringTextBox";
			this.SampleStringTextBox.TabIndex = 3;
			this.SampleStringTextBox.Text = "Hello World!";
			// 
			// ModalEditorPropertyEditorForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(115, 94);
			this.ControlBox = false;
			this.Controls.Add(this.button1);
			this.Controls.Add(this.SampleBooleanCheckBox);
			this.Controls.Add(this.SampleStringTextBox);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ModalEditorPropertyEditorForm";
			this.ShowInTaskbar = false;
			this.Text = "Modal Editor";
			this.ResumeLayout(false);

		}
		#endregion
	}
}
