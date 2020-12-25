app.controller("ProductCtrl", function ($scope, $http, $window, $timeout, $uibModal) {
    $scope.Product_For = "";
    //----------------------------------------------------------------------------------------------------------------------------
    //----------------------------------------------------------------------------------------------------------------------------
    // switch between Inser && edit 
    //----------------------------------------------------------------------------------------------------------------------------
    // $scope.Product = { ID: 0, Name: "", Barcode: 0, Company_Id: "", Price_Unit: "", Taxes: "", OldAmount: "", image:""};
    $scope.Product = {
        ID: 0, Name: "", Barcode: "", Company_Id: "", Company_Name: "",
        Price_Unit: "", Taxes: 5, OldAmount: "", Price_Mowrid: "", image: "",
        ComputerName: "", ComputerUser: "", InDate: ""
        , Offer_TargetAmount: "", Offer_BonusAmount: "", Offer_Product_id: "", Offer_Product_Name: ""
        , Shop_Offer_TargetAmount: "", Shop_Offer_BonusAmount: "", Shop_Offer_Product_id: "", Shop_Offer_Product_Name: "", Shop_Price:0
        , Home_TargetAmount: "", Home_Offer_BonusAmount: "", Home_Offer_Product_id: "", Home_Offer_Product_Name: "", Home_Price:0
    };
    /*
    $http.get("/Product/GetMaxBarcode")
          .success(function (response) {
              $scope.Product.Barcode = response;
          });
    $scope.GetBarcode = function () {
        $http.get("/Product/GetMaxBarcode")
          .success(function (response) {
              $scope.Product.Barcode = response;
          });
    }*/

    $scope.words = [];
    $scope.triggerChar = 9;
    $scope.separatorChar = 13;
    $scope.triggerCallback = function () {
        $scope.lastTrigger = 'Last trigger callback: ' + new Date().toISOString();

    };
    $scope.scanCallback = function (word) {
        // $scope.words.push(word);
        //  $scope.Cls_Product[0].Name = word;
        $http.get("/Product/GetbyBarcode?BarCode=" + word)
      .success(function (response) {
          if (response == null) {
              debugger;

              index = $scope.Cls_Product.length - 1;
              if ($scope.Product.Barcode == "") {
                  $scope.Product.Barcode = word;
              }  
          } else {
              //البحث هل الصنف ده موجود قبل كدة 
              alert("عفوا هذا الصنف تم حفظه من قبل براجاء اختيار صنف جديد");
          }
          //if (response == null || response == "") { otherData = 1; hideLoadMore = null; }
          //else {  otherData = null; hideLoadMore = 1; }
      });

    };
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
    $scope.arrayBufferToBase64 = function (buffer) {
        var binary = '';
        var bytes = new Uint8Array(buffer);
        var len = bytes.byteLength;
        for (var i = 0; i < len; i++) {
            binary += String.fromCharCode(bytes[i]);
        }
        return window.btoa(binary);
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
            var hiddenElements = ["Numbers", "EditElements2"];
            $scope.hideElement(hiddenElements);
            //$http.get("/Product/InitialOperationForm")
            //.success(function (response) {
            //    $scope.Product = response;
            //    $scope.ProductType = "Public";
            //    if (response.SessionAdmin == "1") { $scope.public = 1; $scope.Admin = 1; }
            //    if (response.SessionMajless == "1") { $scope.public = 1; $scope.Majless = 1; }
            //    if (response.SessionMajlessAdmin == "1") { $scope.public = 1; $scope.MajlessAdmin = 1; }
            //    if (response.SessionAdminEmp == "1") { $scope.public = 1; $scope.AdminEmp = 1; }
            //    $scope.today();
            //});
        }
        else {
            debugger;
            var id = urlParams.get('id');    // 1234
            $scope.SaveBtn = 1;
            $scope.dataEntire = 1;
            $scope.EditBtn = null;
            $scope.DeleteBtn = null;


            //document.getElementById('PrintTicket').href = "/Product/PrintTicketReport/" + id;
            //document.getElementById('ProductRefer').href = "/Product/Refer/" + id;
            $http.get('/Product/loadProduct?id=' + id)
               .success(function (response) {
                   if (response == null) {
                       $modelDialog = $uibModal.open({
                           template: " <div class='portlet box red' style='min-height:150px !important;min-width: 541px !important;'> <div class='portlet-title'> <div class='caption'> <i class='fa-lg fa fa-warning'></i> تنبيه هام </div><div class='tools'> </div></div><div class='portlet-body form portlet-empty' style='min-height: 140px;'> <div class='col-md-12 page-404'> <div class='number'>" + response.ErrorNumber + "</div><div class='details'> <h3 style='font-family: sans-serif; text-align: center; font-weight: bold;'>" + response.ErrorFullNumber + "</h3> <p style='font-size: 12px;'> " + response.ErrorName + " <br/> <br/> <a class='btn btn-default btn-sm' href='/home'> الصفحة الرئيسية </a> <a class='btn btn-default btn-sm' href='" + response.Url + "' style='margin-right: 4px;'>المحاولة مرة آخري</a> </p></div></div></div></div>",
                           closeOnEscape: false,
                           closeOnOverlayClick: false
                       });
                   }
                   else {

                       $scope.Product = response.ClsProduct;

                       // $('#Company_Id').val(response.ClsProduct.Company_Id);
                       //$('#Company_Name').val(response.ClsProduct.Company_Name);
                       //  document.getElementById('BarcodNumber').innerHTML = response.ClsProduct.Barcode;
                       document.getElementById("BarcodeImg").src = "data:image/png;base64," + response.BarCodeArr;

                   }
               });

        }
    }
    $scope.switchCase();
    var View_Report;
    $scope.BtnProductBarcode = function () {

        if ($scope.Product != null && $scope.Product.ID != "" && $scope.Product.ID != 0) {

            $http.get("/Product/PrintBarCodeReport/" + $scope.Product.ID)
             .success(function (response) {
                 View_Report = response;

                 $modelDialog = $uibModal.open({
                     scope: $scope,
                     template: View_Report,
                     resolve: {
                         Cls_Invoice_Mandob: function () {
                             return $scope.Cls_Invoice_Mandob;
                         }
                     }
                 });


             });
        }
    };

    


    //----------------------------------------------------------------------------------------------------------------------------
    //----------------------------------------------------------------------------------------------------------------------------
    // load all popuop data
    //----------------------------------------------------------------------------------------------------------------------------
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
        $scope.getAllRecords();
        //------------
        $modelDialog = $uibModal.open({
            scope: $scope,
            template: View_Companies,
            resolve: {
                Product: function () {
                    return $scope.Product;
                }
            }
        });
    };

    //************************************************
    var View_Products;
    $scope.getProductsView = function () {
        $http.get("/Product/GetProducts")
         .success(function (response) {
             View_Products = response;
         });
    };
    $scope.getProductsView();
    $scope.popup_Products = function (Product_for) {
        $scope.Product_For = Product_for;
        //-----
        $scope.ProductList = null;
        $scope.getAllProducts();
        //------------
        $modelDialog = $uibModal.open({
            scope: $scope,
            template: View_Products,
            resolve: {
                ProductList: function () {
                    return $scope.ProductList;
                },
                Product_For: function () {
                    return $scope.Product_For;
                }
            }
        });
    };
    $scope.getAllProducts = function (Page, Search) {

        var parameter = { 'Search': Search};
        var sentData = "page=" + Page;
        $http.get("/Product/getAllProducts?" + sentData, { params: parameter })
       .success(function (response) {
           if (response != null) {

               if ($scope.ProductList == null) {
                   $scope.ProductList = response;
               }
               else {
                   $scope.ProductList = $scope.ProductList.concat(response);
               }
           }
           //if (response == null || response == "") { otherData = 1; hideLoadMore = null; }
           //else {  otherData = null; hideLoadMore = 1; }
       });
    }
    //------------------تحميل الشركات بعد فتح النافذة
    $scope.getAllRecords = function (Page, Search) {

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
    // $scope.getAllRecords();
    //===================
    //=======عند الضفط على صف
    $scope.clicktr = function (row) {

        $scope.Product.Company_Id = row.ID;
        $scope.Product.Company_Name = row.Name;
        $modelDialog.close();
        this.$close();
    }

    $scope.clicktr_Products = function (row) {
        
        if ($scope.Product_For == "Mandob") {
            $scope.Product.Offer_Product_id = row.ID;
            $scope.Product.Offer_Product_Name = row.Name;
        } else if ($scope.Product_For == "Shop") {
            $scope.Product.Shop_Offer_Product_id = row.ID;
            $scope.Product.Shop_Offer_Product_Name = row.Name;
        } else if ($scope.Product_For == "Home") {
            $scope.Product.Home_Offer_Product_id = row.ID;
            $scope.Product.Home_Offer_Product_Name = row.Name;
        }
        $modelDialog.close();
    }
    $scope.closeUiModal = function () {
        $modelDialog.close();
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

        debugger;

        if ($scope.Product.Name == null || $scope.Product.Name == "") {
            $scope.ErrorName = "برجاء ادخال اسم الصنف"
            var ListID = ["Name"];
            $scope.changeBorder(ListID, "red");
            return false;
        } else {
            var ListID = ["Name"];
            $scope.changeBorder(ListID, "#c2cad8");
        }

        if ($scope.Product.Company_Id == null || $scope.Product.Company_Id == "") {
            $scope.ErrorName = "برجاء ادخال اسم الشركة"
            var ListID = ["Company_Name"];
            $scope.changeBorder(ListID, "red");
            return false;
        } else {
            var ListID = ["Company_Name"];
            $scope.changeBorder(ListID, "#c2cad8");
        }
       ;
        var Price_UnitCheck = String($scope.Product.Price_Unit).match(/^[0-9]+(\.[0-9]{1,2})?$/);
        if (Price_UnitCheck == null) {
            $scope.ErrorName = "سعر البيع غير صحيح";
            var ListID = ["Price_Unit"];
            $scope.changeBorder(ListID, "red");
            return false;
        } else if ($scope.Product.Price_Unit == null || $scope.Product.Price_Unit == "" || $scope.Product.Price_Unit == 0) {
            $scope.ErrorName = "سعر البيع غير صحيح";
            var ListID = ["Price_Unit"];
            $scope.changeBorder(ListID, "red");
            return false;
        } else {
            var ListID = ["Price_Unit"];
            $scope.changeBorder(ListID, "#c2cad8");
        }

        var Price_MowridCheck = String($scope.Product.Price_Mowrid).match(/^[0-9]+(\.[0-9]{1,2})?$/);
        if (Price_MowridCheck == null) {
            $scope.ErrorName = "سعر الشراء غير صحيح";
            var ListID = ["Price_Mowrid"];
            $scope.changeBorder(ListID, "red");
            return false;
        } else if ($scope.Product.Price_Mowrid == null || $scope.Product.Price_Mowrid == "" || $scope.Product.Price_Mowrid == 0) {
            $scope.ErrorName = "سعر الشراء غير صحيح";
            var ListID = ["Price_Mowrid"];
            $scope.changeBorder(ListID, "red");
            return false;
        } else {
            var ListID = ["Price_Unit"];
            $scope.changeBorder(ListID, "#c2cad8");
        }
       

        //if ($scope.Product.Barcode == null || $scope.Product.Barcode == "" || $scope.Product.Barcode == 0) {
        //    $scope.ErrorName = "برجاء ادخال الباركود"
        //    var ListID = ["Barcode"];
        //    $scope.changeBorder(ListID, "red");
        //    return false;
        //} else {
        //    var ListID = ["Barcode"];
        //    $scope.changeBorder(ListID, "#c2cad8");
        //}



        if ($scope.Product.Taxes == null || $scope.Product.Taxes == "") {
            $scope.ErrorName = "برجاء اختيار القيمة المضافة"
            var ListID = ["Taxes"];
            $scope.changeBorder(ListID, "red");

            return false;
        } else {
            var ListID = ["Taxes"];
            $scope.changeBorder(ListID, "#c2cad8");
        }



        if ($scope.Product.OldAmount == null || $scope.Product.OldAmount == "") {
            $scope.ErrorName = "برجاء ادخال الرصيد الإبتدائي للصنف"
            var ListID = ["OldAmount"];
            $scope.changeBorder(ListID, "red");
            return false;
        } else {
            var ListID = ["OldAmount"];
            $scope.changeBorder(ListID, "#c2cad8");
        }
        $scope.AlertParameter = null;
        return true;
    }
    $scope.SaveProduct = function () {
        $scope.Product.Taxes = document.getElementById("Taxes").value;
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
        var parameter = JSON.stringify($scope.Product);
        if (urlParams.has('id') == false) {
            var ProductID = urlParams.get('id');    // 1234
           
            $http.post('/Product/Insert', parameter)
                 .success(function (response) {
                     if (response.ErrorName.indexOf("صلاحية") > -1 || response.ErrorName.indexOf("المسموح") > -1) {
                         $modelDialog = $uibModal.open({
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
                                 $window.location.href = '/Product/Operation?id=' + response.ID;
                             }, 500);
                         }
                     }
                 });
        }
        else {
            //Product.BarCodeArr = null;
            $http.post('/Product/Edit', parameter)
               .success(function (response) {
                   if (response.ErrorName.indexOf("صلاحية") > -1 || response.ErrorName.indexOf("المسموح") > -1) {
                       $modelDialog = $uibModal.open({
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
                               $window.location.href = '/Product/Operation?id=' + response.ID;
                           }, 500);
                       }
                   }
               });
        }
    }
    $scope.DeleteProduct = function () {
        var QueryString = location.search;
        var urlParams = new URLSearchParams(QueryString);
        if (urlParams.has('id') == true) {
            var ID = urlParams.get('id');    // 1234
            $modelDialog = $uibModal.open({
                template: " <div class='portlet light' style='max-width: 333px;'> <div class='portlet-title'><div class='caption font-red-sunglo'> <i class='fa fa-link font-red-sunglo'></i> <span class='caption-subject bold'> هل أنت متأكد من حذف المعاملة الصادرة</span> </div></div><div class='portlet-body'> <div class='row' style='text-align:center;margin-top:5px;'> <div class='form-actions'> <a class='btn btn-default red btn-sm' href='/Product/Delete?id=" + ID + "'>حذف</a><a class='btn btn-default btn-sm fancymodal-close' style='margin-right: 4px;' href='#'> إلغاء </a> </div></div></div></div>",
                closeOnEscape: false,
                closeOnOverlayClick: false
            });
        }
    }
    //----------------------------------------------------------------------------------------------------------------------------
    //----------------------------------------------------------------------------------------------------------------------------
    // Today
    //----------------------------------------------------------------------------------------------------------------------------
    $scope.today = function () {
        $timeout(function () { angular.element('#DatExport').trigger('focus'); }, 0);
        $timeout(function () {
            var Day = document.getElementsByClassName("calendars-today")[0].textContent;
            if (Day.length == 1) { Day = "0" + Day }
            var YM = document.getElementsByClassName("calendars-month-year")[0];
            var Month = YM.options[YM.selectedIndex].value.split("/")[0];
            if (Month.length == 1) { Month = "0" + Month }
            var Year = YM.options[YM.selectedIndex].value.split("/")[1];
            $scope.Product.DatExport = Year + "/" + Month + "/" + Day;
            var x = document.getElementsByClassName("calendars-cmd-close")[0].click();

            $timeout(function () { angular.element('.calendars-cmd-close').trigger('click'); }, 0);
        }, 400);
    }

    document.onkeydown = function () {

        var f2 = 113;
        var f4 = 115;
        var f8 = 119;
        if (window.event && window.event.keyCode == f2) { //طباعة باركود الصنف
            $scope.BtnProductBarcode();
        }
        
    }

   
});

