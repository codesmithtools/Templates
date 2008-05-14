<% if(objInfo.IsGeneratedClass) {%>
<%=Indent(i)%>#region Business Properties and Methods

<%=GetMemberDeclarations(objInfo, i)%>

<%= GetPropertyDeclarations(objInfo, i)%> 
<% if(objInfo.HasChild && !objInfo.IsReadOnly) {
	string ptrn = "return base.{0}";
	foreach(PropertyInfo prop in objInfo.ChildCollection) 
	{
		ptrn += " {1} " + prop.MemberName + ".{0}";
	}
%>
<%=Indent(i)%>public override bool IsValid
<%=Indent(i++)%>{
<%=Indent(i)%>get { <%= string.Format(ptrn, "IsValid", "&&")%>; }
<%=Indent(--i)%>}

<%=Indent(i)%>public override bool IsDirty
<%=Indent(i++)%>{
<%=Indent(i)%>get { <%= string.Format(ptrn, "IsDirty", "||")%>; }
<%=Indent(--i)%>}

<%}%>
<%
	//build unique id string value
	string uniqueIdFormat = string.Empty;
	string uniqueIdValues = string.Empty;
	string uniqueId = string.Empty;
	if(objInfo.UniqueProperties.Count==1) 
	{
		uniqueId = ((PropertyInfo)objInfo.UniqueProperties[0]).MemberName + ";";
	}
	else
	{
		for (int uniqueIdIdx=0; uniqueIdIdx < objInfo.UniqueProperties.Count; uniqueIdIdx++)
		{
			uniqueIdFormat += ":{" + uniqueIdIdx + "}";
			uniqueIdValues += ", " + ((PropertyInfo)objInfo.UniqueProperties[uniqueIdIdx]).MemberName;
		}
		if(uniqueIdFormat.Length>0)
			uniqueId = "string.Format(\"" + uniqueIdFormat.Substring(1) + "\"" + uniqueIdValues + ");";
	}
%>
<%=Indent(i)%>protected override object GetIdValue()
<%=Indent(i++)%>{
<%=Indent(i)%>return <%= uniqueId %>
<%=Indent(--i)%>}

<%=Indent(i)%>#endregion //Business Properties and Methods
<%} //if(IsGenerated)%>
