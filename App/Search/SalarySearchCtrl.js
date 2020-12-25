var page = 0;
app.controller("SalarySearchCtrl", function ($scope, $http, $window, $timeout, $uibModal) {
    $scope.Srch_Salary = { ID_From: "", ID_To: "", Date_Invoice_From: "", Date_Invoice_TO: "", Salary_Type_Id: "", Salary_Type_Name: "", Money_From: "", Money_To: "", Bian: "" };
    //----------------------------------------------------------------------------------------------------------------
    $scope.emptyInputs = function () {
        $scope.Srch_Salary.ID_From = "";
        $scope.Srch_Salary.ID_To = "";
        $scope.Srch_Salary.Date_Invoice_From = "";
        $scope.Srch_Salary.Date_Invoice_TO = "";
        $scope.Srch_Salary.Money_From = "";
        $scope.Srch_Salary.Money_To = "";
        $scope.Srch_Salary.Bian = "";
       
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

        
        var parameter = JSON.stringify({ 'RModel': $scope.Srch_Salary, 'page': page });
        $http.post("/Search/Salary", parameter)
    .success(function (response) {
        $scope.TableLoading = null;
        if (response.length >= 1) {
            
            if (page == 0 || $scope.data == null) {
                $scope.data = response;
            }
            else {
                $scope.data = $scope.data.concat(response);
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
        
        page = 0;
        $scope.data = null;
        $scope.Srch_Salary.Date_Invoice_From = $("#Date_Invoice_From").val();
        $scope.Srch_Salary.Date_Invoice_TO = $("#Date_Invoice_TO").val();
        $scope.getAllRecords(page);
    };
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
        $window.location.href = '/Employee/Salary?id=' + response.ID;
    }
});