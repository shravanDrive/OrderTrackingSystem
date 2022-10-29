<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Site.Master" MaintainScrollPositionOnPostback="true" CodeBehind="ManageEquipment.aspx.cs" Inherits="ATUClient.ManageEquipment" %>

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
          $('[id*=ScanRB]').width(20).height(20);
          $('[id*=ManualRB]').width(20).height(20);
          $('[id*=BarcodeTextRB]').width(20).height(20);
          });

          var caller;
          $(document).on('shown.bs.modal', '#launchScanner', function(e) {
              // do something...
              startScanner();
          });
          $(document).on('hide.bs.modal', '#launchScanner', function() {
              // do something...
              Quagga.stop();
          })
          function relaunchScanner() {
              Quagga.stop();
              startScanner();
              //document.getElementById("code").innerHTML = "";
          };
          function getBarcode() {

              $('[id*=send]').val(barcode);
              $('#launchScanner').modal('hide');
              $('[id*=barcodeSender]').click();
              
          };

          function confirmBarcodeChange() {

              var res = confirm("Are you sure you wish to change the barcode?");
              return res;

          };

          function getNewQty() {

              var num = prompt("Enter the quantity you wish to add on:");
              if (num > 0)
                  $('[id*=send]').val(num);
              else
                  return false;
          };

          function isNumberKey(evt) {
              var charCode = (evt.which) ? evt.which : evt.keyCode;
              //if (charCode > 31 && (charCode < 48 || charCode > 57))
              //    return false;
              //return true;
              if (charCode == 46)
              {
                  return true;
              }
              else
              {
                  if (charCode > 31 && (charCode < 48 || charCode > 57))
                    return false;
                 return true;
              }
          }
        
    </script>
    
    
</asp:Content>


