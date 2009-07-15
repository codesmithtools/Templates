<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<PLINQO.Tracker.Data.User>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	User Details
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2>Details for <%=Model.FirstName + " " + Model.LastName %></h2>

    <div id="user-edit">
    <fieldset id="edit-form">
        <legend>Fields</legend>
        <p>
            Id:
            <%= Html.Encode(Model.Id) %>
        </p>
        <p>
            Username:
            <%= Html.Encode(Model.EmailAddress) %>
        </p>
        <p>
            First Name:
            <%= Html.Encode(Model.FirstName) %>
        </p>
        <p>
            Last Name:
            <%= Html.Encode(Model.LastName) %>
        </p>
        <p>
            Created Date:
            <%= Html.Encode(String.Format("{0:g}", Model.CreatedDate)) %>
        </p>
        <p>
            Modified Date:
            <%= Html.Encode(String.Format("{0:g}", Model.ModifiedDate)) %>
        </p>
    </fieldset>
    <fieldset id="role-manage">
        <legend>Roles</legend>
        <table>
            <tr>
                <th>Role</th>
            </tr>
            <% foreach (Role role in Model.RoleList) { %>
            <tr>
                <td><%=role.Name %></td>
            </tr>
            <% } %>
        </table>
    </fieldset>
    <fieldset class="avatar-manage">
        <legend>Avatar</legend>
        <p><img src="/user/avatar/<%=Model.Id %>" alt="Avatar" /></p>
    </fieldset>
    </div>
    <p>

        <%=Html.ActionLink("Edit", "Edit", new { id=Model.Id }) %> |
        <%=Html.ActionLink("Back to List", "Index") %>
    </p>

</asp:Content>

