var page = 0;
app.controller("Invoice_MandobSearchCtrl", function ($scope, $http, $window, $timeout, $uibModal) {
    $scope.Srch_Mandob = { Invoice_From: "", Invoice_To: "", Mandob_id: "", DateFrom: "", DateTo: "", Price_From: "", Price_To: "" };
    //----------------------------------------------------------------------------------------------------------------------------
    //----------------------------------------------------------------------------------------------------------------------------
    // load all popuop data
    //----------------------------------------------------------------------------------------------------------------------------

    //----------------------------------------------------------------------------------------------------------------
    // Clear all Fields
    //----------------------------------------------------------------------------------------------------------------
    $scope.emptyInputs = function () {
        $scope.Srch_Mandob.Product_Name = "";
        $scope.Srch_Mandob.Mandob_id = "";
        $scope.Srch_Mandob.Price_From = "";
        $scope.Srch_Mandob.Price_To = "";
    };
    var View_Mandobs;
    $scope.getView = function () {
        $http.get("/Invoice_Mandob/GetMandob")
         .success(function (response) {
             View_Mandobs = response;
         });
    };
    $scope.getView();
    $scope.loadMandobs = function () {
        //-----
        $scope.data = null;
        $scope.getAllMandobs();
        //------------
        $uibModal.open({
            scope: $scope,
            template: View_Mandobs,
            resolve: {
                Srch_Mandob: function () {
                    return $scope.Srch_Mandob;
                }
            }
        });
    };
    $scope.clicktrMandob = function (row) {
        $scope.Srch_Mandob.Mandob_id = row.ID;
        $scope.Srch_Mandob.Mandob_Name = row.Name;
        $timeout(function () { angular.element('#closebox').trigger('click'); }, 0);
        this.$close();
    }
   
    $scope.closeUiModal_Mandob = function () {
        this.$close();
    }
    $scope.getAllMandobs = function (Page, Search) {
        debugger;
        var parameter = { 'Search': Search }
        var sentData = "page=" + Page;
        $http.get("/Mandob/getAll?" + sentData, { params: parameter })
       .success(function (response) {
           if (response != null) {

               if ($scope.MandobList == null) {
                   $scope.MandobList = response;
               }
               else {
                   $scope.MandobList = $scope.MandobList.concat(response);
               }
           }
           //if (response == null || response == "") { otherData = 1; hideLoadMore = null; }
           //else {  otherData = null; hideLoadMore = 1; }
       });
    }
 
    //----------------------------------------------------------------------------------------------------------------
    //----------------------------------------------------------------------------------------------------------------
    // Get AllRecords Functions
    //----------------------------------------------------------------------------------------------------------------
    $scope.getAllRecords = function (Page) {
        $scope.EndSearch = null;
        $scope.TableLoading = 1;
        // $("input[name='Date_Poduction[" + i +"]']").val()
        // ListExport.DatEnd = document.getElementById("DatEnd").value;

        
        var parameter = JSON.stringify({ 'Srch_Mandob': $scope.Srch_Mandob, 'page': page });
        $http.post("/Search/Invoice_Mandob", parameter)
    .success(function (response) {
        $scope.TableLoading = null;
        if (response.length >= 1) {
            
            if (page == 0 || $scope.data == null) {
                $scope.dataMandobList = response;
            }
            else {
                $scope.dataMandobList = $scope.dataMandobList.concat(response);
            }
            page++;
        }
        else if (response == '' || response == null) {
            $scope.EndSearch = 1;
        }
        else if (response.ErrorName.indexOf("المسموح") > -1) {
                $uibModal.open({
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
        $scope.Srch_Mandob.DateFrom = $("#DateFrom").val();
        $scope.Srch_Mandob.DateTo = $("#DateTo").val();
        page = 0;
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
        $window.location.href = '/Invoice_Mandob/Invoice_Mandob?id=' + response.ID;
    }
});