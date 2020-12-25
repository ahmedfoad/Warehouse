var page = 0;
app.controller("Sadad_MandobsListCtrl", function ($scope, $http, $window, $timeout, $uibModal) {
    $scope.Sadad_ID = null;
    $scope.Srch_Sadad_MandobsList = { Sadad_Status: 1, Mandob_id: "", Mandob_Name: "" };
    //----------------------------------------------------------------------------------------------------------------
    $scope.emptyInputs = function () {
        $scope.Srch_Sadad_MandobsList.Sadad_Status = 1;
        $scope.Srch_Sadad_MandobsList.Mandob_id = "";
        $scope.Srch_Sadad_MandobsList.Mandob_Name = "";
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

        
        var parameter = JSON.stringify({ 'RModel': $scope.Srch_Sadad_MandobsList, 'page': page });
        $http.post("/Sadad_Mandob/Sadad", parameter)
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
            $scope.data = null;
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
        $scope.getAllRecords(page);
    };
    //----------------------------------------------------------------------------------------------------------------------------
    // load all popuop data
    //----------------------------------------------------------------------------------------------------------------------------
    var View_Eradat_Type;
    $scope.getView = function () {
        $http.get("/Eradat/GetEradat_Type")
         .success(function (response) {
             View_Eradat_Type = response;
         });
    };
    $scope.getView();
  
    //************************************************
    $scope.getView = function () {
        $http.get("/Invoice_Mandob/GetMandob")
         .success(function (response) {
             View_Mandobs = response;
         });
    };
    $scope.getView();
    $scope.loadMandobs = function () {
        //-----
        $scope.MandobList = null;
        $scope.getAllMandobs();
        //------------
        $uibModal.open({
            scope: $scope,
            template: View_Mandobs,
            resolve: {
                Srch_Sadad_MandobsList: function () {
                    return $scope.Srch_Sadad_MandobsList;
                }
            }
        });
    };
    $scope.clicktrMandob = function (row) {
        $scope.Srch_Sadad_MandobsList.Mandob_id = row.ID;
        $scope.Srch_Sadad_MandobsList.Mandob_Name = row.Name;
        $timeout(function () { angular.element('#closebox').trigger('click'); }, 0);
        this.$close();
    }
    $scope.closeUiModal_Mandob = function () {
        this.$close();
    }
    $scope.getAllMandobs = function (Page, Search) {
        debugger;
        var parameter = { 'Search': Search }
        var sentData = "page=" + Page;
        $http.get("/Mandob/getAll?" + sentData, { params: parameter })
       .success(function (response) {
           if (response != null) {

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
    var lastsearch = "";
    var lastSrc_Barcode = "";
    $scope.SearchMandob = function () {
        var search = $("#Search").val();
        if (search != lastsearch) {
            $scope.MandobList = null;
            page = 0;
            $scope.getAllMandobs(null, search, "");
        }
        lastsearch = search;
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
            $scope.getAllEradat_Type(null, search);
        }
        lastentry = search;
    };
    //----------------------------------------------------------------------------------------------------------------------------
    //=======عند الضفط على صف
    $scope.clicktr = function (row) {

        $scope.Srch_Sadad_MandobsList.Mandob_id = row.ID;
        $scope.Srch_Sadad_MandobsList.Mandob_Name = row.Name;
        $timeout(function () { angular.element('#closebox').trigger('click'); }, 0);
        this.$close();
    }


    $scope.closeUiModal = function () {
        this.$close();
    }

    var html_popup_SadadMandob;
    $scope.New_Sadad = { Mandob_id: "", Mandob_Name: "", Price: "", Total_Sadad: "", Money: 0 };
    $scope.Current_Remain = 0;
    $scope.SadadMandob = function (item) {
        $scope.IsNewSarf = null;
        $scope.IsSucess = 1;
        debugger;
        $scope.New_Sadad.Mandob_id = item.Mandob_id;
        $scope.New_Sadad.Mandob_Name = item.Mandob_Name;
        $scope.New_Sadad.Price = item.Price;
        $scope.New_Sadad.Money = 0;
        $scope.New_Sadad.Remain = item.Remain;
        $scope.Current_Remain = item.Remain;
      //  $scope.New_Sadad.Total_Sadad = item.Total_Sadad;
       
       
       
        $modelDialogSadad = $uibModal.open({
            scope: $scope,
            template: html_popup_SadadMandob,
            resolve: {
                New_Sadad: function () {
                    return $scope.New_Sadad;
                }
                , Current_Remain: function () {
                    return $scope.Current_Remain;
                }
            }
        });

    };
    $scope.MoneyCalculate = function () {
        var remain = $scope.Current_Remain;
        $scope.New_Sadad.Remain = remain
        $scope.New_Sadad.Remain = parseFloat(parseFloat($scope.New_Sadad.Remain - $scope.New_Sadad.Money).toFixed(2));
        if ($scope.New_Sadad.Remain < 0) {
            $scope.New_Sadad.Remain = remain
            $scope.New_Sadad.Money = 0;
        }
    };
    $scope.popup_SadadMandob = function () {
        $http.get("/Sadad_Mandob/popup_SadadMandob")
         .success(function (response) {
             html_popup_SadadMandob = response;
         });


    };
    $scope.popup_SadadMandob();
    //----------------------------------------------------------------------------------------------------------------------------
   

    $scope.BtnConfirm = function () {
        debugger;
        $scope.New_Sadad.Sadad_Type_Id = document.getElementById("Sadad_Type_Id").value;
        var CheckForm = $scope.FormValidation($scope.New_Sadad);
        if (CheckForm == true) {
           $modelDialogSadadConfirm = $uibModal.open({
            scope: $scope,
            template: html_Sadad_Confirm,
            resolve: {
                New_Sadad: function () {
                    return $scope.New_Sadad;
                }
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
    $scope.ReomveAlert = function () {
        $scope.AlertParameter = null;
    }
    $scope.changeBorder = function (arr, color) {
        for (var i = 0; i < arr.length; i++) {
            document.getElementById(arr[i]).style.borderColor = color;
        }
    }
    $scope.BtnSaveSadad = function () {
        var parameter = JSON.stringify($scope.New_Sadad);
        $http.post('/Sadad_Mandob/SaveSadadList', parameter)
             .success(function (response) {
                 debugger;
                 
                 $scope.Sadad_ID = response.Sadad_ID;
                 $modelDialogSadadConfirm.close();
                 // show sucess
                 $scope.IsNewSarf = 1;
                 $scope.IsSucess = null;
                 $scope.SearchButton();
             });

    };
    $scope.BtnCancel = function () {

        $modelDialogSadad.close();
        $modelDialogSadadConfirm.close();
    }
    
    var html_Sadad_Confirm;
    $scope.Sadad_ConfirmView = function () {
        $http.get("/Sadad_Mandob/Sadad_Confirm")
         .success(function (response) {
             html_Sadad_Confirm = response;
         });
    };
    $scope.Sadad_ConfirmView();

    $scope.PrintEradat = function () {
        $http({
            url: "/Sadad_Mandob/Pdf_Eradat/" + $scope.Sadad_ID,
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