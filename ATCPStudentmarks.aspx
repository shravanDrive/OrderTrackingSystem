<%@ Page Title="Batching" Language="C#"   MasterPageFile="~/ATCP.Master"
    CodeBehind="ATCPStudentmarks.aspx.cs" Inherits="ATCPClient.ATCPStudentmarks" MaintainScrollPositionOnPostback = "true" %>

    <asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
        <style>
            [class*="dataTables_"] {
                font-size: small;
            }

            .gridview-header
            {
                font-weight: bold;
                font-size: 10px;
                padding-bottom: 3px;
                color:  #666666;
                padding-top: 3px;
                font-family: Verdana, Geneva, Arial, Helvetica, sans-serif;
                background-color: #EEEEEE;
            }
        </style>

           <script>

        var UIN;
        var studentTable;
        function AssignUINValue()
        {
           // UIN = $("#searchName1").val();
           // console.log(UIN);
        }

        $(document).ready(function () 
        {   
            //var UIN = "687654875";
            //studentTable = $('#dtExample').DataTable();
            var UINExtension = window.location.search;
            console.log("yo");
           // $("#SubmitButton").click(function() 
           // {
                //getvalue(this.id);
                //UIN = $("#searchName1").val();
                //studentTable = $('#dtExample').DataTable();
                //if (UIN != "")
                //{   
                    //studentTable.destroy();
                    //console.log("Inside" + UIN);
                    $('#dtExample').DataTable
                    ({    
                        "ajax":
                        {
                            //url:"GetCourseInfoPerStudent.php",
                            //dataSrc : '',
                            //method: "post",
                            //data: 
                            //{ 
                            //    "UIN": UIN //Get this from the search box
                            //}
                            url: "ATCPStudentmarks.aspx/LoadSubjectGrades" + UINExtension,
                            dataSrc: function (data) {
                                return $.parseJSON(data.d)
                            },
                            dataType: 'json',
                            contentType: "application/json; charset=utf-8",
                            method: "post"
                        },  
                        columns: 
                        [    
                            { data: 'Number' },
                            { data: 'Credits' },
                            { data: 'Year' },
                            { data: 'Term' },
                            { data: 'Grade', class: 'editable text' },             
                            {    
                                //edit button creation    
                                render: function (data, type, row) 
                                {    
                                    return createButton('edit', row.id);    
                                }    
                            },        
                        ],    
                        "searching": false,    
                        "paging": true,    
                        "info": true,    
                        "language": 
                        {    
                            "emptyTable": "No data available"    
                        } //,
                    // "fnRowCallback": function (nRow, aData, iDisplayIndex) 
                    // {    
                    //     $("td:first", nRow).html(iDisplayIndex + 1);    
                    //     return nRow;    
                    // },    
                    });                 
                //}
            //});

        });
    
        function createButton(buttonType, rowID) {
            var buttonText = buttonType == "edit" ? "Edit" : "Delete";
            return '<button runat="server" Onclick="confirmDelivery()" type="submit" class="btn btn-outline-dark w-75 form-control btn-sm + ' + buttonType + '">' + buttonText + '</button>';
            //return '<button runat="server" type="button" id="updateButton" onserverclick="UpdateEmail_Click" class="btn btn-success btn-block"></button>'
        }

        //OnClientClick = "return confirmDelivery();"
        function confirmDelivery() {
            alert("Hello");
            fnResetControls();
            //var dataTable = $('#dtExample').DataTable();
            //console.log(dataTable);
            console.log("fdgf");
            var clickedRow = $($(this).closest('td')).closest('tr');
            alert(clickedRow);
            console.log("hi");
            console.log(clickedRow);

            $(clickedRow).find('td').each(function () {
                // do your cool stuff    
                
                alert("hi");
                if ($(this).hasClass('editable')) {
                    if ($(this).hasClass('text')) {
                        var html = fnCreateTextBox($(this).html(), 'Grade');
                        $(this).html($(html))
                    }
                }
            });

            $('#dtExample tbody tr td .update').removeClass('update').addClass('edit').html('Edit');
            $(clickedRow).find('td .edit').removeClass('edit').addClass('update').html('Update');
        }
    

               //$('')
    //$('#dtExample').on('click', 'tbody td .edit', function (e) {    
    //    fnResetControls();    
    //    //var dataTable = $('#dtExample').DataTable();
    //    alert("after");
    //    //console.log(dataTable);
    //    var clickedRow = $($(this).closest('td')).closest('tr');
    //    console.log(clickedRow);
    //    $(clickedRow).find('td').each(function () {    
    //        // do your cool stuff    
    //        if ($(this).hasClass('editable')) {    
    //            if ($(this).hasClass('text')) {    
    //                var html = fnCreateTextBox($(this).html(), 'Grade');    
    //                $(this).html($(html))    
    //            }    
    //        }    
    //    });     
       
    //    $('#dtExample tbody tr td .update').removeClass('update').addClass('edit').html('Edit');        
    //    $(clickedRow).find('td .edit').removeClass('edit').addClass('update').html('Update');       
    
    //});    
    
        function fnCreateTextBox(value, fieldprop) {
            alert("create");
        return '<input runat="server" class="form-control form-control-sm" data-field="' + fieldprop + '" type="text" value="' + value + '" ></input>';    
    } 

    $('#dtExample').on('click', 'tbody td .cancel', function (e) {    
        fnResetControls();    
        $('#dtExample tbody tr td .update').removeClass('update').addClass('edit').html('Edit');    
    });    
    
    
    function fnResetControls() {
        alert("reset");
        var openedTextBox = $('#dtExample').find('input');    
        $.each(openedTextBox, function (k, $cell) {    
            $(openedTextBox[k]).closest('td').html($cell.value);    
        })
        alert("reset end");
    }

    $('#dtExample').on('click', 'tbody td .update', function (e) {    
    
       var openedTextBox = $('#dtExample').find('input');    
       $.each(openedTextBox, function (k, $cell) {    
           fnUpdateDataTableValue($cell, $cell.value);    
           $(openedTextBox[k]).closest('td').html($cell.value);    
       })    
    
       $('#dtExample tbody tr td .update').removeClass('update').addClass('edit').html('Edit');        
   });    
    
    function fnUpdateDataTableValue($inputCell, value) 
    {    
       var dataTable = $('#dtExample').DataTable();    
       var rowIndex = dataTable.row($($inputCell).closest('tr')).index();    
       var fieldName = $($inputCell).attr('data-field');    
       dataTable.rows().data()[rowIndex][fieldName] = value;
       
       var selectedRow = dataTable.rows().data()[rowIndex];
       //console.log(dataTable.rows().data()[rowIndex]);
       // Update Student Datatable with value here //
       $.ajax
       ({
            url:"UpdateGradeInfoPerStudent.php",
            dataSrc : '',
            method: "post",
            data: 
            { 
                "UIN": selectedRow.UIN, //Get this from the search box
                "courseNumber":selectedRow.Number,
                "courseYear":selectedRow.Year,
                "courseTerm":selectedRow.Term,
                "updatedGrade": selectedRow.Grade
            }
                            //             success: function(result) { //we got the response
                            //     alert(result);
                            // },
                            // error: function(jqxhr, status, exception) {
                            //     //alert('Exception:', exception);
                            // console.log(exception);
                            // console.log(jqxhr);
                            // console.log(status);
                            // console.warn(jqxhr.responseText) // very important
                            // console.log(jqxhr.responseJSON);
                            // } 
       });

   }    
 
