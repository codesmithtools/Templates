<% if(objInfo.IsGeneratedClass) {%>
<%=Indent(i)%>#region Business Properties and Methods
<%-- Register Properties  --%>
<%=Indent(i)%>//register properties
<%  foreach (PropertyInfo prop in objInfo.Properties) { %>        
<% if (prop.IsTimestamp) { %>        
	 
<%=Indent(i)%><%= GetMemberDeclaration(prop) %>
<% } else if (prop.IsSmartType) { %>
<%=Indent(i)%><%=prop.MemberAccess%> static PropertyInfo<<%=prop.Type%>> <%=prop.PropertyInfoName%> = RegisterProperty<<%=prop.Type%>>(typeof(<%=objInfo.Type%>), new PropertyInfo<<%=prop.Type%>>("<%=prop.Name%>", <%=prop.DefaultValue%>));
<% } else { %>
<%=Indent(i)%><%=prop.MemberAccess%> static PropertyInfo<<%=prop.Type%>> <%=prop.PropertyInfoName%> = RegisterProperty<<%=prop.Type%>>(typeof(<%=objInfo.Type%>), new PropertyInfo<<%=prop.Type%>>("<%=prop.Name%>"));
<% } %>
<%  } //foreach %>

<%-- Property Declarations  --%>
<%  foreach (PropertyInfo prop in objInfo.Properties) {
        //skip child collection until later. 
        //exclude timestamp, this field only to support concurrency
        if (objInfo.ChildCollection.Contains(prop) || prop.IsTimestamp) continue; %>
<% if (prop.IsPrimaryKey) { %>        
<%=Indent(i)%>[System.ComponentModel.DataObjectField(<%=prop.IsPrimaryKey.ToString().ToLower()%>, <%=prop.IsIdentity.ToString().ToLower()%>)]
<% } %>
<%=Indent(i)%><%=prop.Modifiers%> <%=prop.Type%> <%=prop.Name%>
<%=Indent(i)%>{
<%=Indent(i+1)%>get { return GetProperty<<%=prop.Type%>>(<%=prop.PropertyInfoName%>); }
<% if (!prop.IsReadOnly && !prop.IsSmartType) {%>
<%=Indent(i+1)%>set { SetProperty<<%=prop.Type%>>(<%=prop.PropertyInfoName%>, value); }
<% } %>
<%=Indent(i)%>}
<% if (prop.IsSmartType) { %>        
	 
<%-- smart type use string as property declaring type --%>
<% if (prop.IsPrimaryKey) { %>        
<%=Indent(i)%>[System.ComponentModel.DataObjectField(<%=prop.IsPrimaryKey.ToString().ToLower()%>, <%=prop.IsIdentity.ToString().ToLower()%>)]
<% } %>
<%=Indent(i)%><%=prop.Modifiers%> string <%=prop.Name%>String
<%=Indent(i)%>{
<%=Indent(i+1)%>get { return GetProperty<<%=prop.Type%>, string>(<%=prop.PropertyInfoName%>); }
<% if (!prop.IsReadOnly) {%>
<%=Indent(i+1)%>set { SetProperty<<%=prop.Type%>, string>(<%=prop.PropertyInfoName%>, value); }
<% } %>
<%=Indent(i)%>}
<% } // if(prop.IsSmartType) %>
	 
<%   }  //foreach %>
<% if(objInfo.HasChild) { %>
<%=Indent(i)%>//child properties
<%  foreach (PropertyInfo prop in objInfo.ChildCollection) { %>
<%=Indent(i)%><%=prop.Modifiers%> <%=prop.Type%> <%=prop.Name%>
<%=Indent(i++)%>{
<%=Indent(i)%>get
<%=Indent(i++)%>{
<%=Indent(i)%>if (!FieldManager.FieldExists(<%=prop.PropertyInfoName%>))
<%=Indent(i+1)%>LoadProperty<<%=prop.Type%>>(<%=prop.PropertyInfoName%>, <%=prop.DefaultValue%>);
<%=Indent(i)%>return GetProperty<<%=prop.Type%>>(<%=prop.PropertyInfoName%>);
<%=Indent(--i)%>}
<%=Indent(--i)%>}

<%   } %>
<% } //objInfo.HasChild%>
<%=Indent(i)%>#endregion //Business Properties and Methods
<%} //if(IsGenerated)%>
