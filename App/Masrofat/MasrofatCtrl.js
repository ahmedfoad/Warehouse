app.controller("MasrofatCtrl", function ($scope, $http, $window, $timeout, $uibModal) {
    //----------------------------------------------------------------------------------------------------------------------------
    //----------------------------------------------------------------------------------------------------------------------------
    // switch between Inser && edit 
    //----------------------------------------------------------------------------------------------------------------------------
   
    $scope.Masrofat = { ID: 0, Date_Invoice: "", Money: 0, Masrofat_Type_Id: "", Bian: "", Notes: "", InDate: "", EditDate: "", Userid_In: "", userid_Edit: "" };
    
 
    $scope.hideElement = function (arr) {
        for (var i = 0; i < arr.length; i++) {
            document.getElementById(arr[i]).style.display = 'none';
        }
    }
    $scope.ReadOnlyInputs = function (arr) {
        for (var i = 0; i < arr.length; i++) {
            document.getElementById(arr[i]).readOnly = true;
        }
    }
   
    $scope.switchCase = function () {
        var QueryString = location.search;
        var urlParams = new URLSearchParams(QueryString);
        if (urlParams.has('id') == false) {
            var id = urlParams.get('id');    // 1234
            $scope.SaveBtn = null;
            $scope.EditBtn = 1;
            $scope.DeleteBtn = 1;
            $scope.dataEntire = null;
        }
        else {
            debugger;
            var id = urlParams.get('id');    // 1234
            $scope.SaveBtn = 1;
            $scope.dataEntire = 1;
            $scope.EditBtn = null;
            $scope.DeleteBtn = null;


            //document.getElementById('PrintTicket').href = "/Masrofat/PrintTicketReport/" + id;
            //document.getElementById('MasrofatRefer').href = "/Masrofat/Refer/" + id;
            $http.get('/Masrofat/loadMasrofat?id=' + id)
               .success(function (response) {
                   if (response == null) {
                       $uibModal.open({
                           template: " <div class='portlet box red' style='min-height:150px !important;min-width: 541px !important;'> <div class='portlet-title'> <div class='caption'> <i class='fa-lg fa fa-warning'></i> تنبيه هام </div><div class='tools'> </div></div><div class='portlet-body form portlet-empty' style='min-height: 140px;'> <div class='col-md-12 page-404'> <div class='number'>" + response.ErrorNumber + "</div><div class='details'> <h3 style='font-family: sans-serif; text-align: center; font-weight: bold;'>" + response.ErrorFullNumber + "</h3> <p style='font-size: 12px;'> " + response.ErrorName + " <br/> <br/> <a class='btn btn-default btn-sm' href='/home'> الصفحة الرئيسية </a> <a class='btn btn-default btn-sm' href='" + response.Url + "' style='margin-right: 4px;'>المحاولة مرة آخري</a> </p></div></div></div></div>",
                           closeOnEscape: false,
                           closeOnOverlayClick: false
                       });
                   }
                   else {

                       $scope.Masrofat = response;
                   }
               });

        }
    }
    $scope.switchCase();
    

    


    //----------------------------------------------------------------------------------------------------------------------------
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
                Masrofat: function () {
                    return $scope.Masrofat;
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
    //------------------تحميل الشركات بعد فتح النافذة
   
    //===================
    //=======عند الضفط على صف
    $scope.clicktr = function (row) {

        $scope.Masrofat.Masrofat_Type_Id = row.ID;
        $scope.Masrofat.Masrofat_Type_Name = row.Name;
        $timeout(function () { angular.element('#closebox').trigger('click'); }, 0);
        this.$close();
    }

   
    $scope.closeUiModal = function () {
        this.$close();
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


    //----------------------------------------------------------------------------------------------------------------------------

    //----------------------------------------------------------------------------------------------------------------------------
    // Save Export List
    //----------------------------------------------------------------------------------------------------------------------------
    $scope.ReomveAlert = function () {
        $scope.AlertParameter = null;
    }
    $scope.changeBorder = function (arr, color) {
        for (var i = 0; i < arr.length; i++) {
            document.getElementById(arr[i]).style.borderColor = color;
        }
    }
    $scope.FormValidation = function () {

        debugger;

        if ($scope.Masrofat.Date_Invoice == null || $scope.Masrofat.Date_Invoice == "") {
            $scope.ErrorName = "برجاء ادخال تاريخ الصرف"
            var ListID = ["Date_Invoice"];
            $scope.changeBorder(ListID, "red");
            return false;
        } else {
            var ListID = ["Date_Invoice"];
            $scope.changeBorder(ListID, "#c2cad8");
        }

        if ($scope.Masrofat.Masrofat_Type_Id == null || $scope.Masrofat.Masrofat_Type_Id == "") {
            $scope.ErrorName = "برجاء ادخال اسم الشركة"
            var ListID = ["Masrofat_Type_Name"];
            $scope.changeBorder(ListID, "red");
            return false;
        } else {
            var ListID = ["Masrofat_Type_Name"];
            $scope.changeBorder(ListID, "#c2cad8");
        }
     
      if ($scope.Masrofat.Money == null || $scope.Masrofat.Money == "" || $scope.Masrofat.Money == 0) {
            $scope.ErrorName = "سعر البيع غير صحيح";
            var ListID = ["Money"];
            $scope.changeBorder(ListID, "red");
            return false;
        } else {
            var ListID = ["Money"];
            $scope.changeBorder(ListID, "#c2cad8");
        }

        $scope.AlertParameter = null;
        return true;
    }
    $scope.SaveMasrofat = function () {
        debugger;
        $scope.Masrofat.Date_Invoice = $("#Date_Invoice").val();
        //$scope.Masrofat.Money = parseFloat($("#Money").val()).toFixed(2);
        var CheckForm = $scope.FormValidation();
        if (CheckForm == true) {
            $scope.DBsave();
        }
        else {
            $scope.AlertParameter = 1;
            document.body.scrollTop = 0;
        }

    }
    $scope.DBsave = function () {
        var QueryString = location.search;
        var urlParams = new URLSearchParams(QueryString);
        var parameter = JSON.stringify($scope.Masrofat);
        if (urlParams.has('id') == false) {
            var MasrofatID = urlParams.get('id');    // 1234
           
            $http.post('/Masrofat/Insert', parameter)
                 .success(function (response) {
                     if (response.ErrorName.indexOf("صلاحية") > -1 || response.ErrorName.indexOf("المسموح") > -1) {
                         $uibModal.open({
                             template: " <div class='portlet box red' style='min-height:150px !important ;min-width: 541px !important;'> <div class='portlet-title'> <div class='caption'> <i class='fa-lg fa fa-warning'></i> تنبيه هام </div><div class='tools'> </div></div><div class='portlet-body form portlet-empty' style='min-height: 140px;'> <div class='col-md-12 page-404'> <div class='number'>" + response.ErrorNumber + "</div><div class='details'> <h3 style='font-family: sans-serif; text-align: center; font-weight: bold;'>" + response.ErrorFullNumber + "</h3> <p> " + response.ErrorName + " <br/> <br/> <a class='btn btn-default btn-sm' href='/home'> الصفحة الرئيسية </a> </p></div></div></div></div>",
                             closeOnEscape: false,
                             closeOnOverlayClick: false
                         });
                     }
                     else {
                         $scope.ErrorName = response.ErrorName;
                         $scope.ID = response.ID;
                         $scope.AlertParameter = 1;
                         document.body.scrollTop = 0;
                         if (response.ErrorName.indexOf("بنجاح") > -1) {
                             $timeout(function () {
                                 $window.location.href = '/Masrofat/Operation?id=' + response.ID;
                             }, 500);
                         }
                     }
                 });
        }
        else {
            //Masrofat.BarCodeArr = null;
            $http.post('/Masrofat/Edit', parameter)
               .success(function (response) {
                   if (response.ErrorName.indexOf("صلاحية") > -1 || response.ErrorName.indexOf("المسموح") > -1) {
                       $uibModal.open({
                           template: " <div class='portlet box red' style='min-height:150px !important;min-width: 541px !important;'> <div class='portlet-title'> <div class='caption'> <i class='fa-lg fa fa-warning'></i> تنبيه هام </div><div class='tools'> </div></div><div class='portlet-body form portlet-empty' style='min-height: 140px;'> <div class='col-md-12 page-404'> <div class='number'>" + response.ErrorNumber + "</div><div class='details'> <h3 style='font-family: sans-serif; text-align: center; font-weight: bold;'>" + response.ErrorFullNumber + "</h3> <p> " + response.ErrorName + " <br/> <br/> <a class='btn btn-default btn-sm' href='/home'> الصفحة الرئيسية </a> </p></div></div></div></div>",
                           closeOnEscape: false,
                           closeOnOverlayClick: false
                       });
                   }
                   else {
                       $scope.ErrorName = response.ErrorName;
                       $scope.AlertParameter = 1;
                       document.body.scrollTop = 0;
                       if (response.ErrorName.indexOf("بنجاح") > -1) {
                           $timeout(function () {
                               $window.location.href = '/Masrofat/Operation?id=' + response.ID;
                           }, 500);
                       }
                   }
               });
        }
    }
    $scope.DeleteMasrofat = function () {
        var QueryString = location.search;
        var urlParams = new URLSearchParams(QueryString);
        if (urlParams.has('id') == true) {
            var ID = urlParams.get('id');    // 1234
            $uibModal.open({
                template: " <div class='portlet light' style='max-width: 333px;'> <div class='portlet-title'><div class='caption font-red-sunglo'> <i class='fa fa-link font-red-sunglo'></i> <span class='caption-subject bold'> هل أنت متأكد من حذف المعاملة الصادرة</span> </div></div><div class='portlet-body'> <div class='row' style='text-align:center;margin-top:5px;'> <div class='form-actions'> <a class='btn btn-default red btn-sm' href='/Masrofat/Delete?id=" + ID + "'>حذف</a><a class='btn btn-default btn-sm fancymodal-close' style='margin-right: 4px;' href='#'> إلغاء </a> </div></div></div></div>",
                closeOnEscape: false,
                closeOnOverlayClick: false
            });
        }
    }
    //----------------------------------------------------------------------------------------------------------------------------
    //----------------------------------------------------------------------------------------------------------------------------
    // Today
    //----------------------------------------------------------------------------------------------------------------------------
    $scope.BtnPrintInvoice = function () {

        if ($scope.Masrofat != null && $scope.Masrofat.ID != "" && $scope.Masrofat.ID != 0) {


            $http({
                url: "/Masrofat/PdfInvoice?id=" + $scope.Masrofat.ID,
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

        }
    };
    $scope.BtnNew = function () {
        $window.location.href = '/Masrofat/Operation/';
    };
    $scope.today = function () {
        $timeout(function () { angular.element('#DatExport').trigger('focus'); }, 0);
        $timeout(function () {
            var Day = document.getElementsByClassName("calendars-today")[0].textContent;
            if (Day.length == 1) { Day = "0" + Day }
            var YM = document.getElementsByClassName("calendars-month-year")[0];
            var Month = YM.options[YM.selectedIndex].value.split("/")[0];
            if (Month.length == 1) { Month = "0" + Month }
            var Year = YM.options[YM.selectedIndex].value.split("/")[1];
            $scope.Masrofat.DatExport = Year + "/" + Month + "/" + Day;
            var x = document.getElementsByClassName("calendars-cmd-close")[0].click();

            $timeout(function () { angular.element('.calendars-cmd-close').trigger('click'); }, 0);
        }, 400);
    }

    document.onkeydown = function () {

        var f2 = 113;
        var f4 = 115;
        var f8 = 119;
        if (window.event && window.event.keyCode == f2) { //طباعة باركود الصنف
            $scope.BtnPrintInvoice();
        }
        
    }

   
});

