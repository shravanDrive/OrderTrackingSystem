<%@ Page Title="Batching" Language="C#"   MasterPageFile="~/ATCP.Master"
    CodeBehind="CourseViewEditForm.aspx.cs" Inherits="ATCPClient.CourseViewEditForm" MaintainScrollPositionOnPostback = "true" %>

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

         </style>
    <script>

        $(document).ready(function () 
        {
            // FIRST : GET FACULTY INFORMATION AND LOAD IT IN ALL DROPDOWNS
            $.ajax
            ({
                url: "CourseViewEditForm.aspx/GetData",
                dataSrc : '',
                dataType: 'json', // IMPORTANT TO GET THE DATA BACK IN A JSON FORMAT
                success: function(result)
                {               
                    AssigningDropdowns(result);
                } 
            });


            // SECOND : LOAD ALL DATA FROM COURSE TABLE
            $( "#SearchButton" ).click(function() 
            { 
                // IF YOU WANT TO ADD LOADING SYMBOL
                //$('#DisplayButton').addClass('disabled');
                // $('#SearchButton').addClass('button--loading');
                // $('#SearchButton').removeClass('disabled');
                // $('#SearchButton').removeClass('button--loading');
                
                BlockUI(); 
    
                var searchTextData = $("#searchTextBox").val();
                $.ajax
                ({
                    url:"GetCourseViewData.php",
                    dataSrc : '',
                    dataType: 'json', // IMPORTANT TO GET THE DATA BACK IN A JSON FORMAT
                    method: "post",
                    data: 
                    { 
                        "SearchData": searchTextData
                    },
                    success: function(result)
                    {
                        AssigningTextBoxes(result);
                    } 
                });

            });

            $( "#UpdateButton" ).click(function()
            {
                
                BlockUI();

                $.ajax
                ({
                    url:"UpdateCourseViewData.php",
                    dataSrc : '',
                    //dataType: 'json', // IMPORTANT TO GET THE DATA BACK IN A JSON FORMAT
                    method: "post",
                    data: 
                    { 
                        "courseNumber": $("#coursenumber").val(),
                        "coursename": $('#coursename').val(),
                        "courseDescription": $('#courseDescription').val(),
                        "courseObjective": $('#courseObjective').val(), 
                        "majorAssignments": $('#majorAssignments').val(),
                        "GuestLecture": $("#GuestLecture").val(),
                        "PrimaryInstructorID": $("#PrimaryInstructorID").val(),
                        "SecondaryInstructor1ID": $("#SecondaryInstructor1ID").val(),
                        "SecondaryInstructor2ID": $("#SecondaryInstructor2ID").val(),
                        "SecondaryInstructor3ID": $("#SecondaryInstructor3ID").val(),
                        "creditOption": $("#creditOption").val(),
                        "CourseAreaOfConcentration":$("#CourseAreaOfConcentration").val(),
                        "Terms": GetUpdatedTerms(),
                        "Assesment":GetAssesmentCourse()
                    },
                    success: function(result) 
                    { 
                        if(result == 1)
                        {
                            $('#UpdatedAlert').addClass('alert alert-success');
                            $('#UpdatedAlert').text('Success!');
                            $("#UpdatedAlert").show('fade');
                        }
                        else
                        {
                            $('#UpdatedAlert').addClass('alert alert-danger');
                            $('#UpdatedAlert').text('Error! No such Course Name found.');
                            $("#UpdatedAlert").show('fade');
                        }
                    },
                    error: function(jqxhr, status, exception) 
                    {
                        $('#UpdatedAlert').addClass('alert alert-danger');
                        $('#UpdatedAlert').text('Error! No such Course Name found.');
                        $("#UpdatedAlert").show('fade');   
                    } 
                    
                });

                setTimeout(function()
                {
                    $("#UpdatedAlert").hide('fade');
                    $('#UpdatedAlert').removeClass('alert alert-danger');
                    $('#UpdatedAlert').removeClass('alert alert-success');
                }, 3000);

            }); 


        });

        function BlockUI()
        {
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

        function GetUpdatedTerms()
        {
            var Terms="";
            if ($('#FallCheckBox').is(":checked"))
            {
                Terms+= "Fall;";
            }
            if($('#SpringCheckBox').is(":checked"))
            {
                Terms+= "Spring;";
            }
            if($('#SummerCheckBox').is(":checked"))
            {
                Terms+= "Summer;";
            }

            return Terms;
        }

        function GetAssesmentCourse()
        {
            if ($('#assesmentCourse').is(":checked"))
            {
                return "Yes";
            }
            else
            {
                return "No";
            }
        }

        function AssigningDropdowns(result)
        {
            var primaryInstructor = $('#PrimaryInstructorID');
            var secondaryInstrcutor1 = $('#SecondaryInstructor1ID'); 
            var secondaryInstructor2 = $('#SecondaryInstructor2ID'); 
            var secondaryInstructor3 = $('#SecondaryInstructor3ID');

            $.each(result, function(index, item) 
            {             
                var fullName = item.FName + " " + item.LName;
                primaryInstructor.append($('<option></option>').val(item.UIN).html(fullName));
                secondaryInstrcutor1.append($('<option></option>').val(item.UIN).html(fullName));
                secondaryInstructor2.append($('<option></option>').val(item.UIN).html(fullName));
                secondaryInstructor3.append($('<option></option>').val(item.UIN).html(fullName));                
            });
        }

        function AssigningTextBoxes(result)
        {
            if(result.length == 0)
            {
                $('#myAlert').addClass('alert alert-danger');
                $('#myAlert').text('Error! No such Course Name found.');
                $("#myAlert").show('fade');
                DeAssigningValues();
            }
            else
            {
                $('#myAlert').addClass('alert alert-success');
                $('#myAlert').text('Success!');
                $("#myAlert").show('fade');

                console.log(result);
                console.log(result[0].CourseNumber);
                $('#coursenumber').val(result[0].CourseNumber);
                $('#coursename').val(result[0].CourseName);          
                $('#courseDescription').val(result[0].CourseDescription);
                $('#courseObjective').val(result[0].CourseObjectives); 
                $('#majorAssignments').val(result[0].MajorAssignments);

                if (/Fall/i.test(result[0].TermsOffered))
                {
                    $("#FallCheckBox").prop('checked', true); 
                }
                else if(/Spring/i.test(result[0].TermsOffered))
                {
                    $("#SpringCheckBox").prop('checked', true);
                }
                else if(/Summer/i.test(result[0].TermsOffered))
                {
                    $("#SummerCheckBox").prop('checked', true);
                }

                // switch (result[0].TermsOffered) 
                // { 
	            //     case 'Fall': 
		        //         $("#FallCheckBox").prop('checked', true); 
		        //         break;
	            //     case 'Spring': 
		        //         $("#SpringCheckBox").prop('checked', true);
		        //         break;
	            //     case 'Summer': 
		        //         $("#SummerCheckBox").prop('checked', true);
		        //         break;		
	            //     default:
                //         $("#FallCheckBox").prop('checked', true);
                // }

                $("#PrimaryInstructorID").val(result[0].PrimaryInstructorID);
                $("#SecondaryInstructor1ID").val(result[0].SecondaryInstructor1ID);
                $("#SecondaryInstructor2ID").val(result[0].SecondaryInstructor2ID);
                $("#SecondaryInstructor3ID").val(result[0].SecondaryInstructor3ID);
                $("#creditOption").val(result[0].CourseCredits);
                $("#CourseAreaOfConcentration").val(result[0].CourseAreaOfConcentration);

                if(result[0].AssessmentCourse == "Yes")
                {
                    $("#assesmentCourse").prop('checked', true);
                }
                else
                {
                    $("#assesmentCourse").prop('checked', false);
                }

                $("#GuestLecture").val(result[0].GuestLecture);
            }

            setTimeout(function()
            {
                $("#myAlert").hide('fade');
                $('#myAlert').removeClass('alert alert-danger');
                $('#myAlert').removeClass('alert alert-success');
            }, 3000); 
            
        }

        function DeAssigningValues()
        {
            $('#coursenumber').val('');
            $('#coursename').val('');          
            $('#courseDescription').val('');
            $('#courseObjective').val(''); 
            $('#majorAssignments').val('');
            $("#FallCheckBox").prop('checked', false); 
		    $("#SpringCheckBox").prop('checked', false); 
		    $("#SummerCheckBox").prop('checked', false);
            $("#assesmentCourse").prop('checked', false);
            $("#GuestLecture").val('');
            $("#PrimaryInstructorID").val('');
            $("#SecondaryInstructor1ID").val('');
            $("#SecondaryInstructor2ID").val('');
            $("#SecondaryInstructor3ID").val('');
            $("#creditOption").val('');
            $("#CourseAreaOfConcentration").val('');

        }
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server" EnableViewState="true">

    <%--<?php include('header.php') ?>--%>
    
    <!-- Content Page Start -->
    <div class="container" id="background">
        <div class="row my-3">
            <div class="col-3 d-flex justify-content-center">
            <!-- d-flex -->
                <input type="text" id="searchTextBox" class="form-control form-control-sm" style="font-family:Arial, FontAwesome" id="firstname1" aria-describedby="firstname" placeholder="&#xF002; Search course by number">
            </div>
            <div class="col-1">
            </div>
            <div class="col-2">
                <button type="button" class="btn btn-dark my-1" style="width:100%" id="SearchButton">Search</button>
            </div>
            <div class="col-6">
                <div class="mb-0 mt-1 pt-2 pb-2 collapse" id="myAlert" style="font-size: small;">
                    <a href="#" class="close" data-dismiss="alert">&times;</a>
                    <strong class="BoldedData"></strong>
                </div>
            </div>
        </div>
        <div class="row">
            <div class="col-3">
                <form>
                    <div class="form-group mb-0">
                        <label class="font-weight-bold" for="coursenumber" style="">Course number</label>
                        <input type="text" class="form-control form-control-sm" id="coursenumber" aria-describedby="coursenumber" > 
                    </div>
                    <div class="form-group">
                        <label class="font-weight-bold" for="coursename" style="">Course name</label>
                        <input type="text" class="form-control form-control-sm" id="coursename" aria-describedby="coursename" >
                    </div>
                </form>           
            </div>
            <div class="col-1">
            </div>
            <div class="col-8">
                <label class="font-weight-bold" for="coursenumber" style="">Course Description</label>
                <textarea class="form-control"  id="courseDescription" style="height: 66%" aria-label="With textarea"></textarea>
            </div>
        </div>
        <div class="row my-2">
            <div class="col-3">
            <label class="font-weight-bold" for="coursenumber" style="">Course Selection</label>
                <select id="creditOption" class="form-select form-select-lg w-100" style="height:32px">
                    <option value="1" class="">1 Credit</option>
                    <option value="2" class="">2 Credit</option>
                    <option value="3" class="">3 Credit</option>
                    <option value="4" class="">4 Credit</option>
                </select>
            </div>
            <div class="col-1">
            </div>
            <div class="col-8" style="height: 130%">
                <label class="font-weight-bold" for="coursenumber" style="">Course Objective</label>
                <textarea class="form-control"   id="courseObjective" aria-label="With textarea"></textarea>
            </div>
        </div>
        <!-- <div class="row mt-3">
        </div> -->
        <div class="row mt-5">
            <div class="col-3">
                <label class="font-weight-bold" for="coursenumber" style="">Term(s) Offered</label>
                <div class="form-check">
                    <input class="form-check-input" type="checkbox" id="FallCheckBox">
                    <label class="form-check-label" for="flexCheckDefault">
                        Fall
                    </label>
                </div>
                <div class="form-check">
                    <input class="form-check-input" type="checkbox" id="SpringCheckBox">
                    <label class="form-check-label" for="flexCheckDefault">
                        Spring
                    </label>
                </div>
                <div class="form-check">
                    <input class="form-check-input" type="checkbox" id="SummerCheckBox">
                    <label class="form-check-label" for="flexCheckDefault">
                        Summer
                    </label>
                </div>
            </div>
            <div class="col-1">
            </div>
            <div class="col-8">
                <label class="font-weight-bold" for="coursenumber" style="">Major Assignments</label>
                <textarea class="form-control"  id="majorAssignments" aria-label="With textarea"></textarea>
            </div>
        </div>
        <div class="row mt-4">
            <div class="col-3">
                <label class="font-weight-bold" for="coursenumber" style="">Instructor Selection</label> 
                <!-- style=""font-size: 13px; -->
                <select id="PrimaryInstructorID" class="form-select w-100" aria-label="Primary Instructor" style="height:32px">
                    <!-- <option ></option>  -->
                    <!-- //selected class="" -->
                </select>
            </div>
            <div class="col-1"></div>
            <div class="col-4">
                <label class="font-weight-bold" for="coursenumber" style="">Area of Concentration</label>
                <select id="CourseAreaOfConcentration" class="form-select w-100" aria-label="Primary Instructor" style="height:32px">
                    <option  value="AT in ED">AT in ED</option>
                    <option  value="Seating-Mobility">Seating-Mobility</option>
                    <option  value="AAC">AAC</option>
                    <option  value="Access">Access</option>
                    <option  value="Design">Design</option>
                    <option  value="Enrollment">Enrollment</option>
                </select>
            </div>
            <div class="col-4">
                <div class="form-check mt-4 pt-2">
                    <input class="form-check-input" type="checkbox" value="" id="assesmentCourse">
                    <label class="form-check-label" for="flexCheckDefault">
                        Assesment Course
                    </label>
                </div>
            </div>
        </div>
        <div class="row mt-2">
            <div class="col-3">
                <select id="SecondaryInstructor1ID" class="form-select w-100 mt-3" aria-label="Primary Instructor" style="height:32px">
                    <!-- <option selected class="">Secondary Instructor1</option> -->
                </select>
            </div>
            <div class="col-1"></div>
            <div class="col-3">
                <button type="button" class="btn btn-dark my-1 mt-3" style="width:100%" id="UpdateButton">Update</button>
            </div>
            <div class="col-5">
                <div class="mb-0 mt-1 pt-2 pb-2 collapse mt-3" id="UpdatedAlert" style="font-size: small;">
                    <a href="#" class="close" data-dismiss="alert">&times;</a>
                    <strong class="BoldedData"></strong>
                </div>
            </div>
        </div>
        <div class="row mt-2">
            <div class="col-3">
            <select id="SecondaryInstructor2ID" class="form-select w-100 mt-3" aria-label="Primary Instructor" style="height:32px">
                    <!-- <option selected class="">Secondary Instructor2</option> -->
            </select>
            </div>
        </div>
        <div class="row mt-2">
            <div class="col-3">
                <select id="SecondaryInstructor3ID" class="form-select w-100 mt-3" aria-label="Primary Instructor" style="height:32px">
                    <!-- <option selected class="">Secondary Instructor3</option> -->
                </select>
            </div>
        </div>
        <div class="row mt-4">
            <div class="col-3">
                <textarea class="form-control"  id="GuestLecture" aria-label="With textarea"></textarea>
            </div> 
        </div>
        <div class="row mt-2">
            <div class="col-3">
            <label class="font-weight-bold" for="coursenumber" style="font-size: 13px;">_</label>
            </div>
        </div>
    </div>
</asp:Content>
   

  <asp:Content ID="Content3" ContentPlaceHolderID="EvaluationContent" runat="server" EnableViewState="true">
    </asp:Content>