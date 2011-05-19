using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using Xml.Schema.Linq;

namespace LinqToEdmx.Model.Storage
{
  /// <summary>
  /// <para>
  /// Regular expression: (Documentation?, CommandText?, Parameter*, any)
  /// </para>
  /// </summary>
  public class Function : XTypedElement, IXMetaData
  {
    private static readonly bool IsComposableDefaultValue = XmlConvert.ToBoolean("true");

    private const string ParameterTypeSemanticsDefaultValue = "AllowImplicitConversion";

    private static readonly Dictionary<XName, Type> LocalElementDictionary = new Dictionary<XName, Type>();

    private static FSM _validationStates;

    private XTypedList<Parameter> _parameterField;

    static Function()
    {
      BuildElementDictionary();
      InitFsm();
    }

    /// <summary>
    /// <para>
    /// Occurrence: optional
    /// </para>
    /// <para>
    /// Regular expression: (Documentation?, CommandText?, Parameter*, any)
    /// </para>
    /// </summary>
    public Documentation Documentation
    {
      get
      {
        var x = GetElement(XName.Get("Documentation", "http://schemas.microsoft.com/ado/2009/02/edm/ssdl"));
        return ((Documentation) (x));
      }
      set
      {
        SetElement(XName.Get("Documentation", "http://schemas.microsoft.com/ado/2009/02/edm/ssdl"), value);
      }
    }

    /// <summary>
    /// <para>
    /// Occurrence: optional
    /// </para>
    /// <para>
    /// Regular expression: (Documentation?, CommandText?, Parameter*, any)
    /// </para>
    /// </summary>
    public string CommandText
    {
      get
      {
        var x = GetElement(XName.Get("CommandText", "http://schemas.microsoft.com/ado/2009/02/edm/ssdl"));
        return XTypedServices.ParseValue<string>(x, XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.String).Datatype);
      }
      set
      {
        SetElementWithValidation(XName.Get("CommandText", "http://schemas.microsoft.com/ado/2009/02/edm/ssdl"), value, "CommandText", Storage.CommandText.TypeDefinition);
      }
    }

    /// <summary>
    /// <para>
    /// Occurrence: optional, repeating
    /// </para>
    /// <para>
    /// Regular expression: (Documentation?, CommandText?, Parameter*, any)
    /// </para>
    /// </summary>
    public IList<Parameter> Parameters
    {
      get
      {
        if ((_parameterField == null))
        {
          _parameterField = new XTypedList<Parameter>(this, LinqToXsdTypeManager.Instance, XName.Get("Parameter", "http://schemas.microsoft.com/ado/2009/02/edm/ssdl"));
        }
        return _parameterField;
      }
      set
      {
        if ((value == null))
        {
          _parameterField = null;
        }
        else
        {
          if ((_parameterField == null))
          {
            _parameterField = XTypedList<Parameter>.Initialize(this, LinqToXsdTypeManager.Instance, value, XName.Get("Parameter", "http://schemas.microsoft.com/ado/2009/02/edm/ssdl"));
          }
          else
          {
            XTypedServices.SetList(_parameterField, value);
          }
        }
      }
    }

    /// <summary>
    /// <para>
    /// Regular expression: (Documentation?, CommandText?, Parameter*, any)
    /// </para>
    /// </summary>
    public IEnumerable<XElement> Any
    {
      get
      {
        return GetWildCards(WildCard.DefaultWildCard);
      }
    }

