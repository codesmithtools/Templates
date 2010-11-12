<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Dashboard>" %>
<%@ Import Namespace="Tracker.MVC.UI.Models"%>
<%@ Import Namespace="System.Xml"%>

<asp:Content ID="indexTitle" ContentPlaceHolderID="TitleContent" runat="server">
    Home Page
</asp:Content>
<asp:Content ID="indexContent" ContentPlaceHolderID="MainContent" runat="server">
    <h1>Dashboard</h1>
    <br />
    
    <div class="dashboard">
        
        <fieldset class="dashboard-totals">
            <legend>Totals</legend>
            <ul>
                <li>Tasks: <%=Model.TotalTasks %></li>
                <li>Created By Me: <%=Model.TotalTasksCreatedByMe %></li>
                <li>Assigned To Me: <%=Model.TotalTasksAssignedToMe %></li>
                <li>Completed: <%=Model.TotalTasksCompleted %></li>
                <li></li>
            </ul>
        </fieldset>
            
          
        <div class="dashboard-box">
            <h2>Tasks Not Started (<%=Model.TasksNotStarted.Count() %>)</h2>    
            <table>
                <tr>
                    <th>Due Date</th>
                    <th>Priority</th>
                    <th>Details</th>
                </tr>
                <%foreach (var item in Model.TasksNotStarted) { %>
                <tr>
                    <td><%=item.DueDate %></td>
                    <td><%=item.Priority %></td>
                    <td><%=item.Details %></td>
                </tr>
            <% } %>
            </table>
        </div>


        <div class="dashboard-box">
            <h2>Tasks In Progress (<%=Model.TasksInProgress.Count() %>)</h2>    
            <table>
                <tr>
                    <th>Due Date</th>
                    <th>Priority</th>
                    <th>Details</th>
                </tr>
                <%foreach (var item in Model.TasksInProgress) { %>
                <tr>
                    <td><%=item.DueDate %></td>
                    <td><%=item.Priority %></td>
                    <td><%=item.Details %></td>
                </tr>
            <% } %>
            </table>
        </div>


        <div class="dashboard-box">
            <h2>Tasks Not Started (<%=Model.TasksNotStarted.Count() %>)</h2>    
            <table>
                <tr>
                    <th>Due Date</th>
                    <th>Priority</th>
                    <th>Details</th>
                </tr>
                <%foreach (var item in Model.TasksNotStarted) { %>
                <tr>
                    <td><%=item.DueDate %></td>
                    <td><%=item.Priority %></td>
                    <td><%=item.Details %></td>
                </tr>
            <% } %>
            </table>
        </div>


        <div class="dashboard-box">
            <h2>Tasks Completed (<%=Model.TasksCompleted.Count() %>)</h2>    
            <table>
                <tr>
                    <th>Due Date</th>
                    <th>Priority</th>
                    <th>Details</th>
                </tr>
                <%foreach (var item in Model.TasksCompleted) { %>
                <tr>
                    <td><%=item.DueDate %></td>
                    <td><%=item.Priority %></td>
                    <td><%=item.Details %></td>
                </tr>
            <% } %>
            </table>
        </div>

    </div>
    
</asp:Content>
