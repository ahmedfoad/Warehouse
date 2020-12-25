app.controller('FireChangePassCtrl', function ($scope, $http, $window, $fancyModal, $timeout) {
    $scope.FireChangePass = function () {
        $http.get('/Users/LoadDataChangePass')
                 .success(function (response) {
                     if (response.ErrorName.indexOf("المسموح") > -1) {
                         $fancyModal.open({
                             template: " <div class='portlet box red' style='min-height:150px !important;min-width: 541px !important;'> <div class='portlet-title'> <div class='caption'> <i class='fa-lg fa fa-warning'></i> تنبيه هام </div><div class='tools'> </div></div><div class='portlet-body form portlet-empty' style='min-height: 140px;'> <div class='col-md-12 page-404'> <div class='number'>" + response.ErrorNumber + "</div><div class='details'> <h3 style='font-family: sans-serif; text-align: center; font-weight: bold;'>" + response.ErrorFullNumber + "</h3> <p style='font-size: 12px;'> " + response.ErrorName + " <br/> <br/> <a class='btn btn-default btn-sm' href='/home'> الصفحة الرئيسية </a> <a class='btn btn-default btn-sm' href='" + response.Url + "' style='margin-right: 4px;'>المحاولة مرة آخري</a> </p></div></div></div></div>",
                             closeOnEscape: false,
                             closeOnOverlayClick: false
                         });
                     }
                     else {
                         $fancyModal.open({
                             template: "<div class='portlet light' ng-controller='ChangePassCtrl'> <div class='portlet-title'> <div class='caption font-red-sunglo'> <i class='fa fa-link font-red-sunglo'></i> <span class='caption-subject bold'> تغير كلمة المرور</span> </div></div><div class='portlet-body'> <div class='row'> <div class='col-md-12'> <form class='login-form' method='post'> <div class='alert alert-danger' style='text-align:center;' ng-show='AlertDiv'> <span>{{AlertSMS}}</span> </div><div class='form-group'> <label>اسم المستخدم</label> <div class='input-icon'> <i class='fa fa-user'></i> <input type='text' class='form-control' placeholder='اسم المستخدم ' ng-model='ChangePass.UserName' readonly> </div></div><div class='form-group'> <label>كلمة المرور القديمة</label> <div class='input-icon'> <i class='fa fa-lock'></i> <input type='password' class='form-control' placeholder='كلمة المرور القديمة' ng-model='ChangePass.OldPass'> </div></div><div class='form-group'> <label>كلمة المرور الجديدة</label> <div class='input-icon'> <i class='fa fa-lock'></i> <input type='password' class='form-control' placeholder='كلمة المرور الجديدة' ng-model='ChangePass.NewPass'> </div></div><div class='form-group'> <label>تأكيد كلمة المرور</label> <div class='input-icon'> <i class='fa fa-lock'></i> <input type='password' class='form-control' placeholder='تأكيد كلمة المرور' ng-model='ChangePass.ConfirmPass'> </div></div></form> </div><div class='row' style='text-align:center;margin-top:5px;'> <div class='form-actions'> <button type='submit' class='btn red' ng-click='ChangePassConfirm(ChangePass)'>تأكيد</button> <a href='/Home' type='button' class='btn default'>إلغاء</a> </div></div></div></div><h5 style='border-top: 1px solid #eee; color: #5b9bd1; text-align: center; padding-top: 15px; '>برجاء عدم إفشاء كلمة المرور</h5></div>",
                             closeOnEscape: false,
                             closeOnOverlayClick: false
                         });
                         document.getElementById('UserName').value = response.ErrorName;
                     }
                 });

    }
});
app.controller('ChangePassCtrl', function ($scope, $http, $window, $fancyModal, $timeout) {
    $scope.ChangePass = { "UserName": document.getElementById('UserName').value };
    $scope.ChangePassConfirm = function (ChangePass) {
        $scope.AlertDiv = null; $scope.AlertSMS = null;
        if (ChangePass.OldPass == null || ChangePass.OldPass == "") {
            $scope.AlertDiv = 1;
            $scope.AlertSMS = "برجاء كتابة كلمة المرور القديمة";
        }
        else if (ChangePass.NewPass == null || ChangePass.NewPass == "") {
            $scope.AlertDiv = 1;
            $scope.AlertSMS = "برجاء كتابة كلمة المرور الجديدة";
        }
        else if (ChangePass.ConfirmPass == null || ChangePass.ConfirmPass == "") {
            $scope.AlertDiv = 1;
            $scope.AlertSMS = "برجاء كتابة تأكيد كلمة المرور الجديدة";
        }
        else if (ChangePass.NewPass != ChangePass.ConfirmPass) {
            $scope.AlertDiv = 1;
            $scope.AlertSMS = "كلمة المرور الجديدة غير متطابقة";
        }
        else {
            $scope.AlertDiv = null; $scope.AlertSMS = null;
            var sentData = "NewPass=" + ChangePass.NewPass
                           + "&OldPass=" + ChangePass.OldPass;
            $http.post('/Users/ChangePassword?' + sentData)
                  .success(function (response) {
                      $scope.AlertSMS = response.ErrorName;
                      $scope.AlertDiv = 1;
                      if (response.ErrorName.indexOf("بنجاح") > -1) {
                          $timeout(function () {
                              $window.location.href = '/Users/LogOff';
                          }, 500);
                      }
                  });

        }
    }
});