    /// <summary>
    /// <para>
    /// Occurrence: required
    /// </para>
    /// </summary>
    public string Name
    {
      get
      {
        var x = Attribute(XName.Get("Name", ""));
        return XTypedServices.ParseValue<string>(x, XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.String).Datatype);
      }
      set
      {
        SetAttribute(XName.Get("Name", ""), value, XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.String).Datatype);
      }
    }

    /// <summary>
    /// <para>
    /// Occurrence: optional
    /// </para>
    /// </summary>
    public object ReturnType
    {
      get
      {
        var x = Attribute(XName.Get("ReturnType", ""));
        return XTypedServices.ParseValue<object>(x, XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.String).Datatype);
      }
      set
      {
        SetAttribute(XName.Get("ReturnType", ""), value, XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.String).Datatype);
      }
    }

    /// <summary>
    /// <para>
    /// Occurrence: optional
    /// </para>
    /// </summary>
    public bool? Aggregate
    {
      get
      {
        var x = Attribute(XName.Get("Aggregate", ""));
        if ((x == null))
        {
          return null;
        }
        return XTypedServices.ParseValue<bool>(x, XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.Boolean).Datatype);
      }
      set
      {
        SetAttribute(XName.Get("Aggregate", ""), value, XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.Boolean).Datatype);
      }
    }

    /// <summary>
    /// <para>
    /// Occurrence: optional
    /// </para>
    /// </summary>
    public bool? BuiltIn
    {
      get
      {
        var x = Attribute(XName.Get("BuiltIn", ""));
        if ((x == null))
        {
          return null;
        }
        return XTypedServices.ParseValue<bool>(x, XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.Boolean).Datatype);
      }
      set
      {
        SetAttribute(XName.Get("BuiltIn", ""), value, XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.Boolean).Datatype);
      }
    }

    /// <summary>
    /// <para>
    /// Occurrence: optional
    /// </para>
    /// </summary>
    public string StoreFunctionName
    {
      get
      {
        var x = Attribute(XName.Get("StoreFunctionName", ""));
        return XTypedServices.ParseValue<string>(x, XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.String).Datatype);
      }
      set
      {
        SetAttribute(XName.Get("StoreFunctionName", ""), value, XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.String).Datatype);
      }
    }

    /// <summary>
    /// <para>
    /// Occurrence: optional
    /// </para>
    /// </summary>
    public bool? NiladicFunction
    {
      get
      {
        var x = Attribute(XName.Get("NiladicFunction", ""));
        if ((x == null))
        {
          return null;
        }
        return XTypedServices.ParseValue<bool>(x, XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.Boolean).Datatype);
      }
      set
      {
        SetAttribute(XName.Get("NiladicFunction", ""), value, XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.Boolean).Datatype);
      }
    }

    /// <summary>
    /// <para>
    /// Occurrence: optional
    /// </para>
    /// </summary>
    public bool IsComposable
    {
      get
      {
        var x = Attribute(XName.Get("IsComposable", ""));
        return XTypedServices.ParseValue(x, XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.Boolean).Datatype, IsComposableDefaultValue);
      }
      set
      {
        SetAttribute(XName.Get("IsComposable", ""), value, XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.Boolean).Datatype);
      }
    }

    /// <summary>
    /// <para>
    /// Occurrence: optional
    /// </para>
    /// </summary>
    public string ParameterTypeSemantics
    {
      get
      {
        var x = Attribute(XName.Get("ParameterTypeSemantics", ""));
        return XTypedServices.ParseValue(x, XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.Token).Datatype, ParameterTypeSemanticsDefaultValue);
      }
      set
      {
        SetAttribute(XName.Get("ParameterTypeSemantics", ""), value, XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.Token).Datatype);
      }
    }

    /// <summary>
    /// <para>
    /// Occurrence: optional
    /// </para>
    /// </summary>
    public string Schema
    {
      get
      {
        var x = Attribute(XName.Get("Schema", ""));
        return XTypedServices.ParseValue<string>(x, XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.String).Datatype);
      }
      set
      {
        SetAttribute(XName.Get("Schema", ""), value, XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.String).Datatype);
      }
    }

    /// <summary>
    /// <para>
    /// Occurrence: optional
    /// </para>
    /// </summary>
    public string Schema1
    {
      get
      {
        var x = Attribute(XName.Get("Schema", "http://schemas.microsoft.com/ado/2007/12/edm/EntityStoreSchemaGenerator"));
        return XTypedServices.ParseValue<string>(x, XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.String).Datatype);
      }
      set
      {
        SetAttribute(XName.Get("Schema", "http://schemas.microsoft.com/ado/2007/12/edm/EntityStoreSchemaGenerator"), value, XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.String).Datatype);
      }
    }

    /// <summary>
    /// <para>
    /// Occurrence: optional
    /// </para>
    /// </summary>
    public string Name1
    {
      get
      {
        var x = Attribute(XName.Get("Name", "http://schemas.microsoft.com/ado/2007/12/edm/EntityStoreSchemaGenerator"));
        return XTypedServices.ParseValue<string>(x, XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.String).Datatype);
      }
      set
      {
        SetAttribute(XName.Get("Name", "http://schemas.microsoft.com/ado/2007/12/edm/EntityStoreSchemaGenerator"), value, XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.String).Datatype);
      }
    }

    #region IXMetaData Members

    Dictionary<XName, Type> IXMetaData.LocalElementsDictionary
    {
      get
      {
        return LocalElementDictionary;
      }
    }

    XName IXMetaData.SchemaName
    {
      get
      {
        return XName.Get("TFunction", "http://schemas.microsoft.com/ado/2009/02/edm/ssdl");
      }
    }

    SchemaOrigin IXMetaData.TypeOrigin
    {
      get
      {
        return SchemaOrigin.Fragment;
      }
    }

    ILinqToXsdTypeManager IXMetaData.TypeManager
    {
      get
      {
        return LinqToXsdTypeManager.Instance;
      }
    }

    FSM IXMetaData.GetValidationStates()
    {
      return _validationStates;
    }

    #endregion

    public static explicit operator Function(XElement xe)
    {
      return XTypedServices.ToXTypedElement<Function>(xe, LinqToXsdTypeManager.Instance);
    }

    public override XTypedElement Clone()
    {
      return XTypedServices.CloneXTypedElement(this);
    }

    private static void BuildElementDictionary()
    {
      LocalElementDictionary.Add(XName.Get("Documentation", "http://schemas.microsoft.com/ado/2009/02/edm/ssdl"), typeof (Documentation));
      LocalElementDictionary.Add(XName.Get("CommandText", "http://schemas.microsoft.com/ado/2009/02/edm/ssdl"), typeof (string));
      LocalElementDictionary.Add(XName.Get("Parameter", "http://schemas.microsoft.com/ado/2009/02/edm/ssdl"), typeof (Parameter));
    }

    private static void InitFsm()
    {
      var transitions = new Dictionary<int, Transitions>();
      transitions.Add(1, new Transitions(new SingleTransition(XName.Get("Documentation", "http://schemas.microsoft.com/ado/2009/02/edm/ssdl"), 2), new SingleTransition(XName.Get("CommandText", "http://schemas.microsoft.com/ado/2009/02/edm/ssdl"), 4), new SingleTransition(XName.Get("Parameter", "http://schemas.microsoft.com/ado/2009/02/edm/ssdl"), 5), new SingleTransition(new WildCard("##other", "http://schemas.microsoft.com/ado/2009/02/edm/ssdl"), 7)));
      transitions.Add(2, new Transitions(new SingleTransition(XName.Get("CommandText", "http://schemas.microsoft.com/ado/2009/02/edm/ssdl"), 4), new SingleTransition(XName.Get("Parameter", "http://schemas.microsoft.com/ado/2009/02/edm/ssdl"), 5), new SingleTransition(new WildCard("##other", "http://schemas.microsoft.com/ado/2009/02/edm/ssdl"), 7)));
      transitions.Add(4, new Transitions(new SingleTransition(XName.Get("Parameter", "http://schemas.microsoft.com/ado/2009/02/edm/ssdl"), 4), new SingleTransition(new WildCard("##other", "http://schemas.microsoft.com/ado/2009/02/edm/ssdl"), 7)));
      transitions.Add(7, new Transitions(new SingleTransition(new WildCard("##other", "http://schemas.microsoft.com/ado/2009/02/edm/ssdl"), 7)));
      transitions.Add(5, new Transitions(new SingleTransition(XName.Get("Parameter", "http://schemas.microsoft.com/ado/2009/02/edm/ssdl"), 5), new SingleTransition(new WildCard("##other", "http://schemas.microsoft.com/ado/2009/02/edm/ssdl"), 7)));
      _validationStates = new FSM(1, new Set<int>(new[]
                                                   {
                                                     2, 1, 4, 5, 7
                                                   }), transitions);
    }
  }
}