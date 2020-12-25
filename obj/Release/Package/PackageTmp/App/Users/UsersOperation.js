app.controller('UserOperationCtrl', function ($scope, $http, $window, $fancyModal, $timeout) {
   
    $scope.Cls_User = {
        User: { ID: "", NAME: "", Username: "", Password: "", Account_Type:0, EMAIL: "", ROLE: "", COMPUTERNAME: "", COMPUTERUSER: "", User_Name: "", INDATE: "", STOPEMP: "", PROGRAMUSERID: "" }
        , View_Roles: null
    };
    var Search;
    var page = 0;
    //----------------------------------------------------------------------------------------------------------------------------
    //----------------------------------------------------------------------------------------------------------------------------
    // switch between Inser && edit 
    //----------------------------------------------------------------------------------------------------------------------------
    $scope.switchCase = function () {
        var QueryString = location.search;
        var urlParams = new URLSearchParams(QueryString);
        if (urlParams.has('id') == false) {
            
            $scope.SaveBtn = 1;
            $scope.EditBtn = null;
            $scope.DeleteBtn = null;
            
        }
        else {
            var UserID = urlParams.get('id');    // 1234
            $scope.SaveBtn = null;
            $scope.EditBtn = 1;
            $scope.DeleteBtn = 1;
                       
            $http.get("/Users/loadUser/" + UserID)
                .success(function (response) {
                    if (response != null) {
                        $scope.dataAuthentication = new Array();
                        for (var i = 0; i < response.View_Roles.length; i++) {
                            $scope.dataAuthentication.push(response.View_Roles[i]);
                        }
                         
                        $scope.Cls_User.User = response.User;// {User: , View_Roles: response.View_Roles};
                        $scope.Cls_User.View_Roles = $scope.dataAuthentication;
                       
                        //$http.get("/Users/loadAuthentication/" + UserID)
                        //    .success(function (AuthenticationData) {
                        //        AuthenticationList = AuthenticationData;
                        //        $scope.dataAuthentication = AuthenticationList;
                        //    });
                    }
                    if (response.ErrorName != null) {
                        $fancyModal.open({
                            template: " <div class='portlet box red' style='min-height:150px !important'> <div class='portlet-title'> <div class='caption'> <i class='fa-lg fa fa-warning'></i> تنبيه هام </div><div class='tools'> </div></div><div class='portlet-body form portlet-empty' style='min-height: 140px;'> <div class='col-md-12 page-404'> <div class='number'>" + response.ErrorNumber + "</div><div class='details'> <h3 style='font-family: sans-serif; text-align: center; font-weight: bold;'>" + response.ErrorFullNumber + "</h3> <p> " + response.ErrorName + " <br/> <br/> <a class='btn btn-default btn-sm' href='/home'> الصفحة الرئيسية </a><a class='btn btn-default btn-sm' href='" + response.Url + "' style='margin-right: 4px;'>المحاولة مرة آخري</a> </p></div></div></div></div>",
                            closeOnEscape: false,
                            closeOnOverlayClick: false
                        });
                    }
                });
        }
    }
    $scope.switchCase();
    //----------------------------------------------------------------------------------------------------------------------------
    //----------------------------------------------------------------------------------------------------------------------------
    //----------------------------------------------------------------------------------------------------------------------------
    // Update Table View
    //----------------------------------------------------------------------------------------------------------------------------
    $scope.updateTableView = function () {
        var nums = document.getElementById("MainMenu");
        var listItem = nums.getElementsByTagName("a");
        var parameter = "";
        for (var i = 0; i < listItem.length; i++) {
            var ViewName = listItem[i].innerHTML;
            var ViewUrl = "/" + listItem[i].href.split('/')[3] + "/" + listItem[i].href.split('/')[4];
            if (ViewName.search('class') == -1) {
                parameter += "ViewName:" + ViewName + ",ViewUrl:" + ViewUrl + "*";
            }
        }
        //var Views = JSON.stringify({ 'Views': parameter });
        //$http.post('/Users/Views', Views)
        //    .success(function (response) {
        //    });
    };
    $scope.updateTableView();
    //----------------------------------------------------------------------------------------------------------------------------
    //----------------------------------------------------------------------------------------------------------------------------
    // function get all Views 
    //----------------------------------------------------------------------------------------------------------------------------
    $scope.getAllViews = function (Page, Search) {
       
        var parameter = { 'Search': Search }
        var sentData = "page=" + Page;
        $http.get("/Users/GetAllViews?" + sentData, { params: parameter })
        .success(function (response) {
           
            if (response != null) {
                if ($scope.data == null) {
                    $scope.data = response;
                }
                else {
                    $scope.data = $scope.data.concat(response);
                }
            }
        });
    };
    $scope.getAllViews(null, null);
    //----------------------------------------------------------------------------------------------------------------------------
    //----------------------------------------------------------------------------------------------------------------------------
    // instanse Search for Views List
    //----------------------------------------------------------------------------------------------------------------------------
    var lastentry = "";
    $scope.keyupevt = function () {
        if ($scope.Search != lastentry) {
            $scope.data = null;
            page = 0;
            $scope.getAllViews(null, $scope.Search);
        }
        lastentry = $scope.Search;
    };
    //----------------------------------------------------------------------------------------------------------------------------
    //----------------------------------------------------------------------------------------------------------------------------
    // get data of Views List with scrolling 
    //----------------------------------------------------------------------------------------------------------------------------
    angular.element("#ViewList").bind("scroll", function () {
        var windowHeight = "innerHeight" in window ? window.innerHeight : document.documentElement.offsetHeight;
        var body = document.body, html = document.documentElement;
        var docHeight = Math.max(body.scrollHeight, body.offsetHeight, html.clientHeight, html.scrollHeight, html.offsetHeight);
        windowBottom = windowHeight + window.pageYOffset;
        if (windowBottom >= docHeight) {
            page++;
            $scope.getAllViews(page, $scope.Search);
        }
    });
    //----------------------------------------------------------------------------------------------------------------------------
    //----------------------------------------------------------------------------------------------------------------------------
    // add authentication after click on element in view list
    //----------------------------------------------------------------------------------------------------------------------------   
    var checkExist = false;
    //var AuthenticationList = new Array();
    $scope.AddAuthentication = function (data, index) {
       
        var AuthenticationList = $scope.dataAuthentication;
        if ($scope.dataAuthentication == null) {
            $scope.dataAuthentication = new Array();
            $scope.dataAuthentication.push(data);
            //$scope.View_Roles = new Array();
        }
        else {
           
            for (var i = 0; i < $scope.dataAuthentication.length; i++) {
                if ($scope.dataAuthentication[i]._View.ID == data._View.ID) {
                    checkExist = true;
                }
            }
            if (checkExist == false) {
                $scope.dataAuthentication.push(data);
            }
        }
        $scope.data.splice(index, 1);
       
        //$scope.dataAuthentication = AuthenticationList;
    };
    //-----------------------------------------------------------------------------------------------------------------------------
    //-----------------------------------------------------------------------------------------------------------------------------
    // function show/hide password 
    //-----------------------------------------------------------------------------------------------------------------------------
    var pwShown = 0;
    $scope.ShowHidePass = function () {
        if (pwShown == 0) {
            var p = document.getElementById('Password');
            p.setAttribute('type', 'text');
            var x = document.getElementById('ShowPass');
            x.innerHTML = "إخفاء كلمة المرور";
            pwShown = 1;
        }
        else {
            var p = document.getElementById('Password');
            p.setAttribute('type', 'password');
            var x = document.getElementById('ShowPass');
            x.innerHTML = "إظهار كلمة المرور";
            pwShown = 0;
        }
    };
    //-----------------------------------------------------------------------------------------------------------------------------
    //-----------------------------------------------------------------------------------------------------------------------------
    // Check All Enter
    //-----------------------------------------------------------------------------------------------------------------------------
    $scope.EnterMarkAll = function () {
        
        var enter = document.getElementsByName("Role_Enter");
        var enterall = document.getElementById("EnterMarkAll");
        if (enterall.checked == true) {
            for (var i = 0; i < enter.length; i++) {
                enter[i].checked = true;
                $scope.dataAuthentication[i].Enter = 1;
                $scope.dataAuthentication[i].Role_Enter = true;
            }
        }
        if (enterall.checked == false) {
            for (var i = 0; i < enter.length; i++) {
                enter[i].checked = false;
                $scope.dataAuthentication[i].Enter = 0;
                $scope.dataAuthentication[i].Role_Enter = false;
            }
        }
    };
    //-----------------------------------------------------------------------------------------------------------------------------
    //-----------------------------------------------------------------------------------------------------------------------------
    // Check All Save
    //-----------------------------------------------------------------------------------------------------------------------------
    $scope.SaveMarkAll = function () {
        var Save = document.getElementsByName("Role_Save");
        var SaveAll = document.getElementById("SaveMarkAll");
        if (SaveAll.checked == true) {
            for (var i = 0; i < Save.length; i++) {
                Save[i].checked = true;
                $scope.dataAuthentication[i].Save = 1;
                $scope.dataAuthentication[i].Role_Save = true;
            }
        }
        if (SaveAll.checked == false) {
            for (var i = 0; i < Save.length; i++) {
                Save[i].checked = false;
                $scope.dataAuthentication[i].Save = 0;
                $scope.dataAuthentication[i].Role_Save = false;
            }
        }
    };
    //-----------------------------------------------------------------------------------------------------------------------------
    //-----------------------------------------------------------------------------------------------------------------------------
    // Check All Edit
    //-----------------------------------------------------------------------------------------------------------------------------
    $scope.EditMarkAll = function () {
        var Edit = document.getElementsByName("Role_Edit");
        var EditAll = document.getElementById("EditMarkAll");
        if (EditAll.checked == true) {
            for (var i = 0; i < Edit.length; i++) {
                Edit[i].checked = true;
                $scope.dataAuthentication[i].Edit = 1;
                $scope.dataAuthentication[i].Role_Edit = true;
            }
        }
        if (EditAll.checked == false) {
            for (var i = 0; i < Edit.length; i++) {
                Edit[i].checked = false;
                $scope.dataAuthentication[i].Edit = 0;
                $scope.dataAuthentication[i].Role_Edit = false;
            }
        }
    };
    //-----------------------------------------------------------------------------------------------------------------------------
    //-----------------------------------------------------------------------------------------------------------------------------
    // Check All Delete
    //-----------------------------------------------------------------------------------------------------------------------------
    $scope.DeleteMarkAll = function () {
        var Delete = document.getElementsByName("Role_Delete");
        var DeleteAll = document.getElementById("DeleteMarkAll");
        if (DeleteAll.checked == true) {
            for (var i = 0; i < Delete.length; i++) {
                Delete[i].checked = true;
                $scope.dataAuthentication[i].Remove = 1;
                $scope.dataAuthentication[i].Role_Delete = true;
            }
        }
        if (DeleteAll.checked == false) {
            for (var i = 0; i < Delete.length; i++) {
                Delete[i].checked = false;
                $scope.dataAuthentication[i].Remove = 0;
                $scope.dataAuthentication[i].Role_Delete = false;
            }
        }
    };
    //-----------------------------------------------------------------------------------------------------------------------------
    //-----------------------------------------------------------------------------------------------------------------------------
    // delete Authentication after add
    //-----------------------------------------------------------------------------------------------------------------------------
    $scope.DeleteAuthentication = function (data) {
        $scope.DeleteAuthenticationDiv = 1;
        $scope.WindowID = data.ID;
        $scope.WindowName = data.ViewName;
        $window.scrollTo(0, 0);
    };
    $scope.RemoveAuthenticationDiv = function () {
        $scope.DeleteAuthenticationDiv = null;
        $scope.WindowName = null;
    };
    $scope.DeleteAuthenticationConfirm = function () {
        var x;
        for (var i = 0; i < AuthenticationList.length; i++) {
            if (AuthenticationList[i].ID == $scope.WindowID) {
                x = AuthenticationList.indexOf(AuthenticationList[i]);
            }
        }
        AuthenticationList.splice(x, 1);
        $scope.RemoveAuthenticationDiv();
    };
    //-----------------------------------------------------------------------------------------------------------------------------
    //-----------------------------------------------------------------------------------------------------------------------------
    // Check Element 
    //-----------------------------------------------------------------------------------------------------------------------------
    $scope.EnterChecked = function (data) {
        if (data.Enter == 0) { data.Enter = 1; data.Role_Enter = true; }
        else if (data.Enter == 1) { data.Enter = 0; data.Role_Enter = false; }
    }
    $scope.SaveChecked = function (data) {
        if (data.Save == 0) { data.Save = 1; data.Role_Save = true; }
        else if (data.Save == 1) { data.Save = 0; data.Role_Save = false; }
    }
    $scope.EditChecked = function (data) {
        if (data.Edit == 0) { data.Edit = 1; data.Role_Edit = true; }
        else if (data.Edit == 1) { data.Edit = 0; data.Role_Edit = false; }
    }
    $scope.RemoveChecked = function (data) {
        if (data.Remove == 0) { data.Remove = 1; data.Role_Delete = true; }
        else if (data.Remove == 1) { data.Remove = 0; data.Role_Delete = false; }
    }
    //-----------------------------------------------------------------------------------------------------------------------------
    //-----------------------------------------------------------------------------------------------------------------------------
    // Save/Edit/Delete User With Authentications
    //-----------------------------------------------------------------------------------------------------------------------------
    $scope.SaveUser = function () {
        $scope.Cls_User.User.Account_Type = $("#Account_Type").val();
        $scope.Cls_User.View_Roles = new Array();
        $scope.Cls_User.View_Roles= $scope.dataAuthentication;
        var CheckForm = $scope.Form_Validation($scope.Cls_User);
        if (CheckForm == true) {
            $scope.DBsave($scope.Cls_User);
        }
        else {
            $scope.AlertParameter = 1;
            document.body.scrollTop = 0;
        }

    };
    //----------------------------------------------------------------------------------------------------------------------------
    // Form Validation
    //----------------------------------------------------------------------------------------------
    $scope.Form_Validation = function (Cls_User) {

        if (Cls_User.User.NAME == null || Cls_User.User.NAME == "") {
            $scope.ErrorName = "برجاء ادخال اسم الموظف"
            var ListID = ["NAME"];
            $scope.changeBorder(ListID, "red");
            return false;
        } else {
            var ListID = ["NAME"];
            $scope.changeBorder(ListID, "#c2cad8");
        }

        if (Cls_User.User.Username == null || Cls_User.User.Username == "") {
            $scope.ErrorName = "برجاء ادخال اسم المستخدم"
            var ListID = ["Username"];
            $scope.changeBorder(ListID, "red");
            return false;
        } else {
            var ListID = ["Username"];
            $scope.changeBorder(ListID, "#c2cad8");
        }

        if (Cls_User.User.Password == null || Cls_User.User.Password == "") {
            $scope.ErrorName = "برجاء ادخال اسم المستخدم"
            var ListID = ["Password"];
            $scope.changeBorder(ListID, "red");
            return false;
        } else {
            var ListID = ["Password"];
            $scope.changeBorder(ListID, "#c2cad8");
        }


        if (Cls_User.View_Roles == null || Cls_User.View_Roles.length == 0) {
            $scope.ErrorName = "برجاء اختيار نوافذ المستخدم"
            return false;
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
    //----------------------------------------------------
    $scope.DBsave = function (Cls_User) {

        var parameter = JSON.stringify(Cls_User);
        if (Cls_User.User.ID == "") {
            $http.post('/Users/Insert', parameter)
                 .success(function (response) {
                    
                     if (response.ErrorName.indexOf("صلاحية") > -1 || response.ErrorName.indexOf("المسموح") > -1) {
                         $fancyModal.open({
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
                                 $window.location.href = '/users/operation?id=' + response.ID;
                             }, 500);
                         }
                     }
                 });
        }
        else {

            $http.post('/Users/Edit', parameter)
               .success(function (response) {
                   if (response.ErrorName.indexOf("صلاحية") > -1 || response.ErrorName.indexOf("المسموح") > -1) {
                       $fancyModal.open({
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
                               $window.location.href = '/users/operation?id=' + response.ID;
                           }, 500);
                       }
                   }
               });
        }
    }
    //-----------------------------------------------------
    $scope.DeleteListExport = function () {
        var QueryString = location.search;
        var urlParams = new URLSearchParams(QueryString);     
        if (urlParams.has('id') == true) {
            var UserID = urlParams.get('id');    // 1234
            $fancyModal.open({
                template: " <div class='portlet light' style='max-width: 291px;'> <div class='portlet-title'><div class='caption font-red-sunglo'> <i class='fa fa-link font-red-sunglo'></i> <span class='caption-subject bold'> هل أنت متأكد من حذف المستخدم</span> </div></div><div class='portlet-body'> <div class='row' style='text-align:center;margin-top:5px;'> <div class='form-actions'> <a class='btn btn-default red btn-sm' href='/users/Delete/" + UserID + "'>حذف</a><a class='btn btn-default btn-sm fancymodal-close' style='margin-right: 4px;' href='javascript:;'> إلغاء </a> </div></div></div></div>",
                closeOnEscape: false,
                closeOnOverlayClick: false
            });
        }
    }
    $scope.ReomveAlert = function () {
        $scope.AlertParameter = null;
    }
    //-----------------------------------------------------------------------------------------------------------------------------
    //-----------------------------------------------------------------------------------------------------------------------------
    // Statistics Report
    //-----------------------------------------------------------------------------------------------------------------------------
    $('#StatisticsReport').bind('click', function (e) {
        $fancyModal.open({
            template: "<div class='portlet light' style='max-width: 450px;'> <script>(function ($) { $(function () { var calendar = $.calendars.instance('ummalqura'); $('#DatBegin,#DatEnd').calendarsPicker({ calendar: calendar }); }); })(jQuery);$('#PrintStatisticsReport').bind('click', function (e){var datBegin=document.getElementById('DatBegin').value; var datEnd=document.getElementById('DatEnd').value; var UserID=document.URL.split('/')[5]; document.getElementById('PrintStatisticsReport').href='/Users/PrintStatisticsReport?DatBegin=' + datBegin + '&DatEnd=' + datEnd + '&UserID=' + UserID; + '&old=false'}); </script> <div class='portlet-title'> <div class='caption font-red-sunglo'> <i class='fa fa-link font-red-sunglo'></i> <span class='caption-subject bold'>طباعة التقرير الإحصائي لموظف</span> </div></div><div class='portlet-body'> <div class='row'> <div class='col-md-12 form-body'> <div class='col-md-6 form-group'> <label>في الفترة من</label> <div class='input-icon'> <i class='fa fa-calendar'></i> <input id='DatBegin' type='text' class='form-control' placeholder='في الفترة من'> </div></div><div class='col-md-6 form-group'> <label>إلي</label> <div class='input-icon'> <i class='fa fa-calendar'></i> <input id='DatEnd' type='text' class='form-control' placeholder='إلي'> </div></div></div><div class='form-actions' style='text-align:center;margin-top:5px;'> <a class='btn btn-default purple btn-sm' href='javascript:;' id='PrintStatisticsReport'>طباعة</a> <a class='btn btn-default btn-sm fancymodal-close' style='margin-right: 4px;' href='javascript:;'> إلغاء </a> </div></div></div></div>",
            closeOnEscape: false,
            closeOnOverlayClick: false
        });
    });
});
