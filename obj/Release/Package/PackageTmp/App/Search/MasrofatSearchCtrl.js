var page = 0;
app.controller("MasrofatSearchCtrl", function ($scope, $http, $window, $timeout, $uibModal) {
    $scope.Srch_Masrofat = { ID_From: "", ID_To: "", Date_Invoice_From: "", Date_Invoice_TO: "", Masrofat_Type_Id: "", Masrofat_Type_Name: "", Money_From: "", Money_To: "", Bian: "" };
    //----------------------------------------------------------------------------------------------------------------
    $scope.emptyInputs = function () {
        $scope.Srch_Masrofat.ID_From = "";
        $scope.Srch_Masrofat.ID_To = "";
        $scope.Srch_Masrofat.Date_Invoice_From = "";
        $scope.Srch_Masrofat.Date_Invoice_TO = "";
        $scope.Srch_Masrofat.Masrofat_Type_Id = "";
        $scope.Srch_Masrofat.Money_From = "";
        $scope.Srch_Masrofat.Money_To = "";
        $scope.Srch_Masrofat.Bian = "";
        $scope.Srch_Masrofat.Masrofat_Type_Id = "";
        $scope.Srch_Masrofat.Masrofat_Type_Name = "";
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

        
        var parameter = JSON.stringify({ 'RModel': $scope.Srch_Masrofat, 'page': page });
        $http.post("/Search/Masrofat", parameter)
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
        $scope.Srch_Masrofat.Date_Invoice_From = $("#Date_Invoice_From").val();
        $scope.Srch_Masrofat.Date_Invoice_TO = $("#Date_Invoice_TO").val();
        $scope.getAllRecords(page);
    };
    //----------------------------------------------------------------------------------------------------------------------------
    // load all popuop data
    //----------------------------------------------------------------------------------------------------------------------------
    var View_Masrofat_Type;
    $scope.getView = function () {
        $http.get("/Masrofat/GetMasrofat_Type")
         .success(function (response) {
             View_Masrofat_Type = response;
         });
    };
    $scope.getView();
    $scope.loadMasrofat_Type = function () {

        //-----
        $scope.data = null;
        $scope.getAllMasrofat_Type();
        //------------
        $uibModal.open({
            scope: $scope,
            template: View_Masrofat_Type,
            resolve: {
                Srch_Masrofat: function () {
                    return $scope.Srch_Masrofat;
                }
            }
        });
    };
    //************************************************
    var View_Masrofat_Types;
    $scope.getMasrofat_TypesView = function () {
        $http.get("/Masrofat/GetMasrofat_Type")
         .success(function (response) {
             View_Masrofat_Types = response;
         });
    };
    $scope.getMasrofat_TypesView();
    $scope.getAllMasrofat_Type = function (Page, Search) {

        var parameter = { 'Search': Search }
        var sentData = "page=" + Page;
        $http.get("/Masrofat/getAllMasrofat_Type?" + sentData, { params: parameter })
       .success(function (response) {
           if (response != null) {

               if ($scope.Masrofat_TypeList == null) {
                   $scope.Masrofat_TypeList = response;
               }
               else {
                   $scope.Masrofat_TypeList = $scope.Masrofat_TypeList.concat(response);
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
            $scope.getAllMasrofat_Type(null, search);
        }
        lastentry = search;
    };
    //----------------------------------------------------------------------------------------------------------------------------
    //=======عند الضفط على صف
    $scope.clicktr = function (row) {

        $scope.Srch_Masrofat.Masrofat_Type_Id = row.ID;
        $scope.Srch_Masrofat.Masrofat_Type_Name = row.Name;
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
        $window.location.href = '/Masrofat/Operation?id=' + response.ID;
    }
});