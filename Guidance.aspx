<%@ Page Title="Guidance" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="True"
    CodeBehind="Guidance.aspx.cs" Inherits="ATUClient.Guidance" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <style type="text/css">
        .style1
        {
            width: 100%;
        }
        .Control
        {
            height: 13px;
            width: 285px;
        }
        .style4
        {
            height: 23px;
        }
        .style5
        {
            height: 22px;
            text-align: center;
        }
        #content
        {
            padding: 15px 30px 0 0;
        }
        #main-content
        {
            margin-left: 51%;
            max-width: 50%;
        }
        #sidebar
        {
            width: 50%;
            float: left;
            padding: 0 0 0 5px;
        }
        #sidebar ul
        {
            list-style: inside;
        }
        .style7
        {
            height: 22px;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <table>
        <tr>
            <td>
                <div id="sidebar">
                    <table class="style1">
                        <tr>
                            <td class="style4" style="text-align: left; width: 30%;">
                                <b>Search</b>
                            </td>
                            <td class="style4" style="text-align: left; width: 30%;">
                                &nbsp;
                            </td>
                            <td class="style4" style="text-align: leftwidth: 40%">
                                &nbsp;
                            </td>
                        </tr>
                        <tr>
                            <td style="width: 30%">
                                Last Name:
                            </td>
                            <td style="width: 30%">
                                First Name:
                            </td>
                            <td style="width: 40%">
                                &nbsp;
                            </td>
                        </tr>
                        <tr>
                            <td style="width: 30%">
                                <asp:TextBox ID="txtbx_LN" runat="server" Width="96%"></asp:TextBox>
                            </td>
                            <td style="width: 30%">
                                <asp:TextBox ID="txtbx_FN" runat="server" Width="96%"></asp:TextBox>
                            </td>
                            <td style="width: 40%">
                                <asp:Button ID="SearchButton" runat="server" Text="Search" OnClick="Search_Click"
                                    Width="49%" />
                                <asp:Button ID="NewSearchButton" runat="server" Text="New Search" OnClick="NewSearch_Click"
                                    Width="49%" /><br />
                            </td>
                        </tr>
                        <tr>
                            <td style="width: 30%">
                                &nbsp;
                            </td>
                            <td style="width: 30%">
                                &nbsp;
                            </td>
                            <td style="width: 40%">
                                &nbsp;
                            </td>
                        </tr>
                        <tr>
                            <td style="text-align: left; vertical-align: middle; width: 30%;">
                                <asp:Label ID="lbl_ReferralDate" runat="server" BorderStyle="None" Font-Bold="True"
                                    Text="Referral Date :"></asp:Label><br />
                            </td>
                            <td style="width: 30%">
                                <asp:DropDownList ID="ddlst_ReferralDate" Width="150" runat="server" OnSelectedIndexChanged="ReferralDate_Changed"
                                    AutoPostBack="true">
                                </asp:DropDownList>
                            </td>
                            <td style="width: 40%">
                            </td>
                        </tr>
                        <tr>
                            <td colspan="3" style="text-align: left;">
                                &nbsp;<asp:ScriptManager ID="ScriptManager1" runat="server">
                                </asp:ScriptManager>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="3" width="50%">
                                <asp:UpdatePanel ID="updatepanel_Dynamic" runat="server">
                                    <ContentTemplate>
                                        <asp:UpdatePanel ID="updatepanel_Section1" runat="server">
                                            <ContentTemplate>
                                                <table style="border: thin solid #000000; background-color: #F5DEB3; font-size: medium;"
                                                    width="100%">
                                                    <tr>
                                                        <td class="style7">
                                                            <asp:Label ID="lbl_MobilityAidHeader" runat="server" Text="Mobility Aid" Style="font-weight: 700">
                                                            </asp:Label>
                                                        </td>
                                                        <td align="right">
                                                            <asp:Label ID="lbl_SectionNumber1" runat="server" Text="Section 1 of 3" Style="font-weight: 700;
                                                                font-style: italic; color: Gray; font-size: x-small; vertical-align: middle">
                                                            </asp:Label>
                                                        </td>
                                                    </tr>
                                                </table>
                                                <asp:GridView ID="GrdView_MobilityAid" runat="server" AlternatingRowStyle-BackColor="LightGray"
                                                    AutoGenerateColumns="false" EnablePersistedSelection="true" ShowHeader="false"
                                                    Width="100%">
                                                    <RowStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                    <Columns>
                                                        <asp:TemplateField ItemStyle-Width="30%">
                                                            <ItemTemplate>
                                                                <asp:Label ID="lbl_MobAid" runat="server" Font-Size="Small" Text='<%# Eval("Description") %>' />
                                                            </ItemTemplate>
                                                            <ItemStyle Width="30%" />
                                                        </asp:TemplateField>
                                                        <asp:TemplateField>
                                                            <ItemTemplate>
                                                                <asp:TextBox ID="txtbx_InputMobAidVals" runat="server" AutoPostBack="true" MaxLength="1"
                                                                    Height="18px" Width="10%" Font-Size="Small" />
                                                                    <%--TabIndex='<%# TabIndex %>'/>--%>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                    </Columns>
                                                    <HeaderStyle BackColor="Wheat" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                    <AlternatingRowStyle BackColor="LightGray" />
                                                </asp:GridView>
                                            </ContentTemplate>
                                        </asp:UpdatePanel>
                                        <asp:UpdatePanel ID="updatepanel_Section2" runat="server">
                                            <ContentTemplate>
                                                <table style="border: thin solid #000000; background-color: #F5DEB3; font-size: medium;"
                                                    width="100%">
                                                    <tr>
                                                        <td class="style7">
                                                            <asp:Label ID="lbl_ADLAreaHeader" runat="server" Text="ADL Area" Style="font-weight: 700">
                                                            </asp:Label>
                                                        </td>
                                                        <td align="right">
                                                            <asp:Label ID="lbl_SectionNumber2" runat="server" Text="Section 2 of 3" Style="font-weight: 700;
                                                                font-style: italic; color: Gray; font-size: x-small; vertical-align: middle">
                                                            </asp:Label>
                                                        </td>
                                                    </tr>
                                                </table>
                                                <asp:CheckBoxList ID="chkbxlst_ADLArea" runat="server" Font-Size="X-Small" BorderStyle="Solid"
                                                    BorderWidth="1px" RepeatDirection="Horizontal" OnSelectedIndexChanged="chkbxlst_ADLArea_SelectedIndexChanged"
                                                    AutoPostBack="True" RepeatColumns="4" Width="100%">
                                                </asp:CheckBoxList>
                                                <asp:GridView ID="GrdView" runat="server" EnablePersistedSelection="true" OnRowDataBound="GrdView_RowDataBound"
                                                    HeaderStyle-BackColor="Wheat" AlternatingRowStyle-BackColor="LightGray" AutoGenerateColumns="false"
                                                    Width="100%">
                                                    <RowStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                    <Columns>
                                                        <asp:TemplateField HeaderText="ADL" ItemStyle-Width="30%">
                                                            <ItemTemplate>
                                                                <asp:Label ID="lblADL" runat="server" Font-Size="Small" Text='<%# Eval("Description") %>' />
                                                            </ItemTemplate>
                                                            <ItemStyle Width="30%" />
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="Degree Ability">
                                                            <ItemTemplate>
                                                                <asp:CheckBoxList ID="chkbxlstADL" runat="server" onclick="javascript:deselectAllRest (this.checked, this.id);"
                                                                    Font-Size="X-Small" AutoPostBack="true" RepeatDirection="Horizontal" OnSelectedIndexChanged="chkbxlst_SelectedIndexChanged"
                                                                    TabIndex='<%# TabIndex %>' />
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                    </Columns>
                                                    <HeaderStyle BackColor="Wheat" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                    <AlternatingRowStyle BackColor="LightGray" />
                                                </asp:GridView>
                                            </ContentTemplate>
                                        </asp:UpdatePanel>
                                        <asp:UpdatePanel ID="updatepanel_Section3" runat="server">
                                            <ContentTemplate>
                                                <table style="border: thin solid #000000; background-color: #F5DEB3; font-size: medium;"
                                                    width="100%">
                                                    <tr>
                                                        <td class="style7">
                                                            <asp:Label ID="lbl_DisabilitiesHeader" runat="server" Text="Disabilities" Style="font-weight: 700">
                                                            </asp:Label>
                                                        </td>
                                                        <td align="right">
                                                            <asp:Label ID="lbl_SectionNumber3" runat="server" Text="Section 3 of 3" Style="font-weight: 700;
                                                                font-style: italic; color: Gray; font-size: x-small; vertical-align: middle">
                                                            </asp:Label>
                                                        </td>
                                                    </tr>
                                                </table>
                                                <asp:CheckBoxList ID="chkbxlst_DisabilitiesArea" runat="server" Font-Size="X-Small"
                                                    BorderStyle="Solid" BorderWidth="1px" RepeatDirection="Horizontal" OnSelectedIndexChanged="chkbxlst_DisabilitiesArea_SelectedIndexChanged"
                                                    AutoPostBack="True" RepeatColumns="4" Width="100%">
                                                </asp:CheckBoxList>
                                                <asp:GridView ID="GrdView_Disabilities" runat="server" AlternatingRowStyle-BackColor="LightGray"
                                                    AutoGenerateColumns="false" EnablePersistedSelection="true" ShowHeader="false"
                                                    Width="100%">
                                                    <RowStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                    <Columns>
                                                        <asp:TemplateField ItemStyle-Width="30%">
                                                            <ItemTemplate>
                                                                <asp:Label ID="lbl_Disabilities" runat="server" Font-Size="Small" Text='<%# Eval("Description") %>' />
                                                            </ItemTemplate>
                                                            <ItemStyle Width="30%" />
                                                        </asp:TemplateField>
                                                        <asp:TemplateField>
                                                            <ItemTemplate>
                                                                <asp:RadioButtonList ID="radiobtnlst_Disabilities" runat="server" RepeatDirection="Horizontal"
                                                                    AutoPostBack="true" OnSelectedIndexChanged="radiobtnlst_Disabilities_SelectedIndexChanged"
                                                                    Font-Size="Small" TabIndex='<%# TabIndex %>'>
                                                                    <asp:ListItem Value="Yes" Text="Yes" />
                                                                    <asp:ListItem Value="No" Text="No" />
                                                                </asp:RadioButtonList>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                    </Columns>
                                                    <HeaderStyle BackColor="Wheat" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                    <AlternatingRowStyle BackColor="LightGray" />
                                                </asp:GridView>
                                            </ContentTemplate>
                                        </asp:UpdatePanel>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="3" width="50%" class="style5">
                                <asp:Button ID="btn_PreviousSection" runat="server" Text="&lt;&lt; Previous" OnClick="btn_PreviousSection_Click" />
                                &nbsp;<asp:Button ID="btn_NextSection" runat="server" Text="Next &gt;&gt;" OnClick="btn_NextSection_Click" />
                            </td>
                        </tr>
                    </table>
                </div>
                <div id="main-content">
                    <table width="100%">
                        <tr>
                            <td>
                                <asp:Label ID="lbl_CurrentClient" runat="server" Font-Bold="True" ForeColor="Red"
                                    Text="Current Client: "></asp:Label><asp:Label ID="lbl_CurrentClientName" runat="server"
                                        Font-Bold="False" ForeColor="Black">No user selected</asp:Label>
                            </td>
                            <td>
                                &nbsp;
                            </td>
                        </tr>
                        <tr>
                            <td>
                                &nbsp;
                            </td>
                            <td>
                                &nbsp;
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2">
                                <%--<asp:Panel ID="StatusPanel" runat="server" ScrollBars="Both" Height="300">--%>
                                <asp:GridView ID="GridView1" runat="server" AutoGenerateSelectButton="true" AllowPaging="true"
                                    HeaderStyle-BackColor="Wheat" OnPageIndexChanging="grdSearch_Changing" OnRowCommand="gridSelect_Command"
                                    PageSize="20">
                                </asp:GridView>
                                <asp:UpdatePanel ID="updatepanel_DynamicViews" runat="server">
                                    <ContentTemplate>
                                        <asp:UpdatePanel ID="updatepanel_SavedData" runat="server">
                                            <ContentTemplate>
                                                <table width="100%">
                                                    <tr>
                                                        <td align="center" width="100%">
                                                            <asp:Label ID="lbl_SavedDataHeader" runat="server" Text="Saved Information" Font-Bold="true" />
                                                        </td>
                                                    </tr>
                                                </table>
                                                <asp:GridView ID="GrdView_MobAidView" runat="server" EnablePersistedSelection="true"
                                                    AlternatingRowStyle-BackColor="LightGray" AutoGenerateColumns="false" Width="100%">
                                                    <RowStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                    <Columns>
                                                        <asp:TemplateField HeaderText="Mobility Aid" ItemStyle-Width="30%">
                                                            <ItemTemplate>
                                                                <asp:Label ID="lbl_MobAidView" runat="server" Font-Size="Small" Text='<%# Eval("MobilityAid") %>' />
                                                            </ItemTemplate>
                                                            <ItemStyle Width="30%" />
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="Quantity">
                                                            <ItemTemplate>
                                                                <asp:Label ID="lbl_MobAidQtyView" runat="server" Font-Size="Small" Text='<%# Eval("Quantity") %>' />
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                    </Columns>
                                                    <HeaderStyle HorizontalAlign="Left" VerticalAlign="Middle" BackColor="#D6FCFE" />
                                                    <AlternatingRowStyle BackColor="LightGray" />
                                                </asp:GridView>
                                                <asp:GridView ID="GrdView_ADLView" runat="server" EnablePersistedSelection="true"
                                                    AlternatingRowStyle-BackColor="LightGray" AutoGenerateColumns="false" Width="100%">
                                                    <RowStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                    <Columns>
                                                        <asp:TemplateField HeaderText="ADL Area" ItemStyle-Width="30%">
                                                            <ItemTemplate>
                                                                <asp:Label ID="lbl_ADLAreaView" runat="server" Font-Size="Small" Text='<%# Eval("ADLArea") %>' />
                                                            </ItemTemplate>
                                                            <ItemStyle Width="30%" />
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="ADL">
                                                            <ItemTemplate>
                                                                <asp:Label ID="lbl_ADLView" runat="server" Font-Size="Small" Text='<%# Eval("ADL") %>' />
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="ADL Ability">
                                                            <ItemTemplate>
                                                                <asp:Label ID="lbl_ADLAbilityView" runat="server" Font-Size="Small" Text='<%# Eval("ADLAbility") %>' />
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                    </Columns>
                                                    <HeaderStyle HorizontalAlign="Left" VerticalAlign="Middle" BackColor="#D6FCFE" />
                                                    <AlternatingRowStyle BackColor="LightGray" />
                                                </asp:GridView>
                                                <asp:GridView ID="GrdView_DisabilityView" runat="server" EnablePersistedSelection="true"
                                                    AlternatingRowStyle-BackColor="LightGray" AutoGenerateColumns="false" Width="100%">
                                                    <RowStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                    <Columns>
                                                        <asp:TemplateField HeaderText="Disability Area" ItemStyle-Width="30%">
                                                            <ItemTemplate>
                                                                <asp:Label ID="lbl_DisabilityView" runat="server" Font-Size="Small" Text='<%# Eval("DisabilityArea") %>' />
                                                            </ItemTemplate>
                                                            <ItemStyle Width="30%" />
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="Disability" ItemStyle-Width="30%">
                                                            <ItemTemplate>
                                                                <asp:Label ID="lbl_DisabilityView" runat="server" Font-Size="Small" Text='<%# Eval("Disability") %>' />
                                                            </ItemTemplate>
                                                            <ItemStyle Width="30%" />
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="Answer">
                                                            <ItemTemplate>
                                                                <asp:Label ID="lbl_AnswerView" runat="server" Font-Size="Small" Text='<%# Eval("Answer") %>' />
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                    </Columns>
                                                    <HeaderStyle HorizontalAlign="Left" VerticalAlign="Middle" BackColor="#D6FCFE" />
                                                    <AlternatingRowStyle BackColor="LightGray" />
                                                </asp:GridView>
                                            </ContentTemplate>
                                        </asp:UpdatePanel>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                                <%--Search Grid--%>
                                <asp:Label ID="NotFoundLabel" runat="server" ForeColor="Red" Font-Bold="true" BorderStyle="None"
                                    Text="No search results found" Visible="true">
                                </asp:Label>
                            </td>
                        </tr>
                    </table>
                </div>
            </td>
        </tr>
        <tr>
            <td>
                <div style="width: 100%">
                    <table style="width: 1142px">
                        <tr>
                            <td colspan="3" width="50%" style="width: 100%; text-align: center">
                                <asp:Button ID="btn_Back" runat="server" Text="Back" PostBackUrl="~/ServiceMilestone.aspx"
                                    Width="85px" />
                                &nbsp;<asp:Button ID="btn_Save" runat="server" Text="Save" OnClick="Save_Click" ValidationGroup="PersonalInfoGroup"
                                    CausesValidation="true" Width="85px" />
                                &nbsp;<asp:Button ID="btn_Continue" runat="server" Text="Continue" Width="85px" 
                                    onclick="btn_Continue_Click" />
                            </td>
                        </tr>
                        <tr>
                            <td colspan="3" width="50%" style="width: 100%; text-align: center">
                                <asp:Label ID="lbl_ErrorGeneric" runat="server" ForeColor="#FF3300" 
                                    style="font-weight: 700"></asp:Label>
                                <asp:Label ID="lbl_Response" runat="server" ForeColor="Green" 
                                    style="font-weight: 700"></asp:Label>
                            </td>
                        </tr>
                    </table>
                </div>
            </td>
        </tr>
    </table>
</asp:Content>
