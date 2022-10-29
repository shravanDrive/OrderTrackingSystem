<%@ Page Title="Batching" Language="C#"   MasterPageFile="~/ATCP.Master"
    CodeBehind="ATCPCourseInfo.aspx.cs" Inherits="ATCPClient.ATCPCourseInfo" MaintainScrollPositionOnPostback = "true" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">

     <style>
        .loader 
        {
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

        .loader--hidden 
        {
            opacity: 0;
            visibility: hidden;
        }

        .loader::after 
        {
            content: "";
            width: 75px;
            height: 75px;
            border: 15px solid #dddddd;
            border-top-color: #009578;
            border-radius: 50%;
            animation: loading 0.75s ease infinite;
        }

        @keyframes loading 
        {
            from 
            {
                transform: rotate(0turn);
            }
            to 
            {
                transform: rotate(1turn);
            }
        }

    </style>

        <script>

            function confirmDelivery() {
                // confirm if the user wants to deliver the product
                return (confirm("Are you sure you want to deliver the product?"));

            };

            function BlackInitial() {
                BlockUI();
            };

            $(document).ready(function () 
            {                       
                $( "#UpdateButton" ).click(function()
                {               
                    BlockUI(); 
                });

                $("<%=updateID.ClientID%>").click(function(e){
                    BlockUI();
      });

                function BlockUI() {
                    alert("Hello");
                    $.blockUI
                    ({
                        message: '<div class="loader"></div>'
                        // css: 
                        // { 
                        //     border: 'none', 
                        //     padding: '15px', 
                        //     backgroundColor: '#000', 
                        //     '-webkit-border-radius': '10px', 
                        //     '-moz-border-radius': '10px', 
                        //     opacity: .5, 
                        //     color: '#fff' 
                        // } 
                    });
                    setTimeout($.unblockUI, 1000);
                }

            });




    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server" EnableViewState="true">
        <div class="container" id="background">
        <div class="row my-3">
            <div class="col-5">
                <div class="input-group col-12" style="padding-left:0px;">
                    <span class="input-group-text" style="font-family: Calibri; font-size: small;">Course Number</span>
                    <asp:TextBox ID="CourseNumberTextBox" ReadOnly="true" runat="server" Style="margin-right:15px;background-color:white" class="form-control form-control-sm" aria-describedby="lastName"></asp:TextBox>
                 </div>
<%--                <form> font-size: small EDITED OUT FROM TEXTBOX UP
                    <div class="form-group mb-0">
                        <label class="font-weight-bold" for="coursenumber" style="">Course number</label>
                        <input type="text" class="form-control form-control-sm" id="coursenumber" aria-describedby="coursenumber" > 
                    </div>
                    <div class="form-group">
                        <label class="font-weight-bold" for="coursename" style="">Course name</label>
                        <input type="text" class="form-control form-control-sm" id="coursename" aria-describedby="coursename" >
                    </div>
                </form>--%>
                <%--<div class="row">--%>
<%--                    <div class="col-6">
                        <%--<asp:TextBox style="width: 100%;height:100%;margin-right:30px; font-family:Calibri, FontAwesome" ID="searchTextBox" placeholder="&#xF002; Search course by number" runat="server" />
                    </div>
                    <div class="col-6">
                        <%--<asp:Button style="width:85%;font-family:Calibri" class="btn btn-dark" Text="Search" runat="server" Height="39px" />
                    </div>--%>
                <%--</div>--%>
            <!-- d-flex -->                               
            </div>
            <%--<div class="col-1">
            </div>--%>
            <div class="col-7">
                 <div class="input-group col-12" style="padding-left:0px">
                    <span class="input-group-text" style="font-family: Calibri; font-size: small">Course Name</span>
                    <asp:TextBox ID="CourseName" runat="server" Style="height:200%;font-family: Calibri;" TextMode="MultiLine" Rows="3" class="form-control form-control-sm"></asp:TextBox>
                 </div>
                <%--<label class="font-weight-bold" for="coursenumber" style="">Course Description</label>
                <textarea class="form-control"  id="courseDescription" style="height: 66%" aria-label="With textarea"></textarea>--%>
            </div>
<%--            <div class="col-3">              
            </div>
            <div class="col-5">
                <div class="mb-0 mt-1 pt-2 pb-2 collapse" id="myAlert" style="font-size: small;">
                    <a href="#" class="close" data-dismiss="alert">&times;</a>
                    <strong class="BoldedData"></strong>
                </div>
            </div>--%>
        </div>
        <div class="row my-4">
            <div class="col-5">
                <div class="input-group col-12" style="padding-left:0px">
                    <span class="input-group-text" style="font-family: Calibri; font-size: small;">Course Credits</span>
                    <asp:DropDownList ID="CreditList" style="width:73%;font-family:Calibri" runat="server" Visible="true" OnSelectedIndexChanged="CreditList_SelectedIndexChanged" AutoPostBack="True" AppendDataBoundItems="true" EnableViewState="true"></asp:DropDownList> 
                </div>
<%--                 <div class="input-group col-12" style="padding-left:0px;">
                    <span class="input-group-text" style="font-family: Calibri; font-size: small;">Course Number</span>
                    <asp:TextBox ID="CourseNumberTextBox" runat="server" Style="margin-right:15px;" class="form-control form-control-sm" aria-describedby="lastName"></asp:TextBox>
                 </div>--%>
<%--                <form> font-size: small EDITED OUT FROM TEXTBOX UP
                    <div class="form-group mb-0">
                        <label class="font-weight-bold" for="coursenumber" style="">Course number</label>
                        <input type="text" class="form-control form-control-sm" id="coursenumber" aria-describedby="coursenumber" > 
                    </div>
                    <div class="form-group">
                        <label class="font-weight-bold" for="coursename" style="">Course name</label>
                        <input type="text" class="form-control form-control-sm" id="coursename" aria-describedby="coursename" >
                    </div>
                </form>--%>           
            </div>
            <%--<div class="col-1">
            </div>--%>
            <div class="col-7">
                 <div class="input-group col-12" style="padding-left:0px">
                    <span class="input-group-text" style="font-family: Calibri; font-size: small">Course Description</span>
                    <asp:TextBox ID="courseDescription" runat="server" Style="height:200%;font-family: Calibri;" TextMode="MultiLine" Rows="3" class="form-control form-control-sm"></asp:TextBox>
                 </div>
                <%--<label class="font-weight-bold" for="coursenumber" style="">Course Description</label>
                <textarea class="form-control"  id="courseDescription" style="height: 66%" aria-label="With textarea"></textarea>--%>
            </div>
        </div>
        <div class="row my-4">
            <div class="col-5">
                <div class="input-group col-12" style="padding-left:0px">
                    <span class="input-group-text" style="font-family: Calibri; font-size: small;">Primary Instructor</span>
                    <asp:DropDownList ID="primaryInstructorDropDown" style="width:70%;font-family:Calibri" runat="server" Visible="true" OnSelectedIndexChanged="primaryInstructorDropDown_SelectedIndexChanged" AutoPostBack="True" AppendDataBoundItems="true" EnableViewState="true"></asp:DropDownList> 
                </div>
<%--                <div class="input-group col-12" style="padding-left:0px">
                    <span class="input-group-text" style="font-family: Calibri; font-size: small;">Course Credits</span>
                    <asp:DropDownList ID="CreditList" style="width:73%;font-family:Calibri" runat="server" Visible="true" OnSelectedIndexChanged="CreditList_SelectedIndexChanged" AutoPostBack="True" AppendDataBoundItems="true" EnableViewState="true"></asp:DropDownList> 
                </div>--%>                
            </div>

            <%--<div class="col-1"> width:100%;
            </div>--%>
            <div class="col-7" style="height: 130%">
                <%--<label class="font-weight-bold" for="coursenumber" style="">Course Objective</label>
                <textarea class="form-control"   id="courseObjective" aria-label="With textarea"></textarea>--%>
                <div class="input-group col-12" style="padding-left:0px">
                    <span class="input-group-text" style="font-family: Calibri; font-size: small;width:20%">Course Objective</span>
                    <asp:TextBox ID="CourseObjective" runat="server" Style="height:200%;font-family: Calibri;" TextMode="MultiLine" Rows="3" class="form-control form-control-sm"></asp:TextBox>
                </div>
            </div>
        </div>
        <!-- <div class="row mt-3">
        </div> -->
        <div class="row">
            <div class="col-3" >
<%--                <div class="input-group col-12" style="padding-left:0px">
                    <span class="input-group-text" style="font-family: Calibri; font-size: small;">Term</span>
                    <asp:CheckBox ID="FallCheckBoxID" style="margin-top:10px;margin-left:8px" Text="Fall" runat="server" AutoPostBack="true" OnCheckedChanged="FallCheckBoxID_CheckedChanged" />
                    <asp:CheckBox ID="SpringCheckBoxID" style="margin-top:10px;margin-left:8px" Text="Spring" runat="server" AutoPostBack="true" OnCheckedChanged="SpringCheckBoxID_CheckedChanged" />
                    <asp:CheckBox ID="SummerCheckBoxID" style="margin-top:10px;margin-left:8px" Text="Summer" runat="server" AutoPostBack="true" OnCheckedChanged="SummerCheckBoxID_CheckedChanged" />
                </div>--%>  
                <div class="input-group col-6" style="padding-left:0px">              
                       <div class="input-group-prepend">
                             <div class="input-group-text">
                                <asp:CheckBox ID="isFall" onclick="return false;" runat="server" Style="font-size: small" aria-label="Checkbox for following text input" />
                            </div>
                        </div>                            
                   <input type="text" class="form-control" style="font-size: small" disabled aria-label="Fall" placeholder="Fall">
               </div>                                        
            </div>
            <div class="col-3" style="margin-left:-130px">
               <div class="input-group col-6" style="padding-left:0px">              
                       <div class="input-group-prepend">
                             <div class="input-group-text">
                                <asp:CheckBox ID="isSpring" onclick="return false;" runat="server" Style="font-size: small" aria-label="Checkbox for following text input" />
                            </div>
                        </div>                            
                   <input type="text" class="form-control" style="font-size: small" disabled aria-label="Spring" placeholder="Spring">
               </div>
            </div>
            <div class="col-3" style="margin-left:-135px">
               <div class="input-group col-6" style="padding-left:0px">              
                       <div class="input-group-prepend">
                             <div class="input-group-text">
                                <asp:CheckBox ID="isSummer" onclick="return false;" runat="server" Style="font-size: small" aria-label="Checkbox for following text input" />
                            </div>
                        </div>                            
                   <input type="text" class="form-control" style="font-size: small" disabled aria-label="Summer" placeholder="Summer">
               </div>
            </div>
            <div class="col-7" style="margin-left:-115px">
                <%--<label class="font-weight-bold" for="coursenumber" style="">Major Assignments</label>
                <textarea class="form-control"  id="majorAssignments" aria-label="With textarea"></textarea>--%>
                <div class="input-group col-12" style="padding-left:0px">
                    <span class="input-group-text" style="font-family: Calibri; font-size: small;">Major Assignments</span>
                    <asp:TextBox ID="majorAssignments" runat="server" Style="height:200%;font-family: Calibri;" TextMode="MultiLine" Rows="3" class="form-control form-control-sm"></asp:TextBox>
                </div>
            </div>
        </div>
        <div class="row my-4">
            <div class="col-5">
                <div class="input-group col-12" style="padding-left:0px">
                    <span class="input-group-text" style="font-family: Calibri; font-size: small;">Secondary Instructor1</span>
                    <asp:DropDownList ID="secondaryInstructorID1" style="width:66%;font-family:Calibri" runat="server" Visible="true" OnSelectedIndexChanged="secondaryInstructorID1_SelectedIndexChanged" AutoPostBack="True" AppendDataBoundItems="true" EnableViewState="true"></asp:DropDownList> 
                </div>
<%--                <div class="input-group col-12" style="padding-left:0px">
                    <span class="input-group-text" style="font-family: Calibri; font-size: small;">Primary Instructor</span>
                    <asp:DropDownList ID="primaryInstructorDropDown" style="width:70%;font-family:Calibri" runat="server" Visible="true" OnSelectedIndexChanged="primaryInstructorDropDown_SelectedIndexChanged" AutoPostBack="True" AppendDataBoundItems="true" EnableViewState="true"></asp:DropDownList> 
                </div>--%>               
            </div>
            <%--<div class="col-1"></div>--%>
            <div class="col-4">
                <div class="input-group col-12" style="padding-left:0px">
                    <span class="input-group-text" style="font-family: Calibri; font-size: small;">Area of Concentration</span>
                    <asp:DropDownList ID="CourseAreaOfConcentration" style="width:56%;font-family:Calibri" runat="server" Visible="true" OnSelectedIndexChanged="CourseAreaOfConcentration_SelectedIndexChanged" AutoPostBack="True" AppendDataBoundItems="true" EnableViewState="true"></asp:DropDownList> 
                </div>
<%--                <label class="font-weight-bold" for="coursenumber" style="">Area of Concentration</label>
                <select id="CourseAreaOfConcentration" class="form-select w-100" aria-label="Primary Instructor" style="height:32px">
                    <option  value="AT in ED">AT in ED</option>
                    <option  value="Seating-Mobility">Seating-Mobility</option>
                    <option  value="AAC">AAC</option>
                    <option  value="Access">Access</option>
                    <option  value="Design">Design</option>
                    <option  value="Enrollment">Enrollment</option>
                </select>--%>
            </div>
            <div class="col-3">
               <div class="input-group col-9" style="padding-left:0px;margin-left:63px">              
                       <div class="input-group-prepend">
                             <div class="input-group-text">
                                <asp:CheckBox ID="assessmentCourseID" runat="server" Style="font-size: small" aria-label="Checkbox for following text input" />
                            </div>
                        </div>                            
                   <input type="text" class="form-control" style="font-size: small" disabled aria-label="Assesment Course" placeholder="Assesment Course">
               </div>
<%--                <div class="form-check mt-4 pt-2">
                    <input class="form-check-input" type="checkbox" value="" id="assesmentCourse">
                    <label class="form-check-label" for="flexCheckDefault">
                        Assesment Course
                    </label>
                </div>--%>
            </div>
        </div>
        <div class="row my-4">
            <div class="col-5">
                <div class="input-group col-12" style="padding-left:0px">
                    <span class="input-group-text" style="font-family: Calibri; font-size: small;">Secondary Instructor2</span>
                    <asp:DropDownList ID="secondaryInstructorID2" style="width:66%;font-family:Calibri" runat="server" Visible="true" OnSelectedIndexChanged="secondaryInstructorID2_SelectedIndexChanged" AutoPostBack="True" AppendDataBoundItems="true" EnableViewState="true"></asp:DropDownList> 
                </div>
<%--                <div class="input-group col-12" style="padding-left:0px">
                    <span class="input-group-text" style="font-family: Calibri; font-size: small;">Secondary Instructor1</span>
                    <asp:DropDownList ID="secondaryInstructorID1" style="width:66%;font-family:Calibri" runat="server" Visible="true" OnSelectedIndexChanged="secondaryInstructorID1_SelectedIndexChanged" AutoPostBack="True" AppendDataBoundItems="true" EnableViewState="true"></asp:DropDownList> 
                </div>--%>               
            </div>
            <%--<div class="col-1"></div>--%>
            <div class="col-4">
                <asp:Button style="width:94%;font-family:Calibri" ID="updateID" class="btn btn-dark" OnClick="Update_Click" OnClientClick="return BlackInitial();" Text="Update" runat="server" Height="39px" />
            </div>
            <div class="col-3">
                <asp:LinkButton runat="server" id="SomeLinkButton" style="font-family:Calibri;width:200px;height:41px;margin-left:40px" href="ATCPCourseDetails.aspx" CssClass="btn btn-dark">Return to Course Details</asp:LinkButton>
                <%--<asp:Button style="width:94%;font-family:Calibri" ID="ReturnButton" class="btn btn-dark" PostBackUrl="ATCPCourseDetails.aspx" OnClientClick="return BlackInitial();" Text="Exit to Course Details" runat="server" Height="39px" />--%>
<%--                <div class="mb-0 mt-1 pt-2 pb-2 collapse mt-3" id="UpdatedAlert" style="font-size: small;">
                    <a href="#" class="close" data-dismiss="alert">&times;</a>
                    <strong class="BoldedData"></strong>
                </div>--%>
            </div>
        </div>
        <div class="row my-4">
            <div class="col-5">
                <div class="input-group col-12" style="padding-left:0px">
                    <span class="input-group-text" style="font-family: Calibri; font-size: small;">Secondary Instructor3</span>
                    <asp:DropDownList ID="secondaryInstructorID3" style="width:66%;font-family:Calibri" runat="server" Visible="true" OnSelectedIndexChanged="secondaryInstructorID3_SelectedIndexChanged" AutoPostBack="True" AppendDataBoundItems="true" EnableViewState="true"></asp:DropDownList> 
                </div>
<%--                <div class="input-group col-12" style="padding-left:0px">
                    <span class="input-group-text" style="font-family: Calibri; font-size: small;">Secondary Instructor2</span>
                    <asp:DropDownList ID="secondaryInstructorID2" style="width:66%;font-family:Calibri" runat="server" Visible="true" OnSelectedIndexChanged="secondaryInstructorID2_SelectedIndexChanged" AutoPostBack="True" AppendDataBoundItems="true" EnableViewState="true"></asp:DropDownList> 
                </div>--%>               
            </div>
            <div class="col-4">
                 <asp:Label ID="lblSuccessMessage" Text="" runat="server" ForeColor="Green" />
                    <br />
                 <asp:Label ID="lblErrorMessage" Text="" runat="server" ForeColor="Red" />            
           </div>
        </div>
        <div class="row my-4">
            <div class="col-5">
                <div class="input-group col-12" style="padding-left:0px">
                    <span class="input-group-text" style="font-family: Calibri; font-size: small;">Guest Lecturer</span>
                    <asp:TextBox ID="GuestLecture" runat="server" Style="font-family: Calibri;" class="form-control form-control-sm"></asp:TextBox>
                </div>
<%--                <div class="input-group col-12" style="padding-left:0px">
                    <span class="input-group-text" style="font-family: Calibri; font-size: small;">Secondary Instructor3</span>
                    <asp:DropDownList ID="secondaryInstructorID3" style="width:66%;font-family:Calibri" runat="server" Visible="true" OnSelectedIndexChanged="secondaryInstructorID3_SelectedIndexChanged" AutoPostBack="True" AppendDataBoundItems="true" EnableViewState="true"></asp:DropDownList> 
                </div>--%>               
            </div>
        </div>
        <div class="row my-4">
            <div class="col-5">
<%--                <div class="input-group col-12" style="padding-left:0px">
                    <span class="input-group-text" style="font-family: Calibri; font-size: small;">Guest Lecturer</span>
                    <asp:TextBox ID="GuestLecture" runat="server" Style="font-family: Calibri;" class="form-control form-control-sm"></asp:TextBox>
                </div>--%>
            </div> 
        </div>
        <div class="row my-4">
            <div class="col-5">
            <label class="font-weight-bold" for="coursenumber" style="font-size: 13px;"></label>
            </div>
        </div>
    </div>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="EvaluationContent" runat="server" EnableViewState="true">
</asp:Content>
