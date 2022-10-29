<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="ATUClient.Login" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:TextBox ID="TextBox1" runat="server"></asp:TextBox>
        <asp:Button ID="Button1"
            runat="server" Text="login" onclick="Button1_Click" /></div>
    <DIV style="Z-INDEX: 103; LEFT: 8px; POSITION: absolute; TOP: 48px" ms_positioning="text2D">
				<H1>This is the Prod Login Page, for which Bluestem Authentication is not needed.</H1>
    </div>
    </form>
</body>
</html>
