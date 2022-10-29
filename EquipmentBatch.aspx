<%@ Page Title="Batching" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="EquipmentBatch.aspx.cs" Inherits="ATUClient.EquipmentBatch" MaintainScrollPositionOnPostback = "true" %>

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
            min-width:400px;
        }
        #sidebar ul
        {
            list-style: inside;
        }
        .style7
        {
            height: 22px;
        }
        .fade.in {
  opacity: 1;
}
.modal-backdrop .fade .in {
  opacity: 0.5 !important;
}


.modal-backdrop.fade {
    opacity: 0.5 !important;
}

canvas.drawing, canvas.drawingBuffer {
            position: absolute;
            left: 0;
            top: 0;
        }
.modal-lg{ max-width: 75% !important;}

.hiddencol
        {
            display: none;
        }

    </style>
    
    
    <script src="scripts/jquery-3.3.1.min.js" type="text/javascript"></script>

    <link href="Styles/bootstrap.full.min.css" rel="stylesheet" type="text/css" />
    
    <script src="scripts/bootstrap.full.min.js" type="text/javascript"></script>
    
    <script src="scripts/quagga.min.js" type="text/javascript"></script>
    
    <script src="scripts/manage-scanner.js" type="text/javascript"></script>
    
      <script type="text/javascript">

          $(function() {
          $('[id*=chkItem]').width(20).height(20);
          });

          var caller;
          $(document).on('shown.bs.modal', '#launchScanner', function(e) {
              // below is the method to create an instance of Quagga class, which basically runs the scanner
              startScanner();
          });
          $(document).on('hide.bs.modal', '#launchScanner', function() {
              // Stop the scanner when modal is closed
              Quagga.stop();
          })
          function relaunchScanner() {
              Quagga.stop();
              startScanner();
          };
          
          function getBarcode() {
                
              $('[id*=barcode]').val(barcode); //assign the detected barcode to the text field of a hidden textbox (tweakkk)
              $('#launchScanner').modal('hide'); // close the scanner
              $('[id*=barcodeSender]').click(); // click the hidden button that transfers control to the server page where you operate on barcode
              console.log("barcode - ", barcode); 
          };

          function getReturningQuantity() {
            // to get the number of returning items; gets called by the clientclick property which evaluates this before transfer of control to server
              var num = prompt("Enter the quantity you wish to return", 1);
              if (num > 0)
                  $('[id*=sendQty]').val(num); // assign the entered number (quantity) to the hiddenfield
              else
                  return false; 

          };

          function confirmDelivery() {
            // confirm if the user wants to deliver the product
              return (confirm("Are you sure you want to deliver the product?"));

          };

          function isNumberKey(evt) {
              var charCode = (evt.which) ? evt.which : evt.keyCode;
              //if (charCode > 31 && (charCode < 48 || charCode > 57))
              //    return false;
              //return true;
              if (charCode == 46) {
                  return true;
              }
              else {
                  if (charCode > 31 && (charCode < 48 || charCode > 57))
                      return false;
                  return true;
              }
          };
        
    </script>
    
    
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <table class="style1">
        <tr>
            <td>
            <div class="row">
                <div class="card card-body bg-light mt-5 pb-3 mb-3 col col-lg-6 col-sm-12" style="margin-left:3%;">
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
                                <asp:Button ID="SearchButton" runat="server" Text="Search" OnClick="Search_Click" style="margin-bottom:10px"/>
                                <asp:Button ID="NewSearchButton" runat="server" Text="New Search" OnClick="NewSearch_Click"
                                   /><br />
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
                                <asp:DropDownList ID="ddlst_ReferralDate" Width="150" runat="server" OnSelectedIndexChanged="ReferralDate_Changed" AutoPostBack="true">
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
                        
                     <tbody style="margin-top:10%; margin-bottom:10%; display:none" id="batchMethodControls" runat="server">                      
                         <tr style="margin-top:10%; margin-bottom:10%">
                            <td style="text-align: left; vertical-align: middle; width: 30%;">
                               <asp:Label ID="Label2" runat="server" BorderStyle="None" Font-Bold="True"
                                   class="mt-3" Text="Batch method :"></asp:Label>
                            </td>
                          <td style="width: 70%; vertical-align: bottom;" colspan="2">
                          
                           <span style="margin-left:10px"> <asp:RadioButton ID="ScanRB" runat="server" Text="Scan" AutoPostBack="true" OnCheckedChanged="BatchTypeCheckedChanged" GroupName="batchtype" checked="true" Width="60px" /> 
                           </span><asp:RadioButton ID="ManualRB" runat="server" Text="Manual" GroupName="batchtype" AutoPostBack="true" OnCheckedChanged="BatchTypeCheckedChanged" />
                           <asp:RadioButton ID="BarcodeTextRB" runat="server" Text="Enter Barcode" GroupName="batchtype" AutoPostBack="true" OnCheckedChanged="BatchTypeCheckedChanged" style="margin-left:10px" />
                           
                            </td>
                            
                        </tr>
                        
                        <tr id="scanType" runat="server">
                            <td style="text-align: left; vertical-align: middle; width: 30%;">
                               <asp:Label ID="Label1" runat="server" BorderStyle="None" Font-Bold="True"
                                   class="mt-3" Text="Batch Equipment :"></asp:Label>
                            </td>
                            <td style="width: 30%">
                                <button type="button" class="btn btn-lg btn-info mt-3" data-toggle="modal" data-target="#launchScanner" id="Button1">Scan</button>
                            </td>
                            <td style="width: 40%">
                            </td>
                        </tr>
                               
                      </tbody>  

                      <tbody style="margin-top:10%; display:none" id="manualControls" runat="server">
                        <tr >
                           <td style="text-align: left; vertical-align: middle; width: 30%;">
                               <asp:Label ID="Label3" runat="server" BorderStyle="None" Font-Bold="True"
                                   class="mt-3" Text="AT Area* :"></asp:Label>
                            </td>
                            <td style="width: 30%">
                           <asp:DropDownList ID="AtAreaDd" runat="server" Visible="true" OnSelectedIndexChanged="ATArea_SelectionChanged" AutoPostBack="True" AppendDataBoundItems="true" EnableViewState="true"></asp:DropDownList> 
                            </td>
                            <td style="width: 40%">
                            </td>
                        </tr>
                        
                        <tr style="margin-top:10%; display:none" id="tier1" runat="server">
                            
                           <td style="text-align: left; vertical-align: middle; width: 30%;">
                               <asp:Label ID="Label4" runat="server" BorderStyle="None" Font-Bold="True"
                                   class="mt-3" Text="Tier1 * :"></asp:Label>
                            </td>
                            <td style="width: 30%">
                           <asp:DropDownList ID="Tier1Dd" runat="server" Visible=true OnSelectedIndexChanged="Tier1_SelectionChanged" AutoPostBack="True" AppendDataBoundItems="true" ></asp:DropDownList> 
                            </td>
                            <td style="width: 40%">
                            </td>
                        </tr>
                        
                        <tr style="margin-top:10%; display:none" id="tier2" runat="server">
                            
                           <td style="text-align: left; vertical-align: middle; width: 30%;">
                               <asp:Label ID="Label5" runat="server" BorderStyle="None" Font-Bold="True"
                                   class="mt-3" Text="Tier2 * :"></asp:Label>
                            </td>
                            <td style="width: 30%">
                          <asp:DropDownList  ID="Tier2Dd" runat="server"  OnSelectedIndexChanged="Tier2_SelectionChanged"  AutoPostBack="True" AppendDataBoundItems="true"></asp:DropDownList> 
                            </td>
                            <td style="width: 40%">
                            </td>
                        </tr>
                        
                        <tr style="margin-top:10%; display:none" id="Equip" runat="server">
                            
                           <td style="text-align: left; vertical-align: middle; width: 30%;">
                               <asp:Label ID="Label6" runat="server" BorderStyle="None" Font-Bold="True"
                                   class="mt-3" Text="Equipment * :"></asp:Label>
                            </td>
                            <td style="width: 30%">
                          <asp:DropDownList ID="EquipDd" runat="server" OnSelectedIndexChanged="Equip_SelectionChanged"  AutoPostBack="True" AppendDataBoundItems="true"></asp:DropDownList> 
                            </td>
                            <td style="width: 40%">
                            </td>
                        </tr>
                        
                        <tr style="margin-top:10%; display:none" id="prod" runat="server">
                            
                           <td style="text-align: left; vertical-align: middle; width:30%">
                               <asp:Label ID="Label7" runat="server" BorderStyle="None" Font-Bold="True"
                                   class="mt-3" Text="Product * :"></asp:Label>
                            </td>
                            <td style="text-align: left; vertical-align: middle; width:30%">
                          <asp:DropDownList  ID="productDd" runat="server"  AutoPostBack="True" AppendDataBoundItems="true"></asp:DropDownList> 
                            </td>
                            
                           <td style="width: 40%">
                            </td>

                        </tr>
                        
                         <tr style="margin-top:30px; display:none" id="prodQty" runat="server">
                           
                           <td style="text-align: left; vertical-align: middle; width:30%">
                              <asp:Label ID="Label7a" class="mt-3" runat="server" Text="Quantity: *" Font-Bold="True" Visible="true"></asp:Label>
                            </td>
                            <td style="width: 30%">
                             <asp:TextBox width="50" ID="quantityTB" runat="server" Text="1"></asp:TextBox>
                           <asp:Button  CssClass="btn btn-warning ml-3"  runat="server" id="Button2" OnClick="ProdAddBtnClick" AutoPostBack="True" Text="Add" Height="40" Width="100"/>
                            </td>
                            
                            <td style="width: 40%">
                            </td>
                        </tr>

                          <tr style="margin-top:30px; display:none" id="submitOP" runat="server">
                              <td style="text-align: left; vertical-align: middle; width:30%">
                                  <asp:Label ID="LabelOP" class="mt-3" runat="server" Text="Overall Project:(ATU $)" Font-Bold="True" Visible="true"></asp:Label>
                              </td>
                              <td style="width: 30%">
                                  <asp:TextBox width="135" ID="TextBoxOP" runat="server"></asp:TextBox>
                                  <%--<asp:Button  CssClass="btn btn-warning ml-3"  runat="server" id="Button4" OnClick="OPAddBtnClick" AutoPostBack="True" Text="Add" Height="40" Width="100" onkeypress="return isNumberKey(event)"/>--%>
                              </td>
                              <td style="width: 40%">
                              </td>
                          </tr>

                          <tr style="margin-top:30px; display:none" id="submitSupp" runat="server">
                              <td style="text-align: left; vertical-align: middle; width:30%">
                                  <asp:Label ID="LabelOPSupp" class="mt-3" runat="server" Text="Overall Project:(Supp $)" Font-Bold="True" Visible="true"></asp:Label>
                              </td>
                              <td style="width: 30%">
                                  <asp:TextBox width="135" ID="TextBoxSupp" runat="server"></asp:TextBox>
                                  <asp:Button  CssClass="btn btn-warning ml-3"  runat="server" id="Button4" OnClick="OPAddBtnClick" AutoPostBack="True" Text="Add" Height="40" Width="100" onkeypress="return isNumberKey(event)"/>
                              </td>
                              <td style="width: 40%">
                              </td>
                          </tr>
                        
                     </tbody>
                     
                      <tbody style="margin-top:15%; display:none" id="barcodeTextControls" runat="server">
                        <tr style="margin-top:10%; height:75px" >
                            <td style="text-align: left; vertical-align: middle; width:30%">
                              <asp:Label ID="Label9" class="mt-3" runat="server" Text="Barcode:*" Font-Bold="True" Visible=true></asp:Label>
                            </td>
                            <td style="width: 70%" colspan="2">
                             <asp:TextBox ID="barcodeTB" runat="server"></asp:TextBox>

                            </td>
  
                        </tr>
                        
                        <tr style="margin-top:10%;  height:50px" id="qtyRow" runat="server">
                           
                           <td style="text-align: left; vertical-align: middle; width:30%">
                              <asp:Label ID="Label10" class="mt-3" runat="server" Text="Quantity: *" Font-Bold="True" Visible=true></asp:Label>
                            </td>
                            <td style="width: 30%">
                             <asp:TextBox width="50" ID="quantityTB2" runat="server" Text="1"></asp:TextBox>
                           <asp:Button  CssClass="btn btn-warning ml-3"  runat="server" id="Button3" OnClick="AddProdBtnClick" AutoPostBack="True" Text="Add" Height="40" Width="100"/>
                            </td>
                            
                            <td style="width: 40%">
                            </td>
                        </tr>
                      </tbody>   
               </table>
               <br /><br />
              </div>
               <div class="card card-body bg-light mt-5 mb-3 col col-lg-5 col-sm-12" style="margin-left:2%">
                                <table width="95%" style="text-align:center">
                        <tr>
                            <td class="mt-3">
                            <div id="errorMsg" class="alert alert-danger" role="alert" runat=server visible=false></div>
                            <div id="warningMsg" class="alert alert-warning mt-3" role="alert" runat=server visible=false></div>
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
                                <%--<asp:Panel ID="StatusPanel" runat="server" ScrollBars="Both" Height="300">--%>
                                <asp:GridView ID="GridView1" runat="server" AutoGenerateSelectButton="true" AllowPaging="true"
                                    HeaderStyle-BackColor="Wheat" OnPageIndexChanging="grdSearch_Changing" OnRowCommand="gridSelect_Command"
                                    PageSize="20" Width="96%" CssClass="ml-4">
                                </asp:GridView>
                                
                                  <p runat="server" style="display:none;margin-top:20px;font-size:large" id="batchFailed">Could not find the product. Please scan another barcode or contact admininstrator.</p>       
                                
                                <h5 runat="server" class="mb-3" style="display:none;margin-top:50px;" id="itemsHeader">Current batch</h5>       
                                
                                
                                <asp:GridView AutoGenerateColumns="false" 
                                 OnRowDeleting="RemoveItemClick" CssClass="mt-3" OnPageIndexChanging="BatchGridView_pageChanging"
                                ID="BatchGridView" runat="server" HeaderStyle-BackColor="Wheat"
                                 PageSize="10" Width="100%">
                                 
                                    <RowStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                    <Columns>
                                     <asp:TemplateField ItemStyle-CssClass="hiddencol" HeaderStyle-CssClass="hiddencol">
                                            <ItemTemplate>
                                                <asp:Label ID="lblProductId" runat="server" Font-Size="Large" Text='<%# Eval("EquipmentProductId") %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Item" HeaderStyle-Font-Size="Medium" 
                                            ItemStyle-HorizontalAlign="Center" ItemStyle-Width="50%">
                                            <ItemTemplate>
                                                <asp:Label ID="lblDescription" runat="server" Font-Size="Medium" Text='<%# Eval("Description") %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Quantity" HeaderStyle-Font-Size="Medium" ItemStyle-HorizontalAlign="Center">
                                            <ItemTemplate>
                                                <asp:Label ID="lblQuantity" runat="server" style="text-align: center" Font-Size="Medium" Width="50px" Text='<%# Eval("Quantity") %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField ItemStyle-CssClass="hiddencol" HeaderStyle-CssClass="hiddencol">
                                            <ItemTemplate>
                                                <asp:Label ID="lblMinQty" runat="server" Font-Size="Large" Text='<%# Eval("MinQuantity") %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        
                                        <asp:TemplateField HeaderText="Cost" HeaderStyle-Font-Size="Medium" ItemStyle-Width="20%" ItemStyle-HorizontalAlign="Center">
                                            <ItemTemplate>
                                                <asp:Label ID="lblCost" runat="server" Font-Size="Medium" Text='<%# Eval("Cost") %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        
                                         <asp:TemplateField HeaderText="Remove" HeaderStyle-Font-Size="Medium" ItemStyle-Width="30%" ItemStyle-HorizontalAlign="Center">
                                            <ItemTemplate>
                                             <asp:Button ID="removeItem" runat="server" AutoPostBack="True" Text="Remove" CommandName="Delete" CssClass="btn btn-sm btn-warning p-2"/>   
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                       
                                      
                                    </Columns>
                                    <HeaderStyle BackColor="Wheat" HorizontalAlign="Center" VerticalAlign="Middle" />
                                    <AlternatingRowStyle BackColor="LightGray" />
                                
                                
                                </asp:GridView>
                                
                                <asp:Button ID="saveItems" runat="server" AutoPostBack="True" OnClick="SaveItemsClick" Text="Save" CssClass="btn btn-sm btn-success mb-5 mt-3" Visible="false"/>   
                                
                                 <h5 runat="server" style="display:none;margin-top:50px;" id="prevBatch">Batched items</h5> <br />
                             
                              <asp:GridView ID="BatchedItemsGV" runat="server" AllowPaging="true" OnPageIndexChanging="BatchedItemsGV_pageChanging"
                                    HeaderStyle-BackColor="Wheat" AutoGenerateColumns="false" OnRowDeleting="ReturnItemClick" OnRowUpdating="DeliverItemClick"
                                    PageSize="10" Width="96%" CssClass="ml-4" OnDataBound="BatchedItemsGV_DataBound" OnRowCommand="BatchedItemsGV_RowCommand1" OnRowEditing="DeleteBatchedItemClick">
                                    
                                    <%--onselectedindexchanged="BatchedItemsGV_SelectedIndexChanged"--%>
                                    <RowStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                    <Columns>
                                    
                                     <asp:TemplateField Visible="false">
                                            <ItemTemplate>
                                            <asp:CheckBox ID="chkItem" runat="server" Width="50px" style="text-align:center"/>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                    
                                    
                                     <asp:TemplateField ItemStyle-CssClass="hiddencol" HeaderStyle-CssClass="hiddencol">
                                            <ItemTemplate>
                                                <asp:Label ID="lblBatchId" runat="server" Font-Size="Large" Text='<%# Eval("EquipmentBatchId") %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        
                                        <asp:TemplateField ItemStyle-CssClass="hiddencol" HeaderStyle-CssClass="hiddencol">
                                            <ItemTemplate>
                                                <asp:Label ID="lblProductId" runat="server" Font-Size="Large" Text='<%# Eval("EquipmentProductId") %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        
                                        <asp:TemplateField HeaderText="Item" HeaderStyle-Font-Size="Medium" 
                                            ItemStyle-HorizontalAlign="Center" ItemStyle-Width="40%">
                                            <ItemTemplate>
                                                <asp:Label ID="lblDescription" runat="server" Font-Size="Medium" Text='<%# Eval("Description") %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Qty" HeaderStyle-Font-Size="Medium" ItemStyle-HorizontalAlign="Center">
                                            <ItemTemplate>
                                            <asp:Label ID="lblBatchedQuantity" runat="server" Font-Size="Medium" Text='<%# Eval("Quantity") %>'/>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Date" HeaderStyle-Font-Size="Medium" ItemStyle-Width="25%" ItemStyle-HorizontalAlign="Center">
                                            <ItemTemplate>
                                                <asp:Label ID="lblInputDate" runat="server" Font-Size="Medium" Text='<%# Eval("InputDate") %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Input user" HeaderStyle-Font-Size="Medium" ItemStyle-HorizontalAlign="Center">
                                            <ItemTemplate>
                                                <asp:Label ID="lblBatchedBy" runat="server" Font-Size="Medium" Text='<%# Eval("InputNetId") %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        
                                         <asp:TemplateField HeaderText="Deliver" HeaderStyle-Font-Size="Medium" ItemStyle-Width="20%" ItemStyle-HorizontalAlign="Center">
                                            <ItemTemplate>
                                             <asp:Button ID="deliverItem" runat="server" AutoPostBack="True" Text="Deliver" CommandName="Update" OnClientClick="return confirmDelivery();" CssClass="btn btn-sm btn-info p-2"/>   
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        
                                        <asp:TemplateField HeaderText="Return" HeaderStyle-Font-Size="Medium" ItemStyle-Width="20%" ItemStyle-HorizontalAlign="Center">
                                            <ItemTemplate>
                                             <asp:Button ID="removeItem" runat="server" AutoPostBack="True" Text="Return" CommandName="Delete" OnClientClick="return getReturningQuantity();" CssClass="btn btn-sm btn-danger p-2"/>   
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:CommandField HeaderText="Delete" ShowEditButton="True" EditImageUrl="Images\61391.png"
                                        EditText="Delete Record" ItemStyle-Height="5%" ItemStyle-Width="5%" ButtonType="Image"
                                        ControlStyle-Height="30%" ControlStyle-Width="40%" ItemStyle-HorizontalAlign="Center">
                                        </asp:CommandField>

