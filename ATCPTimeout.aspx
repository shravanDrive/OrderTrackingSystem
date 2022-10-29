<%@ Page Title="" Language="C#" MasterPageFile="~/ATCP.Master" AutoEventWireup="true"
    CodeFile="ATCPTimeout.aspx.cs" Inherits="ATCPClient.ATCPTimeout" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <meta http-equiv="refresh" content="3;url=RedirectToBluemStem.html" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
            <div style="text-align: center; height: 200px; font-size: large">
        <br />
        <br />
        <br />
        <br />
        <br />
        You have been signed out because of session timeout or you might have logged out.
        <br />
        <br />
        Please try logging in again by clicking on the link given below or wait for 5 seconds
        to get automatically redirected to the login page.
    </div>
    <div style="text-align: center; height: 263px;">
        <br />
        <br />
        <br />
        <br />

            <asp:Button ID="TimeOutButton" runat="server" Text="Log me in!" Height="25px" OnClick="TimeOutButton_Click" />
       
    </div>
</asp:Content>

