<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Site.Master" MaintainScrollPositionOnPostback="true"
    CodeBehind="ServiceNeeded.aspx.cs" Inherits="ATUClient.ServiceNeeded" %>

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
        #sidebar
        {
            width: 75%;
            float: left;
            padding: 0 0 0 5px;
            min-width: 400px;
        }
        #sidebar ul
        {
            list-style: inside;
        }
        .style7
        {
            height: 22px;
        }
        .fade.in
        {
            opacity: 1;
        }
        .modal-backdrop .fade .in
        {
            opacity: 0.5 !important;
        }
        .modal-backdrop.fade
        {
            opacity: 0.5 !important;
        }
        canvas.drawing, canvas.drawingBuffer
        {
            position: absolute;
            left: 0;
            top: 0;
        }
        .modal-lg
        {
            max-width: 75% !important;
        }
        .hiddencol
        {
            display: none;
        }
    </style>

    <script src="scripts/jquery-3.3.1.min.js" type="text/javascript"></script>

    <link href="Styles/bootstrap.full.min.css" rel="stylesheet" type="text/css" />

    <script src="scripts/bootstrap.full.min.js" type="text/javascript"></script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <table class="style1">
        <tr>
            <td>
                <div class="row">
                    <div class="card card-body bg-light mt-5 pb-3 mb-3 col col-lg-6 col-sm-12" style="margin-left: 3%;">
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
                            <tr class="mt-2">
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
                                <td style="width: 25%;">
                                    <asp:TextBox ID="txtbx_LN" runat="server" Width="96%" Height="40px" Font-Size="Large"></asp:TextBox>
                                </td>
                                <td style="width: 25%">
                                    <asp:TextBox ID="txtbx_FN" runat="server" Width="96%" Height="40px" Font-Size="Large"></asp:TextBox>
                                </td>
                                <td style="width: 50%">
                                    <asp:Button ID="SearchButton" runat="server" Text="Search" OnClick="Search_Click"
                                        Style="margin-bottom: 10px" />
                                    <asp:Button ID="NewSearchButton" runat="server" Text="New Search" OnClick="NewSearch_Click" /><br />
                                </td>
                            </tr>
                            <tr>
                                <td style="width: 40%">
                                    &nbsp;
                                </td>
                                <td style="width: 40%">
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
                            
                      <tbody style="margin-top:10%; margin-bottom:10%; display:none" id="serviceNeededControls" runat="server">                      
                         <tr style="margin-top:10%; margin-bottom:10%">
                            <td style="text-align: left; vertical-align: top; width: 30%;">
                               <asp:Label ID="Label2" runat="server" BorderStyle="None" Font-Bold="True"
                                   class="mt-3" Text="Service Needed:"></asp:Label>
                            </td>
                          <td style="width: 70%; vertical-align: bottom;" colspan="2">
                          
                           
                           <asp:CheckBoxList id="servicesList" 
                                        AutoPostBack="True"
                                        CellPadding="5"
                                        CellSpacing="5"
                                        RepeatDirection="Vertical"
                                        RepeatLayout="Flow"
                                        TextAlign="Right"
                                        runat="server"
                                        OnSelectedIndexChanged="ServiceList_Selected">         
 
                            </asp:CheckBoxList>

                           
                            </td>
                            
                        </tr>
                        
                        <tr style="margin-top:10%; display:none" id="commentBox" runat="server">
                            <td style="text-align: left; vertical-align: top; width: 30%;">
                               <asp:Label ID="Label1" runat="server" BorderStyle="None" Font-Bold="True"
                                   class="mt-3" Text="Comments:"></asp:Label>
                            </td>
                          <td style="width: 70%; vertical-align: bottom;" colspan="2">
                          
                           
                          <asp:TextBox id="comments" TextMode="multiline" Columns="50" Rows="5" runat="server" />

                           
                            </td>
                            
                        </tr>
                        
                      </tbody>  
                        </table>
                        <br />
                        <br />
                        
                        <div class="row" style="margin-left:60%; display:none" id="addButtons" runat="server">
                        <asp:Button ID="AddServiceBtn" runat="server" AutoPostBack="True"  Text="Add" CssClass="btn btn-sm btn-info mb-5 mt-3" Width="100px" OnClick="AddServiceBtnClick" />
                        <asp:Button ID="CancelBtn" runat="server" AutoPostBack="True"  Text="Cancel" CssClass="btn btn-sm btn-info mb-5 mt-3 ml-3 pull-right" Width="100px" OnClick="CancelBtnClick"/>
                        </div>
                    </div>
                    <div class="card card-body bg-light mt-5 mb-3 col col-lg-5 col-sm-12" style="margin-left: 2%">
                        <table width="95%" style="text-align: center">
                            <tr>
                                <td class="mt-3">
                                    <div id="errorMsg" class="alert alert-danger" role="alert" runat="server" visible="false">
                                    </div>
                                    <div id="warningMsg" class="alert alert-warning mt-3" role="alert" runat="server"
                                        visible="false">
                                    </div>
                                    <asp:Label ID="lbl_CurrentClient" runat="server" Font-Bold="True" ForeColor="Red"
                                        Text="Current Client: "></asp:Label><asp:Label ID="lbl_CurrentClientName" runat="server"
                                            Font-Bold="False" ForeColor="Black">No user selected</asp:Label>
                                    <asp:Label ID="NotFoundLabel" runat="server" ForeColor="Red" Font-Bold="true" BorderStyle="None"
                                        Text="No results found" Visible="false"> </asp:Label>
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
                                <td colspan="5">
                                
                                 <asp:GridView ID="GridView1" runat="server" AutoGenerateSelectButton="true" AllowPaging="true"
                                    HeaderStyle-BackColor="Wheat" OnPageIndexChanging="grdSearch_Changing" OnRowCommand="gridSelect_Command"
                                    PageSize="20" Width="96%" CssClass="ml-4">
                                </asp:GridView>
                                
                                  <p runat="server" style="display:none;margin-top:20px;font-size:large" id="batchFailed">Could not find the product. Please scan another barcode or contact admininstrator.</p>       
                                
                                <h5 runat="server" class="mb-3" style="display:none;margin-top:50px;" id="itemsHeader">Current batch</h5>       
                                
                                    <h5 runat="server" style="display: none; margin-top: 50px;" id="prevBatch">
                                        Needed Services</h5>
                                    <br />
                                    <asp:GridView ID="ServicesGV" runat="server" AllowPaging="true" HeaderStyle-BackColor="Wheat"
                                        AutoGenerateColumns="false" PageSize="15" Width="96%" CssClass="ml-4">
                                        <RowStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                        <Columns>
                                            <asp:TemplateField ItemStyle-CssClass="hiddencol" HeaderStyle-CssClass="hiddencol">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblBatchId" runat="server" Font-Size="Large" Text='<%# Eval("LookUpServiceNeededID") %>' />
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField ItemStyle-CssClass="hiddencol" HeaderStyle-CssClass="hiddencol">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblBatchId" runat="server" Font-Size="Large" Text='<%# Eval("ServiceNeededID") %>' />
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Service" HeaderStyle-Font-Size="Medium" ItemStyle-HorizontalAlign="Center"
                                                ItemStyle-Width="40%">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblDescription" runat="server" Font-Size="Medium" Text='<%# Eval("ServiceNeeded") %>' />
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Date" HeaderStyle-Font-Size="Medium" ItemStyle-HorizontalAlign="Center">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblBatchedQuantity" runat="server" Font-Size="Medium" Text='<%# Eval("InputDate") %>' />
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Input User" HeaderStyle-Font-Size="Medium" ItemStyle-Width="25%"
                                                ItemStyle-HorizontalAlign="Center">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblInputDate" runat="server" Font-Size="Medium" Text='<%# Eval("InputNetID") %>' />
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Comments" HeaderStyle-Font-Size="Medium" ItemStyle-HorizontalAlign="Center">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblBatchedBy" runat="server" Font-Size="Medium" Text='<%# Eval("Comments") %>' />
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                        </Columns>
                                        <HeaderStyle BackColor="Wheat" HorizontalAlign="Center" VerticalAlign="Middle" />
                                        <AlternatingRowStyle BackColor="LightGray" />
                                    </asp:GridView>
                                    <br />
                                    <%--Uncomment this button and comment the one above, to activate selection of items in the batched gridview for report--%>
                                    <%--<asp:Button ID="genReportBtn" runat="server" AutoPostBack="True" OnClick="genReportClick" Text="Make selections for report" CssClass="btn btn-sm btn-info mb-5 mt-3 pull-right" Visible="false"/>--%>
                                </td>
                            </tr>
                        </table>
                    </div>
                </div>
            </td>
        </tr>
    </table>
</asp:Content>
