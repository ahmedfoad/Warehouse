app.controller("HomeCtrl", function ($scope, $http, $window, $timeout, $fancyModal) {
    $scope.InitialHome = function () {       
        $http.get("/Home/Statistics/37")
          .success(function (response) {
              
              new Morris.Area({
                  element: 'statistics',
                  data: response,
                  xkey: 'الشهر',
                  ykeys: ['الصادر', 'الوارد'],
                  labels: ['الصادر', 'الوارد'],
                  gridTextFamily: "OmarRezk-Arabic-Bold",
                  gridTextColor: "fff",
                  parseTime: false,
                  smooth: false,
                  fillOpacity: .8,
                  lineWidth: 1,
                  pointSize: 2,
                  lineColors: ["#578EBE"],
                  xLabelAngle: 90,
                  resize: true,
              });
          });       
    }
    $scope.InitialHome();
    var i = 0;
    function InvestCountLoop(count) {
        setTimeout(function () {
            document.getElementById("InvestCount").innerHTML = i;
            i++;
            if (i <= count) {
                InvestCountLoop(count);
            }
        }, 1);
    }
    var x = 0;
    function BulidingCountLoop(Ecount) {
        setTimeout(function () {
            document.getElementById("BulidingCount").innerHTML = x;
            x++;
            if (x <= Ecount) {
                BulidingCountLoop(Ecount);
            }
        }, 1)
    }
});