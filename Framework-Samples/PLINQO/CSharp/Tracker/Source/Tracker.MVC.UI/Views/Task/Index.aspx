<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<TaskListViewData>" %>
<%@ Import Namespace="PLINQO.Mvc.UI"%>
<%@ Import Namespace="PLINQO.Mvc.UI.Models"%>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Tasks
</asp:Content>

<asp:Content ID="Head1" ContentPlaceHolderID="HeadContent" runat="server">
<script language="javascript" type="text/javascript">

    function selectTasks(isChecked) {
        $('#check-all').attr("checked", isChecked);
        $('input[name=SelectedTasks]').each(function() {
            this.checked = isChecked;
        });
    }

    $(document).ready(function() {
        $('#select-all-tasks').click(function() {
            selectTasks(true);
        });

        $('#unselect-all-tasks').click(function() {
            selectTasks(false);
        });

        $('#check-all').click(function() {
            selectTasks(this.checked);
        });

    });
</script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2>Tasks</h2>

<% using (Html.BeginForm("UpdateStatus", "Task")) { %>
    <div style="padding: 0 0 6px 0">
        <%= Html.ActionLink("Create Task", "Create")%>
    </div>

    <div style="padding: 0 0 6px 0">
        Select: <a id="select-all-tasks" value="false" href="#">All</a> <a id="unselect-all-tasks" value="false" href="#">None</a>
        <%=Html.DropDownList("Status", Model.Statuses, "Select Status")%>
        <input type="submit" value="Save All" />
    </div>
    <table>
        <tr>
            <th><%=Html.CheckBox("check-all")%></th>
            <th></th>
            <th>Id</th>
            <th>Status</th>
            <th>Assigned</th>
            <th>Summary</th>
            <th>Start Date</th>
            <th>Due Date</th>
            <th>Complete Date</th>
            <th>Created By</th>
        </tr>

    <%
       foreach (var item in Model.Tasks)
       {%>
    
        <tr>
            <td><input type="checkbox" name="SelectedTasks" value="<%=item.Id %>" /></td>
            <td class="edit-actions">
                <a class="image-action-link" href="<%= Url.Action("Edit", new {id = item.Id}) %>"><img src="/lib/images/page_white_edit.png" alt="Edit" /></a>
                <a class="image-action-link" href="<%= Url.Action("Details", new {id = item.Id}) %>"><img src="/lib/images/details.gif" alt="Details" /></a>
                <a class="image-action-link" href="<%= Url.Action("Delete", new {id = item.Id}) %>"><img src="/lib/images/delete.gif" alt="Delete" /></a>
            </td>
            <td><%=Html.Encode(item.Id)%></td>
            <td><%=Html.Encode(UIHelper.GetDescription(item.Status))%></td>
            <%
           if (item.AssignedUser != null) {%>
            <td><%=Html.Encode(item.AssignedUser.FirstName + " " + item.AssignedUser.LastName)%></td>
            <% } else {%>
            <td>&nbsp;</td>
            <% }%>
            <td><%=Html.Encode(item.Summary)%></td>
            <td><%=
               Html.Encode(String.Format("{0:g}",
                                         item.StartDate != null ? item.StartDate.Value.ToShortDateString() : "None"))%></td>
            <td><%=
               Html.Encode(String.Format("{0:g}", item.DueDate != null ? item.DueDate.Value.ToShortDateString() : "None"))%></td>
            <td><%=
               Html.Encode(String.Format("{0:g}",
                                         item.CompleteDate != null
                                             ? item.CompleteDate.Value.ToShortDateString()
                                             : "None"))%></td>
            <td><%=Html.Encode(item.CreatedUser.FirstName + " " + item.CreatedUser.LastName)%></td>
        </tr>
    
    <%
       }%>

    </table>
<%}%>


</asp:Content>

