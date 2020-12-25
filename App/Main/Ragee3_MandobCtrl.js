app.controller("Ragee3_MandobCtrl", function ($scope, $http, $window, $timeout, $uibModal) {
    $scope.Intialize_product = function () {
        $scope.Cls_Invoice_Mandob = {

            ID: "", Invoice_Number: "", Mandob_id: "", Mandob_Name: "", Date_Invoice: "", Date_Invoice_Hijri: "", Price: 0, Total_Sadad: "", Price_Mowrid: 0, Payment_Type: "", Payment_Type_Name: "",
            Customer_Type: "", Customer_Name: "",
            ClsInvoiceMandob_Product: null
        };
        var product = { ID: "", Product_Id: "", Product_Name: "", Product_Name_Orginal: "", Amount: "", Price: 0, Price_Mowrid: 0, TotalPrice: 0, Offer_TargetAmount: "", Offer_BonusAmount: "", Offer_Product_id: "", Offer_BonusAmount_Orginal: "", Offer_Product_id_Orginal: "", Offer_Product_Name_Orginal: "" };
        $scope.Cls_Invoice_Mandob.ClsInvoiceMandob_Product = new Array();
        $scope.Cls_Invoice_Mandob.ClsInvoiceMandob_Product.push(product);
        $scope.Cls_Invoice_Mandob.Price = parseFloat(parseFloat($scope.Cls_Invoice_Mandob.Price).toFixed(2));
        document.getElementById('InvoicePrice').innerHTML = $scope.Cls_Invoice_Mandob.Price;

    };
    $scope.Intialize_product();
    $scope.Search = "";
    $scope.product_Index = -1;

    $scope.Cls_LogIn = { UserName: "", Password: "" };
    //-- عملية حفظ / تعديل / حذف الفاتورة
    $scope.switchCase = function () {

        var QueryString = location.search;
        var urlParams = new URLSearchParams(QueryString);
        if (urlParams.has('id') == true) {
            $scope.SaveBtn = 1;
            $scope.dataEntire = 1;
            $scope.EditBtn = null;
            $scope.DeleteBtn = null;

            var id = urlParams.get('id');    // 1234
            //document.getElementById('PrintInvoice').href = "/Invoice_Cancel/PrintInvoice/" + id;
            $http.get('/Ragee3_Mandob/loadInvoice/' + id)
                .success(function (response) {
                    if (response == null) {
                        $uibModal.open({
                            template: " <div class='portlet box red' style='min-height:150px !important;min-width: 541px !important;'> <div class='portlet-title'> <div class='caption'> <i class='fa-lg fa fa-warning'></i> تنبيه هام </div><div class='tools'> </div></div><div class='portlet-body form portlet-empty' style='min-height: 140px;'> <div class='col-md-12 page-404'> <div class='number'>" + response.ErrorNumber + "</div><div class='details'> <h3 style='font-family: sans-serif; text-align: center; font-weight: bold;'>" + response.ErrorFullNumber + "</h3> <p style='font-size: 12px;'> " + response.ErrorName + " <br/> <br/> <a class='btn btn-default btn-sm' href='/home'> الصفحة الرئيسية </a> <a class='btn btn-default btn-sm' href='" + response.Url + "' style='margin-right: 4px;'>المحاولة مرة آخري</a> </p></div></div></div></div>",
                            closeOnEscape: false,
                            closeOnOverlayClick: false
                        });
                    }
                    else {

                        $scope.Cls_Invoice_Mandob = response;
                        $('#Date_Invoice').val($scope.Cls_Invoice_Mandob.Date_Invoice);
                        $("#Invoice_ID").val($scope.Cls_Invoice_Mandob.ID);
                        document.getElementById('InvoicePrice').innerHTML = $scope.Cls_Invoice_Mandob.Price;
                        document.getElementById('Total_Sadad').innerHTML = $scope.Cls_Invoice_Mandob.Total_Sadad;
                        var product = { ID: "", Product_Id: "", Product_Name: "", Product_Name_Orginal: "", Amount: "", Price: 0, Price_Mowrid: 0, TotalPrice: 0, Offer_TargetAmount: "", Offer_BonusAmount: "", Offer_Product_id: "", Offer_BonusAmount_Orginal: "", Offer_Product_id_Orginal: "", Offer_Product_Name_Orginal: "" };
                        $scope.Cls_Invoice_Mandob.ClsInvoiceMandob_Product.push(product);
                    }
                });

        } else {
            $scope.SaveBtn = null;
            $scope.EditBtn = 1;
            $scope.DeleteBtn = 1;
            $scope.dataEntire = null;
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
    // فاتورة شركة
    //----------------------------------------------------------------------------------------------------------------------------
    var DealingTypeView;
    //-------------------------------------------------
    $scope.getView = function () {
        $http.get("/Ragee3_Mandob/GetProducts")
            .success(function (response) {
                DealingTypeView = response;
            });
    };
    $scope.getView();

    $scope.loadproduct = function (index) {
        $scope.ProductList = null;
        page = 0;
        $scope.getAllRecords(0, "");
        $scope.product_Index = index;
        //-----
        $uibModal.open({
            scope: $scope,
            template: DealingTypeView,
            resolve: {
                Cls_Invoice_Mandob: function () {
                    return $scope.Cls_Invoice_Mandob;
                },
                product_Index: function () {
                    return $scope.product_Index;
                }
            }
        });

    };
    $scope.Validation_SavePrevProducts = function () {
        $scope.AlertParameter = 0;
        if ($scope.Cls_Invoice_Mandob.Payment_Type_Name == null || $scope.Cls_Invoice_Mandob.Payment_Type_Name == "") {
            $scope.ErrorName = "برجاء ادخال طريقة السداد";
            var ListID = ["Payment_Type_Name"];
            $scope.changeBorder(ListID, "red");
            return false;
        } else {
            var ListID = ["Payment_Type_Name"];
            $scope.changeBorder(ListID, "#c2cad8");
        }

        if ($scope.Cls_Invoice_Mandob.Customer_Name == null || $scope.Cls_Invoice_Mandob.Customer_Name == "") {
            $scope.ErrorName = "برجاء ادخال نوع العميل";
            var ListID = ["Customer_Name"];
            $scope.changeBorder(ListID, "red");
            return false;
        } else {
            var ListID = ["Customer_Name"];
            $scope.changeBorder(ListID, "#c2cad8");
        }

        if ($scope.Cls_Invoice_Mandob.Mandob_Name == null || $scope.Cls_Invoice_Mandob.Mandob_Name == "") {
            $scope.ErrorName = "برجاء ادخال اسم مندوب المبيعات";
            var ListID = ["Mandob_Name"];
            $scope.changeBorder(ListID, "red");
            return false;
        } else {
            var ListID = ["Mandob_Name"];
            $scope.changeBorder(ListID, "#c2cad8");
        }


        var len = $scope.Cls_Invoice_Mandob.ClsInvoiceMandob_Product.length;

        for (var i = 0; i < len; i++) {

            //-----------------------------------------------------------------------------------------
            // -- التأكد اولا انه تم حفظ جميع الاصناف السابقة بدون خطأ او فقد تاريخ انتهاء الصلاحية
            if (len > 1 && i < (len - 1)) {
                if ($scope.Cls_Invoice_Mandob.ClsInvoiceMandob_Product[i].ID == null || $scope.Cls_Invoice_Mandob.ClsInvoiceMandob_Product[i].ID == "") {
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
    //-----------------------------------------------
    var SearchProductView;
    //-------------------------------------------------
    $scope.getView = function () {
        $http.get("/Product/GetSearchProducts")
            .success(function (response) {
                SearchProductView = response;
            });
    };
    $scope.getView();

    $scope.Sreachproduct = function () {
        $uibModal.open({

            scope: $scope,
            template: SearchProductView,
            resolve: {
                Cls_Invoice_Mandob: function () {
                    return $scope.Cls_Invoice_Mandob;
                }
            }
        });

    };
    //-----------------------------------------------

    $scope.getAllMandob = function (Page, Search) {

        var parameter = { 'Search': Search }
        var sentData = "page=" + Page;
        $http.get("/Ragee3_Mandob/getAllMandob?" + sentData, { params: parameter })
            .success(function (response) {
                if (response != null && response != "") {
                    if ($scope.MandobList == null) {
                        $scope.MandobList = response;
                    }
                    else {
                        $scope.MandobList = $scope.MandobList.concat(response);
                    }
                }
                //if (response == null || response == "") { otherData = 1; hideLoadMore = null; }
                //else {  otherData = null; hideLoadMore = 1; }
            });
    }
    $scope.getAllMandob();
    //-------------------------------------------------
    var Mandoblistview;

    $scope.getMandob = function () {
        $http.get("/Ragee3_Mandob/GetMandob")
            .success(function (response) {
                Mandoblistview = response;
            });
    };
    $scope.getMandob();
    $scope.loadMandob = function () {


        $uibModal.open({
            template: Mandoblistview,
            scope: $scope,
            resolve: {
                Invoice_Mandob: function () {
                    return $scope.Cls_Invoice_Mandob;
                },
                MandobList: function () {
                    return $scope.MandobList;
                }
            },
            backdrop: 'static',
            keyboard: false
        });

    };


    // instanse Search for Departments
    //----------------------------------------------------------------------------------------------------------------------------
    var lastsearch = "";
    var lastSrc_Barcode = "";
    $scope.SearchMandob = function () {
        var search = $("#Search").val();


        if (search != lastsearch) {
            $scope.MandobList = null;
            page = 0;
            $scope.getAllMandob(null, search, "");
        }
        lastsearch = search;
    }
    //----------------------------------------------------------------------------------------------------------------------------
    //------------------تحميل الصنفات بعد فتح النافذة

    //===================
    //=======عند الضفط على صف

    $scope.removeRow = function (index) {

        if ($scope.Cls_Invoice_Mandob.ClsInvoiceMandob_Product.length > 1) {
            var Invoice_Mandob_Product_ID = $scope.Cls_Invoice_Mandob.ClsInvoiceMandob_Product[index].ID;
            $scope.Cls_Invoice_Mandob.ClsInvoiceMandob_Product.splice(index, 1);
            $scope.Calculate_price();
            $scope.DeleteItem(Invoice_Mandob_Product_ID);
        }
    }

    $scope.DeleteItem = function (invoice_product_id) {
        var sentData = "Invoice_Mandob_Product_ID=" + invoice_product_id;
        var parameter = JSON.stringify($scope.Cls_Invoice_Mandob);
        $http.post('/Ragee3_Mandob/DeleteItem?' + sentData, parameter)
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


    };
    $scope.closeUiModal = function () {
        $scope.product_Index = -1;
        this.$close();

    }


    $scope.closeUiModal_Mandob = function () {
        // $scope.Cls_Invoice_Mandob.Payment_Type = 1;
        this.$close();
    }
    //----------------------------------------------------------------------------------------------------------------------------
    //----------------------------------------------------------------------------------------------------------------------------
    // instanse Search for Departments
    //----------------------------------------------------------------------------------------------------------------------------
    var lastsearch = "";
    var lastSrc_Barcode = "";
    $scope.keyupevt = function () {
        var search = $("#Search").val();
        var Src_Barcode = $("#Src_Barcode").val();

        if (Src_Barcode != "" && Src_Barcode != lastSrc_Barcode) {
            $scope.ProductList = null;
            page = 0;
            $scope.getAllRecords(null, "", Src_Barcode);
        }
        else if (search != lastsearch) {
            $scope.ProductList = null;
            page = 0;
            $scope.getAllRecords(null, search, "");
        }
        lastsearch = search;
        lastSrc_Barcode = Src_Barcode;
    };
    //----------------------------------------------------------------------------------------------------------------------------

    //***************العمليات الحسابية -----------------------------------------------------------------------------
    $scope.Calculate_price = function () {

        $scope.Cls_Invoice_Mandob.Price = 0;
        for (var i = 0; i < $scope.Cls_Invoice_Mandob.ClsInvoiceMandob_Product.length; i++) {
            if (isNaN(parseFloat($scope.Cls_Invoice_Mandob.ClsInvoiceMandob_Product[i].TotalPrice)) == false) {
                if (isNaN(parseFloat($scope.Cls_Invoice_Mandob.ClsInvoiceMandob_Product[i].TotalPrice)) == false) {
                    $scope.Cls_Invoice_Mandob.Price += parseFloat($scope.Cls_Invoice_Mandob.ClsInvoiceMandob_Product[i].TotalPrice);
                }
            }
        }
        $scope.Cls_Invoice_Mandob.Price = parseFloat(parseFloat($scope.Cls_Invoice_Mandob.Price).toFixed(2));
        document.getElementById('InvoicePrice').innerHTML = $scope.Cls_Invoice_Mandob.Price;
    }
    //-----
    $scope.Calculate_itemPrice = function (item, index) {

        if (isNaN(parseFloat(item.Price)) == false && isNaN(parseFloat(item.Amount)) == false) {
            item.TotalPrice = parseFloat((parseFloat(item.Price) * parseFloat(item.Amount)).toFixed(2));
            $scope.Calculate_price();
           
        }
    }

    $scope.Calculate_Amount = function (item, index) {
        var CheckForm = $scope.FormValidation();
        if (CheckForm == true) {
            debugger;
            if (isNaN(parseFloat(item.Price)) == false && isNaN(parseFloat(item.Amount)) == false) {
                item.TotalPrice = parseFloat((parseFloat(item.Price) * parseFloat(item.Amount)).toFixed(2));
                var amount = parseFloat(item.Amount);
                var TargetAmount = parseFloat(item.Offer_TargetAmount);
                if (TargetAmount != null && amount >= TargetAmount) {
                    var result = parseInt((amount / TargetAmount).toFixed(2));
                    item.Offer_BonusAmount = parseFloat(result) * parseFloat(item.Offer_BonusAmount_Orginal);
                    item.Offer_Product_id = item.Offer_Product_id_Orginal;
                    item.Product_Name = item.Product_Name_Orginal + " + كمية مجانية عدد " + item.Offer_BonusAmount + " من " + item.Offer_Product_Name_Orginal;

                    $scope.Cls_Invoice_Mandob.ClsInvoiceMandob_Product[index].Offer_BonusAmount = parseFloat(result) * parseFloat(item.Offer_BonusAmount_Orginal);
                    $scope.Cls_Invoice_Mandob.ClsInvoiceMandob_Product[index].Offer_Product_id = item.Offer_Product_id_Orginal;
                } else {
                    item.Product_Name = item.Product_Name_Orginal;
                    item.Offer_BonusAmount = "";
                    item.Offer_Product_id = "";
                    $scope.Cls_Invoice_Mandob.ClsInvoiceMandob_Product[index].Offer_BonusAmount = "";
                    $scope.Cls_Invoice_Mandob.ClsInvoiceMandob_Product[index].Offer_Product_id = "";
                }
                $scope.Calculate_price();
              
            }
        } else {
            $scope.AlertParameter = 1;
            document.body.scrollTop = 0;
        }

    }
    //---------------------------------------------------------------------------------ب-------------------------------------------
    //------------------تحميل الصنفات بعد فتح النافذة
    $scope.getAllRecords = function (Page = 0, Search = '') {
        var Customer_Type = $scope.Cls_Invoice_Mandob.Customer_Type;
        var parameter = { 'Search': Search, 'Customer_Type': Customer_Type };
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

        var current_index = $scope.Cls_Invoice_Mandob.ClsInvoiceMandob_Product.length - 1;
        // تعديل صنف محفوظ بصنف جديد - حتى لو نفس الصنف
        if (current_index != $scope.product_Index && $scope.product_Index != -1) {
            index = $scope.product_Index;
            $scope.Cls_Invoice_Mandob.ClsInvoiceMandob_Product[index].Product_Id = row.ID;
            $scope.Cls_Invoice_Mandob.ClsInvoiceMandob_Product[index].Product_Name = row.Name;
            $scope.Cls_Invoice_Mandob.ClsInvoiceMandob_Product[index].Product_Name_Orginal = row.Name;
            $scope.Cls_Invoice_Mandob.ClsInvoiceMandob_Product[index].Taxes_Orginal = parseFloat(parseFloat(row.Taxes).toFixed(3));

            $scope.Cls_Invoice_Mandob.ClsInvoiceMandob_Product[index].Price = parseFloat(row.Price_Unit);
            $scope.Cls_Invoice_Mandob.ClsInvoiceMandob_Product[index].Price_Mowrid = parseFloat(parseFloat(row.Price_Mowrid).toFixed(2));
            $scope.Cls_Invoice_Mandob.ClsInvoiceMandob_Product[index].Offer_TargetAmount = parseFloat(row.Offer_TargetAmount);

            $scope.Cls_Invoice_Mandob.ClsInvoiceMandob_Product[index].Offer_BonusAmount = "";
            $scope.Cls_Invoice_Mandob.ClsInvoiceMandob_Product[index].Offer_BonusAmount_Orginal = parseFloat(row.Offer_BonusAmount);
            $scope.Cls_Invoice_Mandob.ClsInvoiceMandob_Product[index].Offer_Product_id = "";
            $scope.Cls_Invoice_Mandob.ClsInvoiceMandob_Product[index].Offer_Product_id_Orginal = parseFloat(row.Offer_Product_id);
            $scope.Cls_Invoice_Mandob.ClsInvoiceMandob_Product[index].Offer_Product_Name = "";
            $scope.Cls_Invoice_Mandob.ClsInvoiceMandob_Product[index].Offer_Product_Name_Orginal = row.Offer_Product_Name;
            //*******************************
            //Saving ......
            $scope.Calculate_itemPrice($scope.Cls_Invoice_Mandob.ClsInvoiceMandob_Product[index], index);
            //************************************
            $scope.product_Index = -1;
        } else {
            //البحث هل الصنف ده موجود قبل كدة 
            var index = $scope.Cls_Invoice_Mandob.ClsInvoiceMandob_Product.findIndex(function (item, i) {
                return ((item.Product_Id === row.ID) && (item.Product_Name === row.Name));
            });

            if (index == -1) {
                //لو الصنف مش موجود ضيف سطر جديد بعدد الاصناف = 1
                index = $scope.Cls_Invoice_Mandob.ClsInvoiceMandob_Product.length - 1;
                $scope.Cls_Invoice_Mandob.ClsInvoiceMandob_Product[index].Product_Id = row.ID;
                $scope.Cls_Invoice_Mandob.ClsInvoiceMandob_Product[index].Product_Name = row.Name;
                $scope.Cls_Invoice_Mandob.ClsInvoiceMandob_Product[index].Product_Name_Orginal = row.Name;
                $scope.Cls_Invoice_Mandob.ClsInvoiceMandob_Product[index].Amount = 1;
                $scope.Cls_Invoice_Mandob.ClsInvoiceMandob_Product[index].Price = parseFloat(row.Price_Unit);
                $scope.Cls_Invoice_Mandob.ClsInvoiceMandob_Product[index].Price_Mowrid = parseFloat(parseFloat(row.Price_Mowrid).toFixed(2));
                $scope.Cls_Invoice_Mandob.ClsInvoiceMandob_Product[index].Offer_TargetAmount = parseFloat(row.Offer_TargetAmount);

                $scope.Cls_Invoice_Mandob.ClsInvoiceMandob_Product[index].Offer_BonusAmount = "";
                $scope.Cls_Invoice_Mandob.ClsInvoiceMandob_Product[index].Offer_BonusAmount_Orginal = parseFloat(row.Offer_BonusAmount);
                $scope.Cls_Invoice_Mandob.ClsInvoiceMandob_Product[index].Offer_Product_id = "";
                $scope.Cls_Invoice_Mandob.ClsInvoiceMandob_Product[index].Offer_Product_id_Orginal = parseFloat(row.Offer_Product_id);
                $scope.Cls_Invoice_Mandob.ClsInvoiceMandob_Product[index].Offer_Product_Name = "";
                $scope.Cls_Invoice_Mandob.ClsInvoiceMandob_Product[index].Offer_Product_Name_Orginal = row.Offer_Product_Name;

                //*******************************
                //Saving ......
                $scope.Calculate_itemPrice($scope.Cls_Invoice_Mandob.ClsInvoiceMandob_Product[index], index);
                //************************************

                var product = { ID: "", Product_Id: "", Product_Name: "", Product_Name_Orginal: "", Amount: "", Price: 0, Price_Mowrid: 0, TotalPrice: 0, Offer_TargetAmount: "", Offer_BonusAmount: "", Offer_Product_id: "", Offer_BonusAmount_Orginal: "", Offer_Product_id_Orginal: "", Offer_Product_Name_Orginal: "" };
                $scope.Cls_Invoice_Mandob.ClsInvoiceMandob_Product.push(product);

            } else {
                // لو الصنف موجود بنفس الاسم (وزن-او كرتون او حبة) نزود العدد +1
                var x1 = $scope.Cls_Invoice_Mandob.ClsInvoiceMandob_Product[index].Product_Name;
                var x2 = row.Name;
                if ($scope.Cls_Invoice_Mandob.ClsInvoiceMandob_Product[index].Product_Name == row.Name) {
                    $scope.Cls_Invoice_Mandob.ClsInvoiceMandob_Product[index].Amount = parseFloat($scope.Cls_Invoice_Mandob.ClsInvoiceMandob_Product[index].Amount) + 1;

                    //*******************************
                    //Saving ......
                    $scope.Calculate_itemPrice($scope.Cls_Invoice_Mandob.ClsInvoiceMandob_Product[index], index);
                    //************************************
                } else {
                    //الاسم مختلف لاخلاف الاسم - لاختلاف الميزان - او الحبة و الكرتون و الصندوق ** نضيف سطر جديد 
                    //لو الصنف مش موجود ضيف سطر جديد بعدد الاصناف = 1
                    index = $scope.Cls_Invoice_Mandob.ClsInvoiceMandob_Product.length - 1;
                    $scope.Cls_Invoice_Mandob.ClsInvoiceMandob_Product[index].Product_Id = row.ID;
                    $scope.Cls_Invoice_Mandob.ClsInvoiceMandob_Product[index].Product_Name = row.Name;
                    $scope.Cls_Invoice_Mandob.ClsInvoiceMandob_Product[index].Product_Name_Orginal = row.Name;
                    $scope.Cls_Invoice_Mandob.ClsInvoiceMandob_Product[index].Amount = 1;
                    $scope.Cls_Invoice_Mandob.ClsInvoiceMandob_Product[index].Price = parseFloat(row.Price_Unit);
                    $scope.Cls_Invoice_Mandob.ClsInvoiceMandob_Product[index].Price_Mowrid = parseFloat(parseFloat(row.Price_Mowrid).toFixed(2));

                    $scope.Cls_Invoice_Mandob.ClsInvoiceMandob_Product[index].Offer_TargetAmount = parseFloat(row.Offer_TargetAmount);
                    $scope.Cls_Invoice_Mandob.ClsInvoiceMandob_Product[index].Offer_BonusAmount = "";
                    $scope.Cls_Invoice_Mandob.ClsInvoiceMandob_Product[index].Offer_BonusAmount_Orginal = parseFloat(row.Offer_BonusAmount);
                    $scope.Cls_Invoice_Mandob.ClsInvoiceMandob_Product[index].Offer_Product_id = "";
                    $scope.Cls_Invoice_Mandob.ClsInvoiceMandob_Product[index].Offer_Product_id_Orginal = parseFloat(row.Offer_Product_id);
                    $scope.Cls_Invoice_Mandob.ClsInvoiceMandob_Product[index].Offer_Product_Name = "";
                    $scope.Cls_Invoice_Mandob.ClsInvoiceMandob_Product[index].Offer_Product_Name_Orginal = row.Offer_Product_Name;

                    var product = { ID: "", Product_Id: "", Product_Name: "", Product_Name_Orginal: "", Amount: "", Price: 0, Price_Mowrid: 0, TotalPrice: 0, Offer_TargetAmount: "", Offer_BonusAmount: "", Offer_Product_id: "", Offer_BonusAmount_Orginal: "", Offer_Product_id_Orginal: "", Offer_Product_Name_Orginal: "" };
                    $scope.Cls_Invoice_Mandob.ClsInvoiceMandob_Product.push(product);
                    //*******************************
                    //Saving ......
                    $scope.Calculate_itemPrice($scope.Cls_Invoice_Mandob.ClsInvoiceMandob_Product[index], index);
                    //************************************
                }
            }

        }

        this.$close();


    }

    //=======عند الضفط على مندوب المبيعات
    $scope.clicktrMandob = function (row) {

        $scope.Cls_Invoice_Mandob.Mandob_id = row.ID;
        $scope.Cls_Invoice_Mandob.Mandob_Name = row.Name;
        this.$close();
    }
    //----------------------------------------------------------------------------------------------------------------------------

    //----------------------------------------------------------------------------------------------------------------------------

  
    //=======عند الضفط على مندوب المبيعات
    $scope.clicktrMandob = function (row) {

        $scope.Cls_Invoice_Mandob.Mandob_id = row.ID;
        $scope.Cls_Invoice_Mandob.Mandob_Name = row.Name;
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

    $scope.ReomveAlert = function () {
        $scope.AlertParameter = null;
    }
    $scope.FormValidation = function () {
        $scope.AlertParameter = 0;
        $scope.Cls_Invoice_Mandob.Date_Invoice = $('#Date_Invoice').val();
        if ($scope.Cls_Invoice_Mandob.Payment_Type_Name == null || $scope.Cls_Invoice_Mandob.Payment_Type_Name == "") {
            $scope.ErrorName = "برجاء ادخال طريقة السداد";
            var ListID = ["Payment_Type_Name"];
            $scope.changeBorder(ListID, "red");
            return false;
        } else {
            var ListID = ["Payment_Type_Name"];
            $scope.changeBorder(ListID, "#c2cad8");
        }

        if ($scope.Cls_Invoice_Mandob.Customer_Name == null || $scope.Cls_Invoice_Mandob.Customer_Name == "") {
            $scope.ErrorName = "برجاء ادخال نوع العميل";
            var ListID = ["Customer_Name"];
            $scope.changeBorder(ListID, "red");
            return false;
        } else {
            var ListID = ["Customer_Name"];
            $scope.changeBorder(ListID, "#c2cad8");
        }


        if ($scope.Cls_Invoice_Mandob.Mandob_Name == null || $scope.Cls_Invoice_Mandob.Mandob_Name == "") {
            $scope.ErrorName = "برجاء ادخال اسم مندوب المبيعات";
            var ListID = ["Mandob_Name"];
            $scope.changeBorder(ListID, "red");
            return false;
        } else {
            var ListID = ["Mandob_Name"];
            $scope.changeBorder(ListID, "#c2cad8");
        }
        if ($scope.Cls_Invoice_Mandob.Date_Invoice == null || $scope.Cls_Invoice_Mandob.Date_Invoice == "") {
            $scope.ErrorName = "برجاء ادخال تاريخ البيع";
            var ListID = ["Date_Invoice"];
            $scope.changeBorder(ListID, "red");
            return false;
        } else {
            var ListID = ["Date_Invoice"];
            $scope.changeBorder(ListID, "#c2cad8");
        }

        var len = $scope.Cls_Invoice_Mandob.ClsInvoiceMandob_Product.length;
        for (var i = 0; i < len; i++) {
            if (len = 1
                ||
                ((len > 1) && i < (len - 1))) {

                if ($scope.Cls_Invoice_Mandob.ClsInvoiceMandob_Product[i].Product_Name == null || $scope.Cls_Invoice_Mandob.ClsInvoiceMandob_Product[i].Product_Name == "") {
                    $scope.ErrorName = "برجاء ادخال اسم الصنف";
                    var ListID = ["Product_Name[" + i + "]"];
                    $scope.changeBorderJQ(ListID, "red");
                    return false;
                }
                else {
                    var ListID = ["Product_Name[" + i + "]"];
                    $scope.changeBorderJQ(ListID, "#c2cad8");
                }


                if ($scope.Cls_Invoice_Mandob.ClsInvoiceMandob_Product[i].Amount == null
                    || $scope.Cls_Invoice_Mandob.ClsInvoiceMandob_Product[i].Amount == "") {
                    $scope.ErrorName = "برجاء ادخال العدد";
                    var ListID = ["Amount[" + i + "]"];
                    $scope.changeBorderJQ(ListID, "red");
                    return false;
                }
                else {
                    var ListID = ["Amount[" + i + "]"];
                    $scope.changeBorderJQ(ListID, "#c2cad8");
                }



                if ($scope.Cls_Invoice_Mandob.ClsInvoiceMandob_Product[i].Price == null || $scope.Cls_Invoice_Mandob.ClsInvoiceMandob_Product[i].Price == "") {
                    $scope.ErrorName = "برجاء ادخال السعر";
                    var ListID = ["Price[" + i + "]"];
                    $scope.changeBorderJQ(ListID, "red");
                    return false;
                }
                else {
                    var ListID = ["Price[" + i + "]"];
                    $scope.changeBorderJQ(ListID, "#c2cad8");
                }
            }

        }


        return true;
    }

    $scope.Validation_Omola = function () {
        $scope.AlertParameter = 0;
        // var JawalNOCheck = Mandob.JawalNO.match("^05[0-9]{8}$");

        if ($scope.Cls_Invoice_Mandob.Price != $scope.Cls_Invoice_Mandob.Total_Sadad) {
            $scope.ErrorName = "برجاء سداد الفاتورة بالكامل أولا";
            return false;
        }

        return true;
    }

    var View_Invoice;


    $scope.Save = function (index) {
        var CheckForm = $scope.FormValidation();
        if (CheckForm == true) {
            var sentData = "index=" + index;
            var parameter = JSON.stringify($scope.Cls_Invoice_Mandob);
            $http.post('/Ragee3_Mandob/AddItem?' + sentData, parameter)
                .success(function (response) {

                    if (response.ErrorName.indexOf("صلاحية") > -1 || response.ErrorName.indexOf("المسموح") > -1) {
                        $uibModal.open({
                            template: " <div class='portlet box red' style='min-height:150px !important ;min-width: 541px !important;'> <div class='portlet-title'> <div class='caption'> <i class='fa-lg fa fa-warning'></i> تنبيه هام </div><div class='tools'> </div></div><div class='portlet-body form portlet-empty' style='min-height: 140px;'> <div class='col-md-12 page-404'> <div class='number'>" + response.ErrorNumber + "</div><div class='details'> <h3 style='font-family: sans-serif; text-align: center; font-weight: bold;'>" + response.ErrorFullNumber + "</h3> <p> " + response.ErrorName + " <br/> <br/> <a class='btn btn-default btn-sm' href='/home'> الصفحة الرئيسية </a> </p></div></div></div></div>",
                            closeOnEscape: false,
                            closeOnOverlayClick: false
                        });
                    } else {

                        $scope.Cls_Invoice_Mandob.ID = response.ID;
                        $scope.Cls_Invoice_Mandob.Invoice_Number = response.Invoice_Number;
                        $scope.Cls_Invoice_Mandob.Total_Sadad = response.Total_Sadad;
                        document.getElementById('Total_Sadad').innerHTML = $scope.Cls_Invoice_Mandob.Total_Sadad;
                        var current_index = response.index;
                        $scope.Cls_Invoice_Mandob.ClsInvoiceMandob_Product[current_index].ID = response.Invoice_Product_ID;
                        $("#Invoice_ID").val(response.ID);
                    }
                });
        }
        else {
            $scope.AlertParameter = 1;
            document.body.scrollTop = 0;
        }


    };

    $scope.Delete = function () {


        var sentData = "ID=" + $scope.Cls_Invoice_Mandob.ID;

        $http.post('/Ragee3_Mandob/DeleteInvoice?' + sentData)
            .success(function (response) {

                var test = response;
                if (response.ErrorName.indexOf("صلاحية") > -1 || response.ErrorName.indexOf("المسموح") > -1) {
                    $uibModal.open({
                        template: " <div class='portlet box red' style='min-height:150px !important ;min-width: 541px !important;'> <div class='portlet-title'> <div class='caption'> <i class='fa-lg fa fa-warning'></i> تنبيه هام </div><div class='tools'> </div></div><div class='portlet-body form portlet-empty' style='min-height: 140px;'> <div class='col-md-12 page-404'> <div class='number'>" + response.ErrorNumber + "</div><div class='details'> <h3 style='font-family: sans-serif; text-align: center; font-weight: bold;'>" + response.ErrorFullNumber + "</h3> <p> " + response.ErrorName + " <br/> <br/> <a class='btn btn-default btn-sm' href='/home'> الصفحة الرئيسية </a> </p></div></div></div></div>",
                        closeOnEscape: false,
                        closeOnOverlayClick: false
                    });
                }
                else {
                    $scope.ErrorName = response.ErrorName;
                    $scope.AlertParameter = 1;
                    document.body.scrollTop = 0;
                    if (response.ErrorName.indexOf("بنجاح") > -1) {
                        $window.location.href = '/Ragee3_Mandob/Invoice_Mandob/';
                    }
                }

            });



    };
    $scope.Edit = function (index, invoice_product_id) {
        var CheckForm = $scope.FormValidation();
        if (CheckForm == true) {
            var sentData = "index=" + index + "&Invoice_Mandob_Product_ID=" + invoice_product_id;
            var parameter = JSON.stringify($scope.Cls_Invoice_Mandob);
            $http.post('/Ragee3_Mandob/EditItem?' + sentData, parameter)
                .success(function (response) {
                    if (response.ErrorName.indexOf("صلاحية") > -1 || response.ErrorName.indexOf("المسموح") > -1) {
                        $uibModal.open({
                            template: " <div class='portlet box red' style='min-height:150px !important ;min-width: 541px !important;'> <div class='portlet-title'> <div class='caption'> <i class='fa-lg fa fa-warning'></i> تنبيه هام </div><div class='tools'> </div></div><div class='portlet-body form portlet-empty' style='min-height: 140px;'> <div class='col-md-12 page-404'> <div class='number'>" + response.ErrorNumber + "</div><div class='details'> <h3 style='font-family: sans-serif; text-align: center; font-weight: bold;'>" + response.ErrorFullNumber + "</h3> <p> " + response.ErrorName + " <br/> <br/> <a class='btn btn-default btn-sm' href='/home'> الصفحة الرئيسية </a> </p></div></div></div></div>",
                            closeOnEscape: false,
                            closeOnOverlayClick: false
                        });
                    } else {

                        $scope.Cls_Invoice_Mandob.ID = response.ID;
                        $scope.Cls_Invoice_Mandob.Total_Sadad = response.Total_Sadad;
                        document.getElementById('Total_Sadad').innerHTML = $scope.Cls_Invoice_Mandob.Total_Sadad;
                        var current_index = response.index;
                        $scope.Cls_Invoice_Mandob.ClsInvoiceMandob_Product[current_index].ID = response.Invoice_Product_ID;
                        $("#Invoice_ID").val(response.ID);
                    }
                });
        }
        else {
            $scope.AlertParameter = 1;
            document.body.scrollTop = 0;
        }


    };

    $scope.BtnNew = function () {

        $window.location.href = '/Ragee3_Mandob/Invoice_Mandob/';
    };

    //الغاء الحفظ------------------------------------------
    var AuthorizeManager;
    $scope.getView = function () {
        $http.get("/Ragee3_Mandob/AuthorizeManager")
            .success(function (response) {
                AuthorizeManager = response;
            });
    };

    $scope.getView();
    $scope.BtnLogin = function () {
        var CheckForm = $scope.Cls_LogIn_Validation();
        if (CheckForm == true) {
            $scope.AutorizeRemove();
        }
        else {
            $scope.AlertParameter = 1;
            document.body.scrollTop = 0;
        }

    };
    $scope.AutorizeRemove = function () {

        var parameter = JSON.stringify($scope.Cls_LogIn);

        $http.post('/Ragee3_Mandob/IsAuthorizeManager', parameter)
            .success(function (response) {

                if (response == "1") {
                    $modelDialog.close();
                    $scope.Delete();
                }
                else if (response == "2") {
                    $scope.ErrorName = "ليس لديك صلاحية حذف";
                    $scope.AlertParameter = 1;
                    document.body.scrollTop = 0;
                } else if (response == "3") {
                    $scope.ErrorName = "اسم المستخدم او كلمة المرور غير صحيحة";
                    $scope.AlertParameter = 1;
                    document.body.scrollTop = 0;
                }
            });


    }
    $scope.Cls_LogIn_Validation = function () {
        if ($scope.Cls_LogIn.UserName == null || $scope.Cls_LogIn.UserName == "") {
            $scope.ErrorName = "برجاء ادخال اسم المستخدم";
            var ListID = ["UserName"];
            $scope.changeBorderJQ(ListID, "red");
            return false;
        }
        else {
            var ListID = ["UserName"];
            $scope.changeBorderJQ(ListID, "#c2cad8");
        }


        if ($scope.Cls_LogIn.Password == null || $scope.Cls_LogIn.Password == "") {
            $scope.ErrorName = "برجاء ادخال كلمة المرور";
            var ListID = ["Password"];
            $scope.changeBorderJQ(ListID, "red");
            return false;
        }
        else {
            var ListID = ["Password"];
            $scope.changeBorderJQ(ListID, "#c2cad8");
        }
        return true;
    }


    $('#Date_Invoice').Zebra_DatePicker({
        onSelect: function (date) {
            $scope.Cls_Invoice_Mandob.Date_Invoice = $('#Date_Invoice').val();
            var CheckForm = $scope.FormValidation();
            if (CheckForm == true) {
                var parameter = JSON.stringify($scope.Cls_Invoice_Mandob);
                if ($scope.Cls_Invoice_Mandob.ID != "" && $scope.Cls_Invoice_Mandob.ID != "0") //saving ....
                {
                    $http.post('/Ragee3_Mandob/EditDateInvoice', parameter)
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


    $scope.isAmount_focus = false;
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
            if ($scope.Cls_Invoice_Mandob.ClsInvoiceMandob_Product.length > 1) {

                var index = $scope.Cls_Invoice_Mandob.ClsInvoiceMandob_Product.length - 2;

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


    var html_Omola;


    $scope.BtnOmolaConfirm = function () {
        $modelDialogOmolaConfirm = $uibModal.open({
            scope: $scope,
            template: html_Omola_Confirm,
            resolve: {
                Cls_Invoice_Mandob: function () {
                    return $scope.Cls_Invoice_Mandob;
                }
            }
        });

    };
    $scope.BtnSaveOmola = function () {

        var parameter = JSON.stringify($scope.Cls_Invoice_Mandob);
        $http.post('/Ragee3_Mandob/SaveOmola', parameter)
            .success(function (response) {

                $scope.Cls_Invoice_Mandob.Omola_Money = response.Omola_Money;
                $modelDialogOmolaConfirm.close();
                // show sucess
                $scope.IsNewSarf = 1;
                $scope.IsSucess = null;
            });

    };
    $scope.BtnCancel = function () {

        $modelDialogOmola.close();
        $modelDialogOmolaConfirm.close();
    }
    $scope.OmolaView = function () {
        $http.get("/Ragee3_Mandob/Omola")
            .success(function (response) {
                html_Omola = response;
            });


    };
    $scope.OmolaView();
    var html_Omola_Confirm;
    $scope.Omola_ConfirmView = function () {
        $http.get("/Ragee3_Mandob/Omola_Confirm")
            .success(function (response) {
                html_Omola_Confirm = response;
            });
    };
    $scope.Omola_ConfirmView();

    $scope.PDF_SarfOmola = function () {
        $http({
            url: "/Ragee3_Mandob/PDF_SarfOmola/" + $scope.Cls_Invoice_Mandob.ID,
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

    $scope.PrintMasrofat = function () {
        $http({
            url: "/Ragee3_Mandob/Pdf_Masrofat/" + $scope.Cls_Invoice_Mandob.ID,
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
});



