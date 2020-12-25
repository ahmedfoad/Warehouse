var page = 0;
app.controller("Invoice_CompanySearchCtrl", function ($scope, $http, $window, $timeout, $uibModal) {
    $scope.Srch_Company = { Invoice_From: "", Invoice_To: "", Mowrid_id: "", DateFrom: "", DateTo: "", Price_From: "", Price_To: "" };
    //----------------------------------------------------------------------------------------------------------------------------
    //----------------------------------------------------------------------------------------------------------------------------
    // load all popuop data
    //----------------------------------------------------------------------------------------------------------------------------

    //----------------------------------------------------------------------------------------------------------------
    // Clear all Fields
    //----------------------------------------------------------------------------------------------------------------
    $scope.emptyInputs = function () {
        $scope.Srch_Company.Product_Name = "";
        $scope.Srch_Company.Company_id = "";
        $scope.Srch_Company.Price_From = "";
        $scope.Srch_Company.Price_To = "";
        $scope.Srch_Company.Barcode_From = "";
        $scope.Srch_Company.Barcode_To = "";
        $scope.Srch_Company.Taxes = "";
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

       
        var parameter = JSON.stringify({ 'Srch_Company': $scope.Srch_Company, 'page': page });
        $http.post("/Search/Invoice_Company", parameter)
    .success(function (response) {
        $scope.TableLoading = null;
        if (response.length >= 1) {
            
            if (page == 0 || $scope.data == null) {
                $scope.data_ListItems = response;
            }
            else {
                $scope.data_ListItems = $scope.data_ListItems.concat(response);
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
        $scope.Srch_Company.DateFrom = $("#DateFrom").val();
        $scope.Srch_Company.DateTo = $("#DateTo").val();
        page = 0;
        $scope.getAllRecords(page);
    };

    $scope.loadCompanies = function () {
        //-----
        $scope.data = null;
        $scope.getAllCompanies();
        //------------
        $uibModal.open({
            scope: $scope,
            template: View_Companies,
            resolve: {
                Srch_Company: function () {
                    return $scope.Srch_Company;
                }
            }
        });
    };
    $scope.clicktr = function (row) {
        $scope.Srch_Company.Company_id = row.ID;
        $scope.Srch_Company.Company_Name = row.Name;
        $timeout(function () { angular.element('#closebox').trigger('click'); }, 0);
        this.$close();
    }
    $scope.closeUiModal = function () {
        this.$close();
    }

    $scope.getAllCompanies = function (Page, Search) {

        var parameter = { 'Search': Search }
        var sentData = "page=" + Page;
        $http.get("/Company/getAll?" + sentData, { params: parameter })
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
    var View_Companies;
    $scope.getView = function () {
        $http.get("/Product/GetCompanies")
         .success(function (response) {
             View_Companies = response;
         });
    };
    $scope.getView();

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
        $window.location.href = '/Invoice_Company/Invoice_Company?id=' + response.ID;
    }
});