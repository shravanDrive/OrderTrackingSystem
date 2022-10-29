<%@ Page Title="Batching" Language="C#" MasterPageFile="~/ATCP.Master"
    CodeBehind="ATCPStudent.aspx.cs" Inherits="ATCPClient.ATCPStudent" MaintainScrollPositionOnPostback="true" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">

    <style>
        .loader {
            position: fixed;
            top: 0;
            left: 0;
            width: 100vw;
            height: 100vh;
            display: flex;
            align-items: center;
            justify-content: center;
            /* background: #333333; */
            transition: opacity 0.75s, visibility 0.75s;
        }

        .loader--hidden {
            opacity: 0;
            visibility: hidden;
        }

        .loader::after {
            content: "";
            width: 75px;
            height: 75px;
            border: 15px solid #dddddd;
            border-top-color: #009578;
            border-radius: 50%;
            animation: loading 0.75s ease infinite;
        }

        @keyframes loading {
            from {
                transform: rotate(0turn);
            }

            to {
                transform: rotate(1turn);
            }
        }

        .datepicker {
            font-size: 0.875em;
        }
            /* solution 2: the original datepicker use 20px so replace with the following:*/

            .datepicker td, .datepicker th {
                width: 1.5em;
                height: 1.5em;
            }

        [class*="dataTables_"] {
            font-size: small;
        }

        input-group {
            font-size: small;
        }

        tfoot input {
            width: 100%;
            padding: 3px;
            box-sizing: border-box;
        }

        th,
        td {
            //padding: 10px;
            border: 1px solid black;
            border-collapse: collapse;
        }

        tfoot {
            display: table-row-group;
        }
    </style>

    <script>

        function ShowMessage(message, messagetype) {

            //if (messagetype == 'EmptyRow')
            //{
            //    alert("Hello");
            //    $('table tr').filter((i, e) => e.textContent.trim().length == 0).remove();
            //}
            //else
            //{

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

            $('#alert_container').append('<div id="alert_div" style="margin: 3px 0.5%; -webkit-box-shadow: 3px 4px 6px #999;height:100%;" class="' + cssclass + '"><a href="#" class="close" data-dismiss="alert" aria-label="close">&times;</a><div style="padding-top:4px"><strong style="font-family:calibri;">&nbsp' + messagetype + '!</strong> <span style="font-family:calibri;">' + message + '</span></div></div>');

            setTimeout(function () {
                $('#alert_container').fadeOut('slow');
            }, 2000);
        }

        $(document).ready(function () {

            $('#datepicker').datepicker({
                weekStart: 1,
                daysOfWeekHighlighted: "6,0",
                autoclose: true,
                todayHighlight: true,
            });

            //$('#datepicker').datepicker("setDate", new Date(2008, 9, 03));
            $('#datepicker').datepicker("setDate", '<%=GetNewDate()%>');

            $('table tr').filter((i, e) => e.textContent.trim().length == 0).remove();

            //PopulateDatePicker(NewDate);
            //$('table tr').filter((i, e) => e.textContent.trim().length == 0).remove();
        });



        //function RemoveEmptyRows() {

        //    //alert("Hello");
        //    $('table tr').filter((i, e) => e.textContent.trim().length == 0).remove();
        //}

        function confirmCompleted() {
            // confirm if the user wants to deliver the product
            return (confirm("Are you sure you want to mark the course as completed?"));

        };

        function PopulateDatePicker(Intent_To_Completion) {
            $('#datepicker').datepicker({
                weekStart: 1,
                daysOfWeekHighlighted: "6,0",
                autoclose: true,
                todayHighlight: true,
            });

            //$('#datepicker').datepicker("setDate", new Date(2008, 9, 03));
            $('#datepicker').datepicker("setDate", Intent_To_Completion);

            $('table tr').filter((i, e) => e.textContent.trim().length == 0).remove();
        }


    </script>

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server" EnableViewState="true">
    <!-- Content Page Start -->
    <div class="mx-3" id="background">
        <!-- container ml-5 mr-5 -->
        <%--        <div class="row">
