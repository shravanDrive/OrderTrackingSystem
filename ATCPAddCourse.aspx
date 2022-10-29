<%@ Page Title="Batching" Language="C#"   MasterPageFile="~/ATCP.Master"
    CodeBehind="ATCPAddCourse.aspx.cs" Inherits="ATCPClient.ATCPAddCourse" MaintainScrollPositionOnPostback = "true" %>

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

        .messagealert 
        {
            width: 100%;
            position: fixed;
             top:0px;
            z-index: 100000;
            padding: 0;
            font-size: 15px;
        }

        @keyframes loading 
        {
            from 
            {   transform: rotate(0turn);   }
            to 
            {   transform: rotate(1turn);   }
        }
    </style>

    <script type="text/javascript">
        $(document).ready(function () {

        });


        function ShowMessage(message, messagetype)
        {

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


        //alert fade in 
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server" EnableViewState="true">
    <div class="container" id="background">
        <div class="row my-3">
            <div class="col-5">
                <div class="input-group col-12" style="padding-left:0px;">
                    <span class="input-group-text" style="font-family: Calibri; font-size: small;">Course Number</span>
                    <asp:TextBox ID="CourseNumberTextBox" runat="server" Style="margin-right:5px;background-color:white" class="form-control form-control-sm" aria-describedby="lastName"></asp:TextBox>
                </div>
                <asp:Label ID="courseNumberValidatorID" style="font-family:calibri;font-size:small;" Text="" runat="server" ForeColor="Red" />
                <%--<asp:RequiredFieldValidator ID="courseNumberValidator" runat="server" style="font-family:Calibri;font-size:small;margin-left:75%" ErrorMessage="Required Field" ControlToValidate="CourseNumberTextBox"> 
                </asp:RequiredFieldValidator>--%>                               
            </div>
            <div class="col-7">
                 <div class="input-group col-12" style="padding-left:0px">
                    <span class="input-group-text" style="font-family: Calibri; font-size: small">Course Name</span>
                    <asp:TextBox ID="CourseName" runat="server" Style="height:200%;font-family: Calibri;" TextMode="MultiLine" Rows="3" class="form-control form-control-sm"></asp:TextBox>
                 </div>
            </div>
        </div>
        <div class="row my-4">
            <div class="col-5">
                <div class="input-group col-12" style="padding-left:0px">
                    <span class="input-group-text" style="font-family: Calibri; font-size: small;">Course Credits</span>
                    <asp:DropDownList ID="CreditList" style="width:75%;font-family:Calibri" runat="server" Visible="true" OnSelectedIndexChanged="CreditList_SelectedIndexChanged" AutoPostBack="True" AppendDataBoundItems="true" EnableViewState="true"></asp:DropDownList> 
                </div>           
            </div>
            <div class="col-7">
                 <div class="input-group col-12" style="padding-left:0px">
                    <span class="input-group-text" style="font-family: Calibri; font-size: small">Course Description</span>
                    <asp:TextBox ID="courseDescription" runat="server" Style="height:200%;font-family: Calibri;" TextMode="MultiLine" Rows="3" class="form-control form-control-sm"></asp:TextBox>
                 </div>
            </div>
        </div>
        <div class="row my-4">
            <div class="col-5">
                <div class="input-group col-12" style="padding-left:0px">
                    <span class="input-group-text" style="font-family: Calibri; font-size: small;">Primary Instructor</span>
                    <asp:DropDownList ID="primaryInstructorDropDown" style="width:70%;font-family:Calibri" runat="server" Visible="true" OnSelectedIndexChanged="primaryInstructorDropDown_SelectedIndexChanged" AutoPostBack="True" AppendDataBoundItems="true" EnableViewState="true"></asp:DropDownList> 
                </div>
                <asp:Label ID="primaryInstructorValidation" style="font-family:Calibri;font-size:small;" Text="" runat="server" ForeColor="Red" />                
            </div>
            <div class="col-7" style="height: 130%">
                <div class="input-group col-12" style="padding-left:0px">
                    <span class="input-group-text" style="font-family: Calibri; font-size: small;width:20%">Course Objective</span>
                    <asp:TextBox ID="CourseObjective" runat="server" Style="height:200%;font-family: Calibri;" TextMode="MultiLine" Rows="3" class="form-control form-control-sm"></asp:TextBox>
                </div>
            </div>
        </div>
        <div class="row">
            <div class="col-3" >  
                <div class="input-group col-6" style="padding-left:0px">              
                       <div class="input-group-prepend">
                             <div class="input-group-text">
                                <asp:CheckBox ID="isFall" OnCheckedChanged="isFall_CheckedChanged" runat="server" Style="font-size: small" AutoPostBack="true" aria-label="Checkbox for following text input" />
                            </div>
                        </div>                            
                   <input type="text" class="form-control" style="font-size: small" disabled aria-label="Fall" placeholder="Fall">
               </div>
               <asp:Label ID="ChecBoxValidation" style="font-family:calibri;font-size:small;" Text="" runat="server" ForeColor="Red" />                                        
            </div>
            <div class="col-3" style="margin-left:-130px">
               <div class="input-group col-6" style="padding-left:0px">              
                       <div class="input-group-prepend">
                             <div class="input-group-text">
                                <asp:CheckBox ID="isSpring" OnCheckedChanged="isSpring_CheckedChanged" runat="server" Style="font-size: small" AutoPostBack="true" aria-label="Checkbox for following text input" />
                            </div>
                        </div>                            
                   <input type="text" class="form-control" style="font-size: small" disabled aria-label="Spring" placeholder="Spring">
               </div>
               
            </div>
            <div class="col-3" style="margin-left:-135px">
               <div class="input-group col-6" style="padding-left:0px">              
                       <div class="input-group-prepend">
                             <div class="input-group-text">
                                <asp:CheckBox ID="isSummer" OnCheckedChanged="isSummer_CheckedChanged" runat="server" Style="font-size: small" AutoPostBack="true" aria-label="Checkbox for following text input" />
                            </div>
                        </div>                            
                   <input type="text" class="form-control" style="font-size: small" disabled aria-label="Summer" placeholder="Summer">
               </div>
               
            </div>
            <div class="col-7" style="margin-left:-115px">
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
            </div>
            <div class="col-4">
                <div class="input-group col-12" style="padding-left:0px">
                    <span class="input-group-text" style="font-family: Calibri; font-size: small;">Area of Concentration</span>
                    <asp:DropDownList ID="CourseAreaOfConcentration" style="width:56%;font-family:Calibri" runat="server" Visible="true" OnSelectedIndexChanged="CourseAreaOfConcentration_SelectedIndexChanged" AutoPostBack="True" AppendDataBoundItems="true" EnableViewState="true"></asp:DropDownList> 
                </div>
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
            </div>
        </div>
        <div class="row my-4">
            <div class="col-5">
                <div class="input-group col-12" style="padding-left:0px">
                    <span class="input-group-text" style="font-family: Calibri; font-size: small;">Secondary Instructor2</span>
                    <asp:DropDownList ID="secondaryInstructorID2" style="width:66%;font-family:Calibri" runat="server" Visible="true" OnSelectedIndexChanged="secondaryInstructorID2_SelectedIndexChanged" AutoPostBack="True" AppendDataBoundItems="true" EnableViewState="true"></asp:DropDownList> 
                </div>               
            </div>
            <div class="col-4">
                <asp:Button style="width:94%;font-family:Calibri" ID="AddID" class="btn btn-dark" OnClick="AddID_Click" OnClientClick="return BlackInitial();" Text="Add Course" runat="server" Height="39px" />
            </div> 
            <div class="col-3">
                
                <%--class="messagealert"--%>
                <%--<asp:LinkButton runat="server" id="SomeLinkButton" style="font-family:Calibri;width:200px;height:41px;margin-left:40px" href="ATCPCourseDetails.aspx" CssClass="btn btn-dark">Return to Course Details</asp:LinkButton>--%>
            </div>
        </div>
        <div class="row my-4">
            <div class="col-5">
                <div class="input-group col-12" style="padding-left:0px">
                    <span class="input-group-text" style="font-family: Calibri; font-size: small;">Secondary Instructor3</span>
                    <asp:DropDownList ID="secondaryInstructorID3" style="width:66%;font-family:Calibri" runat="server" Visible="true" OnSelectedIndexChanged="secondaryInstructorID3_SelectedIndexChanged" AutoPostBack="True" AppendDataBoundItems="true" EnableViewState="true"></asp:DropDownList> 
                </div>               
            </div>
            <div class="col-4">
                 <asp:Label ID="lblSuccessMessage" Text="" runat="server" ForeColor="Green" />
                 <asp:Label ID="lblErrorMessage" Text="" runat="server" ForeColor="Red" />                            
           </div>
        </div>
        <div class="row my-4">
            <div class="col-5">
                <div class="input-group col-12" style="padding-left:0px">
                    <span class="input-group-text" style="font-family: Calibri; font-size: small;">Guest Lecturer</span>
                    <asp:TextBox ID="GuestLecture" runat="server" Style="font-family: Calibri;" class="form-control form-control-sm"></asp:TextBox>
                </div>               
            </div>
            <div class="col-7"><div  id="alert_container" style="height:100%;"></div></div>
        </div>
        <div class="row my-4">
            <div class="col-5">
            </div> 
        </div>
        <div class="row my-4">
            <div class="col-5">
                <label class="font-weight-bold" for="coursenumber" style="font-size: 13px;">_</label>
            </div>
            <div class="col-4"></div>
            <div class="col-3"></div>
        </div>
    </div>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="EvaluationContent" runat="server" EnableViewState="true">
</asp:Content>