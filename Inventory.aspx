<%@ Page Title="Inventory" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" MaintainScrollPositionOnPostback="true"
    CodeBehind="Inventory.aspx.cs" Inherits="ATUClient.Inventory" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <style type="text/css">
        .Control
        {
            height: 13px;
            width: 285px;
        }
        .style4
        {
            height: 23px;
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
        .style5
        {
            width: 30%;
            height: 31px;
        }
        .style6
        {
            height: 31px;
        }
        .hiddencol
        {
            display: none;
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

    </style>
   
    <script src="scripts/jquery-3.3.1.min.js" type="text/javascript"></script>

    <link href="Styles/bootstrap.full.min.css" rel="stylesheet" type="text/css" />
    
    <script src="scripts/bootstrap.full.min.js" type="text/javascript"></script>
    
    <script src="scripts/quagga.min.js" type="text/javascript"></script>
    
    <script src="scripts/manage-scanner.js" type="text/javascript"></script>
    
    <script type="text/javascript">

        var caller;
        $(document).on('shown.bs.modal', '#launchScanner', function(e) {
            // do something...
            caller = e.relatedTarget;
            startScanner();
            console.log("caller ", caller.id);
        });
        $(document).on('hide.bs.modal', '#launchScanner', function() {
            // do something...
            Quagga.stop();
        })
        function relaunchScanner() {
            Quagga.stop();
            startScanner();
            document.getElementById("code").innerHTML = "";
        };
        function getBarcode() {
            if (caller.id === "scan1") {
                var box = document.getElementById("ctl00_MainContent_barcode1");

            }
            else if(caller.id === "scan2") {
                var box = document.getElementById("ctl00_MainContent_barcode2");

            }
            else if(caller.id === "scan3") {
                var box = document.getElementById("ctl00_MainContent_barcode3");

            }
            else if(caller.id === "scan4") {
                var box = document.getElementById("ctl00_MainContent_barcode4");

            }
             box.value = barcode;
            $('#launchScanner').modal('hide');
            
        
         };
        
    </script>
    

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <div class="row mr-0">
    
     <div class="container-fluid card card-body bg-light mt-5 mb-5 col-md-10 col-sm-12 col-lg-10" id="content" runat="server">
     
     <h1>Inventory</h1>
     <h3 class="mt-3">Add a new product</h3>
     
      <div id="errorMsg" class="alert alert-danger mt-3" role="alert" runat="server" visible="false"></div>
      
       <div class="row mt-5" style="margin-left:3px;">
    
            <h3><asp:Label ID="ATArea" runat="server" Text="AT Area: *" Visible=true ></asp:Label></h3>
           <span style="margin-left:20px"></span><asp:DropDownList CssClass="custom-select-lg" ID="AtAreaDd" runat="server" Visible="true" OnSelectedIndexChanged="ATArea_SelectionChanged" AutoPostBack="True" AppendDataBoundItems="true" EnableViewState="true"></asp:DropDownList> 
           
       
    </div> 
    
    <div id="Tier1" runat="server" class="row mt-5" style="margin-left:3px; display:none">
    
            <h3><asp:Label  runat="server" Text="Tier 1: *" Visible="true"></asp:Label></h3>
           <span style="margin-left:20px"></span><asp:DropDownList  CssClass="custom-select-lg" ID="Tier1Dd" runat="server" Visible=true OnSelectedIndexChanged="Tier1_SelectionChanged" AutoPostBack="True" AppendDataBoundItems="true" ></asp:DropDownList> 
           
       
    </div> 
    
    <div id="Tier2" runat="server" class="row mt-5" style="margin-left:3px; display:none">
    
            <h3><asp:Label  runat="server" Text="Tier 2: *" Visible=true></asp:Label></h3>
           <span style="margin-left:20px"></span><asp:DropDownList CssClass="custom-select-lg"  ID="Tier2Dd" runat="server"  OnSelectedIndexChanged="Tier2_SelectionChanged"  AutoPostBack="True" AppendDataBoundItems="true"></asp:DropDownList> 
           
       
    </div> 
    
    <div id="Equip" runat="server" class="row mt-5" style="margin-left:3px;  display:none">
    
            <h3><asp:Label  runat="server" Text="Equipment: *" Visible=true></asp:Label></h3>
           <span style="margin-left:20px"></span><asp:DropDownList CssClass="custom-select-lg"  ID="EquipDd" runat="server" OnSelectedIndexChanged="Equip_SelectionChanged"  AutoPostBack="True" AppendDataBoundItems="true"></asp:DropDownList> 
                
    </div> 
    
        <div id="desc" runat="server" class="row mt-5" style="margin-left:3px;  display:none">
    
            <h3><asp:Label ID="Label6"  runat="server" Text="Description: *" Visible=true></asp:Label></h3>
           <span style="margin-left:20px"><asp:TextBox width="350" Height="50" ID="description" runat="server"></asp:TextBox> 
                
    </div> 
    
    <div id="price" runat="server" class="row mt-5" style="margin-left:3px;  display:none">
    
            <h3><asp:Label ID="Label5"  runat="server" Text="Price: *" Visible=true></asp:Label></h3>
           <span style="margin-left:20px"><asp:TextBox width="100" Height="40" ID="priceTB" runat="server"></asp:TextBox> 
                
    </div> 
    
    <div id="quantity" runat="server" class="row mt-5" style="margin-left:3px;  display:none">
    
            <h3><asp:Label ID="Label7"  runat="server" Text="Quantity: *" Visible=true></asp:Label></h3>
           <span style="margin-left:20px"><asp:TextBox width="100" Height="40" ID="quantityTB" runat="server"></asp:TextBox> 
                
    </div>
    
    <div id="alertQty" runat="server" class="row mt-5" style="margin-left:3px;  display:none">
    
            <h3><asp:Label ID="Label8"  runat="server" Text="Alert when quantity is less than: *" Visible=true></asp:Label></h3>
           <span style="margin-left:20px"><asp:TextBox width="100" Height="40" ID="alertQtyTB" Text="1" runat="server"></asp:TextBox> 
                
    </div>
    
    <div id="bar1" runat="server" class="row mt-5" style="margin-left:3px; display:none">
    
            <h3><asp:Label ID="Label1"  runat="server" Text="Barcode: *" Visible=true></asp:Label></h3>
           <span style="margin-left:20px"></span><asp:TextBox width="200" Height="40" ID="barcode1" runat="server"></asp:TextBox> 
           <span style="margin-left:20px">
           <button type="button" class="btn btn-lg btn-info" data-toggle="modal" data-target="#launchScanner" id="scan1">Scan</button>
            <span style="margin-left:20px">
            <asp:Button  CssClass="btn btn-warning"  runat="server" id="add1" OnClick="AddBtnClick" AutoPostBack="True" Text="Add" Height="40" Width="100"/>
       
    </div> 
    
    <div id="bar2" runat="server" class="row mt-5" style="margin-left:3px; display:none">
    
            <h3><asp:Label ID="Label2"  runat="server" Text="Barcode: *" Visible=true></asp:Label></h3>
           <span style="margin-left:20px"></span><asp:TextBox width="200" Height="40" ID="barcode2" runat="server"></asp:TextBox> 
           <span style="margin-left:20px">
           <button type="button" class="btn btn-lg btn-info" data-toggle="modal" data-target="#launchScanner" id="scan2">Scan</button>
            <span style="margin-left:20px">
            <asp:Button  CssClass="btn btn-warning" runat="server" id="add2" OnClick="AddBtnClick" Text="Add" Height="40" Width="100"/>
       
    </div> 
    
    <div id="bar3" runat="server" class="row mt-5" style="margin-left:3px; display:none">
    
            <h3><asp:Label ID="Label3"  runat="server" Text="Barcode: *" Visible="true"></asp:Label></h3>
           <span style="margin-left:20px"></span><asp:TextBox width="200" Height="40" ID="barcode3" runat="server"></asp:TextBox> 
           <span style="margin-left:20px"/>
           <button type="button" class="btn btn-lg btn-info" data-toggle="modal" data-target="#launchScanner" id="scan3">Scan</button>
            <span style="margin-left:20px"/>
            <asp:Button  CssClass="btn btn-warning" runat="server" id="add3" OnClick="AddBtnClick" Text="Add" Height="40" Width="100"/>
       
    </div> 
    <div id="bar4" runat="server" class="row mt-5" style="margin-left:3px; display:none">
    
            <h3><asp:Label ID="Label4"  runat="server" Text="Barcode: *" Visible="true"></asp:Label></h3>
           <span style="margin-left:20px"></span><asp:TextBox width="200" Height="40" ID="barcode4" runat="server"></asp:TextBox> 
           <span style="margin-left:20px"/>
           <button type="button" class="btn btn-lg btn-info" data-toggle="modal" data-target="#launchScanner" id="scan4">Scan</button>
       
    </div> 
    
    <span id="submit" class="mt-5" style="margin-left:80%;display:none" runat="server"><asp:Button CssClass="btn btn-success" ID="saveItem" Text="Save" Height="50" Width="180" runat="server" AutoPostBack="True" OnClick="SaveBtnClick"/></span>
      

     </div>
     
            
   </div>
     
     <h2 style="text-align:center;display:none; margin-top:10%" id="success" runat="server">Success!</h2>
   

    
    
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
