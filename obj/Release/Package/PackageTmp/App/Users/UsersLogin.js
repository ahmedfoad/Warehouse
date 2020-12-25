app.controller('UserLoginCtrl', function ($scope, $http, $window) {
    $scope.User = { Username: "", Password: "" };
    $scope.login = function (User) {
      
       
        $scope.AlertDiv = null;
        if ($scope.User.Password == "" && $scope.User.Username == "") {
            $scope.AlertSMS = "برجاء ادخال اسم المستخدم وكلمة المرور"
            $scope.AlertDiv = 1;
            return;
        }
        if ($scope.User.Username == "") {
            $scope.AlertSMS = "برجاء ادخال اسم المستخدم"
            $scope.AlertDiv = 1;
            return;
        }
        if ($scope.User.Password == "") {
            $scope.AlertSMS = "برجاء ادخال كلمة المرور"
            $scope.AlertDiv = 1;
            return;
        }
       
        var ReturnURL;
        try { ReturnURL = document.URL.split("?")[1].split("=")[1]; }
        catch (err) { ReturnURL = null; }   
        var parameter = JSON.stringify({ 'ReturnURL': ReturnURL, 'Username': $scope.User.Username, 'Password': $scope.User.Password });
        $http.post("/Users/Login", parameter)
        .success(function (response) {
          
            if (response != null) {
                if (response == false) {
                    $scope.AlertSMS = "بيانات الدخول غير صحيحة"
                    $scope.AlertDiv = 1;
                }
                else if (response == true) {          
                    //$window.location.href = "/invoice_Mandob/Invoice_Mandob";
                    $window.location.href = "/Home";
                }
            }
        });
    }


    document.onkeydown = function () {
     
        var Enter = 13;
      


        if (window.event && window.event.keyCode == Enter) { //طباعة الفاتورة
            if ($("#Username").is(":focus")) {
                $('#Password').focus();
            } else if ($("#Password").is(":focus")) {
                $('#Username').focus();
            }
        }
       
    }

});