<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <div class="row mr-0">
     <div class="container-fluid card card-body bg-light mt-5 mb-5 col-md-10 col-sm-12 col-lg-10" id="selectProduct" runat="server">
     
     <h1 class="mt-3 text-center">Manage a product</h1> 
     
    <div class="row align-self-md-center"> 
        <asp:Button CssClass="btn btn-sm btn-primary" ID="getHierarchyBtn" Text="Get Hierarchy" OnClick="GetHierarchy"  runat="server" AutoPostBack="True" /> 
     
      <asp:Button CssClass="btn btn-sm btn-secondary ml-3 align-self-md-center" ID="allProdBtn" Text="All products list" OnClick="GetAllProductsList" runat="server" AutoPostBack="True" /> 
    </div> 
     
      <div id="errorMsg" class="alert alert-danger mt-3 text-center" role="alert" runat="server" visible="false"></div>
      
      <div class="row ml-5 mt-5">
      
             <h4><asp:Label ID="Label8" runat="server" BorderStyle="None" Font-Bold="True"
                                   class="mt-3" Text="Select product:"></asp:Label>
            <span style="margin-left:20px"></span> <asp:RadioButton ID="ScanRB" runat="server" Text="Scan" AutoPostBack="true" OnCheckedChanged="ManageTypeCheckedChanged" GroupName="selectiontype" checked="true" CssClass="mb-2" /> 
            <span style="margin-left:20px"></span> <asp:RadioButton ID="ManualRB" runat="server" Text="Manual" GroupName="selectiontype" AutoPostBack="true" OnCheckedChanged="ManageTypeCheckedChanged"/>
            <span style="margin-left:20px"></span> <asp:RadioButton ID="BarcodeTextRB" runat="server" Text="Enter Barcode" GroupName="selectiontype" AutoPostBack="true" OnCheckedChanged="ManageTypeCheckedChanged"/>
            </h4>
             
           <div class="align-content-end"> </div>
      
       </div>      
     
     <div id="scanBtn" class="row ml-5 mt-3" runat="server">
        
        <button type="button" class="btn btn-lg btn-info mt-3" data-toggle="modal" data-target="#launchScanner" id="Button1" style="width:400px">Scan</button>                  
       
       </div> 
      
    <div id="manualSelection" runat="server" style="height:auto;display:none">
        <div class="row mt-5 ml-5" runat="server">
            <div class="col-6" runat="server">
                <div class="row" runat="server">
                    <div class="col-6" runat="server">
                        <h3><asp:Label ID="ATArea" runat="server" Text="AT Area: *" Visible="true" ></asp:Label></h3>
                    </div>
                    <div class="col-5" runat="server">
                        <span style="margin-left:10px"></span><asp:DropDownList style="width: 96%;" CssClass="custom-select-lg" ID="AtAreaDd" runat="server" Visible="true" OnSelectedIndexChanged="ATArea_SelectionChanged" AutoPostBack="True" AppendDataBoundItems="true" EnableViewState="true"></asp:DropDownList>
                    </div>
                    <div class="col-1" runat="server"></div>
                </div>
                <div id="Tier1" class="row mt-5" runat="server" style="margin-left:3px; display:none">             
                    <div class="col-6 pl-0" runat="server">
                        <h3><asp:Label ID="Label1"  runat="server" Text="Tier 1: *" Visible="true"></asp:Label></h3>
                    </div>
                    <div class="col-5" runat="server">
                        <span style="margin-left:0px"></span><asp:DropDownList style="width: 100%;" CssClass="custom-select-lg" ID="Tier1Dd" runat="server" Visible="true" OnSelectedIndexChanged="Tier1_SelectionChanged" AutoPostBack="True" AppendDataBoundItems="true" ></asp:DropDownList> 
                    </div>
                    <div class="col-1" runat="server"></div>
                </div>
                <div id="Tier2" runat="server" class="row mt-5" style="margin-left:3px; display:none">
                    <div class="col-6 pl-0" runat="server">
                        <h3><asp:Label ID="Label2"  runat="server" Text="Tier 2: *" Visible="true"></asp:Label></h3>
                    </div>
                    <div class="col-5" runat="server">
                        <span style="margin-left:0px"></span><asp:DropDownList style="width: 100%;" CssClass="custom-select-lg"  ID="Tier2Dd" runat="server"  OnSelectedIndexChanged="Tier2_SelectionChanged"  AutoPostBack="True" AppendDataBoundItems="true"></asp:DropDownList> 
                    </div>
                    <div class="col-1" runat="server"></div>
                </div>
                <div id="Equip" runat="server" class="row mt-5" style="margin-left:3px;display:none">
                    <div class="col-6 pl-0" runat="server">
                        <h3><asp:Label ID="Label3"  runat="server" Text="Equipment: *" Visible="true"></asp:Label></h3>
                    </div>
                    <div class="col-5" runat="server">
                        <span style="margin-left:0px"></span><asp:DropDownList style="width: 100%;" CssClass="custom-select-lg"  ID="EquipDd" runat="server" OnSelectedIndexChanged="Equip_SelectionChanged"  AutoPostBack="True" AppendDataBoundItems="true"></asp:DropDownList> 
                    </div> <%--style="width:108%;"--%>
                    <div class="col-1" runat="server"></div>      
                </div>
                <div id="prod" runat="server" class="row mt-5" style="margin-left:3px;  display:none">
                    <div class="col-6 pl-0" runat="server">
                        <h3><asp:Label ID="Label5"  runat="server" Text="Product: *" Visible="true"></asp:Label></h3>
                    </div>
                    <div class="col-5" runat="server">
                        <span style="margin-left:0px"></span><asp:DropDownList style="width: 100%;" CssClass="custom-select-lg"  ID="productDd" runat="server"  AutoPostBack="True" AppendDataBoundItems="true"></asp:DropDownList> 
                        <span id="submit" style="margin-left:0%;margin-top:0px" runat="server"><asp:Button style="margin-top:40px" CssClass="btn btn-success" ID="saveItem" OnClick="ManangeBtnClick" Text="Manage" Height="50" Width="195" runat="server" AutoPostBack="True" /></span>
                    </div>
                    <div class="col-1" runat="server"></div>  
                </div> 
                <div id="submitOP" runat="server" class="row mt-5" style="margin-left:3px;  display:none">
                    <div class="col-6 pl-0" runat="server">
                        <h3><asp:Label ID="LabelOP" runat="server" Text="Overall Project:($)" Visible="true" ></asp:Label></h3>
                    </div>
                    <div class="col-5" runat="server">
                        <span style="margin-left:0px"></span><asp:TextBox ID="TextBoxOP" runat="server" style="height: 44px; width: 100%;" onkeypress="return isNumberKey(event)"></asp:TextBox> 
                    </div>
                    <div class="col-1" runat="server"></div>       
                </div>
                <div id="AddButton" runat="server" class="row mt-5" style="margin-left:3px;  display:none">
                    <div class="col-6 pl-0" runat="server"></div>
                    <div class="col-5" runat="server">
                        <asp:Button CssClass="btn btn-success" ID="SubmitOPButton" style="width: 100%;" OnClick="AddOPCost" Text="Add" Height="50" Width="165" runat="server" AutoPostBack="True" />
                    </div>
                    <div class="col-1" runat="server"></div>
                        <%--<span id="SubmitOPSpan" style="margin-left:10%;margin-top:50px" runat="server"></span> ManangeBtnClick--%>
                </div>
            </div>
            <div class="col-6" runat="server">
                <div class="card" id="cardGridOP" runat="server">
                    <div class="card-body" runat="server">
                        <asp:GridView AutoGenerateColumns="false" 
                            OnRowDeleting="RemoveItemClick" CssClass="mt-3" OnPageIndexChanging="BatchGridView_pageChanging"
                            ID="BatchGridView" runat="server" HeaderStyle-BackColor="Wheat"
                            PageSize="10" Width="100%">
                                <RowStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                    <Columns>
                                        <%--<asp:TemplateField HeaderText="Overall Project" ItemStyle-CssClass="hiddencol" HeaderStyle-CssClass="hiddencol">--%>
                                        <asp:TemplateField HeaderText="Overall Project" HeaderStyle-Font-Size="Medium" 
                                            ItemStyle-HorizontalAlign="Center" ItemStyle-Width="35%">
                                            <ItemTemplate>
                                                <asp:Label ID="lblOverallProject" runat="server" Font-Size="Large" Text='<%# Eval("Overallproject") %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Amount" HeaderStyle-Font-Size="Medium" 
                                            ItemStyle-HorizontalAlign="Center" ItemStyle-Width="20%">
                                            <ItemTemplate>
                                                <asp:Label ID="lblAmount" runat="server" Font-Size="Medium" Text='<%# Eval("Amount") %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <%--<asp:TemplateField HeaderText="Input User" ItemStyle-CssClass="hiddencol" HeaderStyle-CssClass="hiddencol">--%>
                                        <asp:TemplateField HeaderText="Input User" HeaderStyle-Font-Size="Medium" 
                                            ItemStyle-HorizontalAlign="Center" ItemStyle-Width="20%">
                                            <ItemTemplate>
                                                <asp:Label ID="lblInputUser" runat="server" Font-Size="Large" Text='<%# Eval("InputUser") %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>                                                                       
                                         <asp:TemplateField HeaderText="Remove" HeaderStyle-Font-Size="Medium" ItemStyle-Width="45%" ItemStyle-HorizontalAlign="Center">
                                            <ItemTemplate>
                                             <asp:Button ID="removeItem" runat="server" AutoPostBack="True" Text="Remove" CommandName="Delete" CssClass="btn btn-sm btn-warning p-2"/>   
                                            </ItemTemplate>
                                        </asp:TemplateField>                   
                                    </Columns>
                                    <HeaderStyle BackColor="Wheat" HorizontalAlign="Center" VerticalAlign="Middle" />
                                    <AlternatingRowStyle BackColor="LightGray" />  
                        </asp:GridView>
                    </div>
                </div>
            </div>
        </div>
 
    
 
    
 
    
 
    



    </div>
    
    <div id="barcodeTextgroup" runat="server" style="height:auto;  display:none"> 
       <div id="Div2" class="row mt-5 ml-5" style="margin-left:3px;" runat="server">
    
            <h3><asp:Label ID="Label11" runat="server" Text="Barcode: *" Visible="true"></asp:Label></h3>
           <span style="margin-left:20px"></span><asp:TextBox ID="barcodeTB" runat="server"></asp:TextBox>
           
    </div> 

    <div id="Div9" runat="server" class="row mt-5 ml-5" style="margin-left:3px">
    
       <span id="Span1" style="margin-left:10%" runat="server"><asp:Button CssClass="btn btn-success" ID="Button3" OnClick="ManangeBarcodeBtnClick" Text="Manage" Height="50" Width="180" runat="server" AutoPostBack="True" /></span>
    </div> 
    
 </div> 

