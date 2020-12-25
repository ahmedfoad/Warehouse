var page = 0;
app.controller("EmployeeSearchCtrl", function ($scope, $http, $window, $timeout, $uibModal) {
    $scope.Srch_Employee = { Name: "", Mobile: "", Job_Id: "", Job_Name: "", Salary_From: "" , Salary_To: "" };
    //----------------------------------------------------------------------------------------------------------------
    $scope.emptyInputs = function () {
        $scope.Srch_Employee.Name = "";
        $scope.Srch_Employee.Mobile = "";
        $scope.Srch_Employee.Job_Id = "";
        $scope.Srch_Employee.Job_Name = "";
        $scope.Srch_Employee.Salary_From = "";
        $scope.Srch_Employee.Salary_To = "";
    };
    //----------------------------------------------------------------------------------------------------------------
    //----------------------------------------------------------------------------------------------------------------
    // Get AllRecords Functions
    //----------------------------------------------------------------------------------------------------------------
    $scope.getAllRecords = function (Page) {
        $scope.EndSearch = null;
        $scope.TableLoading = 1;
        // $("input[name='Date_Poduction[" + i +"]']").val()
        // ListExport.DatEnd = document.getElementById("DatEnd").value;

        
        var parameter = JSON.stringify({ 'RModel': $scope.Srch_Employee, 'page': page });
        $http.post("/Search/Employee", parameter)
    .success(function (response) {
        $scope.TableLoading = null;
        if (response.length >= 1) {
            
            if (page == 0 || $scope.employeelist == null) {
                $scope.employeelist = response;
            }
            else {
                $scope.employeelist = $scope.employeelist.concat(response);
            }
            page++;
        }
        else if (response == '' || response == null) {
            $scope.EndSearch = 1;
        }
        else if (response.ErrorName.indexOf("المسموح") > -1) {
                $fancyModal.open({
                    template: " <div class='portlet box red' style='min-height:150px !important;min-width: 541px !important;'> <div class='portlet-title'> <div class='caption'> <i class='fa-lg fa fa-warning'></i> تنبيه هام </div><div class='tools'> </div></div><div class='portlet-body form portlet-empty' style='min-height: 140px;'> <div class='col-md-12 page-404'> <div class='number'>" + response.ErrorNumber + "</div><div class='details'> <h3 style='font-family: sans-serif; text-align: center; font-weight: bold;'>" + response.ErrorFullNumber + "</h3> <p style='font-size: 12px;'> " + response.ErrorName + " <br/> <br/> <a class='btn btn-default btn-sm' href='/home'> الصفحة الرئيسية </a> <a class='btn btn-default btn-sm' href='" + response.Url + "' style='margin-right: 4px;'>المحاولة مرة آخري</a> </p></div></div></div></div>",
                    closeOnEscape: false,
                    closeOnOverlayClick: false
                });
        }
    });
    };
    //----------------------------------------------------------------------------------------------------------------
    //----------------------------------------------------------------------------------------------------------------
    // search button Functions
    //----------------------------------------------------------------------------------------------------------------
    $scope.SearchButton = function () {
        debugger;
        page = 0;
        //$scope.Srch_Employee.Job_Id = $("#Job_Id").val();
        //$scope.Srch_Employee.Job_Name = $("#Job_Name").val();
        $scope.getAllRecords(page);
    };
    //----------------------------------------------------------------------------------------------------------------------------
    // load all popuop data
    //----------------------------------------------------------------------------------------------------------------------------
    var View_jobs_Type;
    $scope.getView = function () {
        $http.get("/Employee/Getjobs")
         .success(function (response) {
             View_jobs_Type = response;
         });
    };
    $scope.getView();
    $scope.loadjob = function () {

        //-----
        $scope.data = null;
        $scope.getAllJobs();
        //------------
        $uibModal.open({
            scope: $scope,
            template: View_jobs_Type,
            resolve: {
                Srch_Employee: function () {
                    return $scope.Srch_Employee;
                }
            }
        });
    };
    //************************************************
    var View_jobs;
    $scope.getEradat_TypesView = function () {
        $http.get("/Employee/Getjobs")
         .success(function (response) {
             View_jobs = response;
         });
    };
    $scope.getEradat_TypesView();
    $scope.getAllJobs = function (Page, Search) {

        var parameter = { 'Search': Search }
        var sentData = "page=" + Page;
        $http.get("/Job/getAll?" + sentData, { params: parameter })
       .success(function (response) {
           if (response != null) {

               if ($scope.data == null) {
                   $scope.data = response;
               }
               else {
                   $scope.data = $scope.data.concat(response);
               }
           }
           //if (response == null || response == "") { otherData = 1; hideLoadMore = null; }
           //else {  otherData = null; hideLoadMore = 1; }
       });
    }
    //----------------------------------------------------------------------------------------------------------------------------
    //----------------------------------------------------------------------------------------------------------------------------
    // instanse Search for Departments
    //----------------------------------------------------------------------------------------------------------------------------
    var lastentry = "";
    $scope.keyupevt = function () {
        var search = $("#Search").val();
        if (search != lastentry) {
            $scope.data = null;
            page = 0;
            $scope.getAllEradat_Type(null, search);
        }
        lastentry = search;
    };
    //----------------------------------------------------------------------------------------------------------------------------
    //=======عند الضفط على صف
    $scope.clicktr = function (row) {

        $scope.Srch_Employee.Job_Id = row.ID;
        $scope.Srch_Employee.Job_Name = row.Name;
        $timeout(function () { angular.element('#closebox').trigger('click'); }, 0);
        this.$close();
    }


    $scope.closeUiModal = function () {
        this.$close();
    }
    //----------------------------------------------------------------------------------------------------------------------------
    //----------------------------------------------------------------------------------------------------------------------------
    // get data of users with scrolling 
    //----------------------------------------------------------------------------------------------------------------------------
    angular.element($window).bind("scroll", function () {
        var windowHeight = "innerHeight" in window ? window.innerHeight : document.documentElement.offsetHeight;
        var body = document.body, html = document.documentElement;
        var docHeight = Math.max(body.scrollHeight, body.offsetHeight, html.clientHeight, html.scrollHeight, html.offsetHeight);
        windowBottom = windowHeight + window.pageYOffset;
        if (windowBottom >= docHeight) {
            $scope.getAllRecords(page);
        }
    });
    //----------------------------------------------------------------------------------------------------------------------------
    //----------------------------------------------------------------------------------------------------------------------------
    // Redirect To Edit Page
    //----------------------------------------------------------------------------------------------------------------------------
    $scope.CallEditPage = function (response) {
        $window.location.href = '/Employee/Operation?id=' + response.ID;
    }
});