<% if(objInfo.IsGeneratedClass) {%>
<%-- Property Declarations  --%>
<%  foreach (PropertyInfo prop in objInfo.Properties) {
        //skip child collection until later. 
        //exclude timestamp, this field only to support concurrency
        if (objInfo.ChildCollection.Contains(prop) || prop.IsTimestamp) continue; %>
<%=Indent(i)%><%=prop.Modifiers%> <%=prop.NativeType%> <%=prop.Name%> { get; private set; }
<%   }  //foreach %>
<% if(objInfo.HasChild) { %>

<%=Indent(i)%>//child properties
<%  foreach (PropertyInfo prop in objInfo.ChildCollection) { %>
<%=Indent(i)%><%=prop.Modifiers%> <%=prop.Type%> <%=prop.Name%> { get; private set; }
<%   } %>
<% } //objInfo.HasChild%>
<%} //if(IsGeneratedClass)%>
