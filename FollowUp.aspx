<%@ Page Title="Batching" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="FollowUp.aspx.cs" Inherits="ATUClient.FollowUp" MaintainScrollPositionOnPostback = "true" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">

    <style type="text/css">
        table.table_class tbody  tr th  
        {
            text-align:center !important;
        }


        .datepicker {
            font-size: 0.875em;
        }
            /* solution 2: the original datepicker use 20px so replace with the following:*/

            .datepicker td, .datepicker th {
                width: 1.5em;
                height: 1.5em;
            }

            .hiddencol
        {
            display: none;
        }
    </style>       
    <script src="scripts/jquery-3.3.1.min.js" type="text/javascript"></script>
    <link href="Styles/bootstrap.full.min.css" rel="stylesheet" type="text/css" />  
    <script src="scripts/bootstrap.full.min.js" type="text/javascript"></script>
    <link href="https://cdnjs.cloudflare.com/ajax/libs/bootstrap-datepicker/1.7.1/css/bootstrap-datepicker.min.css" rel="stylesheet"/>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/bootstrap-datepicker/1.7.1/js/bootstrap-datepicker.min.js" type="text/javascript"></script>
    <script type="text/javascript">
                $(document).ready(function () {

            $('#datepicker').datepicker({
                weekStart: 1,
                format: 'mm/dd/yyyy',
                daysOfWeekHighlighted: "6,0",
                autoclose: true,
                todayHighlight: true,
            });

            //$('#datepicker').datepicker("setDate", new Date(2008, 9, 03));
            $('#datepicker').datepicker("setDate", '<%=GetNewDate()%>');

            //PopulateDatePicker(NewDate);
            //$('table tr').filter((i, e) => e.textContent.trim().length == 0).remove();
                });

                function confirmSave() {
                    // confirm if the user wants to deliver the product
                    return (confirm("Are you sure you want to save the details?"));

                };
    </script>

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server" EnableViewState="true">
    <div class="row" style="margin-left:0%; margin-right:0%">
<%--        <div class="col-4">
            <div class="card" style="width: 100%; margin-top:4%">
                <div class="card-header"> <b>Follow-Up Operation</b> </div>
                <div class="card-body">
                    <p>Follow-Up Operation Can add Web Elements to make it interact</p>
                </div>
            </div>
        </div>--%>
        <div class="col-12">
            <div class="card" style="width: 100%; margin-top:2%">
                <div class="card-header"> <b>Follow-Up Report</b> </div>
                <div class="card-body" style="padding:0%">
                   <div class="row">
                        <div class="input-group col-3" style="margin-left:1%; height:46px">
                            <span class="input-group-text" style="font-family: Calibri; font-size: small;width:50%;margin-top:2%">APC Date</span>                                      
                            <input data-date-format="dd/mm/yyyy" style="font-size: small; width: 50%;margin-top:2%" name="dueDate" id="datepicker">
                        </div>
                        <div class="col-2" style="padding-left:0px">
                            <asp:Button style="font-family:Calibri" class="btn btn-dark mt-2 px-0" Text="Search" CssClass="" Width="210" runat="server" OnClick="Search_Click" ToolTip="Edit" />
                        </div>
                       <div class="col-1"></div>
                       <div class="col-3">
                            <%--<asp:Button style="font-family:Calibri" class="btn btn-dark form-control my-2 btn-sm mx-3" Text="Generate Report" runat="server" ToolTip="Edit" Height="37px" />--%>
                       </div>
                       <div class="col-2">
                            <%--<asp:Button style="font-family:Calibri" class="btn btn-dark form-control my-2 btn-sm" Text="Save" runat="server" ToolTip="Edit" Height="37px" />--%>
                       </div>
                    </div>
                    </div>
