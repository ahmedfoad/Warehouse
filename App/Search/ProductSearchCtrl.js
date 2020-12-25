var page = 0;
app.controller("ProductSearchCtrl", function ($scope, $http, $window, $timeout, $uibModal) {
    $scope.Srch_Product = { Product_Name: "", Company_id: "", Price_From: "", Price_To: "", Price_Mowrid_From: "", Price_Mowrid_To: "", Barcode: "", Taxes: "", Taxes_Price: "" };
    //----------------------------------------------------------------------------------------------------------------------------
    //----------------------------------------------------------------------------------------------------------------------------
    // load all popuop data
    //----------------------------------------------------------------------------------------------------------------------------

    //----------------------------------------------------------------------------------------------------------------
    // Clear all Fields
    //----------------------------------------------------------------------------------------------------------------
    $scope.emptyInputs = function () {
        $scope.Srch_Product.Product_Name = "";
        $scope.Srch_Product.Company_id = "";
        $scope.Srch_Product.Company_Name = "";
        $scope.Srch_Product.Price_From = "";
        $scope.Srch_Product.Price_To = "";
        $scope.Srch_Product.Barcode_From = "";
        $scope.Srch_Product.Barcode_To = "";
        $scope.Srch_Product.Taxes = "";
    };

    //----------------------------------------------------------------------------------------------------------------------------
    //----------------------------------------------------------------------------------------------------------------------------
    // load all popuop data
    //----------------------------------------------------------------------------------------------------------------------------
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
    $scope.loadCompanies = function () {
        //-----
        $scope.data = null;
        $scope.getAllCompanies();
        //------------
        $uibModal.open({
            scope: $scope,
            template: View_Companies,
            resolve: {
                Srch_Product: function () {
                    return $scope.Srch_Product;
                }
            }
        });
    };
    $scope.clicktr = function (row) {
        $scope.Srch_Product.Company_id = row.ID;
        $scope.Srch_Product.Company_Name = row.Name;
        $timeout(function () { angular.element('#closebox').trigger('click'); }, 0);
        this.$close();
    }
     $scope.closeUiModal = function () {
        this.$close();
    }


    //----------------------------------------------------------------------------------------------------------------
    //----------------------------------------------------------------------------------------------------------------
    // Get AllRecords Functions
    //----------------------------------------------------------------------------------------------------------------
    $scope.getAllRecords = function (Page) {
        debugger;
        $scope.EndSearch = null;
        $scope.TableLoading = 1;
        
        // $("input[name='Date_Poduction[" + i +"]']").val()
        // ListExport.DatEnd = document.getElementById("DatEnd").value;
        var parameter = JSON.stringify({ 'Srch_Product': $scope.Srch_Product, 'page': page });
        $http.post("/Search/Product", parameter)
    .success(function (response) {
        $scope.TableLoading = null;
        if (response.length >= 1) {
            
            if (page == 0 || $scope.ProductList == null) {
               
                $scope.ProductList = response;

            }
            else {
               
                $scope.ProductList = $scope.ProductList.concat(response);
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
        page = 0;
        $scope.ProductList = null;
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
        $window.location.href = '/Product/Operation?id=' + response.ID;
    }
});