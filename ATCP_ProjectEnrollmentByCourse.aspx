<%@ Page Title="Batching" Language="C#"   MasterPageFile="~/ATCP.Master"
    CodeBehind="ATCP_ProjectEnrollmentByCourse.aspx.cs" Inherits="ATCPClient.ATCP_ProjectEnrollmentByCourse" MaintainScrollPositionOnPostback = "true" %>

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
            td 
            {
                padding: 10px;
                border: 1px solid black;
                border-collapse: collapse;
            }

            tfoot 
            {
                display: table-row-group;
            }

            .card
            {
                border: 1px solid black;
            }

        </style>

        <script>
            var usersTable;
            var courseNumber;
            var termsOffered;
            var studentDetailsTable;
            function format (row) 
            {
                courseNumber = row.data().CourseNumber;
                termsOffered = row.data().TermsOffered;
                if(studentDetailsTable != null){
                    studentDetailsTable.destroy();
                }
                
                var inputChildtable = $(
                    //  '<div class="card border-primary">' + 
                    //     '<div class="card-header font-weight-bold">' +
                    //         'Enrollment By Course' + 
                    //     '</div>' +
                        '<table id="childTable" class=" table display table-bordered" style="width:100%; font-size: small;border: 2px solid black;">' +
                            '<thead>' +
                                '<tr class="table-success">' + 
                                    '<th>Year</th>' +
                                    '<th>Term</th>' +
                                    '<th>Enrollment Projection</th>' +
                                '</tr>' +
                            '</thead>' +
                        '</table>');
                    // '</div>');

                row.child(inputChildtable).show();
                usersTable = inputChildtable.DataTable
                (
                    { 
                        "ajax":
                        {
                            url: "ATCP_ProjectEnrollmentByCourse.aspx/GetCourseEnrollment?courseNumber=" + courseNumber + "&TermsOffered=" + termsOffered,
                            //dataSrc : '',
                            dataSrc: function (data) {
                                return $.parseJSON(data.d)
                            },
                            dataType: 'json',
                            contentType: "application/json; charset=utf-8",
                            method: "post"
                            //data: '{courseNumber:"' + courseNumber + '"}',
                            //data: '{email:"' + forgotEmail + '"}',
                            //data: '{"courseNumber": "DHD 440"}',
                            //      UTPILIZED TO SEE THE ERROR
                                  //,buildsearchdata(row.data().coursenumber), //{ "coursenumber":row.data().coursenumber},
                                  //success: function(result) { //we got the response
                                  //alert('successfully called');
                                  //},
                                  //error: function(jqxhr, status, exception) {
                                  //alert('exception:', exception);
                                  //console.log(exception);
                                  //console.log(jqxhr);
                                  //console.log(status);
                                  //console.warn(jqxhr.responsetext) // very important
                                  //// console.log(jqxhr.responsejson);
                                  //} 
                        },
                        "columns":
                        [
                            {"data":"Year"},
                            {"data":"Term"},
                            {"data":"Enrollment_Projection"}
                        ]
                    }
                );

            }

            $(document).ready(function () 
            { 
                var table = $('#table_id').DataTable
                ({ 
                    "ajax":
                    {
                        url: "ATCP_ProjectEnrollmentByCourse.aspx/GetAllCourses",
                        //dataSrc : '',
                        dataSrc: function (data) {
                            return $.parseJSON(data.d)
                        },
                        dataType: 'json',
                        contentType: "application/json; charset=utf-8",
                        method: "post"
                    },
                    "columns":
                    [
                        {
                            "className": 'dt-control',
                            "orderable": false,
                            "data":  null,
                            "defaultContent": ''
                        },
                        {"data":"CourseNumber"},
                        { "data": "CourseName" },
                        { "data": "TermsOffered" }
                    ],
                    "order": [[1, 'asc']]
                });

                // Add event listener for opening and closing details
                $('#table_id tbody').on('click', 'td.dt-control', function () 
                {

                    var tr = $(this).closest('tr');
                    var row = table.row( tr );

                    if ( row.child.isShown()) 
                    {
                        // This row is already open - close it
                        row.child.hide();
                        tr.removeClass('shown');
                    }
                    else 
                    {
                        // Open this row
                        row.child(format(row)).show();
                        tr.addClass('shown');
                    }
                });

                ///******** PER STUDENT ENROLLMENT VIEW *********////

                $('#table_id').on('click','#childTable tr', function() 
                {
                    var row = usersTable.row(this);
                    console.log(row);
                    var base = $(this).closest('tr');
                    console.log(base);
                    var tr = $(this).closest('tr').parents('tr');
                    console.log(tr);
                    var prevtr = tr.prev('tr')[0];
                    console.log(prevtr);
                    var data = table.row(prevtr).data();
                    console.log(data);
                    //console.log(data.CourseNumber); // course number perfect
                    var valuerow = table.row(base);
                    console.log(valuerow.data());
                    //var prevtr = tr.prev('tr')[0];

                    
                    //alert(courseNumber);
                    //if (studentDetailsTable != null) {
                    //    studentDetailsTable.destroy();
                    $('#studentDetails').DataTable().destroy();
                    
                    //}
                    //******  ADDING DATA TO HEADER ********//
                    var paragraph = document.getElementById('perStudentData');
                    paragraph.textContent = "Per Term Student Enrollment - { Course Number - " + courseNumber + ", Term - " + row.data().Term + "}";

                    studentDetailsTable = $('#studentDetails').DataTable
                    ({
                        "ajax":
                        {
                            url: "ATCP_ProjectEnrollmentByCourse.aspx/GetIndividualStudentDetails?courseNumber=" + courseNumber + "&Term=" + row.data().Term + "&Year=" + row.data().Year,
                            //dataSrc : '',
                            dataSrc: function (data) {
                                return $.parseJSON(data.d)
                            },
                            dataType: 'json',
                            contentType: "application/json; charset=utf-8",
                            method: "post"
                            //data: 
                            //{ 
                            //    "courseNumber": courseNumber,
                            //    "Term": row.data().Term,
                            //    "Year": row.data().Year
                            //},
                            // success: function(result) { //we got the response
                            //     alert('Successfully called');
                            // },
                            // error: function(jqxhr, status, exception) {
                            //     //alert('Exception:', exception);
                            // console.log(exception);
                            // console.log(jqxhr);
                            // console.log(status);
                            // console.warn(jqxhr.responseText) // very important
                            // console.log(jqxhr.responseJSON);
                            // }  
                        },
                        "columns":
                        [
                            {"data":"Year"},
                            {"data":"Term"},
                            {"data":"UIN"},
                            {"data":"FirstName"},
                            {"data":"LastName"}
                        ]
                    });
                    
                    //console.log(row.data());                   
                    //console.log(courseNumber);
                    $("html, body").animate({ scrollTop: $(document).height() }, "slow");
                
                });
            });

        </script>
    </asp:Content>

    <asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server" EnableViewState="true">
        <div class="card"> 
            <div class="card-header font-weight-bold">
                Enrollment By Course
            </div>
            <div class="row my-2"></div>
            <table id="table_id" class=" table display" style="width:100%; font-size: small;">
                <thead class="thead-dark">
                    <tr>
                        <th></th>
                        <th>Course Number</th>
                        <th>Course Name</th>
                        <th>TermsOffered</th>
                    </tr>
                </thead>
            </table>
        </div>
        <div class="row my-3"></div>
        <div class="card">
            <div class="card-header font-weight-bold" id="perStudentData">
                Per Term Student Enrollment
            </div>
            <div class="row my-2"></div>
            <table id="studentDetails" class=" table display" style="width:100%; font-size: small;">
                <thead>
                    <tr class="table-warning">
                        <th>Year</th>
                        <th>Term</th>
                        <th>UIN</th>
                        <th>First Name</th>
                        <th>Last Name</th>
                    </tr>
                </thead>
            </table>
        </div>
         <%--<asp:Button ID="CheckerID" runat="server" Text="Search" OnClick="Checker" Style="margin-bottom: 10px" />--%>
    </asp:Content>

    <asp:Content ID="Content3" ContentPlaceHolderID="EvaluationContent" runat="server" EnableViewState="true">
    </asp:Content>