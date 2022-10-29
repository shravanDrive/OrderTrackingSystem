<%@ Page Language="C#" Title="Create New Referral Agency" AutoEventWireup="true" CodeBehind="NewReferralAgency.aspx.cs" Inherits="ATUClient.NewReferralAgency" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title></title>
    <link href="~/ATUStyleSheet.css" type="text/css" rel="Stylesheet"/>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    <asp:Table ID="NewReferralPageTable" runat="server" HorizontalAlign="left" >
        <asp:TableRow ID="NewReferralPageRow" runat="server">
            <asp:TableCell ID="NewReferralUICLogoCell" runat="server" HorizontalAlign="Left" VerticalAlign="Top" Width="700">
                <img alt = "UIC logo" style="WIDTH: 568px; HEIGHT: 86px" height="86" src="images/Logo.bmp" width="568"/>
            </asp:TableCell>
            <asp:TableCell ID="ATUClientCell" runat="server" HorizontalAlign="Left" VerticalAlign="Top" Width="700">
                <h1><span style="font-size: 24pt">ATU Client Database</span></h1>
            </asp:TableCell>
        </asp:TableRow>
   
        <asp:TableRow ID="NewReferralRow" runat="server">
            <asp:TableCell ID="NewReferralCell" runat="server">
                
                <asp:Table ID="ReferralAgencyFormTable" runat="server">
                    <asp:TableRow ID="NewReferralLabelRow" runat="server" Width="50">
                        <asp:TableCell ID="NewReferralLabelCell" runat="server">
                            <h3>Create New Referral Agency</h3>
                        </asp:TableCell>
                    </asp:TableRow>
        
                    <asp:TableRow ID="AgencyNameRow" runat="server" >
                        <asp:TableCell ID="AgencyLabelCell" runat="server">
                            <asp:Label ID="AgencyNameLabel" runat="server" BorderStyle="None" Text="Agency Name : " ></asp:Label>
                        </asp:TableCell>    
                        <asp:TableCell ID="AgencyTextCell" runat="server">    
                            <asp:TextBox ID="AgencyNameText" Width="150" runat="server" ></asp:TextBox>
                            <asp:Label ID="AgencyErrorLabel" runat="server" BorderStyle="None" Text="Enter Agency Name." Visible="false" ForeColor="Red"></asp:Label>
                            <%--<asp:requiredfieldvalidator id="AgencyNameRequiredFieldValidator"
                                                        controltovalidate="AgencyNameText"
                                                        validationgroup="AgencyInfoGroup"
                                                        errormessage="Enter Agency Name."
                                                        runat="Server" EnableClientScript="false">
                            </asp:requiredfieldvalidator> --%>
                        </asp:TableCell>
                    </asp:TableRow>
                    
                    <asp:TableRow ID="ContactRow" runat="server" >
                        <asp:TableCell ID="ContactCell" runat="server">
                            <asp:Label ID="ContactLabel" runat="server" BorderStyle="None" Text="Contact Name : "></asp:Label>
                        </asp:TableCell>    
                        <asp:TableCell ID="ContactTextCell" runat="server">    
                            <asp:TextBox ID="ContactTextBox" Width="150" runat="server" ></asp:TextBox>
                        </asp:TableCell>
                    </asp:TableRow>
                    
                    <asp:TableRow ID="AddressRow" runat="server" >
                        <asp:TableCell ID="AddressCell" runat="server">
                            <asp:Label ID="AddressLabel" runat="server" BorderStyle="None" Text="Address : "></asp:Label>
                        </asp:TableCell>    
                        <asp:TableCell ID="AddressTextCell" runat="server">    
                            <asp:TextBox ID="AddressTextBox" Width="150" runat="server" ></asp:TextBox>
                        </asp:TableCell>
                    </asp:TableRow>
                    
                    <asp:TableRow ID="CityRow" runat="server" >
                        <asp:TableCell ID="CityCell" runat="server">
                            <asp:Label ID="CityLabel" runat="server" BorderStyle="None" Text="City : "></asp:Label>
                        </asp:TableCell>    
                        <asp:TableCell ID="CityTextCell" runat="server">    
                            <asp:TextBox ID="CityTextBox" Width="150" runat="server" ></asp:TextBox>
                        </asp:TableCell>
                    </asp:TableRow>
                    
                    <asp:TableRow ID="StateRow" runat="server" >
                        <asp:TableCell ID="StateCell" runat="server">
                            <asp:Label ID="StateLabel" runat="server" BorderStyle="None" Text="State : "></asp:Label>
                        </asp:TableCell>    
                        <asp:TableCell ID="StateTextCell" runat="server">
                            <asp:DropDownList ID="StateDropDownList" Width="156" runat="server">
                            </asp:DropDownList>    
                        </asp:TableCell>
                    </asp:TableRow>
                   
                    <asp:TableRow ID="ZipRow" runat="server" >
                        <asp:TableCell ID="ZipCell" runat="server">
                            <asp:Label ID="ZipLabel" runat="server" BorderStyle="None" Text="Zip : "></asp:Label>
                        </asp:TableCell>    
                        <asp:TableCell ID="ZipTextCell" runat="server">    
                            <asp:TextBox ID="ZipTextBox" Width="150" runat="server" ></asp:TextBox>
                        </asp:TableCell>
                    </asp:TableRow> 
                
                    <asp:TableRow ID="PhoneRow" runat="server" >
                        <asp:TableCell ID="PhoneCell" runat="server">
                            <asp:Label ID="PhoneLabel" runat="server" BorderStyle="None" Text="Phone : "></asp:Label>
                        </asp:TableCell>    
                        <asp:TableCell ID="PhoneTextCell" runat="server">    
                            <asp:TextBox ID="PhoneTextBox" Width="150" runat="server" ></asp:TextBox>
                        </asp:TableCell>
                    </asp:TableRow>
                    
                    <asp:TableRow ID="FaxRow" runat="server" >
                        <asp:TableCell ID="FaxCell" runat="server">
                            <asp:Label ID="FaxLabel" runat="server" BorderStyle="None" Text="Fax : "></asp:Label>
                        </asp:TableCell>    
                        <asp:TableCell ID="FaxTextCell" runat="server">    
                            <asp:TextBox ID="FaxTextBox" Width="150" runat="server" ></asp:TextBox>
                        </asp:TableCell>
                    </asp:TableRow>
                    
                    <asp:TableRow ID="EmailRow" runat="server">
                        <asp:TableCell ID="EmailCell" runat="server">
                            <asp:Label ID="EmailLabel" runat="server" BorderStyle="None" Text="Email : "></asp:Label>
                        </asp:TableCell>    
                        <asp:TableCell ID="EmailTextCell" runat="server">    
                            <asp:TextBox ID="EmailTextBox" Width="150" runat="server" ></asp:TextBox>
                        </asp:TableCell>
                    </asp:TableRow>
                    
                    <asp:TableRow ID="TTYRow" runat="server" >
                        <asp:TableCell ID="TTYCell" runat="server">
                            <asp:Label ID="TTYLabel" runat="server" BorderStyle="None" Text="TTY : "></asp:Label>
                        </asp:TableCell>    
                        <asp:TableCell ID="TTYTextCell" runat="server">    
                            <asp:TextBox ID="TTYTextBox" Width="150" runat="server" ></asp:TextBox>
                        </asp:TableCell>
                    </asp:TableRow>
                    
                    <asp:TableRow ID="SaveButtonRow" runat="server" >
                        <asp:TableCell ID="SaveButtonCell" runat="server" HorizontalAlign="Center" VerticalAlign="Middle">
                            <asp:Button ID="SaveButton" runat="server" Text="Save" OnClick="Save_Click" ValidationGroup="AgencyInfoGroup" CausesValidation="true"/>
                        </asp:TableCell>
                    </asp:TableRow>
                    
                
                
                </asp:Table>
                
            </asp:TableCell>
        </asp:TableRow>
     </asp:Table>
    </div>
    </form>
</body>
</html>
