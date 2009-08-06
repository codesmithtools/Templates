<%@ Page Title="Search" Language="vb" AutoEventWireup="true" MasterPageFile="~/Master.Master" CodeBehind="Search.aspx.vb" Inherits="PetShop.UI.Search" %>
<%@ Register Src="Controls/SearchControl.ascx" TagName="SearchControl" TagPrefix="pc" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphPage" runat="server">
    <pc:SearchControl ID="SearchControl1" runat="server" />
</asp:Content>