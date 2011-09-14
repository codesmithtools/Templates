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
using CodeSmith.Engine;
using System.ComponentModel;

namespace CodeSmith.Samples
{
	[PropertySerializer(typeof(ModalEditorPropertySerializer))]
	[Editor(typeof(ModalEditorPropertyEditor), typeof(System.Drawing.Design.UITypeEditor))]
	public class ModalEditorProperty
	{
	    public ModalEditorProperty()
		{
		}

		public ModalEditorProperty(string sampleString, bool sampleBoolean)
		{
			SampleString = sampleString;
			SampleBoolean = sampleBoolean;
		}

	    public string SampleString { get; set; }

	    public bool SampleBoolean { get; set; }

	    /// <summary>
		/// The value that we return here will be shown in the property grid.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return string.Format("{0}: {1}", SampleString, SampleBoolean);
		}
	}
}
