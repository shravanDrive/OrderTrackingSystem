<%@ Page Title="Batching" Language="C#" MasterPageFile="~/ATCP.Master"
    CodeBehind="ATCPHome.aspx.cs" Inherits="ATCPClient.ATCPHome" MaintainScrollPositionOnPostback="true" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <style>
        [class*="dataTables_"] {
            font-size: small;
        }

        tfoot input {
            width: 100%;
            padding: 3px;
            box-sizing: border-box;
        }


        th,
        td {
            padding: 10px;
            border: 1px solid black;
            border-collapse: collapse;
        }

        tfoot {
            display: table-row-group;
        }
    </style>
    <script>
        $(document).ready(function () {


            //Setup - add a text input to each footer cell OLD
            $('#table_id tfoot th').each(function () {
                var title = $('#table_id tfoot th').eq($(this).index()).text();
                // var title = $(this).text();
                if (title != 'Edit Grades') {
                    $(this).html('<input type="text" class="my-1" placeholder="Search ' + title + '" />');
                }
                else {
                    $(this).html('<input type="text" class="my-1" disabled placeholder="Search ' + title + '" />');
                }
            });

            $('#table_id').DataTable(
            {
                // "scrollY": "300px",
                // "scrollCollapse": true,
                // "fixedheader":true,
                "bDestroy": true,
                columnDefs:
                [
                    {
                        targets: [0],
                        render: function (data) {
                            return '<a href="' + "ATCPViewStudent.aspx?UIN=" + data + '" target_blank>' + data + '</a>'
                        }
                    },
                    {
                        targets: [7],
                        render: function (data, type, row, meta) {
                            return '<a class="fa fa-edit fa-lg" href="' + "ATCPStudentmarks.aspx?UIN=" + row['UIN'] + '" target_blank></a>'
                        }
                    }
                ],
                "ajax": {
                    url: "ATCPHome.aspx/GetBaseData",
                    //url: "@Url.Action("GetBaseData","ATCPHome")",
                    //dataSrc: '',
                    dataSrc: function (data) {
                        return $.parseJSON(data.d)
                    },
                    dataType: 'json',
                    contentType: "application/json; charset=utf-8",
                    method: "post"
                    //data:
                    //{
                    //    "UIN": UIN,
                    //    "Lname": Lname,
                    //    "FName": FName,
                    //    "EntryYear" : EntryYear,
                    //    "EntryTerm" : EntryTerm,
                    //    "ProgramStatus" : ProgramStatus,
                    //    "Advisor" : Advisor
                    //},
                    //success: function(result) { //we got the response
                    ////    alert('Successfully called');
                    //    console.log(result);
                    //}
                    //error: function(jqxhr, status, exception) {
                    //alert('Exception:', exception);
                    //console.log(exception);
                    //console.log(jqxhr);
                    //console.log(status);
                    //console.warn(jqxhr.responseText) // very important
                    //console.log(jqxhr.responseJSON);
                    //}  
                },
                "columns": [
                                { "data": "UIN" },
                                { "data": "Lname" },
                                { "data": "FName" },
                                { "data": "EntryYear" },
                                { "data": "EntryTerm" },
                                { "data": "ProgramStatus" },
                                { "data": "Advisor" },
                                {
                                    "className": "dt-center editor-edit",//'dt-control',
                                    "orderable": false,
                                    "data": null,
                                    "defaultContent": '<i class="fa fa-pencil"/>' //''
                                },
                ],
                initComplete: function () {
                    // console.log("start");
                    //className: "dt-center editor-edit",
                    defaultContent: '<i class="fa fa-pencil"/>',
                    // var inputFilterOnColumn = [2,3];
                    // var showFilterBox = 'afterHeading'; //beforeHeading, afterHeading
                    // $('.gtp-dt-filter-row').remove();
                    // var theadSecondRow = '<tr class="gtp-dt-filter-row">';
                    // $(this).find('thead tr th').each(function(index){
                    //     console.log("Second Row" + index);
                    // 	theadSecondRow += '<td class="gtp-dt-select-filter-' + index + '"></td>';
                    // });
                    // theadSecondRow += '</tr>';

                    // if(showFilterBox === 'beforeHeading'){
                    // 	$(this).find('thead').prepend(theadSecondRow);
                    // }else if(showFilterBox === 'afterHeading'){
                    //     console.log("AfterHeading");
                    // 	$(theadSecondRow).insertAfter($(this).find('thead tr'));
                    // }

                    // this.api().columns().every(function (index) {
                    // 	var column = this;
                    //     console.log("Index Val" + index);
                    // 	if(inputFilterOnColumn.indexOf(index) >= 0){
                    //         console.log("Input Filter Column" + inputFilterOnColumn.indexOf(index));
                    // 		$('td.gtp-dt-select-filter-' + index).html('<input type="text" class="gtp-dt-input-filter">');
                    //         $('td input.gtp-dt-input-filter').on('keyup change clear', function () {
                    //             console.log("Inside Key up function");
                    //             console.log("Object Column" + column);
                    //             console.log("Column Search" + column.search());
                    //             console.log("Value" + this.value);
                    //             if (column.search() !== this.value) {
                    //                 console.log("Inside main if and value of this.value" + this.value);
                    //                 column
                    //                     .search(this.value)
                    //                     .draw();
                    //             }
                    //         });
                    // 	}
                    // });

                    this.api().columns().every(function () {
                        var that = this;
                        $('input', this.footer()).on('keyup change clear', function () {
                            if (that.search() !== this.value) {
                                that
                                .search(this.value)
                                .draw();
                            }
                        });
                    });

                    // Apply the search OLD
                    // this.api().columns().every(function () 
                    // {
                    //     var that = this;
                    //     $( 'input', this.footer()).on( 'keyup change clear', function () 
                    //     {
                    //         if (that.search() !== this.value) 
                    //         {
                    //             that
                    //             .search(this.value)
                    //             .draw();
                    //         }
                    //     });
                    // });
                }
            });

            $('#table_id_filter').hide();
            $('tfoot').each(function () {
                $(this).insertAfter($(this).siblings('thead'));
            });
        });


    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server" EnableViewState="true">
    <div class="row my-3"></div>
    <table id="table_id" class=" table display" style="width: 100%; font-size: small;">
        <thead class="thead-dark">
            <tr>
                <th>UIN</th>
                <th>Last Name</th>
                <th>First Name</th>
                <th>Entry Year</th>
                <th>Entry Term</th>
                <th>Program Status</th>
                <th>Advisor</th>
                <th>Edit Grades</th>
            </tr>
        </thead>

        <%--<tbody>--%>
        <!-- <tr class="gtp-dt-filter-row">
                    <td><input type="text" placeholder="Search UIN" class=" my-2 UIN"></td>
                    <td><input type="text" class="my-2 gtp-dt-input-filter"></td>
                    <td><input type="text" class="my-2 gtp-dt-input-filter"></td>
                    <td><input type="text" class="my-2 gtp-dt-input-filter"></td>
                    <td><input type="text" class="my-2 gtp-dt-input-filter"></td>
                    <td><input type="text" class="my-2 gtp-dt-input-filter"></td>
                    <td><input type="text" class="my-2 gtp-dt-input-filter"></td>
                </tr> -->
        <!-- Getting Data From DB-->
        <%--<?php 

                    include('dbConnection.php');
                    $query = "SELECT * FROM studenthomepage"; 

                    $result = $conn->query($query);

                    // Fetches All records
                    $rows = $result->fetch_all(MYSQLI_ASSOC);
                    foreach ($rows as $row) 
                    {
                        echo "
                                <tr>
                                    <td>".$row["UIN"]."</td>
                                    <td>".$row["Lname"]."</td>
                                    <td>".$row["FName"]."</td>
                                    <td>".$row["EntryYear"]."</td>
                                    <td>".$row["EntryTerm"]."</td>
                                    <td>".$row["ProgramStatus"]."</td>
                                    <td>".$row["Advisor"]."</td>
                                </tr>
                             ";
                    }
        
                    // releasing memory
                    mysqli_free_result($result);

                ?>--%>
        <%--</tbody>--%>
        <tfoot style="display: table-row-group;">
            <tr>
                <th>UIN</th>
                <th>Last Name</th>
                <th>First Name</th>
                <th>Entry Year</th>
                <th>Entry Term</th>
                <th>Program Status</th>
                <th>Advisor</th>
                <th>Edit Grades</th>
            </tr>
        </tfoot>
    </table>


    <!-- DataTable Code End -->
    <!-- Content Page End -->

    <!-- Footer -->
    <%--<?php include('footer.php') ?>--%>

    <!-- JavaScript Extensions -->
    <%--<?php include('./Helpers/jsExtensions.php') ?>--%>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="EvaluationContent" runat="server" EnableViewState="true">
</asp:Content>