<%--                    <div class="row" style="margin-top:2%">
                    </div>
                    <div class="row">
                        <div class="col-7" style="padding-right:25px">   
                            <%--<input class="form-control mt-2" type="text" placeholder="N.I - No Issues &nbsp&nbsp&nbsp C.C.N - Clinician Call Needed &nbsp&nbsp&nbsp C.V.N - Clinician Visit Needed" readonly>                     
                        </div>
                        <div class="col-3">
                            <asp:Button style="font-family:Calibri" class="btn btn-dark form-control my-2 btn-sm mx-3" Text="Generate Report" runat="server" ToolTip="Edit" Height="37px" />
                        </div>
                        <div class="col-2">
                            <asp:Button style="font-family:Calibri" class="btn btn-dark form-control my-2 btn-sm" Text="Save" runat="server" ToolTip="Edit" Height="37px" />
                        </div>
                    </div>--%>
                    <div class="row" style="margin-top:2%">   
                        <div class="col-12">

                    <asp:GridView
                        ID="FollowUpGrid"
                        runat="server"
                        OnRowDataBound="OnRowDataBound"
                        OnRowCommand="FollowUpGrid_RowCommand"
                        AllowPaging="true"
                        HeaderStyle-BackColor="Wheat"
                        HeaderStyle-HorizontalAlign="Center"
                        AutoGenerateColumns="false"
                        PageSize="20"
                        OnPageIndexChanging="FollowUpGrid_PageIndexChanging"
                        Width="100%"
                        CssClass="font-family: Calibri;">

                        <%--OnPageIndexChanging="BatchedItemsGV_pageChanging"--%>
                        <%--OnRowDeleting="ReturnItemClick"--%>
                        <%--OnRowUpdating="DeliverItemClick"--%>

                        <%--OnRowCommand="BatchedItemsGV_RowCommand1"--%>
                        <%--OnRowEditing="DeleteBatchedItemClick"--%>

                        <RowStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                        <Columns>                           
                            <asp:TemplateField HeaderText="Last Name" HeaderStyle-Font-Size="Medium">                               
                                <ItemTemplate>
                                    <asp:Label ID="lblLast_Name" runat="server" Font-Size="Medium" Text='<%# Eval("LastName") %>' />
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="First Name" HeaderStyle-Font-Size="Medium" ItemStyle-HorizontalAlign="Center">
                                <ItemTemplate>
                                    <asp:Label ID="lblFirstName" runat="server" Font-Size="Medium" Text='<%# Eval("FirstName") %>' />
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="APC" HeaderStyle-Font-Size="Medium" ItemStyle-HorizontalAlign="Center">
                                <ItemTemplate>
                                    <asp:Label ID="lblAPC" runat="server" Font-Size="Medium" Text='<%# Eval("APC") %>' />
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="1-Week" HeaderStyle-Font-Size="Medium"  ItemStyle-HorizontalAlign="Center">
                                <ItemTemplate>
                                    <asp:Label ID="lbl1Week" runat="server" Font-Size="Medium" Text='<%# Eval("OneWeek") %>' />
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="1-Month" HeaderStyle-Font-Size="Medium"  ItemStyle-HorizontalAlign="Center">
                                <ItemTemplate>
                                    <asp:Label ID="lbl1Month" runat="server" Font-Size="Medium" Text='<%# Eval("OneMonth") %>' />
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="3-Month" HeaderStyle-Font-Size="Medium"  ItemStyle-HorizontalAlign="Center">
                                <ItemTemplate>
                                    <asp:Label ID="lbl3Month" runat="server" Font-Size="Medium" Text='<%# Eval("ThreeMonth") %>' />
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="Status Completed" HeaderStyle-Font-Size="Medium"  ItemStyle-HorizontalAlign="Center">
                                <ItemTemplate>
                                    <asp:DropDownList ID="ListID" Style="font-family: Calibri, FontAwesome;width:96%"  runat="server" Visible="true" OnSelectedIndexChanged="ListID_SelectedIndexChanged" AutoPostBack="True" AppendDataBoundItems="true" EnableViewState="true"></asp:DropDownList>
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="Comment" HeaderStyle-Font-Size="Medium"  ItemStyle-HorizontalAlign="Center">
                                <ItemTemplate>
                                    <asp:DropDownList ID="CommentID" Style="font-family: Calibri, FontAwesome;width:96%" runat="server" Visible="true" OnSelectedIndexChanged="CommentID_SelectedIndexChanged" AutoPostBack="True" AppendDataBoundItems="true" EnableViewState="true"></asp:DropDownList>
                                </ItemTemplate>
                            </asp:TemplateField>

<%--                            <asp:TemplateField HeaderText="Comment" HeaderStyle-Font-Size="Medium"  ItemStyle-HorizontalAlign="Center">
                                <ItemTemplate>
                                    <div class="row">
                                        <div class="col-6" style="font-size:small;margin-left:0px">
                                            <asp:CheckBoxList runat="server" id="lstBoxTest" selectionmode="Multiple">
                                                <asp:listitem Text="No issues" Value="1"></asp:listitem>
                                                <asp:listitem Text="Clinician Call needed" Value="2"></asp:listitem>
                                                <asp:listitem Text="Clinician visit needed" Value="3"></asp:listitem>
                                                <asp:listitem Text="Call made-couldn't reach client(Prime Notified)" Value="7"></asp:listitem>
                                            </asp:CheckBoxList>
                                        </div>
                                        <div class="col-6" style="font-size:small">
                                            <asp:CheckBoxList runat="server" id="CheckBoxList1" selectionmode="Multiple">
                                                <asp:listitem Text="Equipment Specialist visit needed" Value="4"></asp:listitem>
                                                <asp:listitem Text="Client no longer in community" Value="5"></asp:listitem>
                                                <asp:listitem Text="Call made - couldn't reach client" Value="6"></asp:listitem>
                                            </asp:CheckBoxList>
                                        </div>
                                    </div>--%>
