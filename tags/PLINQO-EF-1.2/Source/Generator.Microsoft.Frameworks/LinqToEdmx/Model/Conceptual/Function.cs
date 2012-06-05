using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Xml.Schema;
using LinqToEdmx.Model.Storage;
using Xml.Schema.Linq;

namespace LinqToEdmx.Model.Conceptual
{
  /// <summary>
  /// <para>
  /// Regular expression: (Documentation?, (Parameter* | DefiningExpression? | ReturnType? | any)*)
  /// </para>
  /// </summary>
  public class Function : XTypedElement, IXMetaData
  {
    private static readonly Dictionary<XName, Type> LocalElementDictionary = new Dictionary<XName, Type>();

    private static FSM _validationStates;

    private XSimpleList<string> _definingExpressionField;

    private XTypedList<FunctionParameter> _parameterField;

    private XTypedList<FunctionReturnType> _returnTypeField;

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
    /// Regular expression: (Documentation?, (Parameter* | DefiningExpression? | ReturnType? | any)*)
    /// </para>
    /// </summary>
    public Documentation Documentation
    {
      get
      {
        var x = GetElement(XName.Get("Documentation", "http://schemas.microsoft.com/ado/2008/09/edm"));
        return ((Documentation) (x));
      }
      set
      {
        SetElement(XName.Get("Documentation", "http://schemas.microsoft.com/ado/2008/09/edm"), value);
      }
    }

    /// <summary>
    /// <para>
    /// Occurrence: optional, repeating, choice
    /// </para>
    /// <para>
    /// Setter: Appends
    /// </para>
    /// <para>
    /// Regular expression: (Documentation?, (Parameter* | DefiningExpression? | ReturnType? | any)*)
    /// </para>
    /// </summary>
    public IList<FunctionParameter> Parameters
    {
      get
      {
        if ((_parameterField == null))
        {
          _parameterField = new XTypedList<FunctionParameter>(this, LinqToXsdTypeManager.Instance, XName.Get("Parameter", "http://schemas.microsoft.com/ado/2008/09/edm"));
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
            _parameterField = XTypedList<FunctionParameter>.Initialize(this, LinqToXsdTypeManager.Instance, value, XName.Get("Parameter", "http://schemas.microsoft.com/ado/2008/09/edm"));
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
    /// Occurrence: optional, choice
    /// </para>
    /// <para>
    /// Setter: Appends
    /// </para>
    /// <para>
    /// Regular expression: (Documentation?, (Parameter* | DefiningExpression? | ReturnType? | any)*)
    /// </para>
    /// </summary>
    public IList<string> DefiningExpression
    {
      get
      {
        if ((_definingExpressionField == null))
        {
          _definingExpressionField = new XSimpleList<string>(this, XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.String).Datatype, XName.Get("DefiningExpression", "http://schemas.microsoft.com/ado/2008/09/edm"));
        }
        return _definingExpressionField;
      }
      set
      {
        if ((value == null))
        {
          _definingExpressionField = null;
        }
        else
        {
          if ((_definingExpressionField == null))
          {
            _definingExpressionField = XSimpleList<string>.Initialize(this, XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.String).Datatype, value, XName.Get("DefiningExpression", "http://schemas.microsoft.com/ado/2008/09/edm"));
          }
          else
          {
            XTypedServices.SetList(_definingExpressionField, value);
          }
        }
      }
    }

    /// <summary>
    /// <para>
    /// Occurrence: optional, choice
    /// </para>
    /// <para>
    /// Setter: Appends
    /// </para>
    /// <para>
    /// Regular expression: (Documentation?, (Parameter* | DefiningExpression? | ReturnType? | any)*)
    /// </para>
    /// </summary>
    public IList<FunctionReturnType> ReturnTypes
    {
      get
      {
        if ((_returnTypeField == null))
        {
          _returnTypeField = new XTypedList<FunctionReturnType>(this, LinqToXsdTypeManager.Instance, XName.Get("ReturnType", "http://schemas.microsoft.com/ado/2008/09/edm"));
        }
        return _returnTypeField;
      }
      set
      {
        if ((value == null))
        {
          _returnTypeField = null;
        }
        else
        {
          if ((_returnTypeField == null))
          {
            _returnTypeField = XTypedList<FunctionReturnType>.Initialize(this, LinqToXsdTypeManager.Instance, value, XName.Get("ReturnType", "http://schemas.microsoft.com/ado/2008/09/edm"));
          }
          else
          {
            XTypedServices.SetList(_returnTypeField, value);
          }
        }
      }
    }

    /// <summary>
    /// <para>
    /// Regular expression: (Documentation?, (Parameter* | DefiningExpression? | ReturnType? | any)*)
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
    public object ReturnType1
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
    public bool? Nullable
    {
      get
      {
        var x = Attribute(XName.Get("Nullable", ""));
        if ((x == null))
        {
          return null;
        }
        return XTypedServices.ParseValue<bool>(x, XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.Boolean).Datatype);
      }
      set
      {
        SetAttribute(XName.Get("Nullable", ""), value, XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.Boolean).Datatype);
      }
    }

