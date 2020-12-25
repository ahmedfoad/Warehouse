var Search;
var page = 0;
app.controller('ActionSearchCtrl', function ($scope, $http, $window, $fancyModal) {
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

    };
    $scope.updateTableView();
    //----------------------------------------------------------------------------------------------------------------
    //----------------------------------------------------------------------------------------------------------------
    // Clear all Fields
    //----------------------------------------------------------------------------------------------------------------
    $scope.emptyInputs = function () {
        $scope.Email = null;
        $scope.UserName = null;
    }
    //----------------------------------------------------------------------------------------------------------------
    //----------------------------------------------------------------------------------------------------------------
    // Get Actions Functions
    //----------------------------------------------------------------------------------------------------------------
    $scope.getAllActions = function (Page, Email, UserName) {
        $scope.EndSearch = null;
        $scope.TableLoading = 1;
        if ($scope.data == null || $scope.data.length == 0) {
            page = 1;
        }
        var parameter = JSON.stringify({ 'Email': Email, 'page': page, 'UserName': UserName });
        $http.post("/Actions/Search", parameter)
        .success(function (response) {
            if (response != null) {
                if ($scope.data == null) {
                    $scope.data = response;
                }
                else {
                    $scope.data = $scope.data.concat(response);
                }
            }
            $scope.TableLoading = null;
            if (response == '' || response == null) {
                $scope.EndSearch = 1;
            }
        });
    };
    //----------------------------------------------------------------------------------------------------------------
    //----------------------------------------------------------------------------------------------------------------
    // search button Functions
    //----------------------------------------------------------------------------------------------------------------
    $scope.SearchButton = function () {
        $scope.data = null;
        page = 0;
        $scope.getAllActions(null, $scope.Email, $scope.UserName);
    };
    //----------------------------------------------------------------------------------------------------------------------------
    //----------------------------------------------------------------------------------------------------------------------------
    // get data of Actions with scrolling 
    //----------------------------------------------------------------------------------------------------------------------------
    angular.element($window).bind("scroll", function () {

        var windowHeight = "innerHeight" in window ? window.innerHeight : document.documentElement.offsetHeight;
        var body = document.body, html = document.documentElement;
        var docHeight = Math.max(body.scrollHeight, body.offsetHeight, html.clientHeight, html.scrollHeight, html.offsetHeight);
        windowBottom = windowHeight + window.pageYOffset;
        if (windowBottom >= docHeight) {
            page++;
            $scope.getAllActions(page, $scope.Email, $scope.UserName);
        }
    });
});