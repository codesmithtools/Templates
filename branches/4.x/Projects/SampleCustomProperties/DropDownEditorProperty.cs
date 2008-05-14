//------------------------------------------------------------------------------
//
// Copyright (c) 2002-2006 CodeSmith Tools, LLC.  All rights reserved.
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
	[PropertySerializer(typeof(CodeSmith.Samples.DropDownEditorPropertySerializer))]
	[Editor(typeof(CodeSmith.Samples.DropDownEditorPropertyEditor), typeof(System.Drawing.Design.UITypeEditor))]
	public class DropDownEditorProperty
	{
		private string _sampleString;
		private bool _sampleBoolean;
		
		public DropDownEditorProperty()
		{
		}

		public DropDownEditorProperty(string sampleString, bool sampleBoolean)
		{
			_sampleString = sampleString;
			_sampleBoolean = sampleBoolean;
		}
		
		public string SampleString
		{
			get {return _sampleString;}
			set {_sampleString = value;}
		}
		
		public bool SampleBoolean
		{
			get {return _sampleBoolean;}
			set {_sampleBoolean = value;}
		}
		
		/// <summary>
		/// The value that we return here will be shown in the property grid.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return SampleString + ": " + SampleBoolean;
		}
	}
}