<%--            <div class="col-5">
                <div class="card" style="font-size: small">
                    <div class="card-header"><strong>SINGLE STUDENT VIEW</strong></div>
                </div>
            </div>
            <div class="col-1">
            </div>
            <div class="col-6">
                <div class="mb-0 mt-1 pt-2 pb-2 collapse" id="myAlert" style="font-size: small;">
                    <a href="#" class="close" data-dismiss="alert">&times;</a>
                    <strong class="BoldedData"></strong>
                </div>
            </div>
        </div>--%>
        <div class="row my-3">
            <div class="col-6">
                <div class="row">
                    <div class="col-6">
                        <asp:Button ID="ViewMode" runat="server" Text="Click for View Mode" class="btn btn-dark" Style="width: 100%; font-size: small" OnClick="ViewMode_Click" />
                    </div>
                    <div class="col-6">
                        <asp:Button ID="UpdateID" runat="server" Text="Update" class="btn btn-dark" Style="width: 100%; font-size: small" OnClick="UpdateID_Click" />
                    </div>
                    <%--<div class="input-group col-9">
                        <span class="input-group-text" style="font-family: Calibri, FontAwesome; font-size: small">&#xF002; Search by UIN</span>
                        <asp:TextBox ID="searchTextBox" runat="server" style="font-size: small" class="form-control form-control-sm" aria-describedby="firstname"></asp:TextBox>
                    </div>
                    <div class="col-3">
                        <asp:Button ID="Student_SearchButton" runat="server" Text="Search" OnClick="Search_Click_Student" class="btn btn-dark" Style="width: 100%; font-size: small" />
                    </div>--%>
                </div>
            </div>
            <%--<div class=""></div>--%>
            <div class="col-6">
                <div class="row">
                    <div class="input-group col-9">
                        <div  id="alert_container" style="height:100%;width:100%"></div>
                        <%--<span class="input-group-text" style="font-family: Calibri, FontAwesome; font-size: small">&#xF002; Faculty Advisor</span>--%>
                        <%--<asp:TextBox ID="facultyAdvisor" runat="server" style="font-size: small" class="form-control form-control-sm" aria-describedby="firstname"></asp:TextBox>--%>
                        <%--<asp:DropDownList ID="facultyAdvisor" Style="font-family: Calibri, FontAwesome; font-size: small" runat="server" Visible="true" OnSelectedIndexChanged="facultyAdvisor_SelectedIndexChanged" AutoPostBack="True" AppendDataBoundItems="true" EnableViewState="true"></asp:DropDownList>--%>
                    </div>
                    <div class="col-3">
