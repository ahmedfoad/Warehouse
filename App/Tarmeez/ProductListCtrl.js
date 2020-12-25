app.controller("ProductListCtrl", function ($scope, $http, $window, $timeout, $uibModal) {
    $scope.Intaize_product = function () {
        var Product = { ID: 0, Name: "", Barcode: "", Company_Id: "", Company_Name: "", Price_Unit: "", Taxes: "" , Price_Mowrid: "" };
        $scope.Cls_Product = new Array();
        
        $scope.Cls_Product.push(Product);
    };
     
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
          if (response != null) {
              debugger;
              alert("عفوا هذا الصنف تم حفظه من قبل براجاء اختيار صنف جديد");
              index = $scope.Cls_Product.length - 1;
              if ($scope.Cls_Product[index].Barcode != "") {
                  $scope.Cls_Product[index].Barcode = "";
              }
          } else {
              //البحث هل الصنف ده موجود قبل كدة 
              var index = $scope.Cls_Product.findIndex(function (item, i) {
                  return item.Barcode == word;
              });

              if (index == -1) {
                  //لو الصنف مش موجود ضيف سطر جديد
                  index = $scope.Cls_Product.length - 1;
                  if ($scope.Cls_Product[index].Barcode == "") {
                      $scope.Cls_Product[index].Barcode = parseFloat(word);
                  }  
                  
              } else {
                  // لو الصنف موجود فى جدول الانتظار ليتم حفظه
                  alert("عفوا هذا الصنف تم اضافته فى الجدول ليتم حفظه" + "\n" + "يرجى اختيار صنف جديد مع مراعاه عدم تكرار الصنف");

              }
          }
          //if (response == null || response == "") { otherData = 1; hideLoadMore = null; }
          //else {  otherData = null; hideLoadMore = 1; }
      });
       
    };

    $scope.AddNewItem = function () {
        var Product = { ID: 0, Name: "", Barcode: "", Company_Id: "", Company_Name: "", Price_Unit: "", Taxes: "" , Price_Mowrid: "" };
        debugger;
        $scope.Cls_Product.push(Product);
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
    $scope.loadCompanies = function (index) {
       
        //-----
        $scope.data = null;
        $scope.getAllRecords();
        //------------
        $scope.selected_Index = parseFloat(index);
        
        $uibModal.open({
            scope: $scope,
            template: View_Companies,
            resolve: {
                Cls_Product: function () {
                    return $scope.Cls_Product;
                }
            }
        });
    };
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
       
        $scope.Cls_Product[$scope.selected_Index].Company_Id = row.ID;
        $scope.Cls_Product[$scope.selected_Index].Company_Name = row.Name;
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
            $scope.getAllRecords(null, search);
        }
        lastentry = search;
    };
    //----------------------------------------------------------------------------------------------------------------------------
    
    //----------------------------------------------------------------------------------------------------------------------------
    //----------------------------------------------------------------------------------------------------------------------------
    //----------------------------------------------------------------------------------------------------------------------------

    //----------------------------------------------------------------------------------------------------------------------------
    //===================
    //=======عند الضفط على صف
  
    $scope.removeRow = function (index) {
        
        if ($scope.Cls_Product.length > 1) {
            $scope.Cls_Product.splice(index, 1);
        } else {
            $scope.Cls_Product[index].Barcode = "";
        }
    }
    $scope.closeUiModal = function () {
        this.$close();
    }
    //****************************************************************************
    $scope.changeBorder = function (arr, color) {
        for (var i = 0; i < arr.length; i++) {
            document.getElementById(arr[i]).style.borderColor = color;
        }
    }
    $scope.GetBarcode = function (i) {
        var index = parseFloat(i);
        $http.get("/Product/GetMaxBarcode")
          .success(function (response) {
              $scope.Cls_Product[i].Barcode = response;
              var Product = { ID: 0, Name: "", Barcode: "", Company_Id: "", Company_Name: "", Price_Unit: "", Taxes: ""  , Price_Mowrid: "" };
              debugger;
              $scope.Cls_Product.push(Product);
          });
    }
    $scope.changeBorderJQ = function (input, color) {
        $('input[name="' + input + '"]').css("borderColor", color);
    }
    $scope.Save = function () {

        var CheckForm = $scope.FormValidation();
        if (CheckForm == true) {
            $scope.DBsave($scope.Cls_Product);
        }
        else {
            $scope.AlertParameter = 1;
            document.body.scrollTop = 0;
        }

    }
    $scope.ReomveAlert = function () {
        $scope.AlertParameter = null;
    }
    $scope.FormValidation = function () {
        var len = $scope.Cls_Product.length;

       
        
        for (var i = 0; i < len; i++) {
            if (len = 1
                ||
               ((len > 1) && i < (len - 1))) {

                if ($scope.Cls_Product[i].Name == null || $scope.Cls_Product[i].Name == "") {
                    $scope.ErrorName = "برجاء ادخال اسم الصنف";
                    var ListID = ["Name[" + i +"]"];
                    $scope.changeBorderJQ(ListID, "red");
                    return false;
                }
                else {
                    var ListID = ["Name[" + i +"]"];
                    $scope.changeBorderJQ(ListID, "#c2cad8");
                }

                
                if ($scope.Cls_Product[i].Company_Id == null
                   || $scope.Cls_Product[i].Company_Id == "") {
                    $scope.ErrorName = "برجاء ادخال اسم الشركة";
                    var ListID = ["Company_Name[" + i + "]"];
                    $scope.changeBorderJQ(ListID, "red");
                    return false;
                }
                else {
                    var ListID = ["Company_Name[" + i + "]"];
                    $scope.changeBorderJQ(ListID, "#c2cad8");
                }

              

                if ($scope.Cls_Product[i].Price_Unit == null || $scope.Cls_Product[i].Price_Unit == "") {
                    $scope.ErrorName = "برجاء ادخال السعر";
                    var ListID = ["Price_Unit[" + i + "]"];
                    $scope.changeBorderJQ(ListID, "red");
                    return false;
                }
                else {
                    var ListID = ["Price_Unit[" + i + "]"];
                    $scope.changeBorderJQ(ListID, "#c2cad8");
                }

                if ($scope.Cls_Product[i].Taxes == null || $scope.Cls_Product[i].Taxes == "") {
                    $scope.ErrorName = "برجاء ادخال ضريبة القيمة المضافة";
                    var ListID = ["Taxes[" + i + "]"];
                    $scope.changeBorderJQ(ListID, "red");
                    return false;
                }
                else {
                    var ListID = ["Taxes[" + i + "]"];
                    $scope.changeBorderJQ(ListID, "#c2cad8");
                }
            }
           
        }
       
        
        return true;
    }
    $scope.DBsave = function (Mandob) {
        var parameter = JSON.stringify($scope.Cls_Product);
       
        $http.post('/Product/InsertProductList', parameter)
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
                                 $window.location.href = '/Product/ProductList/';
                             }, 500);
                         }
                     }
                 });
       
      
    }

});