app.controller("InvoiceCompanyCtrl", function ($scope, $http, $window, $timeout, $uibModal) {
    $scope.Search = "";
    $scope.page = 0;
    $scope.product_Index = -1;
    $scope.Cls_LogIn = { UserName: "", Password: "" };
    $scope.words = [];
    $scope.triggerChar = 9;
    $scope.separatorChar = 13;
    $scope.triggerCallback = function () {
        $scope.lastTrigger = 'Last trigger callback: ' + new Date().toISOString();
        $scope.$apply();
    };

    $scope.scanCallback = function (word) {
        
        // $scope.words.push(word);
        //  $scope.Cls_Invoice_Company.ClsInvoiceCompany_Product[0].Product_Name = word;
        
        var x1 = $("#Src_Barcode").val();
        var x2 = $("#Search").val();
        // var x2 = $("#Search").is(":focus");
        //للتحكم هل استخدام الباركود للحفظ ام للبحث عن صنف
        if (typeof(x1) == 'undefined' && typeof(x2) == 'undefined') { //معنى ذلك انه لا يتم البحث عن الصنف فيتم حفظ الصنف فى قاعدة البيانات 
            // يقوم كوداخر بجلب الصنف فى شاشة البحث عن الصنف بالباركود  falseاما فى حالة ال   
            var CheckForm = $scope.Validation_SavePrevProducts();
            if (CheckForm == false) {
                $scope.AlertParameter = 1;
                document.body.scrollTop = 0;
                return;
            }
            
            $http.get("/Product/GetbyBarcode_ForOrders?BarCode=" + word)
          .success(function (response) {

              if (response != null && response != "") {
                  //البحث هل الصنف ده موجود قبل كدة 
                  var index = $scope.Cls_Invoice_Company.ClsInvoiceCompany_Product.findIndex(function (item, i) {
                      return ((item.Product_Id === response.ID) && (item.Product_Name === response.Name));
                  });
                  
                  if (index == -1) {
                      //لو الصنف مش موجود ضيف سطر جديد بعدد الاصناف = 1
                      index = $scope.Cls_Invoice_Company.ClsInvoiceCompany_Product.length - 1;
                      $scope.Cls_Invoice_Company.ClsInvoiceCompany_Product[index].Product_Id = response.ID;
                      $scope.Cls_Invoice_Company.ClsInvoiceCompany_Product[index].Product_Name = response.Name;
                      $scope.Cls_Invoice_Company.ClsInvoiceCompany_Product[index].Amount = 1;

                    

                      //*******************************
                      //Saving ......
                      $scope.Calculate_itemPrice($scope.Cls_Invoice_Company.ClsInvoiceCompany_Product[index], index);
                      //************************************
                      var product = { ID: "", Product_Id: "", Product_Name: "", Amount: "", Price: "", Taxes:"", Taxes_Orginal : "", TotalPrice: "" };
                      $scope.Cls_Invoice_Company.ClsInvoiceCompany_Product.push(product);
                     
                  } else {
                      // لو الصنف موجود بنفس الاسم (وزن-او كرتون او حبة) نزود العدد +1
                      var x1 = $scope.Cls_Invoice_Company.ClsInvoiceCompany_Product[index].Product_Name;
                      var x2 = response.Name;
                      if ($scope.Cls_Invoice_Company.ClsInvoiceCompany_Product[index].Product_Name == response.Name) {
                          $scope.Cls_Invoice_Company.ClsInvoiceCompany_Product[index].Amount = parseFloat($scope.Cls_Invoice_Company.ClsInvoiceCompany_Product[index].Amount) + 1;
                           
                          //*******************************
                          //Saving ......
                          $scope.Calculate_itemPrice($scope.Cls_Invoice_Company.ClsInvoiceCompany_Product[index], index);
                          //************************************
                        
                      } else {
                          //الاسم مختلف لاخلاف الاسم - لاختلاف الميزان - او الحبة و الكرتون و الصندوق ** نضيف سطر جديد 
                          //لو الصنف مش موجود ضيف سطر جديد بعدد الاصناف = 1
                          index = $scope.Cls_Invoice_Company.ClsInvoiceCompany_Product.length - 1;
                          $scope.Cls_Invoice_Company.ClsInvoiceCompany_Product[index].Product_Id = response.ID;
                          $scope.Cls_Invoice_Company.ClsInvoiceCompany_Product[index].Product_Name = response.Name;
                          $scope.Cls_Invoice_Company.ClsInvoiceCompany_Product[index].Amount = 1;
                           

                          var product = { ID: "", Product_Id: "", Product_Name: "", Amount: "", Price: "", Taxes:"", Taxes_Orginal : "", TotalPrice: "" };
                          $scope.Cls_Invoice_Company.ClsInvoiceCompany_Product.push(product);
                          //*******************************
                          //Saving ......
                          $scope.Calculate_itemPrice($scope.Cls_Invoice_Company.ClsInvoiceCompany_Product[index], index);
                          //************************************
                          
                      }
                  }



              } else {
                  alert("عفوا الصنف غير مسجل");
              }
              //if (response == null || response == "") { otherData = 1; hideLoadMore = null; }
              //else {  otherData = null; hideLoadMore = 1; }
          });
            $scope.$apply();
        }
    };
    $scope.Intaize_product = function () {
        $scope.Cls_Invoice_Company = {
            ID: "", Invoice_Number:"", Payment_Type: "", Company_id: "", Company_Name: "", Date_Invoice: "", Date_Invoice_Hijri: "", Nakl_Cost: "", Price: 0, Taxes: "", Total_Sadad: "", Payment_Type: "",
            ClsInvoiceCompany_Product: null
        };
        var product = { ID: "", Product_Id: "", Product_Name: "", Amount: "", Price: "", Taxes:"", Taxes_Orginal : "", TotalPrice: "" };
        $scope.Cls_Invoice_Company.ClsInvoiceCompany_Product = new Array();
        $scope.Cls_Invoice_Company.ClsInvoiceCompany_Product.push(product);
        $scope.Cls_Invoice_Company.Price = parseFloat(parseFloat($scope.Cls_Invoice_Company.Price).toFixed(2));
        $scope.Cls_Invoice_Company.Taxes = parseFloat(parseFloat($scope.Cls_Invoice_Company.Taxes).toFixed(3));
        document.getElementById('InvoicePrice').innerHTML = $scope.Cls_Invoice_Company.Price;
    };

    //-- عملية حفظ / تعديل / حذف الفاتورة
    $scope.switchCase = function () {
        
        var QueryString = location.search;
        var urlParams = new URLSearchParams(QueryString);
        if (urlParams.has('id') == true) {
            var id = urlParams.get('id');    // 1234
            //document.getElementById('PrintInvoice').href = "/Invoice_Cancel/PrintInvoice/" + id;
            $http.get('/Invoice_Company/loadInvoice/' + id)
               .success(function (response) {
                   if (response == null) {
                       $uibModal.open({
                           template: " <div class='portlet box red' style='min-height:150px !important;min-width: 541px !important;'> <div class='portlet-title'> <div class='caption'> <i class='fa-lg fa fa-warning'></i> تنبيه هام </div><div class='tools'> </div></div><div class='portlet-body form portlet-empty' style='min-height: 140px;'> <div class='col-md-12 page-404'> <div class='number'>" + response.ErrorNumber + "</div><div class='details'> <h3 style='font-family: sans-serif; text-align: center; font-weight: bold;'>" + response.ErrorFullNumber + "</h3> <p style='font-size: 12px;'> " + response.ErrorName + " <br/> <br/> <a class='btn btn-default btn-sm' href='/home'> الصفحة الرئيسية </a> <a class='btn btn-default btn-sm' href='" + response.Url + "' style='margin-right: 4px;'>المحاولة مرة آخري</a> </p></div></div></div></div>",
                           closeOnEscape: false,
                           closeOnOverlayClick: false
                       });
                   }
                   else {
                      
                       $scope.Cls_Invoice_Company = response;
                       $('#Date_Invoice').val($scope.Cls_Invoice_Company.Date_Invoice);
                       $("#Invoice_ID").val($scope.Cls_Invoice_Company.ID);
                       document.getElementById('InvoicePrice').innerHTML = $scope.Cls_Invoice_Company.Price;
                       document.getElementById('Total_Sadad').innerHTML = $scope.Cls_Invoice_Company.Total_Sadad;
                       var product = { ID: "", Product_Id: "", Product_Name: "", Amount: "", Price: "", Taxes:"", Taxes_Orginal : "", TotalPrice: "" };
                       $scope.Cls_Invoice_Company.ClsInvoiceCompany_Product.push(product);
                   }
               });
           
        }
        
    }
    $scope.switchCase();

   

    $scope.hideElement = function (arr) {

        for (var i = 0; i < arr.length; i++) {
            if (document.getElementById(arr[i]) != null)
                document.getElementById(arr[i]).style.display = 'none';
        }
    }
    //----------------------------------------------------------------------------------------------------------------------------
    //----------------------------------------------------------------------------------------------------------------------------
    //------------------1- جلب بيانات الشركات من قاعدة البيانات
    $scope.getAllCompanies = function (Page, Search) {
        var parameter = { 'Search': Search }
        var sentData = "page=" + Page;
        $http.get("/Company/getAll?" + sentData, { params: parameter })
       .success(function (response) {
           if (response != null) {

               if ($scope.CompanyList == null) {
                   $scope.CompanyList = response;
               }
               else {
                   $scope.CompanyList = $scope.CompanyList.concat(response);
               }
           }
           //if (response == null || response == "") { otherData = 1; hideLoadMore = null; }
           //else {  otherData = null; hideLoadMore = 1; }
       });
    }
    $scope.getAllCompanies();
    //----------------------------------------------------------------------------------------------------------------------------
    // get more data with click more
    //----------------------------------------------------------------------------------------------------------------------------
    $scope.LoadMore = function () {
        
        $scope.page++;
        $scope.getAllCompanies($scope.page, $scope.Search);
    }
    //----------------------------------------------------------------------------------------------------------------------------

    // 2- جلب ال partial view للشكركات
    // فيه بيانات الشركات CompanyList اللى حامل 
   
    var View_Companies;
    $scope.getView = function () {
        $http.get("/Invoice_Company/GetCompanies")
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
                Cls_Product: function () {
                    return $scope.Cls_Product;
                }
            }
        });
    };

    $scope.getAllCompanies = function (Page, Search) {
        
        var parameter = { 'Search': Search }
        var sentData = "page=" + Page;
        $http.get("/Company/getAll?" + sentData, { params: parameter })
       .success(function (response) {
           if (response != null && response != "") {
               
               if ($scope.CompanyList == null || Page == null || Page == 0) {
                   $scope.CompanyList = response;
               }
               else {
                   $scope.CompanyList = $scope.CompanyList.concat(response);
               }
           }
         
       });
    }

    
    $scope.clicktr_Company = function (row) {
       
        $scope.Cls_Invoice_Company.Company_id = row.ID;
        $scope.Cls_Invoice_Company.Company_Name = row.Name;
        this.$close();
        //Saving ......
        $scope.EditCompany();
        if ($scope.FormValidation() == false)
        {
            $scope.AlertParameter = 1;
        }
    }
    $scope.EditCompany = function () {
        var CheckForm = $scope.FormValidation();
        if (CheckForm == true) {

            var parameter = JSON.stringify($scope.Cls_Invoice_Company);
            $http.post('/Invoice_Company/EditCompanyName', parameter)
                 .success(function (response) {


                     var test = response;
                     if (response.ErrorName.indexOf("صلاحية") > -1 || response.ErrorName.indexOf("المسموح") > -1) {
                         $uibModal.open({
                             template: " <div class='portlet box red' style='min-height:150px !important ;min-width: 541px !important;'> <div class='portlet-title'> <div class='caption'> <i class='fa-lg fa fa-warning'></i> تنبيه هام </div><div class='tools'> </div></div><div class='portlet-body form portlet-empty' style='min-height: 140px;'> <div class='col-md-12 page-404'> <div class='number'>" + response.ErrorNumber + "</div><div class='details'> <h3 style='font-family: sans-serif; text-align: center; font-weight: bold;'>" + response.ErrorFullNumber + "</h3> <p> " + response.ErrorName + " <br/> <br/> <a class='btn btn-default btn-sm' href='/home'> الصفحة الرئيسية </a> </p></div></div></div></div>",
                             closeOnEscape: false,
                             closeOnOverlayClick: false
                         });
                     }
                 });
        }
        else {
            $scope.AlertParameter = 1;
            document.body.scrollTop = 0;
        }
    };
     
    $scope.EditNakl_Cost = function () {
        var CheckForm = $scope.FormValidation();
        if (CheckForm == true) {
            $scope.Calculate_price();
            var parameter = JSON.stringify($scope.Cls_Invoice_Company);
            $http.post('/Invoice_Company/EditNakl_Cost', parameter)
                 .success(function (response) {
                     var test = response;
                     if (response.ErrorName.indexOf("صلاحية") > -1 || response.ErrorName.indexOf("المسموح") > -1) {
                         $uibModal.open({
                             template: " <div class='portlet box red' style='min-height:150px !important ;min-width: 541px !important;'> <div class='portlet-title'> <div class='caption'> <i class='fa-lg fa fa-warning'></i> تنبيه هام </div><div class='tools'> </div></div><div class='portlet-body form portlet-empty' style='min-height: 140px;'> <div class='col-md-12 page-404'> <div class='number'>" + response.ErrorNumber + "</div><div class='details'> <h3 style='font-family: sans-serif; text-align: center; font-weight: bold;'>" + response.ErrorFullNumber + "</h3> <p> " + response.ErrorName + " <br/> <br/> <a class='btn btn-default btn-sm' href='/home'> الصفحة الرئيسية </a> </p></div></div></div></div>",
                             closeOnEscape: false,
                             closeOnOverlayClick: false
                         });
                     }
                 });
        }
        else {
            $scope.AlertParameter = 1;
            document.body.scrollTop = 0;
        }
    };
    //----------------------------------------------------------------------------------------------------------------------------
    var lastentry = "";
    $scope.Srchkey_Company = function () {
        
        var search = $("#Search").val();
        if (search != lastentry) {
            
            $scope.CompanyList = null;
            $scope.page = 0;
            $scope.getAllCompanies(null, search);
        }
        lastentry = search;
    };
    //----------------------------------------------------------------------------------------------------------------------------

    //------------------تحميل الشركات بعد فتح النافذة
  

   
    //===================
    //=======عند الضفط على صف
  
    $scope.removeRow = function (index) {
        debugger;
        var My_Invoice_Product_id = $scope.Cls_Invoice_Company.ClsInvoiceCompany_Product[index].ID;
        if ($scope.Cls_Invoice_Company.ClsInvoiceCompany_Product.length > 1) {
            $scope.Cls_Invoice_Company.ClsInvoiceCompany_Product.splice(index, 1);
            $scope.Calculate_price();

            var parameter = JSON.stringify($scope.Cls_Invoice_Company);
           
            $http.post('/Invoice_Company/DeleteItem?My_Invoice_Product_id=' + My_Invoice_Product_id, parameter)
                .success(function (response) {
                    var test = response;
                    if (response.ErrorName.indexOf("صلاحية") > -1 || response.ErrorName.indexOf("المسموح") > -1) {
                        $uibModal.open({
                            template: " <div class='portlet box red' style='min-height:150px !important ;min-width: 541px !important;'> <div class='portlet-title'> <div class='caption'> <i class='fa-lg fa fa-warning'></i> تنبيه هام </div><div class='tools'> </div></div><div class='portlet-body form portlet-empty' style='min-height: 140px;'> <div class='col-md-12 page-404'> <div class='number'>" + response.ErrorNumber + "</div><div class='details'> <h3 style='font-family: sans-serif; text-align: center; font-weight: bold;'>" + response.ErrorFullNumber + "</h3> <p> " + response.ErrorName + " <br/> <br/> <a class='btn btn-default btn-sm' href='/home'> الصفحة الرئيسية </a> </p></div></div></div></div>",
                            closeOnEscape: false,
                            closeOnOverlayClick: false
                        });
                    } 
                });
        }
    }
    $scope.closeUiModal = function () {
        $scope.product_Index = -1;
        this.$close();
    }
   
   
    //=======عند الضفط على صف
    $scope.clicktrMowarid = function (row) {
       $scope.Cls_Invoice_Company.Company_id = row.ID;
       $scope.Cls_Invoice_Company.Company_Name = row.Name;
        this.$close();
    }
    //****************************************************************************
    //-- عملية حفظ / تعديل / حذف الفاتورة
    
    $scope.changeBorder = function (arr, color) {
        for (var i = 0; i < arr.length; i++) {
            document.getElementById(arr[i]).style.borderColor = color;
        }
    }

    $scope.changeBorderJQ = function (input, color) {
        $('input[name="' + input + '"]').css("borderColor", color);
    }
    $scope.Save = function (index) {
        var CheckForm = $scope.FormValidation();
        if (CheckForm == true) {
            var sentData = "index=" + index;
            var parameter = JSON.stringify($scope.Cls_Invoice_Company);
            $http.post('/Invoice_Company/AddItem?' + sentData, parameter)
                 .success(function (response) {
                     var test = response;
                     if (response.ErrorName.indexOf("صلاحية") > -1 || response.ErrorName.indexOf("المسموح") > -1) {
                         $uibModal.open({
                             template: " <div class='portlet box red' style='min-height:150px !important ;min-width: 541px !important;'> <div class='portlet-title'> <div class='caption'> <i class='fa-lg fa fa-warning'></i> تنبيه هام </div><div class='tools'> </div></div><div class='portlet-body form portlet-empty' style='min-height: 140px;'> <div class='col-md-12 page-404'> <div class='number'>" + response.ErrorNumber + "</div><div class='details'> <h3 style='font-family: sans-serif; text-align: center; font-weight: bold;'>" + response.ErrorFullNumber + "</h3> <p> " + response.ErrorName + " <br/> <br/> <a class='btn btn-default btn-sm' href='/home'> الصفحة الرئيسية </a> </p></div></div></div></div>",
                             closeOnEscape: false,
                             closeOnOverlayClick: false
                         });
                     } else {
                         $scope.Cls_Invoice_Company.ID = response.ID;
                         $scope.Cls_Invoice_Company.Invoice_Number = response.Invoice_Number;
                         var current_index = response.index;
                         $scope.Cls_Invoice_Company.ClsInvoiceCompany_Product[current_index].ID = response.Invoice_Product_ID;
                         $("#Invoice_ID").val(response.ID);
                     }
                 });
        }
        else {
            $scope.AlertParameter = 1;
            document.body.scrollTop = 0;
        }


    };

  
    $scope.Edit = function (index, invoice_product_id) {
        var CheckForm = $scope.FormValidation();
        if (CheckForm == true) {
            var sentData = "index=" + index + "&Invoice_Product_ID=" + invoice_product_id;
            var parameter = JSON.stringify($scope.Cls_Invoice_Company);
            $http.post('/Invoice_Company/EditItem?' + sentData, parameter)
                 .success(function (response) {


                     var test = response;
                     if (response.ErrorName.indexOf("صلاحية") > -1 || response.ErrorName.indexOf("المسموح") > -1) {
                         $uibModal.open({
                             template: " <div class='portlet box red' style='min-height:150px !important ;min-width: 541px !important;'> <div class='portlet-title'> <div class='caption'> <i class='fa-lg fa fa-warning'></i> تنبيه هام </div><div class='tools'> </div></div><div class='portlet-body form portlet-empty' style='min-height: 140px;'> <div class='col-md-12 page-404'> <div class='number'>" + response.ErrorNumber + "</div><div class='details'> <h3 style='font-family: sans-serif; text-align: center; font-weight: bold;'>" + response.ErrorFullNumber + "</h3> <p> " + response.ErrorName + " <br/> <br/> <a class='btn btn-default btn-sm' href='/home'> الصفحة الرئيسية </a> </p></div></div></div></div>",
                             closeOnEscape: false,
                             closeOnOverlayClick: false
                         });
                     }
                 });
        }
        else {
            $scope.AlertParameter = 1;
            document.body.scrollTop = 0;
        }


    };
   

    $scope.BtnNew = function () {

        $window.location.href = '/Invoice_Company/Invoice_Company/';
    };


    $scope.ReomveAlert = function () {
        $scope.AlertParameter = null;
    }
    $scope.FormValidation = function () {
        $scope.AlertParameter = 0;
       // $scope.Cls_Invoice_Company.Date_Invoice = document.getElementById("Date_Invoice").value;
        $scope.Cls_Invoice_Company.Date_Invoice = $('#Date_Invoice').val();
       // var JawalNOCheck = Company.JawalNO.match("^05[0-9]{8}$");
        if ($scope.Cls_Invoice_Company.Company_Name == null || $scope.Cls_Invoice_Company.Company_Name == "") {
            $scope.ErrorName = "برجاء ادخال اسم الشركة";
            var ListID = ["Company_Name"];
            $scope.changeBorder(ListID, "red");
            return false;
        } else {
            var ListID = ["Company_Name"];
            $scope.changeBorder(ListID, "#c2cad8");
        }

        if ($scope.Cls_Invoice_Company.Date_Invoice == null || $scope.Cls_Invoice_Company.Date_Invoice == "") {
            $scope.ErrorName = "برجاء ادخال تاريخ الشراء";
            var ListID = ["Date_Invoice"];
            $scope.changeBorder(ListID, "red");
            return false;
        } else {
            var ListID = ["Date_Invoice"];
            $scope.changeBorder(ListID, "#c2cad8");
        }

        if ($scope.Cls_Invoice_Company.Nakl_Cost == null || $scope.Cls_Invoice_Company.Nakl_Cost == "") {
            $scope.ErrorName = "برجاء ادخال مصاريف النقل";
            var ListID = ["Nakl_Cost"];
            $scope.changeBorder(ListID, "red");
            return false;
        } else {
            var ListID = ["Nakl_Cost"];
            $scope.changeBorder(ListID, "#c2cad8");
        }
        var len = $scope.Cls_Invoice_Company.ClsInvoiceCompany_Product.length;
         
        for (var i = 0; i < len; i++) {
             
            if (len = 1
                ||
               ((len > 1) && i < (len - 1))) {

                if ($scope.Cls_Invoice_Company.ClsInvoiceCompany_Product[i].Product_Name == null || $scope.Cls_Invoice_Company.ClsInvoiceCompany_Product[i].Product_Name == "") {
                    $scope.ErrorName = "برجاء ادخال اسم الصنف";
                    var ListID = ["Product_Name[" + i +"]"];
                    $scope.changeBorderJQ(ListID, "red");
                    return false;
                }
                else {
                    var ListID = ["Product_Name[" + i +"]"];
                    $scope.changeBorderJQ(ListID, "#c2cad8");
                }


                 

                if ($scope.Cls_Invoice_Company.ClsInvoiceCompany_Product[i].Amount == null
                   || $scope.Cls_Invoice_Company.ClsInvoiceCompany_Product[i].Amount == "") {
                    $scope.ErrorName = "برجاء ادخال العدد بالجملة";
                    var ListID = ["Amount[" + i +"]"];
                    $scope.changeBorderJQ(ListID, "red");
                    return false;
                }
                else {
                    var ListID = ["Amount[" + i +"]"];
                    $scope.changeBorderJQ(ListID, "#c2cad8");
                }
               
                if ($scope.Cls_Invoice_Company.ClsInvoiceCompany_Product[i].Price == null || $scope.Cls_Invoice_Company.ClsInvoiceCompany_Product[i].Price == "") {
                    $scope.ErrorName = "برجاء ادخال سعر الشراء الافرادي";
                    var ListID = ["Price[" + i +"]"];
                    $scope.changeBorderJQ(ListID, "red");
                    return false;
                }
                else {
                    var ListID = ["Price[" + i +"]"];
                    $scope.changeBorderJQ(ListID, "#c2cad8");
                }

                if ($scope.Cls_Invoice_Company.ClsInvoiceCompany_Product[i].TotalPrice == null || $scope.Cls_Invoice_Company.ClsInvoiceCompany_Product[i].TotalPrice == "") {
                    $scope.ErrorName = "برجاء ادخال سعر الشراء الإجمالي";
                    var ListID = ["TotalPrice[" + i + "]"];
                    $scope.changeBorderJQ(ListID, "red");
                    return false;
                }
                else {
                    var ListID = ["TotalPrice[" + i + "]"];
                    $scope.changeBorderJQ(ListID, "#c2cad8");
                }
            }
           
        }
       
        
        return true;
    }
    $scope.Validation_SavePrevProducts = function () {
        $scope.AlertParameter = 0;
        //$scope.Cls_Invoice_Company.Date_Invoice = document.getElementById("Date_Invoice").value;
        $scope.Cls_Invoice_Company.Date_Invoice = $('#Date_Invoice').val();
        if ($scope.Cls_Invoice_Company.Company_Name == null || $scope.Cls_Invoice_Company.Company_Name == "") {
            $scope.ErrorName = "برجاء ادخال اسم الشركة";
            var ListID = ["Company_Name"];
            $scope.changeBorder(ListID, "red");
            return false;
        } else {
            var ListID = ["Company_Name"];
            $scope.changeBorder(ListID, "#c2cad8");
        }

        if ($scope.Cls_Invoice_Company.Date_Invoice == null || $scope.Cls_Invoice_Company.Date_Invoice == "") {
            $scope.ErrorName = "برجاء ادخال تاريخ الشراء";
            var ListID = ["Date_Invoice"];
            $scope.changeBorder(ListID, "red");
            return false;
        } else {
            var ListID = ["Date_Invoice"];
            $scope.changeBorder(ListID, "#c2cad8");
        }

        if ($scope.Cls_Invoice_Company.Nakl_Cost == null || $scope.Cls_Invoice_Company.Nakl_Cost == "") {
            $scope.ErrorName = "برجاء ادخال مصاريف النقل";
            var ListID = ["Nakl_Cost"];
            $scope.changeBorder(ListID, "red");
            return false;
        } else {
            var ListID = ["Nakl_Cost"];
            $scope.changeBorder(ListID, "#c2cad8");
        }
        var len = $scope.Cls_Invoice_Company.ClsInvoiceCompany_Product.length;

        for (var i = 0; i < len; i++) {
            
            //-----------------------------------------------------------------------------------------
            // -- التأكد اولا انه تم حفظ جميع الاصناف السابقة بدون خطأ او فقد تاريخ انتهاء الصلاحية
            if (len > 1 && i < (len - 1)) {
                if ($scope.Cls_Invoice_Company.ClsInvoiceCompany_Product[i].ID == null || $scope.Cls_Invoice_Company.ClsInvoiceCompany_Product[i].ID == "") {
                    $scope.ErrorName = "برجاء حفظ الصنف اولا قبل اضافة صنف جديد";
                    var ListID = ["Product_Name[" + i + "]"];
                    $scope.changeBorderJQ(ListID, "red");
                    return false;
                }
                else {
                    var ListID = ["Product_Name[" + i + "]"];
                    $scope.changeBorderJQ(ListID, "#c2cad8");
                }
            }
            //-----------------------------------------------------------------------------------------
        }
        return true;
    }
    //----------------------------------------------------------------------------------------------------------------------------
    var DealingTypeView;
    //-------------------------------------------------
    $scope.getView = function () {
        $http.get("/Invoice_Company/GetProducts")
         .success(function (response) {
             DealingTypeView = response;
         });
    };
    $scope.getView();

    $scope.loadproduct = function (index) {
   
        var CheckForm = $scope.Validation_SavePrevProducts();
        if (CheckForm == true) {
            $scope.product_Index = index;
            //-----
            $scope.ProductList = null;
            $scope.getAllProducts();
            //------------
            $uibModal.open({

                scope: $scope,
                template: DealingTypeView,
                resolve: {
                    Cls_Invoice_Company: function () {
                        return $scope.Cls_Invoice_Company;
                    },
                    product_Index: function () {
                        return $scope.product_Index;
                    },
                    Account_Type: function () {
                        return $scope.Account_Type;
                    }

                }
            });
        }
        else {
            $scope.AlertParameter = 1;
            document.body.scrollTop = 0;
        }
    };
    //-----------------------------------------------
    var SearchProductView;
    //-------------------------------------------------
    $scope.Account_Type = 0;
    $scope.getAccount_Type = function () {
        $http.get("/users/GetAccount_Type")
         .success(function (response) {
             if (response != 1) {
                 $("#InvoicePrice_box").hide();
                 $("#Total_Sadad_box").hide();
                 $("#print_invoice").hide();
                 $scope.Account_Type = null;
             } else {
                 $("#InvoicePrice_box").show();
                 $("#Total_Sadad_box").show();
                 $("#print_invoice").show();
                 $scope.Account_Type = 1;
             }
         });
    };
    $scope.getAccount_Type();

    $scope.getView = function () {
        $http.get("/Invoice_Company/GetSearchProducts")
         .success(function (response) {
             SearchProductView = response;
         });
    };
    $scope.getView();

    $scope.Sreachproduct = function () {
        //-----
        $scope.ProductList = null;
        $scope.getAllProducts();
        //------------
        $uibModal.open({

            scope: $scope,
            template: SearchProductView,
            resolve: {
                Cls_Invoice_Company: function () {
                    return $scope.Cls_Invoice_Company;
                }
            }
        });

    };
   
    $scope.getAllProducts = function (Page, Search, Src_Barcode) {
        var parameter = { 'Search': Search, 'Src_Barcode': Src_Barcode }
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

    $scope.clicktr = function (row) {
        debugger;
       
        var current_index = $scope.Cls_Invoice_Company.ClsInvoiceCompany_Product.length - 1;
        // تعديل صنف محفوظ بصنف جديد - حتى لو نفس الصنف
        if (current_index != $scope.product_Index && $scope.product_Index != -1) {
            index = $scope.product_Index;
            $scope.Cls_Invoice_Company.ClsInvoiceCompany_Product[index].Product_Id = row.ID;
            $scope.Cls_Invoice_Company.ClsInvoiceCompany_Product[index].Product_Name = row.Name;
            $scope.Cls_Invoice_Company.ClsInvoiceCompany_Product[index].Taxes_Orginal = parseFloat(parseFloat(row.Taxes).toFixed(3));
            $scope.Cls_Invoice_Company.ClsInvoiceCompany_Product[index].Price = row.Price_Mowrid;
            //*******************************
            //Saving ......
            $scope.Calculate_itemPrice($scope.Cls_Invoice_Company.ClsInvoiceCompany_Product[index], index);
            //************************************
            $scope.product_Index = -1;
        } else {

            //البحث هل الصنف ده موجود قبل كدة 
            var index = $scope.Cls_Invoice_Company.ClsInvoiceCompany_Product.findIndex(function (item, i) {
                return ((item.Product_Id === row.ID) && (item.Product_Name === row.Name));
            });

            if (index == -1) {
                //لو الصنف مش موجود ضيف سطر جديد بعدد الاصناف = 1
                index = $scope.Cls_Invoice_Company.ClsInvoiceCompany_Product.length - 1;
                $scope.Cls_Invoice_Company.ClsInvoiceCompany_Product[index].Product_Id = row.ID;
                $scope.Cls_Invoice_Company.ClsInvoiceCompany_Product[index].Product_Name = row.Name;
                $scope.Cls_Invoice_Company.ClsInvoiceCompany_Product[index].Amount = 1;
                $scope.Cls_Invoice_Company.ClsInvoiceCompany_Product[index].Price = row.Price_Mowrid;
                $scope.Cls_Invoice_Company.ClsInvoiceCompany_Product[index].TotalPrice = row.Price_Mowrid;
                $scope.Cls_Invoice_Company.ClsInvoiceCompany_Product[index].Taxes = parseFloat(parseFloat(row.Taxes_Price).toFixed(3));
                $scope.Cls_Invoice_Company.ClsInvoiceCompany_Product[index].Taxes_Orginal = parseFloat(parseFloat(row.Taxes).toFixed(3));


                //*******************************
                //Saving ......
                $scope.Calculate_itemPrice($scope.Cls_Invoice_Company.ClsInvoiceCompany_Product[index], index);
                //************************************
                var product = { ID: "", Product_Id: "", Product_Name: "", Amount: "", Price: "", Taxes: "", Taxes_Orginal: "", TotalPrice: "" };
                $scope.Cls_Invoice_Company.ClsInvoiceCompany_Product.push(product);


            } else {
                // لو الصنف موجود بنفس الاسم (وزن-او كرتون او حبة) نزود العدد +1
                var x1 = $scope.Cls_Invoice_Company.ClsInvoiceCompany_Product[index].Product_Name;
                var x2 = row.Name;
                if ($scope.Cls_Invoice_Company.ClsInvoiceCompany_Product[index].Product_Name == row.Name) {
                    $scope.Cls_Invoice_Company.ClsInvoiceCompany_Product[index].Amount = parseFloat($scope.Cls_Invoice_Company.ClsInvoiceCompany_Product[index].Amount) + 1;
                     
                    //*******************************
                    //Saving ......
                    $scope.Calculate_itemPrice($scope.Cls_Invoice_Company.ClsInvoiceCompany_Product[index], index);
                    //************************************
                } else {
                    //الاسم مختلف لاخلاف الاسم - لاختلاف الميزان - او الحبة و الكرتون و الصندوق ** نضيف سطر جديد 
                    //لو الصنف مش موجود ضيف سطر جديد بعدد الاصناف = 1
                    index = $scope.Cls_Invoice_Company.ClsInvoiceCompany_Product.length - 1;
                    $scope.Cls_Invoice_Company.ClsInvoiceCompany_Product[index].Product_Id = row.ID;
                    $scope.Cls_Invoice_Company.ClsInvoiceCompany_Product[index].Product_Name = row.Name;
                    $scope.Cls_Invoice_Company.ClsInvoiceCompany_Product[index].Taxes = parseFloat(parseFloat(row.Taxes_Price).toFixed(3));
                    $scope.Cls_Invoice_Company.ClsInvoiceCompany_Product[index].Taxes_Orginal = parseFloat(parseFloat(row.Taxes).toFixed(3));
                    $scope.Cls_Invoice_Company.ClsInvoiceCompany_Product[index].Price = row.Price_Mowrid;
                    $scope.Cls_Invoice_Company.ClsInvoiceCompany_Product[index].TotalPrice = row.Price_Mowrid;
                    $scope.Cls_Invoice_Company.ClsInvoiceCompany_Product[index].Amount = 1;

                    var product = { ID: "", Product_Id: "", Product_Name: "", Amount: "", Price: "", Taxes: "", Taxes_Orginal: "", TotalPrice: "" };
                    $scope.Cls_Invoice_Company.ClsInvoiceCompany_Product.push(product);

                    //*******************************
                    //Saving ......
                    $scope.Calculate_itemPrice($scope.Cls_Invoice_Company.ClsInvoiceCompany_Product[index], index);
                    //************************************

                }
        }
       
        }

        this.$close();


    }
    
    
    //-----------------------------------------------
    //***************العمليات الحسابية -----------------------------------------------------------------------------
    $scope.Calculate_itemPrice_And_Edit = function (item, index) {

        if (isNaN(parseFloat(item.Price)) == false && isNaN(parseFloat(item.Taxes)) == false && isNaN(parseFloat(item.Amount)) == false) {
            item.TotalPrice = parseFloat((parseFloat(item.Price) + parseFloat(item.Taxes)) * parseFloat(item.Amount)).toFixed(2);
            $scope.Calculate_price();
            if (item.ID != "") {
                //*******************************
                //Saving ......
                $scope.Calculate_itemPrice(item, index);
                //************************************
            }
        }
    }
    $scope.Calculate_price = function () {
        $scope.Cls_Invoice_Company.Price = 0;
        $scope.Cls_Invoice_Company.Taxes = 0;
        for (var i = 0; i < $scope.Cls_Invoice_Company.ClsInvoiceCompany_Product.length; i++) {
            var _TotalPrice = parseFloat($scope.Cls_Invoice_Company.ClsInvoiceCompany_Product[i].TotalPrice);
            var isnan_TotalPrice = isNaN(parseFloat(_TotalPrice));
            if (isnan_TotalPrice == false) {
                $scope.Cls_Invoice_Company.Price += parseFloat($scope.Cls_Invoice_Company.ClsInvoiceCompany_Product[i].TotalPrice);
            }
            if (isNaN(parseFloat($scope.Cls_Invoice_Company.ClsInvoiceCompany_Product[i].Amount)) == false
                   && isNaN(parseFloat($scope.Cls_Invoice_Company.ClsInvoiceCompany_Product[i].Taxes)) == false) {
                $scope.Cls_Invoice_Company.Taxes += (parseFloat($scope.Cls_Invoice_Company.ClsInvoiceCompany_Product[i].Taxes) * parseFloat($scope.Cls_Invoice_Company.ClsInvoiceCompany_Product[i].Amount));
            }
        }
        $scope.Cls_Invoice_Company.Price = parseFloat((parseFloat($scope.Cls_Invoice_Company.Price) + parseFloat($scope.Cls_Invoice_Company.Nakl_Cost)).toFixed(2));
        $scope.Cls_Invoice_Company.Taxes = parseFloat(parseFloat($scope.Cls_Invoice_Company.Taxes).toFixed(3));
        document.getElementById('InvoicePrice').innerHTML = $scope.Cls_Invoice_Company.Price;
    }
    //-----
    
    //-----
    $scope.Calculate_Amount = function (item, index) {
        var _Amount = item.Amount;
        var _Price = item.Price;
        var _TotalPrice = item.TotalPrice;
        var isnan_Amount = isNaN(parseFloat(_Amount));
        var isnan_TotalPrice = isNaN(parseFloat(_TotalPrice));
        var isnan_Price = isNaN(parseFloat(_Price));
        if (isnan_Amount == false
            && isnan_Price == false) {
            item.Taxes = parseFloat(parseFloat(parseFloat(item.Price) * parseFloat(item.Taxes_Orginal / 100)).toFixed(3));
            item.TotalPrice = parseFloat(parseFloat(parseFloat(item.Amount) * (parseFloat(item.Price) + parseFloat(item.Taxes))).toFixed(2));
            
        }
        $scope.Calculate_price();
        //*******************************
        //Saving ......
        $scope.Calculate_Saving(item, index);
        //*******************************
    }
    $scope.Calculate_itemPrice = function (item, index) {
       
        var _Amount = item.Amount;
        var _Price = item.Price;
        var _TotalPrice = item.TotalPrice;
        var isnan_Amount = isNaN(parseFloat(_Amount));
        var isnan_Price = isNaN(parseFloat(_Price));
        var isnan_TotalPrice = isNaN(parseFloat(_TotalPrice));
        if (isnan_Amount == false
            && isnan_Price == false) {
            item.Taxes = parseFloat(parseFloat(parseFloat(item.Price) * parseFloat(item.Taxes_Orginal / 100)).toFixed(3));
            item.TotalPrice = parseFloat(parseFloat(parseFloat(item.Amount) * (parseFloat(item.Price) + parseFloat(item.Taxes))).toFixed(2));
        }
        $scope.Calculate_price();
        //*******************************
        //Saving ......
        $scope.Calculate_Saving(item, index);
       //*******************************
    }
   

    $scope.Calculate_Saving = function (item, index)
    {
        
        var CheckForm = $scope.FormValidation();
        if (CheckForm == true) {
            if (item.ID == "") //saving ....
            {
                //*******************************
                //Saving ......
                $scope.Save(index);
                //************************************
            } else {
                //*******************************
                //Saving ......
                $scope.Edit(index, $scope.Cls_Invoice_Company.ClsInvoiceCompany_Product[index].ID);
                //************************************
            }
        }
        else {
            $scope.AlertParameter = 1;
            document.body.scrollTop = 0;
        }
    }

    //$('#Date_Invoice').blur(function () {
    //    alert('Changed!');
    //});
    $('#Date_Invoice').Zebra_DatePicker({
        onSelect: function (date) {
            debugger;
            $scope.Cls_Invoice_Company.Date_Invoice = $('#Date_Invoice').val();
            var CheckForm = $scope.FormValidation();
            if (CheckForm == true) {
                var parameter = JSON.stringify($scope.Cls_Invoice_Company);
                if ($scope.Cls_Invoice_Company.ID != "" && $scope.Cls_Invoice_Company.ID != "0") //saving ....
                {
                    $http.post('/Invoice_Company/EditDateInvoice', parameter)
                         .success(function (response) {
                             var test = response;
                             if (response.ErrorName.indexOf("صلاحية") > -1 || response.ErrorName.indexOf("المسموح") > -1) {
                                 $uibModal.open({
                                     template: " <div class='portlet box red' style='min-height:150px !important ;min-width: 541px !important;'> <div class='portlet-title'> <div class='caption'> <i class='fa-lg fa fa-warning'></i> تنبيه هام </div><div class='tools'> </div></div><div class='portlet-body form portlet-empty' style='min-height: 140px;'> <div class='col-md-12 page-404'> <div class='number'>" + response.ErrorNumber + "</div><div class='details'> <h3 style='font-family: sans-serif; text-align: center; font-weight: bold;'>" + response.ErrorFullNumber + "</h3> <p> " + response.ErrorName + " <br/> <br/> <a class='btn btn-default btn-sm' href='/home'> الصفحة الرئيسية </a> </p></div></div></div></div>",
                                     closeOnEscape: false,
                                     closeOnOverlayClick: false
                                 });
                             }
                         });
                }
            }
            else {
                $scope.AlertParameter = 1;
                document.body.scrollTop = 0;
            }
        }
    });
 

    
    $scope.BtnPrintInvoice = function () {

        if ($scope.Cls_Invoice_Company != null && $scope.Cls_Invoice_Company.ID != "" && $scope.Cls_Invoice_Company.ID != 0) {


            $http({
                url: "/Invoice_Company/PdfInvoice?id=" + $scope.Cls_Invoice_Company.ID,
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
    //----------------------------------------------------------------------------------------------------------------------------
    //**** Search
    //----------------------------------------------------------------------------------------------------------------------------
    var lastsearch = "";
    var lastSrc_Barcode = "";
    $scope.keyupevt = function () {
        var search = $("#Search").val();
        var Src_Barcode = $("#Src_Barcode").val();

        if (Src_Barcode != "" && Src_Barcode != lastSrc_Barcode) {
            $scope.ProductList = null;
            page = 0;
            $scope.getAllProducts(null, "", Src_Barcode);
        }
        else if (search != lastsearch) {
            $scope.ProductList = null;
            page = 0;
            $scope.getAllProducts(null, search, "");
        }
        lastsearch = search;
        lastSrc_Barcode = Src_Barcode;
    };
    //----------------------------------------------------------------------------------------------------------------------------
    $scope.Remove = function () {
        var ID = $scope.Cls_Invoice_Company.ID;
        if (ID > 1) {
            $scope.AlertParameter = null;
            document.body.scrollTop = 0;
            $modelDialog = $uibModal.open({

                scope: $scope,
                template: AuthorizeManager,
                resolve: {
                    Cls_Invoice_Company: function () {
                        return $scope.Cls_Invoice_Company;
                    }
                }
            });
        }
    }
  

    document.onkeydown = function () {

        var f2 = 113;
        var f4 = 115;
        var f8 = 119;
        var f9 = 120;

        var Ctrl = 17;
        var Shift = 16;
        var Tab = 9;

        if (window.event && window.event.keyCode == f2) { //طباعة الفاتورة
            $scope.BtnPrintInvoice();
        }
        else if (window.event && window.event.keyCode == f4) { //فاتورة جديدة
            $scope.BtnNew();
        }
        else if (window.event && window.event.keyCode == f8) { //فاتورة جديدة
            $scope.Remove();
        }
        else if (window.event && window.event.keyCode == f9) { //البحث عن صنف
            $scope.Sreachproduct();
        }
        else if (window.event && window.event.keyCode == Tab) { //تغيير العدد
            if ($scope.Cls_Invoice_Company.ClsInvoiceCompany_Product.length > 1) {

                var index = $scope.Cls_Invoice_Company.ClsInvoiceCompany_Product.length - 2;

                var myinput = $("input[name= 'Amount[" + index + "]']");
                if ($scope.isAmount_focus == false) {
                    myinput.focus();
                    myinput.select();
                    $scope.isAmount_focus = true;
                } else {
                    myinput.focusout();
                    myinput.blur();
                    $scope.isAmount_focus = false;
                }

            }
        }
    }
});

$("#Sadad_Invoice").click(function () {
    var _id = $("#Invoice_ID").val();
    if (_id != "") {
        event.preventDefault();
        event.stopPropagation();
        window.open("/Sadad_Company/SadadCompany?id=" + _id, '_blank');
    }
});