<%--                        <nav aria-label="Page navigation example">
                            <ul class="pagination">
                                <li class="page-item">
                                    <asp:Button ID="Button1" runat="server" Text="<<" class="btn btn-dark" Style="width: 100%; font-size: small" OnClick="Search_Click_Advisor" /></li>
                                <li class="page-item">
                                    <asp:Button ID="Button2" runat="server" Text="<" class="btn btn-dark" Style="width: 100%; font-size: small" OnClick="Search_Click_Advisor" /></li>
                                <li class="page-item">
                                    <asp:Button ID="Button3" runat="server" Text=">" class="btn btn-dark" Style="width: 100%; font-size: small" OnClick="Search_Click_Advisor" /></li>
                                <li class="page-item">
                                    <asp:Button ID="Button4" runat="server" Text=">>" class="btn btn-dark" Style="width: 100%; font-size: small" OnClick="Search_Click_Advisor" /></li>
                            </ul>
                        </nav>--%>
                    </div>
                </div>
            </div>
        </div>
        <div class="row my-3">
            <div class="col-6">
                <div class="card" style="font-size: small">
                    <div class="card-header"><strong>Primary Details</strong></div>
                    <div class="card-body">
                        <div class="row">
                            <div class="input-group col-12">
                                <span class="input-group-text" style="font-family: Calibri; font-size: small;width:16%">UIN</span>
                                <asp:TextBox ID="UINTextBox" ReadOnly="true" runat="server" Style="font-size: small" class="form-control form-control-sm" aria-describedby="firstname"></asp:TextBox>
                            </div>
                        </div>
                        <div class="row my-3">
                            <div class="input-group col-12">
                                <span class="input-group-text" style="font-family: Calibri; font-size: small">First Name</span>
                                <asp:TextBox ID="firstNameTextBox" runat="server" Style="font-size: small" class="form-control form-control-sm" aria-describedby="firstname"></asp:TextBox>
                            </div>
                        </div>
                        <div class="row my-3">
                            <div class="input-group col-12">
                                <span class="input-group-text" style="font-family: Calibri; font-size: small;width:16%">Last Name</span>
                                <asp:TextBox ID="lastNameTextBox" runat="server" Style="font-size: small" class="form-control form-control-sm" aria-describedby="lastName"></asp:TextBox>
                            </div>
                        </div>
                        <div class="row my-3">
                            <div class="input-group col-6">
                                <span class="input-group-text" style="font-family: Calibri; font-size: small;width:34%">Gender</span>
                                <asp:DropDownList ID="genderDropDown" runat="server" style="width:66%" Visible="true" OnSelectedIndexChanged="ATArea_SelectionChanged" AutoPostBack="True" AppendDataBoundItems="true" EnableViewState="true"></asp:DropDownList>
                            </div>
                            <div class="input-group col-6">
                                <!-- <span class="input-group-text" style="font-family:Calibri;font-size:small" id="basic-addon1">Hispanic</span> -->
                                <!-- <input type="text" id="isHispanic" style="font-size:small" class="form-control form-control-sm" id="firstname1" aria-describedby="firstname"> -->
                                <div class="input-group-prepend">
                                    <div class="input-group-text">
                                        <asp:CheckBox ID="isHispanic" runat="server" Style="font-size: small" aria-label="Checkbox for following text input" />
                                    </div>
                                </div>
                                <input type="text" class="form-control" style="font-size: small" disabled aria-label="Text input with checkbox" placeholder="Hispanic">
                            </div>
                        </div>
                        <div class="row my-3">
                            <div class="input-group col-12">
                                <span class="input-group-text" style="font-family: Calibri; font-size: small;width:16%" id="basic-addon1">Race</span>
                                <!-- <input type="text" id="raceTextBox" style="font-size:small" class="form-control form-control-sm" id="firstname1" aria-describedby="firstname"> -->
                                <asp:DropDownList ID="raceDropDown" runat="server" style="width:84%" Visible="true" OnSelectedIndexChanged="ATArea_SelectionChanged" AutoPostBack="True" AppendDataBoundItems="true" EnableViewState="true"></asp:DropDownList>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="card my-3" style="font-size: small;">
                    <div class="card-header"><strong>Academic Background</strong></div>
                    <div class="card-body">
                        <div class="row my-3">
                            <div class="input-group col-12">
                                <span class="input-group-text" style="font-family: Calibri; font-size: small">Bachelors Institution</span>
                                <asp:TextBox ID="bInstitution" runat="server" Style="font-size: small" class="form-control form-control-sm" aria-describedby="lastName"></asp:TextBox>
                            </div>
                        </div>
                        <div class="row my-3">
                            <div class="input-group col-12">
                                <span class="input-group-text" style="font-family: Calibri; font-size: small;width:26%">Bachelors Major</span>
                                <asp:TextBox ID="bMajor" runat="server" Style="font-size: small" class="form-control form-control-sm" aria-describedby="lastName"></asp:TextBox>
                            </div>
                        </div>
                        <div class="row my-3">
                            <div class="input-group col-12">
                                <span class="input-group-text" style="font-family: Calibri; font-size: small;width:26%">Masters Institution</span>
                                <asp:TextBox ID="MInstitution" runat="server" Style="font-size: small" class="form-control form-control-sm" aria-describedby="lastName"></asp:TextBox>
                            </div>
                        </div>
                        <div class="row my-3">
                            <div class="input-group col-12">
                                <span class="input-group-text" style="font-family: Calibri; font-size: small;width:26%">Masters Major</span>
                                <asp:TextBox ID="MMajor" runat="server" Style="font-size: small" class="form-control form-control-sm" aria-describedby="lastName"></asp:TextBox>
                            </div>
                        </div>
                        <div class="row my-3">
                            <div class="input-group col-12">
                                <span class="input-group-text" style="font-family: Calibri; font-size: small;width:26%">Doctoral Institution</span>
                                <asp:TextBox ID="DInstitution" runat="server" Style="font-size: small" class="form-control form-control-sm" aria-describedby="lastName"></asp:TextBox>
                            </div>
                        </div>
                        <div class="row my-3">
                            <div class="input-group col-12">
                                <span class="input-group-text" style="font-family: Calibri; font-size: small;width:26%">Doctoral Major</span>
                                <asp:TextBox ID="DMajor" runat="server" Style="font-size: small" class="form-control form-control-sm" aria-describedby="lastName"></asp:TextBox>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="card my-3" style="font-size: small;">
                    <div class="card-header"><strong>Current Employer</strong></div>
                    <div class="card-body" style="margin-bottom:6%">
                        <div class="row my-3">
                            <div class="input-group col-12">
                                <span class="input-group-text" style="font-family: Calibri; font-size: small;width:26%">Current Employer</span>
                                <asp:TextBox ID="CEmployer" runat="server" Style="font-size: small" class="form-control form-control-sm" aria-describedby="lastName"></asp:TextBox>
                            </div>
                        </div>
                        <div class="row my-3">
                            <div class="input-group col-12">
                                <span class="input-group-text" style="font-family: Calibri; font-size: small;width:26%">Current Job Title</span>
                                <asp:TextBox ID="CTitle" runat="server" Style="font-size: small" class="form-control form-control-sm" aria-describedby="lastName"></asp:TextBox>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <%--<div class="col-1">
            </div>--%>
            <div class="col-6">
                <div class="row">
                    <div class="col-12">
                        <div class="card" style="font-size: small">
                            <div class="card-header"><strong>Additional Details</strong></div>
                            <div class="card-body">
                                <div class="row">
                                    <div class="input-group col-12">
                                        <span class="input-group-text" style="font-family: Calibri; font-size: small;width:22%">Faculty Advisor</span>
                                        <asp:TextBox ID="facultyAdvisorText" ReadOnly="true" runat="server" Style="font-size: small" class="form-control form-control-sm" aria-describedby="lastName"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="row my-3">
                                    <div class="input-group col-6">
                                        <span class="input-group-text" style="font-family: Calibri; font-size: small">Professional Category</span>
                                        <asp:DropDownList ID="proCategoryDropdown" Style="font-family: Calibri; font-size: small;" runat="server" Visible="true" OnSelectedIndexChanged="ATArea_SelectionChanged" AutoPostBack="True" AppendDataBoundItems="true" EnableViewState="true"></asp:DropDownList>
                                    </div> <%--width:52%--%>
                                    <div class="input-group col-6">
                                        <span class="input-group-text" style="font-family: Calibri; font-size: small;width:35%">Entry Year</span>
                                        <asp:TextBox ID="entryYear" runat="server" Style="font-size: small" class="form-control form-control-sm" aria-describedby="lastName"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="row my-3">
                                    <div class="input-group col-6">
                                        <span class="input-group-text" style="font-family: Calibri; font-size: small">Area of Focus</span>
                                        <%--<asp:TextBox ID="concentrationArea" runat="server" Style="font-size: small" class="form-control form-control-sm" aria-describedby="lastName"></asp:TextBox>--%>
                                        <asp:DropDownList ID="concentrationID" runat="server" Style="font-family: Calibri; font-size: small;width:52%" Visible="true" OnSelectedIndexChanged="concentrationArea_SelectedIndexChanged" AutoPostBack="True" AppendDataBoundItems="true" EnableViewState="true"></asp:DropDownList>
                                    </div>
                                    <div class="input-group col-6">
                                        <div class="input-group">
                                            <span class="input-group-text" style="font-family: Calibri; font-size: small">Program Status</span>
                                            <asp:DropDownList ID="programStatus" runat="server" Style="font-family: Calibri; font-size: small;width:50%" Visible="true" OnSelectedIndexChanged="ATArea_SelectionChanged" AutoPostBack="True" AppendDataBoundItems="true" EnableViewState="true"></asp:DropDownList>
                                        </div>
                                    </div>
                                </div>
                                <div class="row my-3">
                                    <div class="input-group col-12">
                                        <span class="input-group-text" style="font-family: Calibri; font-size: small;width:22%">UIC Program</span>
                                        <asp:TextBox ID="uicProgram" runat="server" Style="font-size: small" class="form-control form-control-sm" aria-describedby="UIC_Program"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="row my-3">
                                    <div class="input-group col-12">
                                        <span class="input-group-text" style="font-family: Calibri; font-size: small;width:22%">Advisory Notes</span>
                                        <asp:TextBox ID="advisoryNotes" runat="server" Style="font-size: small" class="form-control form-control-sm" aria-label="With textarea"></asp:TextBox>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="row my-3">
                    <div class="col-12">
                        <div class="card" style="font-size: small">
                            <div class="card-header"><strong>Contact Info</strong></div>
                            <div class="card-body">
                                <div class="row">
                                    <div class="input-group col-6">
                                        <span class="input-group-text" style="font-family: Calibri; font-size: small;width:34%">Email</span>
                                        <asp:TextBox ID="uicEmail" runat="server" Style="font-size: small" class="form-control form-control-sm" aria-describedby="UIC_Program"></asp:TextBox>
                                    </div>
                                    <div class="input-group col-6">
                                        <span class="input-group-text" style="font-family: Calibri; font-size: small">Phone</span>
                                        <asp:TextBox ID="phone" runat="server" Style="font-size: small" class="form-control form-control-sm" aria-describedby="Phone"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="row my-3">
                                    <div class="input-group col-12">
                                        <span class="input-group-text" style="font-family: Calibri; font-size: small">Personal Email</span>
                                        <asp:TextBox ID="personalEmail" runat="server" Style="font-size: small" class="form-control form-control-sm" aria-describedby="Phone"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="row my-3">
                                    <div class="input-group col-12">
                                        <span class="input-group-text" style="font-family: Calibri; font-size: small;width:16%">Address</span>
                                        <asp:TextBox ID="address" runat="server" Style="font-size: small" class="form-control form-control-sm" aria-label="With textarea"></asp:TextBox>
                                        <%-- <textarea class="form-control" style="font-size: small" id="address" aria-label="With textarea"></textarea>--%>
                                    </div>
                                </div>
                                <div class="row my-3">
                                    <div class="input-group col-6">
                                        <span class="input-group-text" style="font-family: Calibri; font-size: small; width: 34%">State</span>
                                        <asp:TextBox ID="state" runat="server" Style="font-size: small" class="form-control form-control-sm" aria-describedby="UIC_Program"></asp:TextBox>
                                    </div>
                                    <div class="input-group col-6">
                                        <span class="input-group-text" style="font-family: Calibri; font-size: small">City</span>
                                        <asp:TextBox ID="city" runat="server" Style="font-size: small" class="form-control form-control-sm" aria-describedby="Phone"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="row my-3">
                                    <div class="input-group col-6">
                                        <span class="input-group-text" style="font-family: Calibri; font-size: small; width: 34%">Postal Code</span>
                                        <asp:TextBox ID="postalCode" runat="server" Style="font-size: small" class="form-control form-control-sm" aria-describedby="UIC_Program"></asp:TextBox>
                                    </div>
                                    <div class="input-group col-6">
                                        <span class="input-group-text" style="font-family: Calibri; font-size: small">Country</span>
                                        <asp:TextBox ID="Country" runat="server" Style="font-size: small" class="form-control form-control-sm" aria-describedby="Phone"></asp:TextBox>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="card my-3" style="font-size: small">
                            <div class="card-header"><strong>Graduation Details</strong></div>
                            <div class="card-body">
                                <div class="row my-3">
                                    <div class="input-group col-7">
                                        <span class="input-group-text" style="font-family: Calibri; font-size: small">Actual Completion Term</span>
                                        <asp:TextBox ID="CompletionTerm" runat="server" Style="font-size: small" class="form-control form-control-sm" aria-label="With textarea"></asp:TextBox>
                                    </div>
                                    <div class="input-group col-5">
                                        <span class="input-group-text" style="font-family: Calibri; font-size: small">Actual Completion Year</span>
                                        <asp:TextBox ID="CompletionYear" runat="server" Style="font-size: small" class="form-control form-control-sm" aria-label="With textarea"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="row my-3">
                                    <div class="input-group col-7">
                                        <span class="input-group-text" style="font-family: Calibri; font-size: small;width:50%">Intent to Complete</span>
                                        <%--<asp:calendar id="calendar1" runat="server" onselectionchanged="datechange"></asp:calendar>--%>
                                        <%--<asp:Calendar ID="Calendar1" runat="server" OnSelectionChanged="Calendar1_SelectionChanged"></asp:Calendar>--%>
                                        <%--<asp:TextBox ID="datepicker" runat="server" placeholder="mm/dd/yyyy" Type="Date" ReadOnly = "false"></asp:TextBox>--%>
                                        <input data-date-format="dd/mm/yyyy" style="font-size: small; width: 50%" name="dueDate" id="datepicker">
                                    </div>
                                    <div class="input-group col-5">
                                        <div class="input-group-prepend">
                                            <div class="input-group-text">
                                                <asp:CheckBox ID="approvedCompletion" runat="server" Style="font-size: small" aria-label="Checkbox for following text input" />
                                            </div>
                                        </div>
                                        <input type="text" class="form-control" style="font-family: Calibri;font-size: small" disabled aria-label="Text input with checkbox" placeholder="Approved for Completion by OSA">
                                        <!-- <label class="form-check-label" style="display: flex;align-items: center;" for="flexCheckDefault"></label> -->
                                    </div>
                                </div>
                                <div class="row my-3">
                                    <div class="input-group col-7">
                                        <div class="input-group-prepend">
                                            <div class="input-group-text">
                                                <asp:CheckBox ID="passedATP" runat="server" Style="font-size: small" aria-label="Checkbox for following text input" />
                                            </div>
                                        </div>
                                        <input type="text" class="form-control" style="font-family: Calibri;font-size: small" disabled aria-label="Text input with checkbox" placeholder="Passed ATP">
                                        <!-- <label class="form-check-label" style="display: flex;align-items: center;" for="flexCheckDefault"></label> -->
                                    </div>
                                    <!-- <div class="col-1"></div> -->
                                    <div class="input-group col-5">
                                        <div class="input-group-prepend">
                                            <div class="input-group-text">
                                                <asp:CheckBox ID="attemptedATP" runat="server" Style="font-size: small" aria-label="Checkbox for following text input" />
                                            </div>
                                        </div>
                                        <input type="text" class="form-control" style="font-family: Calibri;font-size: small" disabled aria-label="Text input with checkbox" placeholder="Attempted ATP">
                                        <!-- <label class="form-check-label" style="display: flex;align-items: center;" for="flexCheckDefault"></label> -->
                                    </div>
                                </div>
                                <div class="row my-3">
                                    <div class="input-group col-7">
                                        <span class="input-group-text" style="font-family: Calibri; font-size: small">Total Credits Earned</span>
                                        <asp:TextBox ID="CreditsEarned" ReadOnly="true" runat="server" Style="font-size: small" class="form-control form-control-sm" aria-label="With textarea"></asp:TextBox>
                                    </div>
                                    <div class="input-group col-5">
                                        <span class="input-group-text" style="font-family: Calibri; font-size: small">Total Credits Projected</span>
                                        <asp:TextBox ID="CreditsProjected" ReadOnly="true" runat="server" Style="font-size: small" class="form-control form-control-sm" aria-label="With textarea"></asp:TextBox>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <div class="row my-3">
            <div class="card" style="width: 100%">
                <div class="row">
                    <div class="col-5 pr-0 mr-0">
                    </div>
                    <div class="col-1"></div>
                    <div class="col-6">
                    </div>
                </div>
                <div class="row">
                    <div class="col card mx-4 pr-0 pl-0" style="font-size: small">
                        <div class="card-header"><strong>Projected Courses</strong></div>
                        <div class="card-body">
                            <asp:GridView ID="projectedCourse" runat="server" AutoGeneratedColumns="false" ShowFooter="true" DataKeyNames="CourseNumber"
                                Width="100%"
                                CssClass="gridview"
                                ShowHeaderWhenEmpty="true"
                                OnRowUpdating="projectedCourse_RowUpdating"
                                BackColor="White" BorderColor="#CCCCCC" BorderStyle="None" BorderWidth="1px" CellPadding="3" EnableModelValidation="True" AutoGenerateColumns="False">
                                <%--<FooterStyle BackColor="White" ForeColor="#000066" />--%>
                                <HeaderStyle Font-Names="Calibri" BackColor="#006699" Font-Bold="True" ForeColor="White" />
                                <PagerStyle BackColor="White" ForeColor="#000066" HorizontalAlign="Left" />
                                <RowStyle ForeColor="#000066" />
                                <SelectedRowStyle BackColor="#669999" Font-Bold="True" ForeColor="White" />
                                <%--<SortedAscendingCellStyle BackColor="#F1F1F1">--%>

                                <Columns>
                                    <asp:TemplateField HeaderText="Course">
                                        <ItemTemplate>
                                            <asp:Label Style="font-family: Calibri" ID="CourseNumber" Text='<%# Eval("CourseNumber") %>' runat="server" />
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Credits">
                                        <ItemTemplate>
                                            <asp:Label Style="font-family: Calibri" ID="Credits" Text='<%# Eval("Credits") %>' runat="server" />
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Year_Taken">
                                        <ItemTemplate>
                                            <asp:Label Style="font-family: Calibri" ID="CourseYear" Text='<%# Eval("CourseYear") %>' runat="server" />
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Term_Taken">
                                        <ItemTemplate>
                                            <asp:Label Style="font-family: Calibri" ID="Term" Text='<%# Eval("Term") %>' runat="server" />
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField>
                                        <ItemTemplate>
                                            <asp:Button ID="btnCompleted" Style="font-family: Calibri;width:50%;margin-left:28%" class="btn btn-dark form-control my-2 btn-sm" Text="Completed" runat="server" OnClientClick="return confirmCompleted();" CommandName="Update" ToolTip="Update" Height="30px" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                            </asp:GridView>
                            <%--<table id="projectedCourses" class=" table display" style="width: 100%; font-size: small;">
                                <thead class="table-warning">
                                    <tr>
                                        <!-- class="table-warning" -->
                                        <th>Number</th>
                                        <th>Name</th>
                                        <th>Year</th>
                                        <th>Term</th>
                                        <th>Credits</th>
                                        <%--<th>Grade</th>
                                    </tr>
                                </thead>
                            </table>--%>
                        </div>
                    </div>
                </div>
                <!-- <div class="col-1">
                        </div> -->
                <div class="row my-3">
                    <div class="col card mx-4 pl-0 pr-0" style="font-size: small">
                        <div class="card-header"><strong>Completed Courses</strong></div>
                        <div class="card-body">

                            <%--<table id="completedCourses" class=" table display" style="width: 100%; font-size: small;">
                                <thead class="table-warning">
                                    <tr>
                                        <!--  -->
                                        <th>Number</th>
                                        <th>Name</th>
                                        <th>Year</th>
                                        <th>Term</th>
                                        <th>Credits</th>
                                        <th>Grade</th>
                                    </tr>
                                </thead>
                            </table>--%>
                            <asp:GridView ID="GridView1" runat="server" AutoGeneratedColumns="false" ShowFooter="true" DataKeyNames="CourseNumber"
                                Width="100%"
                                CssClass="gridview"
                                ShowHeaderWhenEmpty="true"
                                OnRowEditing="GridView1_RowEditing"
                                OnRowCancelingEdit="GridView1_CancellingEdit"
                                OnRowUpdating="GridView1_RowUpdating"
                                BackColor="White" BorderColor="#CCCCCC" BorderStyle="None" BorderWidth="1px" CellPadding="3" EnableModelValidation="True" AutoGenerateColumns="False">
                                <%--<FooterStyle BackColor="White" ForeColor="#000066" />--%>
                                <HeaderStyle Font-Names="Calibri" BackColor="#006699" Font-Bold="True" ForeColor="White" />
                                <PagerStyle BackColor="White" ForeColor="#000066" HorizontalAlign="Left" />
                                <RowStyle ForeColor="#000066" />
                                <SelectedRowStyle BackColor="#669999" Font-Bold="True" ForeColor="White" />
                                <%--<SortedAscendingCellStyle BackColor="#F1F1F1">--%>

                                <Columns>
                                    <asp:TemplateField HeaderText="Course">
                                        <ItemTemplate>
                                            <asp:Label Style="font-family: Calibri" Text='<%# Eval("CourseNumber") %>' runat="server" />
                                        </ItemTemplate>
                                        <EditItemTemplate>
                                            <asp:Label Style="font-family: Calibri" ID="txtCourse" Text='<%# Eval("CourseNumber") %>' runat="server" /><%-- to make this editable just change Lable to textboxs--%>
                                        </EditItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Credits">
                                        <ItemTemplate>
                                            <asp:Label Style="font-family: Calibri" Text='<%# Eval("Credits") %>' runat="server" />
                                        </ItemTemplate>
                                        <EditItemTemplate>
                                            <asp:Label ID="txtCredits" Text='<%# Eval("Credits") %>' runat="server" />
                                        </EditItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Year_Taken">
                                        <ItemTemplate>
                                            <asp:Label Style="font-family: Calibri" Text='<%# Eval("CourseYear") %>' runat="server" />
                                        </ItemTemplate>
                                        <EditItemTemplate>
                                            <asp:Label Style="font-family: Calibri" ID="txtYear_Taken" Text='<%# Eval("CourseYear") %>' runat="server" />
                                        </EditItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Term_Taken">
                                        <ItemTemplate>
                                            <asp:Label Style="font-family: Calibri" Text='<%# Eval("Term") %>' runat="server" />
                                        </ItemTemplate>
                                        <EditItemTemplate>
                                            <asp:Label Style="font-family: Calibri" ID="txtTerm_Taken" Text='<%# Eval("Term") %>' runat="server" />
                                        </EditItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Grade">
                                        <ItemTemplate>
                                            <asp:Label Style="font-family: Calibri" Text='<%# Eval("Grade") %>' runat="server" />
                                        </ItemTemplate>
                                        <EditItemTemplate>
                                            <asp:TextBox Style="font-family: Calibri" ID="txtGrade" Text='<%# Eval("Grade") %>' runat="server" />
                                        </EditItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField>
                                        <ItemTemplate>
                                            <asp:Button Style="font-family: Calibri" class="btn btn-dark form-control my-2 btn-sm" Text="Edit" runat="server" CommandName="Edit" ToolTip="Edit" Height="30px" />
                                        </ItemTemplate>
                                        <EditItemTemplate>
                                            <div class="row">
                                                <div class="col-6">
                                                    <asp:Button Style="font-family: Calibri" class="btn btn-dark form-control my-2 btn-sm" Text="Update" runat="server" CommandName="Update" ToolTip="Edit" Height="30px" />
                                                </div>
                                                <div class="col-6">
                                                    <asp:Button Style="font-family: Calibri" class="btn btn-dark form-control my-2 btn-sm" Text="Cancel" runat="server" CommandName="Cancel" ToolTip="Cancel" Height="30px" />
                                                </div>
                                            </div>
                                        </EditItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                            </asp:GridView>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <div class="row my-3">
            <div class="col-12">
                <label>
                </label>
            </div>
        </div>
    </div>
    <!-- Content Page End -->
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="EvaluationContent" runat="server" EnableViewState="true">
</asp:Content>

