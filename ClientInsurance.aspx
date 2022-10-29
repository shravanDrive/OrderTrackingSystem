<%@ Page Language="C#" Title="ClientInsurance" AutoEventWireup="true" MasterPageFile="~/Site.Master"
    CodeBehind="ClientInsurance.aspx.cs" Inherits="ATUClient.ClientInsurance" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <asp:Table ID="ReferralTable" runat="server" HorizontalAlign="left">
        <asp:TableRow ID="ReferralRow" runat="server">
            <asp:TableCell ID="ReferralCell" runat="server" HorizontalAlign="Left" VerticalAlign="Top"
                Width="600" Height="250">
                <asp:Table ID="ReferralInfoTable" runat="server">
                    <%--Search Criteria--%>
                    <asp:TableRow ID="ClientSearchLabelRow" runat="server">
                        <asp:TableCell ID="ClientSearchLabelCell" runat="server">
                            <asp:Label ID="ClientSearchLabel" runat="server" Font-Bold="true" Text="Client Search"></asp:Label>
                        </asp:TableCell>
                    </asp:TableRow>
                    <asp:TableRow ID="SearchRow" runat="server">
                        <asp:TableCell ID="SearchCellFN" runat="server">
                            <asp:Label ID="LastNameLabel" runat="server" BorderStyle="None" Text="Last Name"></asp:Label><br />
                            <asp:TextBox ID="LastNameText" Width="150" runat="server"></asp:TextBox><br />
                            <br />
                        </asp:TableCell>
                        <asp:TableCell ID="SearchCellLN" runat="server">
                            <asp:Label ID="FirstNameLabel" runat="server" BorderStyle="None" Text="First Name"></asp:Label><br />
                            <asp:TextBox ID="FirstNameText" Width="150" runat="server"></asp:TextBox>
                            <asp:Button ID="SearchButton" runat="server" Text="Search" OnClick="Search_Click" />
                            <asp:Button ID="NewSearchButton" runat="server" Text="New Search" OnClick="NewSearch_Click" /><br />
                            <br />
                        </asp:TableCell>
                    </asp:TableRow>
                    <%--Insurance Type--%>
                    <asp:TableRow ID="InsuranceTypeRow" runat="server">
                        <asp:TableCell ID="InsuranceTypeCell" runat="server">
                            <asp:Label ID="InsuranceTypeLabel" runat="server" BorderStyle="None" Text="Insurance Type :"
                                Font-Bold="true"></asp:Label><br />
                        </asp:TableCell>
                        <asp:TableCell ID="InsuranceTypeValueCell" runat="server">
                            <asp:DropDownList ID="InsuranceTypeDropDownList" Width="150" runat="server" AutoPostBack="true"
                                OnSelectedIndexChanged="InsuranceType_Changed">
                            </asp:DropDownList>
                        </asp:TableCell>
                    </asp:TableRow>
                    <%--Carrier--%>
                    <asp:TableRow ID="CarrierRow" runat="server" Visible="false">
                        <asp:TableCell ID="CarrierCell" runat="server">
                            <asp:Label ID="CarrierLabel" runat="server" BorderStyle="None" Text="Carrier :" Font-Bold="true"></asp:Label><br />
                        </asp:TableCell>
                        <asp:TableCell ID="CarrierValueCell" runat="server">
                            <asp:DropDownList ID="CarrierDropDownList" Width="150" runat="server" OnSelectedIndexChanged="Carrier_Changed"
                                AutoPostBack="true">
                            </asp:DropDownList>
                            <asp:CheckBox ID="CarrierCheckBox" Text="Create New Carrier" runat="server" AutoPostBack="true"
                                OnCheckedChanged="Carrier_Checked" Enabled="False" Visible="False" />
                        </asp:TableCell>
                    </asp:TableRow>
                    <%--Plan--%>
                    <asp:TableRow ID="PlanRow" runat="server" Visible="false">
                        <asp:TableCell ID="PlanCell" runat="server">
                            <asp:Label ID="PlanLabel" runat="server" BorderStyle="None" Text="Plan :" Font-Bold="true"></asp:Label><br />
                            <br />
                            <br />
                        </asp:TableCell>
                        <asp:TableCell ID="PlanValueCell" runat="server">
                            <asp:DropDownList ID="PlanDropDownList" Width="150" runat="server" OnSelectedIndexChanged="Plan_Changed"
                                AutoPostBack="true">
                            </asp:DropDownList>
                            <br />
                            <br />
                            <br />
                        </asp:TableCell>
                    </asp:TableRow>
                    <%--Group#--%>
                    <asp:TableRow ID="GroupRow" runat="server" Visible="false">
                        <asp:TableCell ID="GroupCell" runat="server">
                            <asp:Label ID="GroupLabel" runat="server" BorderStyle="None" Text="Group # :"></asp:Label><br />
                        </asp:TableCell>
                        <asp:TableCell ID="GroupValueCell" runat="server">
                            <asp:TextBox ID="GroupTextBox" Width="150" runat="server"></asp:TextBox>
                            <asp:Label ID="ClaimLabel" runat="server" Text="-" Visible="false"></asp:Label>
                            <asp:TextBox ID="MedicareClaimTextBox" Width="20" runat="server" Visible="false"></asp:TextBox>
                        </asp:TableCell>
                    </asp:TableRow>
                    <%--InsuranceId--%>
                    <asp:TableRow ID="InsuranceIdRow" runat="server" Visible="false">
                        <asp:TableCell ID="InsuranceIdCell" runat="server">
                            <asp:Label ID="InsuranceIdLabel" runat="server" BorderStyle="None" Text="Insurance ID :"></asp:Label><br />
                        </asp:TableCell>
                        <asp:TableCell ID="InsuranceIdValueCell" runat="server">
                            <asp:TextBox ID="InsuranceIdTextBox" Width="150" runat="server"></asp:TextBox>
                        </asp:TableCell>
                    </asp:TableRow>
                </asp:Table>
            </asp:TableCell>
            <asp:TableCell ID="SearchGridCell" runat="server" VerticalAlign="Top" Height="250"
                Width="400">
                <asp:Table ID="GridsTable" runat="server" HorizontalAlign="Left">
                    <%--Search Grid--%>
                    <asp:TableRow ID="CurrentClientRow" runat="server" VerticalAlign="Top" HorizontalAlign="Left">
                        <asp:TableCell ID="CurrentClientCell" runat="server">
                            <asp:Label ID="CurrentClientLabel" runat="server" BorderStyle="None" Text="Current Client: "
                                Font-Bold="true" ForeColor="Red"></asp:Label>
                            <asp:Label ID="CurrentClientValueLabel" runat="server" BorderStyle="None" Text=""></asp:Label><br />
                            <br />
                        </asp:TableCell>
                    </asp:TableRow>
                    <asp:TableRow ID="GridsRow" runat="server" VerticalAlign="Top" HorizontalAlign="Left">
                        <asp:TableCell ID="GridsCell" runat="server">
                            <asp:GridView ID="GridView1" runat="server" Width="400" AutoGenerateSelectButton="true"
                                AllowPaging="true" OnPageIndexChanging="grdSearch_Changing" OnRowCommand="gridSelect_Command"
                                PageSize="20">
                            </asp:GridView>
                            <asp:GridView ID="GridView2" runat="server" Width="400" Visible="false">
                            </asp:GridView>
                            <asp:Label ID="NotFoundLabel" runat="server" ForeColor="Red" Font-Bold="true" BorderStyle="None"
                                Text="No search results found" Visible="false"></asp:Label><br />
                        </asp:TableCell>
                    </asp:TableRow>
                    <asp:TableRow ID="AddressInfoRow" runat="server">
                        <asp:TableCell ID="AddressInfoCell" runat="server">
                            <asp:GridView ID="AddressInfoGridView" ShowHeader="false" GridLines="Horizontal"
                                CellSpacing="10" CellPadding="5" runat="server" OnRowDataBound="GridView_RowDataBound"
                                Width="300px">
                            </asp:GridView>
                        </asp:TableCell>
                        <asp:TableCell ID="AddressInfoCell1" runat="server" VerticalAlign="Top" HorizontalAlign="Left">
                            <asp:Label ID="ContactInfoLabel" runat="server" ForeColor="Black" Font-Bold="true"
                                BorderStyle="None" Text="Contact Information" Visible="false" Width="250px"></asp:Label><br />
                            <asp:GridView ID="AddressInfoGridView1" ShowHeader="false" GridLines="Horizontal"
                                CellSpacing="10" CellPadding="5" runat="server" Width="250px">
                            </asp:GridView>
                        </asp:TableCell>
                    </asp:TableRow>
                </asp:Table>
            </asp:TableCell>
        </asp:TableRow>
        <%--New Carrier--%>
        <asp:TableRow ID="NewICarrierRow" runat="server" Visible="false">
            <asp:TableCell ID="NewICarrierCell" runat="server">
                <asp:Label ID="NewICarrierLabel" runat="server" Font-Bold="true" Font-Size="Large"
                    Text="New Insurance Carrier"></asp:Label>
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow ID="NewICarrierTextRow" runat="server" Visible="false">
            <asp:TableCell ID="NewICarrierTextCell" runat="server">
                <asp:TextBox ID="NewICarrierTextBox" runat="server"></asp:TextBox>
                <asp:Button ID="NewICarrierButton" runat="server" Text="Save Carrier" OnClick="Save_Carrier_Click" />
                <asp:Label ID="NewICarrierSaveLabel" runat="server" Text="Saved!!!" Visible="false"
                    ForeColor="Green"></asp:Label>
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow ID="MessageLabelRow" runat="server" Visible="false">
            <asp:TableCell ID="MessageLabelCell" runat="server">
                <asp:Label ID="MessageLabel" runat="server" Font-Size="Large" ForeColor="Red" Text="Contact information for the selected Carrier and Plan is missing. Please enter."></asp:Label>
            </asp:TableCell>
        </asp:TableRow>
        <%--New Insurance Fields --%>
        <asp:TableRow ID="NewOuterRow" runat="server">
            <asp:TableCell ID="NewOuterCell" runat="server">
                <asp:Table ID="NewInsuranceTable" runat="server" Visible="false" BorderWidth="1">
                    <%--New Insurance Label --%>
                    <asp:TableRow ID="NewInsuranceRow" runat="server">
                        <asp:TableCell ID="NewInsuranceCell" runat="server">
                            <asp:Label ID="NewInsuranceLabel" runat="server" Font-Bold="true" Font-Size="Large"
                                Text="New Payer Claim Contact"></asp:Label>
                        </asp:TableCell>
                    </asp:TableRow>
                    <%--Carrier --%>
                    <asp:TableRow ID="ContactNameRow" runat="server">
                        <asp:TableCell ID="ContactNameCell" runat="server">
                            <asp:Label ID="ContactNameLabel" runat="server" Width="160" BorderStyle="None" Text="Contact Name :"></asp:Label>
                        </asp:TableCell>
                        <asp:TableCell ID="ContactNameValueCell" runat="server">
                            <asp:TextBox ID="ContactNameTextBox" Width="150" runat="server"></asp:TextBox>
                            <asp:Label ID="ErrorLabel" runat="server" Width="160" BorderStyle="None" Text="Enter contact name."
                                Visible="false" ForeColor="Red"></asp:Label>
                        </asp:TableCell>
                    </asp:TableRow>
                    <%--Plan --%>
                    <asp:TableRow ID="NewPlanRow" runat="server" Visible="false">
                        <asp:TableCell ID="NewPlanCell" runat="server">
                            <asp:Label ID="NewPlanLabel" runat="server" Width="160" BorderStyle="None" Text="Plan :"></asp:Label>
                        </asp:TableCell>
                        <asp:TableCell ID="NewPlanValueCell" runat="server">
                            <asp:TextBox ID="NewPlanTextBox" Width="150" runat="server" ReadOnly="true"></asp:TextBox>
                        </asp:TableCell>
                    </asp:TableRow>
                    <%--Address --%>
                    <asp:TableRow ID="AddressRow" runat="server">
                        <asp:TableCell ID="AddressCell" runat="server">
                            <asp:Label ID="AddressLabel" runat="server" Width="160" BorderStyle="None" Text="Address :"></asp:Label>
                        </asp:TableCell>
                        <asp:TableCell ID="AddressValueCell" runat="server">
                            <asp:TextBox ID="AddressTextBox" Width="150" runat="server"></asp:TextBox>
                        </asp:TableCell>
                    </asp:TableRow>
                    <%--City --%>
                    <asp:TableRow ID="CityRow" runat="server">
                        <asp:TableCell ID="CityCell" runat="server">
                            <asp:Label ID="CityLabel" runat="server" Width="160" BorderStyle="None" Text="City :"></asp:Label>
                        </asp:TableCell>
                        <asp:TableCell ID="CityValueCell" runat="server">
                            <asp:TextBox ID="CityTextBox" Width="150" runat="server"></asp:TextBox>
                        </asp:TableCell>
                    </asp:TableRow>
                    <%--State --%>
                    <asp:TableRow ID="StateRow" runat="server">
                        <asp:TableCell ID="StateCell" runat="server">
                            <asp:Label ID="StateLabel" runat="server" Width="160" BorderStyle="None" Text="State :"></asp:Label>
                        </asp:TableCell>
                        <asp:TableCell ID="StateValueCell" runat="server">
                            <asp:DropDownList ID="StateDropDownList" Width="150" runat="server">
                            </asp:DropDownList>
                        </asp:TableCell>
                    </asp:TableRow>
                    <%--Zip --%>
                    <asp:TableRow ID="ZipRow" runat="server">
                        <asp:TableCell ID="ZipCell" runat="server">
                            <asp:Label ID="ZipLabel" runat="server" Width="160" BorderStyle="None" Text="Zip :"></asp:Label>
                        </asp:TableCell>
                        <asp:TableCell ID="ZipValueCell" runat="server">
                            <asp:TextBox ID="ZipTextBox" Width="150" runat="server"></asp:TextBox>
                        </asp:TableCell>
                    </asp:TableRow>
                    <%--Phone --%>
                    <asp:TableRow ID="PhoneRow" runat="server">
                        <asp:TableCell ID="PhoneCell" runat="server">
                            <asp:Label ID="PhoneLabel" runat="server" Width="160" BorderStyle="None" Text="Phone :"></asp:Label>
                        </asp:TableCell>
                        <asp:TableCell ID="PhoneValueCell" runat="server">
                            <asp:TextBox ID="PhoneTextBox" Width="150" runat="server"></asp:TextBox>
                        </asp:TableCell>
                    </asp:TableRow>
                    <%--Fax --%>
                    <asp:TableRow ID="FaxRow" runat="server">
                        <asp:TableCell ID="FaxCell" runat="server">
                            <asp:Label ID="FaxLabel" runat="server" Width="160" BorderStyle="None" Text="Fax :"></asp:Label>
                        </asp:TableCell>
                        <asp:TableCell ID="FaxValueCell" runat="server">
                            <asp:TextBox ID="FaxTextBox" Width="150" runat="server"></asp:TextBox>
                        </asp:TableCell>
                    </asp:TableRow>
                    <%--Save Button --%>
                    <asp:TableRow ID="SaveInsuranceRow" runat="server">
                        <asp:TableCell ID="SaveInsuranceCell" HorizontalAlign="Center" VerticalAlign="Middle"
                            runat="server" ColumnSpan="2">
                            <asp:Button ID="SaveInsuranceButton" runat="server" Text="Save Insurance" OnClick="Insurance_Save_Click" />
                        </asp:TableCell>
                    </asp:TableRow>
                </asp:Table>
            </asp:TableCell>
        </asp:TableRow>
        <%--Buttons--%>
        <asp:TableRow ID="SaveRow" runat="server">
            <asp:TableCell ID="SaveCell" runat="server" HorizontalAlign="Center" VerticalAlign="Middle"
                ColumnSpan="2">
                <br />
                <br />
                <asp:Button ID="BackButton" runat="server" Text="Back" OnClick="Back_Click" />
                <asp:Button ID="SaveButton" runat="server" Text="Save" OnClick="Save_Click" />
                <asp:Button ID="ContinueButton" runat="server" Text="Continue" OnClick="Continue_Click" />
            </asp:TableCell>
        </asp:TableRow>
    </asp:Table>
</asp:Content>