</script>
    </asp:Content>

    <asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server" EnableViewState="true">
            <div class="container">
        <div class="row">
            <div class="col-3">
                <label class="font-weight-bold" for="searchName" style="font-size: 13px;">Search By UIN:</label>
                <%--<input type="searchName" class="form-control form-control-sm" id="searchName1" aria-describedby="searchName">--%>
                <asp:TextBox ID="searchTextBox" runat="server" style="font-size: small" class="form-control form-control-sm" aria-describedby="firstname"></asp:TextBox>
            </div>
            <div class="col-2">
                <label class="font-weight-bold" for="searchName" style="font-size: 13px;"></label>
                <%--<button type="submit" class="btn btn-dark form-control my-2 btn-sm" id="SubmitButton">Search</button>--%> 
                <asp:Button style="font-family:Calibri" class="btn btn-dark form-control my-2 btn-sm" Text="Search" runat="server" OnClick="Unnamed_Click" Height="30px" />
                <!-- onclick="AssignUINValue()" -->
            </div>
            <div class="col-5"></div>
            <!-- <div class="col-2">
                <label class="font-weight-bold" for="searchName" style="font-size: 13px;"></label>
                <button type="submit" class="btn btn-dark form-control my-2 btn-sm">Submit</button>
            </div> -->
        </div>
        <div class="row my-4">
            <div class="d-flex col justify-content-start">
                <div class="card" style="width: 70rem;">
                    <div class="card-header font-weight-bold">
                        <asp:Label Id="LblStudentName" style="font-family:Calibri" runat="server" />
                    </div>
                    <div class="card-body" id="cardBodyData">
                        
               <asp:GridView ID="GridView1" runat="server" AutoGeneratedColumns="false" ShowFooter="true" DataKeyNames="CourseNumber"
                   Width="100%"
                   CssClass="gridview"
                ShowHeaderWhenEmpty="true"
                OnRowEditing="GridView1_RowEditing"
                OnRowCancelingEdit="GridView1_CancellingEdit"
                OnRowUpdating="GridView1_RowUpdating"
                BackColor="White" BorderColor="#CCCCCC" BorderStyle="None" BorderWidth="1px" CellPadding="3" EnableModelValidation="True" AutoGenerateColumns="False">
                <%--<FooterStyle BackColor="White" ForeColor="#000066" />--%>
                <HeaderStyle Font-names="Calibri" BackColor="#006699" Font-Bold="True" ForeColor="White"/>
                <PagerStyle BackColor="White" ForeColor="#000066" HorizontalAlign="Left" />
                <RowStyle ForeColor="#000066" />
                <SelectedRowStyle BackColor="#669999" Font-Bold="True" ForeColor="White" />
                <%--<SortedAscendingCellStyle BackColor="#F1F1F1">--%>

                <Columns>
                    <asp:TemplateField HeaderText="Course">
                        <ItemTemplate>
                            <asp:Label style="font-family:Calibri" Text='<%# Eval("CourseNumber") %>' runat="server" />
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:Label style="font-family:Calibri" ID="txtCourse" Text='<%# Eval("CourseNumber") %>' runat="server" /><%-- to make this editable just change Lable to textboxs--%>
                        </EditItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="Credits">
                        <ItemTemplate>
                            <asp:Label style="font-family:Calibri" Text='<%# Eval("Credits") %>' runat="server" />
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:Label ID="txtCredits" Text='<%# Eval("Credits") %>' runat="server" />
                        </EditItemTemplate>