    /// <summary>
    /// <para>
    /// Occurrence: optional
    /// </para>
    /// </summary>
    public string DefaultValue
    {
      get
      {
        var x = Attribute(XName.Get("DefaultValue", ""));
        return XTypedServices.ParseValue<string>(x, XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.String).Datatype);
      }
      set
      {
        SetAttribute(XName.Get("DefaultValue", ""), value, XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.String).Datatype);
      }
    }

    /// <summary>
    /// <para>
    /// Occurrence: optional
    /// </para>
    /// </summary>
    public object MaxLength
    {
      get
      {
        var x = Attribute(XName.Get("MaxLength", ""));
        return XTypedServices.ParseValue<object>(x, XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.String).Datatype);
      }
      set
      {
        SetAttribute(XName.Get("MaxLength", ""), value, XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.String).Datatype);
      }
    }

    /// <summary>
    /// <para>
    /// Occurrence: optional
    /// </para>
    /// </summary>
    public bool? FixedLength
    {
      get
      {
        var x = Attribute(XName.Get("FixedLength", ""));
        if ((x == null))
        {
          return null;
        }
        return XTypedServices.ParseValue<bool>(x, XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.Boolean).Datatype);
      }
      set
      {
        SetAttribute(XName.Get("FixedLength", ""), value, XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.Boolean).Datatype);
      }
    }

    /// <summary>
    /// <para>
    /// Occurrence: optional
    /// </para>
    /// </summary>
    public decimal? Precision
    {
      get
      {
        var x = Attribute(XName.Get("Precision", ""));
        if ((x == null))
        {
          return null;
        }
        return XTypedServices.ParseValue<decimal>(x, XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.NonNegativeInteger).Datatype);
      }
      set
      {
        SetAttribute(XName.Get("Precision", ""), value, XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.NonNegativeInteger).Datatype);
      }
    }

    /// <summary>
    /// <para>
    /// Occurrence: optional
    /// </para>
    /// </summary>
    public decimal? Scale
    {
      get
      {
        var x = Attribute(XName.Get("Scale", ""));
        if ((x == null))
        {
          return null;
        }
        return XTypedServices.ParseValue<decimal>(x, XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.NonNegativeInteger).Datatype);
      }
      set
      {
        SetAttribute(XName.Get("Scale", ""), value, XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.NonNegativeInteger).Datatype);
      }
    }

    /// <summary>
    /// <para>
    /// Occurrence: optional
    /// </para>
    /// </summary>
    public bool? Unicode
    {
      get
      {
        var x = Attribute(XName.Get("Unicode", ""));
        if ((x == null))
        {
          return null;
        }
        return XTypedServices.ParseValue<bool>(x, XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.Boolean).Datatype);
      }
      set
      {
        SetAttribute(XName.Get("Unicode", ""), value, XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.Boolean).Datatype);
      }
    }

    /// <summary>
    /// <para>
    /// Occurrence: optional
    /// </para>
    /// </summary>
    public string Collation
    {
      get
      {
        var x = Attribute(XName.Get("Collation", ""));
        return XTypedServices.ParseValue<string>(x, XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.String).Datatype);
      }
      set
      {
        SetAttribute(XName.Get("Collation", ""), value, XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.String).Datatype);
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
        return XName.Get("TFunction", "http://schemas.microsoft.com/ado/2008/09/edm");
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
      LocalElementDictionary.Add(XName.Get("Documentation", "http://schemas.microsoft.com/ado/2008/09/edm"), typeof (Documentation));
      LocalElementDictionary.Add(XName.Get("Parameter", "http://schemas.microsoft.com/ado/2008/09/edm"), typeof (FunctionParameter));
      LocalElementDictionary.Add(XName.Get("DefiningExpression", "http://schemas.microsoft.com/ado/2008/09/edm"), typeof (string));
      LocalElementDictionary.Add(XName.Get("ReturnType", "http://schemas.microsoft.com/ado/2008/09/edm"), typeof (FunctionReturnType));
    }

    private static void InitFsm()
    {
      var transitions = new Dictionary<int, Transitions>();
      transitions.Add(1, new Transitions(new SingleTransition(XName.Get("Documentation", "http://schemas.microsoft.com/ado/2008/09/edm"), 2), new SingleTransition(XName.Get("Parameter", "http://schemas.microsoft.com/ado/2008/09/edm"), 3), new SingleTransition(XName.Get("DefiningExpression", "http://schemas.microsoft.com/ado/2008/09/edm"), 3), new SingleTransition(XName.Get("ReturnType", "http://schemas.microsoft.com/ado/2008/09/edm"), 3), new SingleTransition(new WildCard("##other", "http://schemas.microsoft.com/ado/2008/09/edm"), 9)));
      transitions.Add(2, new Transitions(new SingleTransition(XName.Get("Parameter", "http://schemas.microsoft.com/ado/2008/09/edm"), 2), new SingleTransition(XName.Get("DefiningExpression", "http://schemas.microsoft.com/ado/2008/09/edm"), 2), new SingleTransition(XName.Get("ReturnType", "http://schemas.microsoft.com/ado/2008/09/edm"), 2), new SingleTransition(new WildCard("##other", "http://schemas.microsoft.com/ado/2008/09/edm"), 9)));
      transitions.Add(9, new Transitions(new SingleTransition(XName.Get("Parameter", "http://schemas.microsoft.com/ado/2008/09/edm"), 3), new SingleTransition(XName.Get("DefiningExpression", "http://schemas.microsoft.com/ado/2008/09/edm"), 3), new SingleTransition(XName.Get("ReturnType", "http://schemas.microsoft.com/ado/2008/09/edm"), 3), new SingleTransition(new WildCard("##other", "http://schemas.microsoft.com/ado/2008/09/edm"), 3)));
      transitions.Add(3, new Transitions(new SingleTransition(XName.Get("Parameter", "http://schemas.microsoft.com/ado/2008/09/edm"), 3), new SingleTransition(XName.Get("DefiningExpression", "http://schemas.microsoft.com/ado/2008/09/edm"), 3), new SingleTransition(XName.Get("ReturnType", "http://schemas.microsoft.com/ado/2008/09/edm"), 3), new SingleTransition(new WildCard("##other", "http://schemas.microsoft.com/ado/2008/09/edm"), 9)));
      _validationStates = new FSM(1, new Set<int>(new[]
                                                   {
                                                     2, 1, 3, 9
                                                   }), transitions);
    }
  }
}