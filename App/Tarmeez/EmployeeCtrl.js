app.controller("EmployeeCtrl", function ($scope, $http, $window, $timeout, $uibModal) {
    //----------------------------------------------------------------------------------------------------------------------------
    //----------------------------------------------------------------------------------------------------------------------------
    // switch between Inser && edit 
    //----------------------------------------------------------------------------------------------------------------------------
   
    $scope.Employee = { ID: 0, Name: "", Job_Id: "", Job_Name: "", Salary: "", Mobile: "", ComputerName: "", ComputerUser: "", InDate: ""};
    
 
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
            //$http.get("/Employee/InitialOperationForm")
            //.success(function (response) {
            //    $scope.Employee = response;
            //    $scope.EmployeeType = "Public";
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

                       $scope.Employee = {
                           ID: response.ID,
                           Name: response.Name,
                           Job_Id: response.Job_Id,
                           Job_Name: response.Job_Name,
                           Salary: response.Salary,
                           Mobile: response.Mobile,
                           ComputerName: response.ComputerName,
                           ComputerUser: response.ComputerUser,
                           InDate: response.InDate,
                           User_Name: response.User_Name
                       };

                      

                   }
               });

        }
    }
    $scope.switchCase();
  
   
    var View_Report;
    $scope.BtnEmployeeBarcode = function () {

        if ($scope.Employee != null && $scope.Employee.ID != "" && $scope.Employee.ID != 0) {

            $http.get("/Employee/PrintBarCodeReport/" + $scope.Employee.ID)
             .success(function (response) {
                 View_Report = response;

                 $uibModal.open({
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
    var View_jobs;
    $scope.getView = function () {
        $http.get("/Employee/Getjobs")
         .success(function (response) {
             View_jobs = response;
         });
    };
    $scope.getView();
    $scope.loadjobs = function () {

        //-----
        $scope.data = null;
        $scope.getAllRecords();
        //------------
        $uibModal.open({
            scope: $scope,
            template: View_jobs,
            resolve: {
                Employee: function () {
                    return $scope.Employee;
                }
            }
        });
    };

    //************************************************
    var View_Employees;
    $scope.getEmployeesView = function () {
        $http.get("/Employee/GetEmployees")
         .success(function (response) {
             View_Employees = response;
         });
    };
    $scope.getEmployeesView();
    $scope.popup_Employees = function () {

        //-----
        $scope.EmployeeList = null;
        $scope.getAllEmployees();
        //------------
        $uibModal.open({
            scope: $scope,
            template: View_Employees,
            resolve: {
                EmployeeList: function () {
                    return $scope.EmployeeList;
                }
            }
        });
    };
    $scope.getAllEmployees = function (Page, Search) {

        var parameter = { 'Search': Search }
        var sentData = "page=" + Page;
        $http.get("/Employee/getAll?" + sentData, { params: parameter })
       .success(function (response) {
           if (response != null) {

               if ($scope.EmployeeList == null) {
                   $scope.EmployeeList = response;
               }
               else {
                   $scope.EmployeeList = $scope.EmployeeList.concat(response);
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
        $http.get("/Job/getAll?" + sentData, { params: parameter })
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

        $scope.Employee.Job_Id = row.ID;
        $scope.Employee.Job_Name = row.Name;
        $timeout(function () { angular.element('#closebox').trigger('click'); }, 0);
        this.$close();
    }

    $scope.clicktr_Employees = function (row) {

        $scope.Employee.Offer_Employee_id = row.ID;
        $scope.Employee.Offer_Employee_Name = row.Name;
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

        if ($scope.Employee.Name == null || $scope.Employee.Name == "") {
            $scope.ErrorName = "برجاء ادخال اسم الموظف";
            var ListID = ["Name"];
            $scope.changeBorder(ListID, "red");
            return false;
        } else {
            var ListID = ["Name"];
            $scope.changeBorder(ListID, "#c2cad8");
        }

        if ($scope.Employee.Job_Id == null || $scope.Employee.Job_Id == "") {
            $scope.ErrorName = "برجاء ادخال اسم المهنة";
            var ListID = ["Job_Name"];
            $scope.changeBorder(ListID, "red");
            return false;
        } else {
            var ListID = ["Job_Name"];
            $scope.changeBorder(ListID, "#c2cad8");
        }
       ;
        

        //if ($scope.Employee.Barcode == null || $scope.Employee.Barcode == "" || $scope.Employee.Barcode == 0) {
        //    $scope.ErrorName = "برجاء ادخال الباركود"
        //    var ListID = ["Barcode"];
        //    $scope.changeBorder(ListID, "red");
        //    return false;
        //} else {
        //    var ListID = ["Barcode"];
        //    $scope.changeBorder(ListID, "#c2cad8");
        //}



     

        if ($scope.Employee.Salary == null || $scope.Employee.Salary == "") {
            $scope.ErrorName = "برجاء ادخال الراتب"
            var ListID = ["Salary"];
            $scope.changeBorder(ListID, "red");
            return false;
        } else {
            var ListID = ["Salary"];
            $scope.changeBorder(ListID, "#c2cad8");
        }

        $scope.AlertParameter = null;
        return true;
    }
    $scope.SaveEmployee = function () {
         
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
        var parameter = JSON.stringify($scope.Employee);
        if (urlParams.has('id') == false) {
            var EmployeeID = urlParams.get('id');    // 1234
           
            $http.post('/Employee/Insert', parameter)
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
                                 $window.location.href = '/Employee/Operation?id=' + response.ID;
                             }, 500);
                         }
                     }
                 });
        }
        else {
            //Employee.BarCodeArr = null;
            $http.post('/Employee/Edit', parameter)
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
                               $window.location.href = '/Employee/Operation?id=' + response.ID;
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