<%--                        <FooterTemplate>
                            <%--<asp:TextBox ID="txtCreditsFooter" runat="server" />
                        </FooterTemplate>--%>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="Year_Taken">
                        <ItemTemplate>
                            <asp:Label style="font-family:Calibri" Text='<%# Eval("CourseYear") %>' runat="server" />
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:Label style="font-family:Calibri" ID="txtYear_Taken" Text='<%# Eval("CourseYear") %>' runat="server" />
                        </EditItemTemplate>
   <%--                     <FooterTemplate>
                            <%--<asp:TextBox ID="txtYear_TakenFooter" runat="server" />
                        </FooterTemplate>--%>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="Term_Taken">
                        <ItemTemplate>
                            <asp:Label style="font-family:Calibri" Text='<%# Eval("Term") %>' runat="server" />
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:Label style="font-family:Calibri" ID="txtTerm_Taken" Text='<%# Eval("Term") %>' runat="server" />
                        </EditItemTemplate>
    <%--                    <FooterTemplate>
                            <%--<asp:TextBox ID="txtTerm_TakenFooter" runat="server" />
                        </FooterTemplate>--%>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="Grade">
                        <ItemTemplate>
                            <asp:Label style="font-family:Calibri" Text='<%# Eval("Grade") %>' runat="server" />
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:TextBox style="font-family:Calibri" ID="txtGrade" Text='<%# Eval("Grade") %>' runat="server" />
                        </EditItemTemplate>
<%--                        <FooterTemplate>
                            <%--<asp:TextBox ID="txtGradeFooter" runat="server" />
                        </FooterTemplate>--%>
                    </asp:TemplateField>

                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:Button style="font-family:Calibri" class="btn btn-dark form-control my-2 btn-sm" Text="Edit" runat="server" CommandName="Edit" ToolTip="Edit" Height="30px" />
                            <%--<asp:ImageButton ImageUrl="~/Images/Cancel.PNG" runat="server" CommandName="Cancel" ToolTip="Cancel" Width="20px" Height="20px" />--%>
                            <%--<asp:LinkButton runat="server" ID="btnRun" Text="<i class='icon-camera-retro'></i> Search" CommandName="Edit" ToolTip="Edit"  OnClick="btnRun_Click" CssClass="greenButton" />--%>
                            <%--<asp:LinkButton runat="server" ID="LinkButton1" Text="<i class='icon-camera-retro'></i> Search" CommandName="Delete" ToolTip="Delete"  OnClick="btnRun_Click" CssClass="greenButton" />--%>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <div class="row">
                                <div class="col-6">
                                     <asp:Button style="font-family:Calibri" class="btn btn-dark form-control my-2 btn-sm" Text="Update" runat="server" CommandName="Update" ToolTip="Edit" Height="30px" />
                                </div>
                                <div class="col-6">
                                    <asp:Button style="font-family:Calibri" class="btn btn-dark form-control my-2 btn-sm" Text="Cancel" runat="server" CommandName="Cancel" ToolTip="Cancel" Height="30px" />
                                </div>
                            </div>
                            <%--<asp:ImageButton ImageUrl="~/Images/Save.PNG" runat="server" CommandName="Update" ToolTip="Edit" Width="35px" Height="30px" />--%>

                            
                            <%--<asp:ImageButton ImageUrl="~/Images/Cancel.PNG" runat="server" CommandName="Cancel" ToolTip="Cancel" Width="20px" Height="20px" />--%>
                            <%--<asp:LinkButton runat="server" ID="btnRun" Text="<i class='icon-camera-retro'></i> Search" CommandName="Update" ToolTip="Update" OnClick="btnRun_Click" CssClass="greenButton" />
                            <asp:LinkButton runat="server" ID="~/Images/Cancel.PNG" Text="<i class='icon-camera-retro'></i> Search" CommandName="Cancel" ToolTip="Cancel"  OnClick="btnRun_Click" CssClass="greenButton" />--%>
                        </EditItemTemplate>
