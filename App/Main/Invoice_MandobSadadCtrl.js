app.controller("Invoice_MandobSadadCtrl", function ($scope, $http, $window, $timeout, $uibModal) {
  
 
    $scope.Cls_Invoice_Mandob = {
        ID: 0, Mandob_Name: "", Price: 0, Total_Sadad: 0, Remain: 0, Sadad_Count: 0, User_Name: "", ComputerName: "", ComputerUser: "",
        ClsInvoice_Mandob_Sadad : null
    };
    $scope.New_Sadad = { ID: 0, Invoice_Id: 0, Sadad_Type_Id: "", Date_Added: "", Money: "", Remain: 0, User_Name: "", User_ID: 0, ComputerName: "", ComputerUser: "", InDate: "" };
    $scope.switchCase = function () {
         
        var QueryString = location.search;
        var urlParams = new URLSearchParams(QueryString);
       
        $scope.EndSearch = null;
        $scope.TableLoading = 1;
        
        if (urlParams.has('id') == true) {
            var id = urlParams.get('id');    // 1234
               
            $http.get('/Sadad_Mandob/Load_SadadMandob?id=' + id)
               .success(function (response) {
                  
                   if (response == '' || response == null) {
                       $scope.EndSearch = 1;
                   }
                   else {
                       debugger;
                       $scope.Cls_Invoice_Mandob.ID = response.ID;
                       $scope.Cls_Invoice_Mandob.Mandob_Name = response.Mandob_Name;
                       $scope.Cls_Invoice_Mandob.Date_Invoice = response.Date_Invoice;
                               
                       $scope.Cls_Invoice_Mandob.Price= response.Price;
                       $scope.Cls_Invoice_Mandob.Total_Sadad = response.Total_Sadad;
                       $scope.Cls_Invoice_Mandob.Total_Return = response.Total_Return;
                       $scope.Cls_Invoice_Mandob.Remain = response.Remain;
                       $scope.Cls_Invoice_Mandob.Sadad_Count = response.Sadad_Count;

                       $scope.Cls_Invoice_Mandob.User_Name= response.User_Name;
                       $scope.Cls_Invoice_Mandob.ComputerName= response.ComputerName;
                       $scope.Cls_Invoice_Mandob.ComputerUser = response.ComputerUser;
                       $scope.Cls_Invoice_Mandob.ClsInvoice_Mandob_Sadad = response.ClsInvoice_Mandob_Sadad;
                       if ($scope.Cls_Invoice_Mandob.ClsInvoice_Mandob_Sadad == null)
                       {
                           $scope.EndSearch = 1;
                       }
                       $scope.TableLoading = null;
                   }
               });
           
        } else {
            $uibModal.open({
                template: " <div class='portlet box red' style='min-height:150px !important;min-width: 541px !important;'> <div class='portlet-title'> <div class='caption'> <i class='fa-lg fa fa-warning'></i> تنبيه هام </div><div class='tools'> </div></div><div class='portlet-body form portlet-empty' style='min-height: 140px;'> <div class='col-md-12 page-404'> <div class='number'>900</div><div class='details'> <h3 style='font-family: sans-serif; text-align: center; font-weight: bold;'>AR-ContainChar-900</h3> <p style='font-size: 12px;'> رقم غير صحيح برجاء المحاولة مرة آخري <br/> <br/> <a class='btn btn-default btn-sm' href='/home'> الصفحة الرئيسية </a> </p></div></div></div></div>",
                closeOnEscape: false,
                closeOnOverlayClick: false
            });
        
        }
    }
    $scope.switchCase();


    var html_create;
    $scope.Create = function () {
        debugger;
        $scope.New_Sadad.Remain = $scope.Cls_Invoice_Mandob.Remain;
        $scope.New_Sadad.Invoice_Id = $scope.Cls_Invoice_Mandob.ID;
        $modelDialog = $uibModal.open({
            scope: $scope,
            template: html_create,
            resolve: {
                Cls_Invoice_Mandob: function () {
                    return $scope.Cls_Invoice_Mandob;
                }, ClsInvoice_Mandob_Sadad: function () {
                    return $scope.ClsInvoice_Mandob_Sadad;
                }
            }
        });
       
    };

    $scope.CreateView = function () {
        $http.get("/Sadad_Mandob/Create")
         .success(function (response) {
             html_create = response;
         });


    };
    $scope.CreateView();
    
    $scope.SaveSadad = function () {
        debugger;
        $scope.New_Sadad.Sadad_Type_Id = document.getElementById("Sadad_Type_Id").value;
        var CheckForm = $scope.FormValidation($scope.New_Sadad);
        if (CheckForm == true) {
            
            var parameter = JSON.stringify($scope.New_Sadad);
            $http.post('/Sadad_Mandob/Save_Sadad', parameter)
                 .success(function (response) {
                     var test = response;
                     if (response.ErrorName.indexOf("صلاحية") > -1 || response.ErrorName.indexOf("المسموح") > -1) {
                         $uibModal.open({
                             template: " <div class='portlet box red' style='min-height:150px !important ;min-width: 541px !important;'> <div class='portlet-title'> <div class='caption'> <i class='fa-lg fa fa-warning'></i> تنبيه هام </div><div class='tools'> </div></div><div class='portlet-body form portlet-empty' style='min-height: 140px;'> <div class='col-md-12 page-404'> <div class='number'>" + response.ErrorNumber + "</div><div class='details'> <h3 style='font-family: sans-serif; text-align: center; font-weight: bold;'>" + response.ErrorFullNumber + "</h3> <p> " + response.ErrorName + " <br/> <br/> <a class='btn btn-default btn-sm' href='/home'> الصفحة الرئيسية </a> </p></div></div></div></div>",
                             closeOnEscape: false,
                             closeOnOverlayClick: false
                         });
                     } else {
                         //$scope.Cls_Invoice_Mandob.ID = response.ID;
                         if ($scope.Cls_Invoice_Mandob.ClsInvoice_Mandob_Sadad == null)
                         {
                             $scope.Cls_Invoice_Mandob.ClsInvoice_Mandob_Sadad = new Array();
                             
                            
                         }
                         var Sadad = {
                             ID: response.Invoice_Product_ID,
                             Invoice_Id: $scope.New_Sadad.Invoice_Id,
                             Sadad_Type_Id: $scope.New_Sadad.Sadad_Type_Id,
                             Date_Added: response.Date_Added,
                             Money: $scope.New_Sadad.Money,
                             Remain: $scope.New_Sadad.Remain,
                             User_Name: $scope.New_Sadad.User_Name,
                             User_ID: $scope.New_Sadad.User_ID,
                             ComputerName: $scope.New_Sadad.ComputerName,
                             ComputerUser: $scope.New_Sadad.ComputerUser,
                             InDate: $scope.New_Sadad.InDate 
                         };
                         $scope.Cls_Invoice_Mandob.ClsInvoice_Mandob_Sadad.push(Sadad);
                         $scope.New_Sadad = { ID: 0, Invoice_Id: 0, Sadad_Type_Id: "", Date_Added: "", Money: 0, Remain: 0, User_Name: "", User_ID: 0, ComputerName: "", ComputerUser: "", InDate: "" };
                         //**************************************************
                         //****** مشكلة لو هو ارجاع بضاعة
                         $scope.Cls_Invoice_Mandob.Total_Sadad = parseFloat(parseFloat(parseFloat($scope.Cls_Invoice_Mandob.Total_Sadad) + parseFloat(Sadad.Money)).toFixed(2));
                         $scope.Cls_Invoice_Mandob.Remain = parseFloat(parseFloat(parseFloat($scope.Cls_Invoice_Mandob.Remain) - parseFloat(Sadad.Money)).toFixed(2));
                         $scope.Cls_Invoice_Mandob.Sadad_Count = parseFloat($scope.Cls_Invoice_Mandob.Sadad_Count) + 1;
                         //**************************************************
                         $modelDialog.close();
                     }
                 });
        }
        else {
            $scope.AlertParameter = 1;
            document.body.scrollTop = 0;
        }
    };
   
    $scope.EditSadad = function () {
        debugger;
        $scope.Edit_Sadad.Sadad_Type_Id = document.getElementById("Sadad_Type_Id").value;
        var CheckForm = $scope.FormValidation($scope.Edit_Sadad);
        if (CheckForm == true) {

            var parameter = JSON.stringify($scope.Edit_Sadad);
            $http.post('/Sadad_Mandob/Edit_Sadad', parameter)
                 .success(function (response) {
                     var test = response;
                     if (response.ErrorName.indexOf("صلاحية") > -1 || response.ErrorName.indexOf("المسموح") > -1) {
                         $uibModal.open({
                             template: " <div class='portlet box red' style='min-height:150px !important ;min-width: 541px !important;'> <div class='portlet-title'> <div class='caption'> <i class='fa-lg fa fa-warning'></i> تنبيه هام </div><div class='tools'> </div></div><div class='portlet-body form portlet-empty' style='min-height: 140px;'> <div class='col-md-12 page-404'> <div class='number'>" + response.ErrorNumber + "</div><div class='details'> <h3 style='font-family: sans-serif; text-align: center; font-weight: bold;'>" + response.ErrorFullNumber + "</h3> <p> " + response.ErrorName + " <br/> <br/> <a class='btn btn-default btn-sm' href='/home'> الصفحة الرئيسية </a> </p></div></div></div></div>",
                             closeOnEscape: false,
                             closeOnOverlayClick: false
                         });
                     } else {
                         //$scope.Cls_Invoice_Mandob.ID = response.ID;
                         if ($scope.Cls_Invoice_Mandob.ClsInvoice_Mandob_Sadad == null) {
                             $scope.Cls_Invoice_Mandob.ClsInvoice_Mandob_Sadad = new Array();


                         }
                         var index = $scope.Edit_index;
                             $scope.Cls_Invoice_Mandob.ClsInvoice_Mandob_Sadad[index].ID= response.Invoice_Product_ID,
                             
                             $scope.Cls_Invoice_Mandob.ClsInvoice_Mandob_Sadad[index].Sadad_Type_Id= $scope.Edit_Sadad.Sadad_Type_Id;
                             $scope.Cls_Invoice_Mandob.ClsInvoice_Mandob_Sadad[index].Date_Added= response.Date_Added;
                             $scope.Cls_Invoice_Mandob.ClsInvoice_Mandob_Sadad[index].Money= $scope.Edit_Sadad.Money;
                             $scope.Cls_Invoice_Mandob.ClsInvoice_Mandob_Sadad[index].Remain= $scope.Edit_Sadad.Remain;
                     
                             $scope.Edit_Sadad = { ID: 0, Invoice_Id: 0, Sadad_Type_Id: "", Date_Added: "", Money: 0, Remain: 0, User_Name: "", User_ID: 0, ComputerName: "", ComputerUser: "", InDate: "" };
                         //**************************************************
                         //****** مشكلة لو هو ارجاع بضاعة
                             $scope.Cls_Invoice_Mandob.Total_Sadad = parseFloat(parseFloat(parseFloat($scope.Cls_Invoice_Mandob.Price) + parseFloat($scope.Cls_Invoice_Mandob.ClsInvoice_Mandob_Sadad[index].Remain)).toFixed(2));
                             $scope.Cls_Invoice_Mandob.Remain = parseFloat(parseFloat($scope.Cls_Invoice_Mandob.ClsInvoice_Mandob_Sadad[index].Remain).toFixed(2));
 
                         //**************************************************
                         $modelDialog.close();
                     }
                 });
        }
        else {
            $scope.AlertParameter = 1;
            document.body.scrollTop = 0;
        }
    };

    $scope.FormValidation = function (Sadad) {
        debugger;
        $scope.AlertParameter = 0;
        // var JawalNOCheck = Company.JawalNO.match("^05[0-9]{8}$");
        if (Sadad.Money == null || Sadad.Money == "" || parseFloat(Sadad.Money) == 0) {
            $scope.ErrorName = "برجاء ادخال مبلغ إجمالي السداد";
            var ListID = ["Money"];
            $scope.changeBorder(ListID, "red");
            return false;
        } else {
            var ListID = ["Money"];
            $scope.changeBorder(ListID, "#c2cad8");
        }
        // var JawalNOCheck = Company.JawalNO.match("^05[0-9]{8}$");
        if (Sadad.Sadad_Type_Id == null || Sadad.Sadad_Type_Id == "") {
            $scope.ErrorName = "برجاء ادخال مبلغ إجمالي السداد";
            var ListID = ["Sadad_Type_Id"];
            $scope.changeBorder(ListID, "red");
            return false;
        } else {
            var ListID = ["Sadad_Type_Id"];
            $scope.changeBorder(ListID, "#c2cad8");
        }


        


        return true;
    }
    //----------------------------------------------------------------------------------------------------------------------------
    var DealingTypeView;
    //-------------------------------------------------
    $scope.getView = function () {
        $http.get("/Sadad_Mandob/GetProducts")
         .success(function (response) {
             DealingTypeView = response;
         });
    };
    $scope.getView();

    $scope.loadproduct = function () {
        debugger;
        if ($scope.New_Sadad.Sadad_Type_Id == 4)// ارجاع بضاعة
        {
            //-----
            $scope.ProductList = null;
            $scope.getAllProducts();
            //------------
            $modelDialog = $uibModal.open({

                scope: $scope,
                template: DealingTypeView,
                resolve: {
                    Cls_Invoice_Mandob: function () {
                        return $scope.Cls_Invoice_Mandob;
                    }
                }
            });
        }
    };
    $scope.getAllProducts = function (Page, Search, Src_Barcode) {
        var parameter = { 'Search': Search, 'Src_Barcode': Src_Barcode }
        var sentData = "invoice_id=" + $scope.Cls_Invoice_Mandob.ID + "&page=" + Page;
        $http.get("/Sadad_Mandob/getAllProducts?" + sentData, { params: parameter })
            .success(function (response) {
                if (response != null) {

                    if ($scope.ProductList == null) {
                        debugger;
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
    $scope.MoneyCalculate = function () {
        var remain = $scope.Cls_Invoice_Mandob.Remain;
        $scope.New_Sadad.Remain = remain
        $scope.New_Sadad.Remain = parseFloat(parseFloat($scope.New_Sadad.Remain - $scope.New_Sadad.Money).toFixed(2));
        if ($scope.New_Sadad.Remain < 0)
        {
            $scope.New_Sadad.Remain = remain
            $scope.New_Sadad.Money = 0;
        }
    };
   
    //-----------------------------------------------
   
     
    $scope.closeUiModal = function () {
        $modelDialog.close();  
    }
    //----------------------------------------------------------------------------------------------------------------------------
    $scope.Print = function (id) {
        $http({
            url: "/Sadad_Mandob/PDF_Sadad/" + id,
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
    $scope.PrintEradat = function (id) {
        $http({
            url: "/Sadad_Mandob/Pdf_Eradat/" + id,
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

    $scope.Edit = function (id) {

        //البحث هل الصنف ده موجود قبل كدة 
        $scope.Edit_index = $scope.Cls_Invoice_Mandob.ClsInvoice_Mandob_Sadad.findIndex(function (item, i) {
            return ((item.ID === id));
        });
        if ($scope.Edit_index != -1)
            $scope.Edit_Sadad = {
                ID: $scope.Cls_Invoice_Mandob.ClsInvoice_Mandob_Sadad[$scope.Edit_index].ID,
                Invoice_Id: $scope.Cls_Invoice_Mandob.ClsInvoice_Mandob_Sadad[$scope.Edit_index].Invoice_Id,
                Sadad_Type_Id: $scope.Cls_Invoice_Mandob.ClsInvoice_Mandob_Sadad[$scope.Edit_index].Sadad_Type_Id,
                Date_Added: $scope.Cls_Invoice_Mandob.ClsInvoice_Mandob_Sadad[$scope.Edit_index].Date_Added,
                Money: $scope.Cls_Invoice_Mandob.ClsInvoice_Mandob_Sadad[$scope.Edit_index].Money,
                Money_Copy: $scope.Cls_Invoice_Mandob.ClsInvoice_Mandob_Sadad[$scope.Edit_index].Money,
                Remain: $scope.Cls_Invoice_Mandob.Remain,
                Remain_Plus_Money: parseFloat($scope.Cls_Invoice_Mandob.Remain) + parseFloat($scope.Cls_Invoice_Mandob.ClsInvoice_Mandob_Sadad[$scope.Edit_index].Money)
            };
        
        $modelDialog = $uibModal.open({
            scope: $scope,
            template: html_Edit,
            resolve: {
                Cls_Invoice_Mandob: function () {
                    return $scope.Cls_Invoice_Mandob;
                }, ClsInvoice_Mandob_Sadad: function () {
                    return $scope.ClsInvoice_Mandob_Sadad;
                }
            }
        });
    }

    $scope.EditMoneyCalculate = function () {
        var remain = $scope.Edit_Sadad.Remain_Plus_Money;
        var result = parseFloat(remain) - parseFloat($scope.Edit_Sadad.Money);
        if (result < 0) {
            $scope.Edit_Sadad.Remain = parseFloat(parseFloat(parseFloat($scope.Edit_Sadad.Remain_Plus_Money) - parseFloat($scope.Edit_Sadad.Money_Copy)).toFixed(2));
            $scope.Edit_Sadad.Money = parseFloat(parseFloat($scope.Edit_Sadad.Money_Copy).toFixed(2));
        } else {
            $scope.Edit_Sadad.Remain = parseFloat(parseFloat(result).toFixed(2));
        }
    };
    var html_Edit;
    $scope.EditView = function () {
        $http.get("/Sadad_Mandob/Edit")
         .success(function (response) {
             html_Edit = response;
         });


    };
    $scope.EditView();
    //----------------------------------------------------------------------------------------------------------------------------
    $scope.Calculate_Return_Amount = function (item) {
        
        item.Remain_Amount = parseFloat(parseFloat(item.Amount - item.Return_Amount).toFixed(2));
        if (item.Remain_Amount < 0) {
            item.Remain_Amount = item.Amount
            item.Return_Amount = 0;
        } else {
            item.Return_Price = parseFloat(parseFloat(item.Return_Amount * item.Price).toFixed(2));
            $scope.Calculate_ReurnPrice();
        }
    };
    $scope.Calculate_ReurnPrice = function () {

        $scope.New_Sadad.Money = 0;
        for (var i = 0; i < $scope.ProductList.length; i++) {
            if (isNaN(parseFloat($scope.ProductList[i].Return_Price)) == false) {
                $scope.New_Sadad.Money += parseFloat($scope.ProductList[i].Return_Price);
            }
        }
        $scope.New_Sadad.Money = parseFloat(parseFloat($scope.New_Sadad.Money).toFixed(2));
        
    }
    $scope.Save_ReturnPorducts = function () {
        $scope.New_Sadad.Sadad_Type_Id = document.getElementById("Sadad_Type_Id").value;
        var parameter = JSON.stringify({ New_Sadad: $scope.New_Sadad, ProductList: $scope.ProductList });
        $http.post('/Sadad_Mandob/Save_ReturnPorducts', parameter)
            .success(function (response) {

                if (response.ErrorName.indexOf("صلاحية") > -1 || response.ErrorName.indexOf("المسموح") > -1) {
                    $uibModal.open({
                        template: " <div class='portlet box red' style='min-height:150px !important ;min-width: 541px !important;'> <div class='portlet-title'> <div class='caption'> <i class='fa-lg fa fa-warning'></i> تنبيه هام </div><div class='tools'> </div></div><div class='portlet-body form portlet-empty' style='min-height: 140px;'> <div class='col-md-12 page-404'> <div class='number'>" + response.ErrorNumber + "</div><div class='details'> <h3 style='font-family: sans-serif; text-align: center; font-weight: bold;'>" + response.ErrorFullNumber + "</h3> <p> " + response.ErrorName + " <br/> <br/> <a class='btn btn-default btn-sm' href='/home'> الصفحة الرئيسية </a> </p></div></div></div></div>",
                        closeOnEscape: false,
                        closeOnOverlayClick: false
                    });
                } else {
                    $window.location.href = '/Sadad_Mandob/SadadMandob?id=' + $scope.Cls_Invoice_Mandob.ID;
                }
            });
    };
    //----------------------------------------------------------------------------------------------------------------------------
    $scope.ReomveAlert = function () {
        $scope.AlertParameter = null;
    }
    $scope.changeBorder = function (arr, color) {
        for (var i = 0; i < arr.length; i++) {
            document.getElementById(arr[i]).style.borderColor = color;
        }
    }
  
   
});

