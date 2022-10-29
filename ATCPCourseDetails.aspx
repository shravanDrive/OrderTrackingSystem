<%@ Page Title="Batching" Language="C#"   MasterPageFile="~/ATCP.Master"
    CodeBehind="ATCPCourseDetails.aspx.cs" Inherits="ATCPClient.ATCPCourseDetails" MaintainScrollPositionOnPostback = "true" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
   <style>
        [class*="dataTables_"]
        {
            font-size: small;
        }

        tfoot input 
        {
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

        tfoot 
        {
            display: table-row-group;
        }

    </style>

    <script>

        $(document).ready(function ()
        {
        
            //Setup - add a text input to each footer cell OLD
            $('#table_id tfoot th').each(function ()
            {
                var title = $('#table_id tfoot th').eq($(this).index()).text();
                // var title = $(this).text();
                if (title != 'Edit Course Details')
                {
                    $(this).html('<input type="text" class="my-1" placeholder="Search ' + title + '" />');
                }
                else
                {
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
                        targets: [3],
                        render: function (data, type, row, meta) {
                            
                            return '<div class="row"><div class="col-4"></div><div class="col-2"><a class="fa fa-edit fa-lg" href="' + "ATCPCourseInfo.aspx?CourseNumber=" + row['CourseNumber'] + "&Term=" + row['TermsOffered'] + '" target_blank></a></div><div class="col-2"><a class="fa fa-trash fa-lg" onclick="return confirm(\'Are you sure you want to delete the course?\')" href="' + "ATCPCourseDetails.aspx?DeleteOperaton=True&CourseNumber=" + row['CourseNumber'] + "&Term=" + row['TermsOffered'] + '"></a></div></div>';
                            //return '<a class="fa fa-edit fa-lg" href="' + "ATCPCourseInfo.aspx?CourseNumber=" + row['CourseNumber'] + "&Term=" + row['TermsOffered'] + '" target_blank></a>'
                        }
                    }
                ],

                "ajax":
                {
                    url: "ATCPCourseDetails.aspx/GetAllCourses",
                    //url: "@Url.Action("GetBaseData","ATCPHome")",
                    //dataSrc: '',
                    dataSrc: function (data){
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
                "columns":
                [
                    { "data": "CourseNumber" },
                    { "data": "CourseName" },  
                    { "data": "TermsOffered" },
                    {
                        "className": "dt-center editor-edit",//'dt-control',
                        "orderable": false,
                        "data": null,
                        "defaultContent": '<i class="fa fa-pencil"/>' //''
                    },
                ],
                initComplete: function () 
                {
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

                    this.api().columns().every(function () 
                    {
                        var that = this;
                        $( 'input', this.footer()).on( 'keyup change clear', function () 
                        {
                            if (that.search() !== this.value) 
                            {
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

        function ShowMessage(message, messagetype) {

            //alert("Hello");
            var cssclass;
            switch (messagetype) {
                case 'Success':
                    cssclass = 'alert-success'
                    break;
                case 'Error':
                    cssclass = 'alert-danger'
                    break;
                case 'Warning':
                    cssclass = 'alert-warning'
                    break;
                default:
                    cssclass = 'alert-info'
            }
            //alert(cssclass);
            $('#alert_container').append('<div id="alert_div" style="margin: 0 0.5%; -webkit-box-shadow: 3px 4px 6px #999;height:100%;" class="' + cssclass + '"><a href="#" class="close" data-dismiss="alert" aria-label="close">&times;</a><div style="padding-top:4px"><strong style="font-family:calibri;">&nbsp' + messagetype + '!</strong> <span style="font-family:calibri;">' + message + '</span></div></div>');

            setTimeout(function () {
                $('#alert_container').fadeOut('slow');
            }, 2000);
        }

    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server" EnableViewState="true">
     <div class="row my-3"></div>
        <table id="table_id" class=" table display" style="width:100%; font-size: small;">
            <thead class="thead-dark">
                <tr>
                    <th>Course Number</th>
                    <th>Course Name</th>
                    <th>Term</th>
                    <th>Edit Course Details</th>
                </tr>
            </thead>

            <tfoot style="display: table-row-group;">
                <tr>
                    <th>Course Number</th>
                    <th>Course Name</th>
                    <th>Term</th>
                    <th>Edit/Delete Course Details</th>
                </tr>
            </tfoot>
        </table>      
        <div class="row my-3">
            <div class="col-3"><asp:LinkButton runat="server" id="SomeLinkButton" style="font-family:Calibri;width:200px;height:41px;margin-left:10px" href="ATCPAddCourse.aspx" CssClass="btn btn-dark">Add Course</asp:LinkButton></div>
            <div class="col-5"></div>
            <div class="col-4"><div  id="alert_container" style="height:100%;"></div></div>           
        </div>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="EvaluationContent" runat="server" EnableViewState="true">
</asp:Content>