<%--                                       <asp:CommandField HeaderText="Edit" ShowSelectButton="True" SelectImageUrl="Images\edit-solid.svg" ButtonType="Image"
                                           ControlStyle-Height="18px" ControlStyle-Width="25px" ItemStyle-HorizontalAlign="Center">
                                        </asp:CommandField>--%>

                                       <asp:TemplateField HeaderText="Edit" HeaderStyle-Font-Size="Medium" ItemStyle-Width="50%" ItemStyle-HorizontalAlign="Center">
                                            <ItemTemplate>
                                                <asp:ImageButton ID="returnEdit" CssClass="ml-2" Visible="false" runat="server" ImageUrl="Images\edit-solid.svg" CommandName="EditReturn"></asp:ImageButton>
                                            </ItemTemplate>
                                       </asp:TemplateField>
                                        <%--<asp:ButtonField HeaderText="Edit" ControlStyle-CssClass="ml-2" ButtonType="Image" ControlStyle-Height="18px" ImageUrl="Images\edit-solid.svg" CommandName="EditReturn" />--%>
                                      
                                    </Columns>
                                    <HeaderStyle BackColor="Wheat" HorizontalAlign="Center" VerticalAlign="Middle" />
                                    <AlternatingRowStyle BackColor="LightGray" />
                                    
                                </asp:GridView>
                                
                                <br />
                                
                                <asp:Button ID="genReportBtn" runat="server" AutoPostBack="True" OnClick="genImplementationReport" Text="Generate report" CssClass="btn btn-sm btn-success mb-5 mt-3 pull-right" Visible="false"/>
                                
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
 
 <asp:HiddenField ID="sendQty" runat="server"/>
 <asp:Textbox ID="barcode" runat="server" style="display:none"/>
 <asp:Button ID="barcodeSender" runat="server" OnClick="HandleBarcode" AutoPostBack="True" style="display:none"/>
 
 <div class="modal fade" id="launchScanner" tabindex="-1" role="dialog" aria-labelledby="exampleModalCenterTitle" aria-hidden="true">
  <div class="modal-dialog modal-lg modal-dialog-centered" style="margin-top:20%;" role="document">
    <div class="modal-content"> 
      <div class="modal-header">
        <h5 class="modal-title" id="exampleModalCenterTitle">Scan barcode</h5>
        <button type="button" class="close" data-dismiss="modal" aria-label="Close">
          <span aria-hidden="true">&times;</span>
        </button>
      </div>
      <div class="modal-body">
      <div class="well well-lg text-center" style="width:700px; text-align:center" id="scanner-container"></div>
      </div>
      <div class="modal-footer">
      <h5 style="margin-right:10%" id="code"></h5>
      
       <span style="margin-right:30%"><asp:Label ID="Label8"  runat="server" Text="Quantity: *" Font-Bold="true"></asp:Label>
                        <asp:TextBox width="100" ID="quantityScan" runat="server" Text="1"></asp:TextBox></span>         

        
        <button type="button" class="btn btn-primary" onclick="relaunchScanner();">Retry</button>
        <button type="button" class="btn btn-success " id="save1" onclick="getBarcode(this);">Batch</button>
      </div>
    </div>
  </div>
</div>
</asp:Content>