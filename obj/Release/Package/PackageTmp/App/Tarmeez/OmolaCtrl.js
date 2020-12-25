app.controller("OmolaCtrl", function ($scope, $http, $window, $timeout, $uibModal) {

   $scope.Omola = { ID: 0, Omola_Type: false, Omola_quota:0 };



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

    $scope.switchCase = function () {

        var hiddenElements = ["lblReyal", "lblPercentage"];
        $scope.hideElement(hiddenElements);
        $scope.EditBtn = 1;

        //document.getElementById('PrintTicket').href = "/Omola/PrintTicketReport/" + id;
        //document.getElementById('OmolaRefer').href = "/Omola/Refer/" + id;
        $http.get('/Omola/loadOmola')
           .success(function (response) {
               if (response == null) {
                   $uibModal.open({
                       template: " <div class='portlet box red' style='min-height:150px !important;min-width: 541px !important;'> <div class='portlet-title'> <div class='caption'> <i class='fa-lg fa fa-warning'></i> تنبيه هام </div><div class='tools'> </div></div><div class='portlet-body form portlet-empty' style='min-height: 140px;'> <div class='col-md-12 page-404'> <div class='number'>" + response.ErrorNumber + "</div><div class='details'> <h3 style='font-family: sans-serif; text-align: center; font-weight: bold;'>" + response.ErrorFullNumber + "</h3> <p style='font-size: 12px;'> " + response.ErrorName + " <br/> <br/> <a class='btn btn-default btn-sm' href='/home'> الصفحة الرئيسية </a> <a class='btn btn-default btn-sm' href='" + response.Url + "' style='margin-right: 4px;'>المحاولة مرة آخري</a> </p></div></div></div></div>",
                       closeOnEscape: false,
                       closeOnOverlayClick: false
                   });
               }
               else {

                   $scope.Omola = {
                       ID: response.ID,
                       Omola_Type: response.Omola_Type,
                       Omola_quota: response.Omola_quota
                   };
                   debugger;
                   if ($scope.Omola.Omola_Type == false) {
                       document.getElementById("lblReyal").style.display = 'block';
                       document.getElementById("lblPercentage").style.display = 'none';
                   } else {
                       document.getElementById("lblReyal").style.display = 'none';
                       document.getElementById("lblPercentage").style.display = 'block';
                   }
               }
           });


    }
    $scope.switchCase();

    $('#Omola_Type').change(function () {
        debugger;
        // $(#Omola_Type).val() will work here
        var x=$('#Omola_Type').val();
        if (x  == "false") {
            document.getElementById("lblReyal").style.display = 'block';
            document.getElementById("lblPercentage").style.display = 'none';
        } else {
            document.getElementById("lblReyal").style.display = 'none';
            document.getElementById("lblPercentage").style.display = 'block';
        }
    });
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
        if ($scope.Omola.Omola_quota == null || $scope.Omola.Omola_quota == "") {
            $scope.ErrorName = "برجاء ادخال قيمة العمولة"
            var ListID = ["Omola_quota"];
            $scope.changeBorder(ListID, "red");
            return false;
        } else {
            var ListID = ["Omola_quota"];
            $scope.changeBorder(ListID, "#c2cad8");
        }


        $scope.AlertParameter = null;
        return true;
    }
    $scope.SaveOmola = function () {
        

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
        
        $scope.Omola.Omola_Type = document.getElementById("Omola_Type").value;
        var parameter = JSON.stringify($scope.Omola);
      
        //Omola.BarCodeArr = null;
        $http.post('/OmolaMandob/Edit', parameter)
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
                           $window.location.href = '/OmolaMandob/Operation';
                       }, 500);
                   }
               }


           });


    }
});

