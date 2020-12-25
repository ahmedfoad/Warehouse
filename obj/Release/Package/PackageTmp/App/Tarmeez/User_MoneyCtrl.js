app.controller("User_MoneyCtrl", function ($scope, $http, $window, $timeout, $uibModal) {
    //----------------------------------------------------------------------------------------------------------------------------
    //----------------------------------------------------------------------------------------------------------------------------
    // switch between Inser && edit 
    //----------------------------------------------------------------------------------------------------------------------------
    // $scope.User_Money = { ID: 0, Name: "", Barcode: 0, Company_Id: "", Price_Unit: "", Taxes: "", image:""};
    $scope.User_Money = { ID: 0, Sadad_Type_ID: "", Sadad_Type_Name: "", User_Id: "", User_Name: "", Money_Invoice_Mandob: 0, Money_Sarf: "", Money_Sandok: "", Money_Mada: "", Money_Remain: "", Save_Date: "" };
    $scope.LoadData = function () {
        debugger;
        var QueryString = location.search;
        var urlParams = new URLSearchParams(QueryString);
        if (urlParams.has('id') == false) {
            
            $http.get("/UserMoney/LoadData/")
                     .success(function (response) {
                             $scope.User_Money.User_Name = response.User_Name;
                             $scope.User_Money.Sadad_Type_Name = response.Sadad_Type_Name;
                             $scope.User_Money.Money_Invoice_Mandob = response.Money_Invoice_Mandob;

                             if (response.Money_Invoice_Mandob == 0) {
                             // **** الوردية متصفرة و مفيش حد اشتغل
                             $scope.BtnSaveShow = 1;
                             $scope.BtnPrintShow = 1;
                             $scope.ErrorName = "تم بالفعل تصفير صندوق الكاشير  ، يرجي بيع الاصناف الى المندوب اولا ثم قم بتصفير الايرادات اليومية مرة آخري ";
                             var ListID = ["Money_Sandok"];
                             $scope.changeBorder(ListID, "red");
                             $scope.AlertParameter = 1;
                             return false;
                         }

                     });
        }
    }
    $scope.LoadData();
    $scope.switchCase = function () {
        debugger;
        var QueryString = location.search;
        var urlParams = new URLSearchParams(QueryString);
        if (urlParams.has('id') == true) {
            var id = urlParams.get('id');    // 1234
            $http.get('/UserMoney/loadUserMoney?id=' + id)
                   .success(function (response) {
                       if (response == null) {
                           $uibModal.open({
                               template: " <div class='portlet box red' style='min-height:150px !important;min-width: 541px !important;'> <div class='portlet-title'> <div class='caption'> <i class='fa-lg fa fa-warning'></i> تنبيه هام </div><div class='tools'> </div></div><div class='portlet-body form portlet-empty' style='min-height: 140px;'> <div class='col-md-12 page-404'> <div class='number'>" + response.ErrorNumber + "</div><div class='details'> <h3 style='font-family: sans-serif; text-align: center; font-weight: bold;'>" + response.ErrorFullNumber + "</h3> <p style='font-size: 12px;'> " + response.ErrorName + " <br/> <br/> <a class='btn btn-default btn-sm' href='/home'> الصفحة الرئيسية </a> <a class='btn btn-default btn-sm' href='" + response.Url + "' style='margin-right: 4px;'>المحاولة مرة آخري</a> </p></div></div></div></div>",
                               closeOnEscape: false,
                               closeOnOverlayClick: false
                           });
                       }
                       else {

                           $scope.User_Money = {
                               ID: response.ID,
                               Sadad_Type_ID: response.Sadad_Type_ID,
                               Sadad_Type_Name: response.Sadad_Type_Name,
                               User_Id: response.User_Id,
                               User_Name: response.User_Name,
                               Money_Invoice_Mandob: response.Money_Invoice_Mandob,
                               Money_Sarf: response.Money_Sarf,
                               Money_Sandok: response.Money_Sandok,
                               Money_Mada: response.Money_Mada,
                               Money_Remain: response.Money_Remain,
                               Save_Date: response.Save_Date
                           };

                       }
                   });
            
        }
    }
    $scope.switchCase();

    var View_Report;
    $scope.BtnUser_MoneyZero = function () {

        if ($scope.User_Money != null && $scope.User_Money.ID != "" && $scope.User_Money.ID != 0) {

            //$http.get("/UserMoney/PrintUserMoney/" + $scope.User_Money.ID)
            // .success(function (response) {
            //     View_Report = response;

            //     $uibModal.open({
            //         scope: $scope,
            //         template: View_Report,
            //         resolve: {
            //             Cls_Invoice_Mandob: function () {
            //                 return $scope.Cls_Invoice_Mandob;
            //             }
            //         }
            //     });


            // });

            $http({
                url: "/UserMoney/PDF_Usermoney/" + $scope.User_Money.ID,
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
                printJS(pdfUrl);
            }).error(function (data, status, headers, config) {
                alert("عذرا، هناك خطأ ما");
            });
        }
    };





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
            $scope.getAllRecords(null, search);
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

        var Money_SandokCheck = $scope.User_Money.Money_Sandok.match(/^[0-9]+(\.[0-9]{1,2})?$/);
        var Money_MadaCheck = $scope.User_Money.Money_Mada.match(/^[0-9]+(\.[0-9]{1,2})?$/);
        //-------------------------------------------------------
        if (Money_SandokCheck == null) {
            $scope.ErrorName = "إجمالي الكاشير غير صحيح";
            var ListID = ["Money_Sandok"];
            $scope.changeBorder(ListID, "red");
            return false;
        } else if ($scope.User_Money.Money_Sandok == null || $scope.User_Money.Money_Sandok == "" || $scope.User_Money.Money_Sandok <= 0) {
            $scope.ErrorName = "إجمالي الكاشير غير صحيح";
            var ListID = ["Money_Sandok"];
            $scope.changeBorder(ListID, "red");
            return false;
        } else {
            var ListID = ["Money_Sandok"];
            $scope.changeBorder(ListID, "#c2cad8");
        }
        //----------------------------------------------------------------
        if (Money_MadaCheck == null) {
            $scope.ErrorName = "إجمالي المدي غير صحيح";
            var ListID = ["Money_Mada"];
            $scope.changeBorder(ListID, "red");
            return false;
        } else if ($scope.User_Money.Money_Mada == null || $scope.User_Money.Money_Mada == "" || $scope.User_Money.Money_Sandok < 0) {
            $scope.ErrorName = "إجمالي المدي غير صحيح";
            var ListID = ["Money_Mada"];
            $scope.changeBorder(ListID, "red");
            return false;
        } else {
            var ListID = ["Money_Mada"];
            $scope.changeBorder(ListID, "#c2cad8");
        }

        $scope.AlertParameter = null;
        return true;
    }
    $scope.Save = function () {

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
        var parameter = JSON.stringify($scope.User_Money);
        if (urlParams.has('id') == false) {
            var UserMoneyID = urlParams.get('id');    // 1234
           
            $http.post('/UserMoney/Insert', parameter)
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
                         $scope.User_Money.ID = response.ID;
                         $scope.AlertParameter = 1;
                         document.body.scrollTop = 0;
                         if (response.ErrorName.indexOf("بنجاح") > -1) {
                             $timeout(function () {
                                 $window.location.href = '/UserMoney/Finish?id=' + response.ID;
                             }, 500);
                         }
                     }
                 });
        }
        else {
            //UserMoney.BarCodeArr = null;
            $http.post('/UserMoney/Edit', parameter)
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
                               $window.location.href = '/UserMoney/Finish?id=' + response.ID;
                           }, 500);
                       }
                   }
               });
        }
    }
    $scope.Calculate = function () {
        if (isNaN(parseFloat($scope.User_Money.Money_Sandok)) == false
            && isNaN(parseFloat($scope.User_Money.Money_Mada)) == false) {
            $scope.User_Money.Money_Remain = ((parseFloat($scope.User_Money.Money_Sandok) + parseFloat($scope.User_Money.Money_Mada)) - parseFloat(parseFloat($scope.User_Money.Money_Invoice_Mandob))).toFixed(2);
        }
    }
    document.onkeydown = function () {

        var f2 = 113;

        if (window.event && window.event.keyCode == f2) { //طباعة تصفير الايرادات اليومية
            $scope.BtnUser_MoneyZero();
        }

    }
});