<%--                        <FooterTemplate>
                            <%--<asp:LinkButton runat="server" ID="LinkButton1" Text="<i class='icon-camera-retro'></i> Search" CommandName="AddNew" ToolTip="Cancel" OnClick="btnRun_Click" CssClass="greenButton" />
                        </FooterTemplate>--%>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
            <br />
            <asp:Label ID="lblSuccessMessage" Text="" runat="server" ForeColor="Green" />
            <br />
            <asp:Label ID="lblErrorMessage" Text="" runat="server" ForeColor="Red" />
                        
                        
                        
                        
                         
<%--                        <table width="100%" id="dtExample" class="display " cellspacing="0">
                            <thead>
                                <tr>
                                    <th>Course</th>
                                    <th>Credits</th>
                                    <th>Year_Taken</th>
                                    <th>Term_Taken</th>
                                    <th>Grade</th>
                                    <th>Action</th> 
                                </tr>
                            </thead>
                        </table>                
                        <!-- <table width="100%" id="dtExample" class="display " cellspacing="0">        
                        </table>     -->
                        <!-- <table id = "table_sm" class="table" style="font-size: small;">
                        <thead>
                            <tr>
                                <th>Course#</th>
                                <th>Credits</th>
                                <th>Year_Taken</th>
                                <th>Term_Taken</th>
                                <th>Grade</th>
                            </tr>
                        </thead>
                        <tbody>
                            <tr>
                                <td><input type="searchName" class="border-0" id="searchName1" aria-describedby="searchName" value="DHD 540"></td>
                                <td><input type="searchName" class="border-0" id="searchName1" aria-describedby="searchName" value="2021"></td>
                                <td><input type="searchName" class="border-0" id="searchName1" aria-describedby="searchName" value="Fall"></td>
                                <td><input type="searchName" class="border-0" id="searchName1" aria-describedby="searchName" value="A"></td>
                                <td><input type="searchName" class="border-0" id="searchName1" aria-describedby="searchName" value=""></td>
                            </tr>
                            <tr>
                            <td><input type="searchName" class="border-0" id="searchName1" aria-describedby="searchName" value="DHD 540"></td>
                                <td><input type="searchName" class="border-0" id="searchName1" aria-describedby="searchName" value="2021"></td>
                                <td><input type="searchName" class="border-0" id="searchName1" aria-describedby="searchName" value="Fall"></td>
                                <td><input type="searchName" class="border-0" id="searchName1" aria-describedby="searchName" value="A"></td>
                                <td><input type="searchName" class="border-0" id="searchName1" aria-describedby="searchName" value=""></td>
                            </tr>
                        </tbody>
                        </table> -->
                        <!-- <ul class="pagination mb-0 justify-content-center" style="font-size: small;">
                            <li class="page-item"><a class="page-link text-dark" href="#">Previous</a></li>
                            <li class="page-item"><a class="page-link text-dark" href="#">1</a></li>
                            <li class="page-item"><a class="page-link text-dark" href="#">2</a></li>
                            <li class="page-item"><a class="page-link text-dark" href="#">3</a></li>
                            <li class="page-item"><a class="page-link text-dark" href="#">Next</a></li>
                        </ul> -->--%>
                    </div>
                </div>
            </div>
        </div>
    </div>
<%--        <asp:Button ID="EditCourse" runat="server" Text="Search" OnClick="Checker" Style="margin-bottom: 10px" />
            <br />--%>

    </asp:Content>

    <asp:Content ID="Content3" ContentPlaceHolderID="EvaluationContent" runat="server" EnableViewState="true">
    </asp:Content>
