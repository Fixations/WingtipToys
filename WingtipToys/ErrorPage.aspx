<%@ Page Title="Error Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ErrorPage.aspx.cs" Inherits="WingtipToys.ErrorPage" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <h2>Error: </h2>
    <p></p>
    <asp:Label ID="FriendlyErrorMsg" runat="server" Text="Lable" Font-Size="Large" style="color:red;"></asp:Label>
    <br /><br />
    <p>Please try one of the links above.</p>
    <asp:Panel ID="DetailedErrorPanel" runat="server" Visible="false">
        <p>&nbsp;</p>
        <h4>Detailed Error:</h4>
        <p>
            <asp:Label ID="ErrorDetailMsg" runat="server" Font-Size="Small"></asp:Label><br />
        </p>
        <h4>Error Handler:</h4>
        <p>
            <asp:Label ID="ErrorHandler" runat="server" Font-Size="Small"></asp:Label><br />
        </p>
        <h4>Detailed Error Message:</h4>
        <p>
            <asp:Label ID="InnerMessage" runat="server" Font-Size="Small"></asp:Label><br />
        </p>
        <p>
            <asp:Label ID="InnnerTrace" runat="server"></asp:Label>
        </p>
    </asp:Panel>
</asp:Content>