</div>   
      
     <div class="container-fluid card card-body bg-light mt-5 mb-5 col-md-10 col-sm-12 col-lg-10" style="display:none" id="productDetails" runat="server">
     
     <h1 id="prodName" runat="server" class="mt-3 text-center"></h1>
     
      <div id="errorMsgDetails" class="alert alert-danger mt-3 text-center" role="alert" runat="server" visible="false"></div>
        
<div class="mt-3 mr-2 card card-body bg-light">      
        <div id="quantity" runat="server" class="row mt-5" style="margin-left:3px;">
    
            <h3><asp:Label ID="existingQty"  runat="server" Visible=true></asp:Label></h3>
           <span style="margin-left:20px"/> <asp:Button  CssClass="btn btn-lg btn-info ml-3"  runat="server" id="Button2" OnClientClick="return getNewQty();" OnClick="AddQtyBtnClick" AutoPostBack="True" Text="Add" />
                
    </div>
 </div>
    
    <div class="mt-3 mr-2 card card-body bg-light">
       
        <div class="row mt-5" style="margin-left:3px;">
    
            <h3><asp:Label ID="desc"  runat="server" Text="Description: *" Visible=true></asp:Label></h3>
           <span style="margin-left:20px"/><asp:TextBox width="350" Height="50" ID="descriptionTB" runat="server"></asp:TextBox> 
                
    </div> 
    
    <div id="price" runat="server" class="row mt-5" style="margin-left:3px">
    
            <h3><asp:Label ID="Labell5"  runat="server" Text="Price: *" Visible=true></asp:Label></h3>
           <span style="margin-left:20px"/><asp:TextBox width="100" Height="40" ID="priceTB" runat="server"></asp:TextBox> 
                
    </div> 
    
   
    
    <div id="minQty" runat="server" class="row mt-5" style="margin-left:3px;">
    
            <h3><asp:Label ID="Label10"  runat="server" Text="Alert when quantity is less than: *" Visible=true></asp:Label></h3>
           <span style="margin-left:20px"/><asp:TextBox width="100" Height="40" ID="minQtyTB" runat="server"></asp:TextBox> 
                
    </div> 
    
     <asp:Button CssClass="btn btn-success" style="margin-left:75%" ID="updateProduct" Text="Update all" Height="50" Width="180" runat="server" OnClick="UpdateAllBtnClick" AutoPostBack="True" />
     
 </div>
 
 <div class="mt-3 mr-2 card card-body bg-light">
    
   <div id="bar1" runat="server" class="row mt-5" style="margin-left:3px;">
    
            <h3><asp:Label ID="Label4"  runat="server" Text="Barcode:" Visible=true></asp:Label></h3>
           <span style="margin-left:20px"></span><asp:TextBox width="200" Height="40" ID="barcode1" runat="server"></asp:TextBox> 
           <button type="button" class="btn btn-lg btn-info ml-2" data-toggle="modal" data-target="#launchScanner" id="scan1">Scan to update</button>
            <asp:Button  CssClass="btn btn-warning ml-2"  runat="server" id="save1" OnClientClick="return confirmBarcodeChange();" OnClick="SaveBarcodeBtnClick" AutoPostBack="True" Text="Save" Height="40" Width="100"/>
            <asp:Button  CssClass="btn btn-warning ml-2"  runat="server" id="add1" OnClick="AddBarcodeBtnClick" AutoPostBack="True" Text="Add" Height="40" Width="100" Visible="false"/>
            <h5 class="ml-2"><asp:Label ID="success1"  runat="server" Text="Success!" ForeColor="Green" Visible="false"></asp:Label></h5>
    </div> 
    
    <div id="bar2" runat="server" class="row mt-5" style="margin-left:3px; display:none">
    
            <h3><asp:Label ID="Label6"  runat="server" Text="Barcode:" Visible=true></asp:Label></h3>
           <span style="margin-left:20px"></span><asp:TextBox width="200" Height="40" ID="barcode2" runat="server"></asp:TextBox> 

           <button type="button" class="btn btn-lg btn-info ml-2" data-toggle="modal" data-target="#launchScanner" id="scan2">Scan to update</button>

            <asp:Button  CssClass="btn btn-warning ml-2" runat="server" id="save2" OnClientClick="return confirmBarcodeChange();" OnClick="SaveBarcodeBtnClick" Text="Save" Height="40" Width="100"/>

            <asp:Button  CssClass="btn btn-warning ml-2"  runat="server" id="add2" OnClick="AddBarcodeBtnClick" AutoPostBack="True" Text="Add" Height="40" Width="100" Visible="false"/>
             <h5 class="ml-2"><asp:Label ID="success2"  runat="server" Text="Success!" ForeColor="Green" Visible="false"></asp:Label></h5>
    </div> 
    
    <div id="bar3" runat="server" class="row mt-5" style="margin-left:3px; display:none">
    
            <h3><asp:Label ID="Label7"  runat="server" Text="Barcode:" Visible="true"></asp:Label></h3>
           <span style="margin-left:20px"></span><asp:TextBox width="200" Height="40" ID="barcode3" runat="server"></asp:TextBox> 

           <button type="button" class="btn btn-lg btn-info ml-2" data-toggle="modal" data-target="#launchScanner" id="scan3">Scan to update</button>

            <asp:Button  CssClass="btn btn-warning ml-2" runat="server" id="save3" OnClientClick="return confirmBarcodeChange();" OnClick="SaveBarcodeBtnClick" Text="Save" Height="40" Width="100"/>

            <asp:Button  CssClass="btn btn-warning ml-2"  runat="server" id="add3" OnClick="AddBarcodeBtnClick" AutoPostBack="True" Text="Add" Height="40" Width="100" Visible="false"/>
         <h5 class=" ml-2"><asp:Label ID="success3"  runat="server" Text="Success!" ForeColor="Green" Visible="false"></asp:Label></h5>
    </div> 
    <div id="bar4" runat="server" class="row mt-5" style="margin-left:3px; display:none">
    
            <h3><asp:Label ID="Label9"  runat="server" Text="Barcode:" Visible="true"></asp:Label></h3>
           <span style="margin-left:20px"></span><asp:TextBox width="200" Height="40" ID="barcode4" runat="server"></asp:TextBox> 

           <button type="button" class="btn btn-lg btn-info ml-2" data-toggle="modal" data-target="#launchScanner" id="scan4">Scan to update</button>

            <asp:Button  CssClass="btn btn-warning ml-2" runat="server" id="save4" OnClick="SaveBarcodeBtnClick" OnClientClick="return confirmBarcodeChange();" Text="Save" Height="40" Width="100"/>
            <h5 class="ml-2"><asp:Label ID="success4"  runat="server" Text="Success!" ForeColor="Green" Visible="false"></asp:Label></h5>
    </div> 
    
    </div>
    
         <span id="back" class="mt-5" style="margin-left:75%;display:none" runat="server"><asp:Button CssClass="btn btn-success" OnClick="BackBtnClick" ID="backBtn" Text="Back" Height="50" Width="180" runat="server" AutoPostBack="True" />
        
         </span>
  
      
     </div>
 
          <div class="container-fluid card card-body bg-light mt-5 mb-5 col-md-10 col-sm-12 col-lg-10" style="display:none" id="updatedDetails" runat="server">
     
     <h1 id="productName" runat="server" class="mt-3 text-center">Updated details</h1>
     
       
        <div class="row mt-5" style="margin-left:3px;">
    
            <h5><asp:Label ID="descLabel"  runat="server" ></asp:Label></h5>
                
       </div> 
    
    <div id="Div3" runat="server" class="row mt-5" style="margin-left:3px">
    
            <h5><asp:Label ID="priceLabel"  runat="server"  Visible=true></asp:Label></h5>
                
    </div> 
    
    <div id="Div4" runat="server" class="row mt-5" style="margin-left:3px;">
    
            <h5><asp:Label ID="qtyLabel"  runat="server" Visible=true></asp:Label></h5>
                
    </div>
    
    <div id="Div5" runat="server" class="row mt-5" style="margin-left:3px;">
    
            <h5><asp:Label ID="minQtyLabel"  runat="server" Visible=true></asp:Label></h5>
                
    </div> 
    
   <div id="updatedBar1" runat="server" class="row mt-5" style="margin-left:3px;">
    
            <h5><asp:Label ID="barcode1Label"  runat="server" Visible=true></asp:Label></h5>
           
    </div> 
    
    <div id="updatedBar2" runat="server" class="row mt-5" style="margin-left:3px; display:none">
    
            <h5><asp:Label ID="barcode2Label"  runat="server" Visible=true></asp:Label></h5>          
    </div> 
    
    <div id="updatedBar3" runat="server" class="row mt-5" style="margin-left:3px; display:none">
    
            <h5><asp:Label ID="barcode3Label"  runat="server" Visible="true"></asp:Label></h5>
  
   </div> 
  
    <div id="updatedBar4" runat="server" class="row mt-5" style="margin-left:3px; display:none">
    
            <h5><asp:Label ID="barcode4Label"  runat="server" Visible="true"></asp:Label></h5>
           
    </div> 
    
         <span id="okBtn" class="mt-5" style="margin-left:5%;display:none" runat="server"><asp:Button CssClass="btn btn-success" OnClick="BackBtnClick" ID="Button12" Text="Okay" Height="50" Width="180" runat="server" AutoPostBack="True" />
         </span>
  
     </div>
  
</div>
     
 <asp:HiddenField ID="send" runat="server"/>
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
        <h5 style="margin-right:50%" id="code"></h5>
        <button type="button" class="btn btn-lg btn-primary" onclick="relaunchScanner();">Retry</button>
        <button type="button" class="btn btn-lg btn-success " id="save1" onclick="getBarcode(this);">Save</button>
      </div>
    </div>
  </div>
</div>
  
</asp:Content>
