<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<UserViewData>" %>
<%@ Import Namespace="PLINQO.Mvc.UI.Controllers"%>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Edit
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2>Editing User:</h2>

    <div>
        <%=Html.ActionLink("Back to List", "Index") %>
    </div>

    <%= Html.ValidationSummary("Edit was unsuccessful. Please correct the errors and try again.") %>

    <div id="user-edit">

    <% using (Html.BeginForm()) {%>

        <fieldset id="edit-form">
            <legend>Fields</legend>
            
            <p>
                <input type="submit" value="Save" />
            </p>

            <p>
                User Id:
                <%= Html.Encode(Model.User.Id) %>
            </p>
            <p>
                <label for="EmailAddress">Email Address</label>
                <%= Html.TextBox("EmailAddress", Model.User.EmailAddress) %>
                <%= Html.ValidationMessage("EmailAddress", "*") %>
            </p>
            <p>
                <label for="FirstName">First Name</label>
                <%= Html.TextBox("FirstName", Model.User.FirstName) %>
                <%= Html.ValidationMessage("FirstName", "*") %>
            </p>
            <p>
                <label for="LastName">Last Name</label>
                <%= Html.TextBox("LastName", Model.User.LastName) %>
                <%= Html.ValidationMessage("LastName", "*") %>
            </p>
            <p>
                <label for="Comment">User Comment</label>
                <%= Html.TextArea("Comment", Model.User.Comment)%>
            </p>
            <p>
                Created:
                <%= Html.Encode(String.Format("{0:g}", Model.User.CreatedDate)) %>
            </p>
            <p>
                Modified:
                <%= Html.Encode(String.Format("{0:g}", Model.User.ModifiedDate)) %>
            </p>
        </fieldset>
    <% } %>
        
        <fieldset id="role-manage">
            <legend>Roles</legend>
            <div id="role-manager-head">
            <% using (Html.BeginForm("AddRole", "User", new {userId = Model.User.Id})) {%>
                <%= Html.DropDownList("RoleId", Model.Roles)%>
                <input type="submit" value="Save" />
            <%} %>
            </div>
            
            <table>
                <tr>
                    <th></th>
                    <th>Role</th>
                </tr>
                <% foreach (Role role in Model.User.RoleList) { %>
                <tr>
                    <td><%=Html.ActionLink("Remove", "RemoveRole", new {UserId = Model.User.Id, RoleId = role.Id}) %></td>
                    <td><%=role.Name %></td>
                </tr>
                <% } %>
            </table>
        </fieldset>

    </div>

    
    <%if( Model.Audits.Count > 0) {%>
    <div class="audit-notes">
    <% foreach (Audit audit in Model.Audits) {%>
    <%if(!String.IsNullOrEmpty(audit.HtmlContent)) {%>
    <div class="audit-note">
    <%=audit.HtmlContent %>
    <% } %>
    <% } %>
    </div>
    <% } %>
    </div>

</asp:Content>

