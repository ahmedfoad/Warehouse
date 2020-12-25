var page = 0;
app.controller("AllProductsCtrl", function ($scope, $http, $window, $timeout, $fancyModal) {
    $scope.Srch = { Company_Id: "", Company_Name: "", ProductName: "" };

    
    //----------------------------------------------------------------------------------------------------------------------------
    // load all popuop data
    //----------------------------------------------------------------------------------------------------------------------------
    var Departmentview, DealingTypeView, SubjectView;
    $scope.LoadDepartment = function (inputType) {
        $fancyModal.open({
            template: Departmentview + "<p id='inputType' style='display: none;'>" + inputType + "</p>",
            closeOnEscape: false,
            closeOnOverlayClick: false
        });
    };
    $scope.LoadDealingType = function () {
        $fancyModal.open({
            template: DealingTypeView,
            closeOnEscape: false,
            closeOnOverlayClick: false
        });
    };
    $scope.LoadSubject = function () {
        $fancyModal.open({
            template: SubjectView,
            closeOnEscape: false,
            closeOnOverlayClick: false
        });
    };
  
    $scope.clicktr = function (row) {
        $scope.Srch.Company_id = row.ID;
        $scope.Srch.Company_Name = row.Name;
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
    $scope.loadCompanies = function () {
        //-----
        $scope.data = null;
        $scope.getAllCompanies();
        //------------
        $uibModal.open({
            scope: $scope,
            template: View_Companies,
            resolve: {
                Srch: function () {
                    return $scope.Srch;
                }
            }
        });
    };
    $scope.getView = function () {
        $http.get("/Product/GetCompanies")
         .success(function (response) {
             View_Companies = response;
         });
    };
    $scope.getView();
    //-----------------------------------------------------------------------------------------------------------------------------
    //-----------------------------------------------------------------------------------------------------------------------------
    // Clear all Fields
    //-----------------------------------------------------------------------------------------------------------------------------
    $scope.emptyInputs = function () {
        $scope.data = null;
        $scope.Srch.Company_id = "";
        $scope.Srch.Company_Name = "";
        $scope.Srch.ProductName  = "";
  
    }
    //-----------------------------------------------------------------------------------------------------------------------------
    //-----------------------------------------------------------------------------------------------------------------------------
    // Get AllRecords Functions
    //-----------------------------------------------------------------------------------------------------------------------------
    $scope.getAllRecords = function (Page) {
        $scope.EndSearch = null;
        $scope.TableLoading = 1;

        // $("input[name='Date_Poduction[" + i +"]']").val()
        // Srch.DatEnd = document.getElementById("DatEnd").value;
        var parameter = JSON.stringify({ 'Srch_Product': $scope.Srch, 'page': page });
        $http.post("/Search/Product", parameter)
    .success(function (response) {
        $scope.TableLoading = null;
        if (response.length >= 1) {

            if (page == 0 || $scope.data == null) {

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
            $fancyModal.open({
                template: " <div class='portlet box red' style='min-height:150px !important;min-width: 541px !important;'> <div class='portlet-title'> <div class='caption'> <i class='fa-lg fa fa-warning'></i> تنبيه هام </div><div class='tools'> </div></div><div class='portlet-body form portlet-empty' style='min-height: 140px;'> <div class='col-md-12 page-404'> <div class='number'>" + response.ErrorNumber + "</div><div class='details'> <h3 style='font-family: sans-serif; text-align: center; font-weight: bold;'>" + response.ErrorFullNumber + "</h3> <p style='font-size: 12px;'> " + response.ErrorName + " <br/> <br/> <a class='btn btn-default btn-sm' href='/home'> الصفحة الرئيسية </a> <a class='btn btn-default btn-sm' href='" + response.Url + "' style='margin-right: 4px;'>المحاولة مرة آخري</a> </p></div></div></div></div>",
                closeOnEscape: false,
                closeOnOverlayClick: false
            });
        }
    });
    };
    //-----------------------------------------------------------------------------------------------------------------------------
    //-----------------------------------------------------------------------------------------------------------------------------
    // search button Functions
    //-----------------------------------------------------------------------------------------------------------------------------
    $scope.SearchButton = function (Srch) {
        $scope.data = null;
        page = 0;
        $scope.getAllRecords(Srch);
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
    //-----------------------------------------------------------------------------------------------------------------------------
    //-----------------------------------------------------------------------------------------------------------------------------
    // print button Functions
    //-----------------------------------------------------------------------------------------------------------------------------   
 

    $scope.BtnPrintInvoice = function () {

       
        $http({
            url: "/Report_Product/PdfProducts?id=" + "$scope.Masrofat.ID",
            method: "GET",
            headers: {
                "Content-type": "application/pdf"
            },
            responseType: "arraybuffer"
        }).success(function (data, status, headers, config) {
            var pdfFile = new Blob([data], {
                type: "application/pdf"
            });
            var pdfUrl = URL.createObjectURL(pdfFile);
            //window.open(pdfUrl);
            printJS(pdfUrl);
            //var printwWindow = $window.open(pdfUrl);
            //printwWindow.print();
        }).error(function (data, status, headers, config) {
            alert("عذرا، هناك خطأ ما");
        });

        
    };
});