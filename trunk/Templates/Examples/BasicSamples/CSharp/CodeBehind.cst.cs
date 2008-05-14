using System.ComponentModel;
using CodeSmith.Engine;

public class SampleCodeBehindClass : CodeTemplate
{
	private bool _propertyFromCodeBehind = false;
	
	[Category("Options")]
	[Description("This property is inherited from the code behind file.")]
	public bool PropertyFromCodeBehind
	{ 
		get {return _propertyFromCodeBehind;}
		set {_propertyFromCodeBehind = value;}
	}
	
	public string GetSomething()
	{
		return "Something";
	}
}