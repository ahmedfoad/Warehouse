var page = 0;
app.controller("ArbahListCtrl", function ($scope, $http, $window, $timeout, $uibModal) {
    $scope.Srch_Eradat = { Date_Invoice_From: "", Date_Invoice_TO: "" };
    //----------------------------------------------------------------------------------------------------------------
    $scope.emptyInputs = function () {
        $scope.Srch_Eradat.Date_Invoice_From = "";
        $scope.Srch_Eradat.Date_Invoice_TO = "";
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
        var parameter = JSON.stringify({ 'RModel': $scope.Srch_Eradat, 'page': page });
        $http.post("/Report_Arbah/Arbah", parameter)
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
        debugger;
        page = 0;
        $scope.data = null;
        $scope.Srch_Eradat.Date_Invoice_From = $("#Date_Invoice_From").val();
        $scope.Srch_Eradat.Date_Invoice_TO = $("#Date_Invoice_TO").val();
        $scope.getAllRecords(page);
    };

    
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
    

    $scope.BtnPrint = function () {
        debugger;
        $scope.Srch_Eradat.Date_Invoice_From = $("#Date_Invoice_From").val();
        $scope.Srch_Eradat.Date_Invoice_TO = $("#Date_Invoice_TO").val();
        var myurl = "/Report_Arbah/Pdf?Date_Invoice_From=" + $scope.Srch_Eradat.Date_Invoice_From + "&Date_Invoice_TO=" + $scope.Srch_Eradat.Date_Invoice_TO;
        $http({
            url: myurl,
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