<%--<%--                                    <%--<asp:TextBox ID="Comment" runat="server" Font-Size="Medium" Text='<%# Eval("Comment") %>' />--%>
                                    <%--<asp:TextBox ID="Comment" runat="server" Style="font-size: small;border-color:black;margin:2%;width:90%" Text='<%# Eval("Comment") %>' class="form-control form-control-sm"></asp:TextBox>--%>
 <%--                                   <div class="row my-1">
                                        <div class="input-group col-3" style="margin-left:1%">
                                            <div class="input-group-prepend">
                                                <div class="input-group-text" style="background-color:white;border:none">
                                                    <asp:CheckBox ID="NoIssues" runat="server" Style="font-size: small" aria-label="Checkbox for following text input" />
                                                </div>
                                            </div>
                                            <input type="text" class="form-control" style="font-family: Calibri;font-weight:600;padding-left:0px;background-color:white;border:none;margin-bottom:3%" disabled="disabled" value="No Issues">
                                        </div>
                                        <div class="input-group col-4" style="margin-left:1%;padding:0%">
                                            <div class="input-group-prepend">
                                                <div class="input-group-text" style="background-color:white;border:none">
                                                    <asp:CheckBox ID="CallNeeded" runat="server" Style="font-size: small" aria-label="Checkbox for following text input" />
                                                </div>
                                            </div>
                                            <input type="text" class="form-control" style="font-family: Calibri;font-weight:600;padding-left:0px;background-color:white;border:none;margin-bottom:2%" disabled="disabled" value="Clinician Call needed">
                                        </div>
                                        <div class="input-group col-4" style="margin-left:1%;padding:0%">
                                            <div class="input-group-prepend">
                                                <div class="input-group-text" style="background-color:white;border:none">
                                                    <asp:CheckBox ID="CheckBox1" runat="server" Style="font-size: small" aria-label="Checkbox for following text input" />
                                                </div>
                                            </div>
                                            <input type="text" class="form-control" style="font-family: Calibri;font-weight:600;padding-left:0px;background-color:white;border:none;margin-bottom:2%" disabled="disabled" value="Clinician Visit needed">
                                        </div>
                                    </div>
                                    <div class="row my-1">
                                        <div class="input-group col-5" style="margin-left:1%">
                                            <div class="input-group-prepend">
                                                <div class="input-group-text" style="background-color:white;border:none">
                                                    <asp:CheckBox ID="CheckBox2" runat="server" Style="font-size: small" aria-label="Checkbox for following text input" />
                                                </div>
                                            </div>
                                            <input type="text" class="form-control" style="font-family: Calibri;font-weight:600;padding-left:0px;background-color:white;border:none;margin-bottom:1%" disabled="disabled" value="Equipment Specialist visit needed">
                                        </div>
                                        <div class="input-group col-5" style="margin-left:1%">
                                            <div class="input-group-prepend">
                                                <div class="input-group-text" style="background-color:white;border:none">
                                                    <asp:CheckBox ID="CheckBox3" runat="server" Style="font-size: small" aria-label="Checkbox for following text input" />
                                                </div>
                                            </div>
                                            <input type="text" class="form-control" style="font-family: Calibri;font-weight:600;padding-left:0px;background-color:white;border:none;margin-bottom:1%" disabled="disabled" value="Client no longer in community">
                                        </div>
                                    </div>--%>
<%--                                </ItemTemplate>
                            </asp:TemplateField>--%>

                            <asp:TemplateField HeaderText="Save" HeaderStyle-Font-Size="Medium"  ItemStyle-HorizontalAlign="Center">
                                <ItemTemplate>
                                    <asp:Button ID="saveID" runat="server" AutoPostBack="True" Text="Save" CommandName="Save" OnClientClick="return confirmSave();" CssClass="btn btn-dark form-control my-2 btn-sm"/>   
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField ItemStyle-CssClass="hiddencol" HeaderStyle-CssClass="hiddencol">
                                 <ItemTemplate>
                                      <asp:Label ID="lblClientId" runat="server" Font-Size="Large" Text='<%# Eval("ClientID") %>' />
                                 </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField ItemStyle-CssClass="hiddencol" HeaderStyle-CssClass="hiddencol">
                                 <ItemTemplate>
                                      <asp:Label ID="lblStatusId" runat="server" Font-Size="Large" Text='<%# Eval("Status") %>' />
                                 </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField ItemStyle-CssClass="hiddencol" HeaderStyle-CssClass="hiddencol">
                                 <ItemTemplate>
                                      <asp:Label ID="lblCommentId" runat="server" Font-Size="Large" Text='<%# Eval("CommentVal") %>' />
                                 </ItemTemplate>
                            </asp:TemplateField>

                        </Columns>
                    </asp:GridView>
                        </div>             
                  </div>
                </div>
            </div>
        </div>
</div>
</asp:Content>
