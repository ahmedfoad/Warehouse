var app = angular.module("App", ['ui.bootstrap', 'barcodeScanner' , 'vesparny.fancyModal']);

app.directive('datepicker', function () {
    return function (scope, element, attrs) {
        element.Zebra_DatePicker({
        });
    }
});

app.directive('datepickerstart', function () {
    return function (scope, element, attrs) {
        element.Zebra_DatePicker({
            direction: false,
           pair: $('.datepickeend')
        });
    }
});

app.directive('datepickeend', function () {
    return function (scope, element, attrs) {
        element.Zebra_DatePicker({
            direction: 1
        });
    }
});

app.directive('numbersOnly', function () {
    return {
        require: 'ngModel',
        link: function (scope, element, attr, ngModelCtrl) {
            function fromUser(text) {
                if (text) {
                    var transformedInput = text.replace(/[^0-9]/g, '');

                    if (transformedInput != text) {
                        ngModelCtrl.$setViewValue(transformedInput);
                        ngModelCtrl.$render();
                    }
                    return transformedInput;
                }
                return undefined;
            }
            ngModelCtrl.$parsers.push(fromUser);
        }
    };
});

app.directive('selectOnClick', ['$window', function ($window) {
    return {
        restrict: 'A',
        link: function (scope, element, attrs) {
            element.on('click', function () {
                if (!$window.getSelection().toString()) {
                    // Required for mobile Safari
                    this.setSelectionRange(0, this.value.length)
                }
            });
        }
    };
}]);


app.directive("focusNext", function () {
    return {
        restrict: "A",
        link: function ($scope, elem, attrs) {
            elem.bind("keydown", function (e) {
              
                var code = e.keyCode || e.which;
               // console.log(e);
                if (code === 9 ) {
                    e.preventDefault();
                    //document.getElementById(attrs.focusNext).focus();
                    $("[name='" + attrs.focusNext + "']").focus();
                } else {
                    return true;
                }
            });
        }
    };
});