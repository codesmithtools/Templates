<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<IEnumerable<Tracker.Core.Data.User>>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Tracker User List
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2>Tracker User List</h2>

    <p>
        <%= Html.ActionLink("Create New", "Create") %>
    </p>

    <table>
        <tr>
            <th></th>
            <th>Id</th>
            <th>Email Address</th>
            <th>First Name</th>
            <th>Last Name</th>
            <th>Created Date</th>
            <th>Modified Date</th>
        </tr>

    <% foreach (var item in Model) { %>
    
        <tr>
            <td>
                <a class="image-action-link" href="<%= Url.Action("Edit", new {id = item.Id}) %>"><img src="/lib/images/page_white_edit.png" alt="Edit" /></a>
                <a class="image-action-link" href="<%= Url.Action("Details", new {id = item.Id}) %>"><img src="/lib/images/details.gif" alt="Details" /></a>
                <a class="image-action-link" href="<%= Url.Action("Delete", new {id = item.Id}) %>"><img src="/lib/images/delete.gif" alt="Delete" /></a>
            </td>
            <td>
                <%= Html.Encode(item.Id) %>
            </td>
            <td>
                <%= Html.Encode(item.EmailAddress) %>
            </td>
            <td>
                <%= Html.Encode(item.FirstName) %>
            </td>
            <td>
                <%= Html.Encode(item.LastName) %>
            </td>
            <td>
                <%= Html.Encode(String.Format("{0:g}", item.CreatedDate)) %>
            </td>
            <td>
                <%= Html.Encode(String.Format("{0:g}", item.ModifiedDate)) %>
            </td>
        </tr>
    
    <% } %>

    </table>
</asp:Content>

