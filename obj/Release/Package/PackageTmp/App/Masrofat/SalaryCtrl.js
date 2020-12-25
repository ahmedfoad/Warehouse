app.controller("SalaryCtrl", function ($scope, $http, $window, $timeout, $uibModal) {
    //----------------------------------------------------------------------------------------------------------------------------
    //----------------------------------------------------------------------------------------------------------------------------
    // switch between Inser && edit 
    //----------------------------------------------------------------------------------------------------------------------------
    //$scope.Cls_Salary = {

    //    ID: "", DateFrom: "", DateTO: "", Notes: "", ComputerName: "", ComputerUser: 0, InDate: "", Userid_In: 0,
    //    Cls_Employee_Salary: null
    //};

    //$scope.Cls_Salary = { ID: 0, Name: "", Job_Id: "", Job_Name: "", Salary: "", Mobile: "", ComputerName: "", ComputerUser: "", InDate: ""};
    
 
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
        if (urlParams.has('id') == true) {
            debugger;
            var id = urlParams.get('id');    // 1234
            $scope.SaveBtn = 1;
            $scope.dataEntire = 1;
            $scope.EditBtn = null;
            $scope.DeleteBtn = null;
            $http.get('/Employee/loadSalary?id=' + id)
               .success(function (response) {
                   if (response == null) {
                       $uibModal.open({
                           template: " <div class='portlet box red' style='min-height:150px !important;min-width: 541px !important;'> <div class='portlet-title'> <div class='caption'> <i class='fa-lg fa fa-warning'></i> تنبيه هام </div><div class='tools'> </div></div><div class='portlet-body form portlet-empty' style='min-height: 140px;'> <div class='col-md-12 page-404'> <div class='number'>" + response.ErrorNumber + "</div><div class='details'> <h3 style='font-family: sans-serif; text-align: center; font-weight: bold;'>" + response.ErrorFullNumber + "</h3> <p style='font-size: 12px;'> " + response.ErrorName + " <br/> <br/> <a class='btn btn-default btn-sm' href='/home'> الصفحة الرئيسية </a> <a class='btn btn-default btn-sm' href='" + response.Url + "' style='margin-right: 4px;'>المحاولة مرة آخري</a> </p></div></div></div></div>",
                           closeOnEscape: false,
                           closeOnOverlayClick: false
                       });
                   }
                   else {
                       debugger;
                       $scope.Cls_Salary = response;
                   }
               });
        }
        else {
            debugger;
            var id = urlParams.get('id');    // 1234
            $scope.SaveBtn = null;
            $scope.EditBtn = 1;
            $scope.DeleteBtn = 1;
            $scope.dataEntire = null;
            //document.getElementById('PrintTicket').href = "/Employee/PrintTicketReport/" + id;
            //document.getElementById('EmployeeRefer').href = "/Employee/Refer/" + id;
            $http.get('/Employee/loadEmployee?id=' + id)
               .success(function (response) {
                   if (response == null) {
                       $uibModal.open({
                           template: " <div class='portlet box red' style='min-height:150px !important;min-width: 541px !important;'> <div class='portlet-title'> <div class='caption'> <i class='fa-lg fa fa-warning'></i> تنبيه هام </div><div class='tools'> </div></div><div class='portlet-body form portlet-empty' style='min-height: 140px;'> <div class='col-md-12 page-404'> <div class='number'>" + response.ErrorNumber + "</div><div class='details'> <h3 style='font-family: sans-serif; text-align: center; font-weight: bold;'>" + response.ErrorFullNumber + "</h3> <p style='font-size: 12px;'> " + response.ErrorName + " <br/> <br/> <a class='btn btn-default btn-sm' href='/home'> الصفحة الرئيسية </a> <a class='btn btn-default btn-sm' href='" + response.Url + "' style='margin-right: 4px;'>المحاولة مرة آخري</a> </p></div></div></div></div>",
                           closeOnEscape: false,
                           closeOnOverlayClick: false
                       });
                   }
                   else {
                       debugger;
                       $scope.Cls_Salary = response;
                   }
               });

        }
    }
    $scope.switchCase();
   
   
    var View_Report;
    $scope.BtnEmployeeBarcode = function () {

        if ($scope.Cls_Salary != null && $scope.Cls_Salary.ID != "" && $scope.Cls_Salary.ID != 0) {

            $http.get("/Employee/PrintBarCodeReport/" + $scope.Cls_Salary.ID)
             .success(function (response) {
                 View_Report = response;

                 $uibModal.open({
                     scope: $scope,
                     template: View_Report,
                     resolve: {
                         Cls_Salary: function () {
                             return $scope.Cls_Salary;
                         }
                     }
                 });


             });
        }
    };

    


    //----------------------------------------------------------------------------------------------------------------------------
    //----------------------------------------------------------------------------------------------------------------------------
    // load all Employee
    //----------------------------------------------------------------------------------------------------------------------------
   
    //************************************************
    $scope.getAllEmployees = function () {
        $http.get("/Employee/getAll")
       .success(function (response) {
           if (response != null) {
                
               $scope.Cls_Salary = response;
           }
       });
    }
    $scope.getAllEmployees();
    
    
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

        if ($scope.Cls_Salary.DateFrom == null || $scope.Cls_Salary.DateFrom == "") {
            $scope.ErrorName = "برجاء ادخال تاريخ بداية الصرف";
            var ListID = ["DateFrom"];
            $scope.changeBorder(ListID, "red");
            return false;
        } else {
            var ListID = ["DateFrom"];
            $scope.changeBorder(ListID, "#c2cad8");
        }

        if ($scope.Cls_Salary.DateTO == null || $scope.Cls_Salary.DateTO == "") {
            $scope.ErrorName = "برجاء ادخال تاريخ بداية الصرف";
            var ListID = ["DateTO"];
            $scope.changeBorder(ListID, "red");
            return false;
        } else {
            var ListID = ["DateTO"];
            $scope.changeBorder(ListID, "#c2cad8");
        }

        if ($scope.Cls_Salary.Notes == null || $scope.Cls_Salary.Notes == "") {
            $scope.ErrorName = "برجاء ادخال البيان";
            var ListID = ["Notes"];
            $scope.changeBorder(ListID, "red");
            return false;
        } else {
            var ListID = ["Notes"];
            $scope.changeBorder(ListID, "#c2cad8");
        }
      
        $scope.AlertParameter = null;
        return true;
    }
    $scope.Save = function () {
        $scope.Cls_Salary.DateFrom = $('#DateFrom').val();
        $scope.Cls_Salary.DateTO = $('#DateTO').val();
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
        var parameter = JSON.stringify($scope.Cls_Salary);
        if (urlParams.has('id') == false) {
            var EmployeeID = urlParams.get('id');    // 1234
           
            $http.post('/Employee/Save_Salary', parameter)
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
                                 $window.location.href = '/Employee/Salary?id=' + response.ID;
                             }, 500);
                         }
                     }
                 });
        }
        else {
            //Employee.BarCodeArr = null;
            $http.post('/Employee/Edit_Salary', parameter)
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
                               $window.location.href = '/Employee/Salary?id=' + response.ID;
                           }, 500);
                       }
                   }
               });
        }
    }
    $scope.DeleteEmployee = function () {
        var QueryString = location.search;
        var urlParams = new URLSearchParams(QueryString);
        if (urlParams.has('id') == true) {
            var ID = urlParams.get('id');    // 1234
            $uibModal.open({
                template: " <div class='portlet light' style='max-width: 333px;'> <div class='portlet-title'><div class='caption font-red-sunglo'> <i class='fa fa-link font-red-sunglo'></i> <span class='caption-subject bold'> هل أنت متأكد من حذف المعاملة الصادرة</span> </div></div><div class='portlet-body'> <div class='row' style='text-align:center;margin-top:5px;'> <div class='form-actions'> <a class='btn btn-default red btn-sm' href='/Employee/Delete?id=" + ID + "'>حذف</a><a class='btn btn-default btn-sm fancymodal-close' style='margin-right: 4px;' href='#'> إلغاء </a> </div></div></div></div>",
                closeOnEscape: false,
                closeOnOverlayClick: false
            });
        }
    }
    //----------------------------------------------------------------------------------------------------------------------------
    document.onkeydown = function () {

        var f2 = 113;
        var f4 = 115;
        var f8 = 119;
        if (window.event && window.event.keyCode == f2) { //طباعة باركود الموظف
            $scope.BtnEmployeeBarcode();
        }
        
    }

   
});

