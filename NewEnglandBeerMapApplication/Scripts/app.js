// intialize Angular app
var NEBreweryApp = angular.module('NEBreweryMapApp', ['ngMap', 'vcRecaptcha', 'ngSanitize']);

// Filter by State Filter
NEBreweryApp.filter('filterByState', function () {
    return function (markers, selectedState) {
        if (!angular.isUndefined(selectedState) && selectedState.length) {
            var stateSortedMarkers = [];
            angular.forEach(markers, function (marker) {
                if (angular.equals(marker.BreweryState,selectedState) ) {
                    stateSortedMarkers.push(marker);
                }                
            })
            return stateSortedMarkers;
        } else {            
            return markers;            
        }               
    }      
})
// Google Map Controller
NEBreweryApp.controller('googleMapCtrl', function ($scope, $filter, $http, NgMap, filterByStateFilter) {
    // Clear markers
    $scope.markers = [];
    $scope.currentmarker;
    $scope.customMarker;
    var infowindow;
    $scope.searchFilter
    $scope.loading = false;
    
    // custom marker icon
    $scope.customMarker = {       
        icon: 'http://www.mattdailey.net/img/beermapmarker.png'
    };
    // Initialize Google Map
    NgMap.getMap().then(function (map) {
        $scope.loading = true;
        $scope.map = map;
    }, function () {
        alert('Could not load map.');        
    }).finally(function () {
        // called no matter success or failure
        $scope.loading = false;
    });
    // Populate map with all locations       
    $http.get('/nebrewerylocator/Home/GetAllMarkers').then(function (data) {        
        $scope.markers = data.data.Data;
    }, function () {
        alert('Could not retrieve map marker data.');
        
    }).finally(function () {
        // called no matter success or failure
        $scope.loading = false;
    });
    // center map, zoom and show info window of selected marker
    $scope.markerZoom = function (marker) {
        $scope.map.setCenter({ lat: marker.latitude, lng: marker.longitude });
        $scope.map.setZoom(11);
        $scope.currentmarker = marker;
        $scope.map.showInfoWindow('currentWindow', 'm' + marker.Id);
    }
    // Trigger info window
    $scope.showInfo = function (evt, marker) {
        $scope.currentmarker = marker;
        $scope.map.showInfoWindow('currentWindow', 'm' + marker.Id);
    };
    // Map will move to state based on corresponding dropdown menu value
    $scope.stateZoom = function (selectedState) {
        statesCoords = {
            CT: {
                latLng: [41.527086145800006, -72.65808142968751],
                zoom: 9
            },
            MA: {
                latLng: [42.02889443064134, -71.65008582421876],
                zoom: 8
            },
            ME: {
                latLng: [44.95702443907399, -68.95843543359376],
                zoom: 7
            },
            NH: {
                latLng: [43.95328236137632, -71.28753699609376],
                zoom: 8
            },
            RI: {
                latLng: [41.64828864411775, -71.30401648828126],
                zoom: 9
            },
            VT: {
                latLng: [43.901850824945996, -72.50152625390626],
                zoom: 8
            },
            default: {
                latLng: [43.8992496, -71.6135105],
                zoom: 7
            }
        }
        if (selectedState.length > 1) {
            $scope.map.hideInfoWindow('currentWindow'); // close any info windows if any are open
            for (var j in statesCoords) {
                if (j === selectedState) {
                    $scope.map.setZoom(statesCoords[j].zoom); // set zoom for the selected state
                    $scope.map.setCenter({lat:statesCoords[j].latLng[0], lng:statesCoords[j].latLng[1]}); // set coordinates for the selected state
                }
            }
        } else {
            $scope.map.hideInfoWindow('currentWindow'); // close any info windows if any are open
            $scope.map.setZoom(statesCoords['default'].zoom); // set zoom to the initial default level
            $scope.map.setCenter({ lat: statesCoords['default'].latLng[0], lng: statesCoords['default'].latLng[1] }); // set coordinates to the initial default level
        }
    };
    $scope.filterBrewery = function (marker) {
        if (!angular.isUndefined($scope.inputBrewery) && $scope.inputBrewery.length) {
            return nameStartsWith(marker.BreweryName, $scope.inputBrewery);
        } else {
            return true;
        }        
    };
    // Search by brewery function
    function nameStartsWith(name, search) {
        if (search === '')
            return true;

        var Regex = /[ _-]+/;
        var names = name.split(Regex);

        //do any of the names in the array start with the search string
        return names.some(function (name) {
            return name.toLowerCase().indexOf(search.toLowerCase()) === 0;
        });
    }
});
// Popup Contact Form Controller
NEBreweryApp.controller('popupContactFormCtrl', function ($scope, $sce, $http, vcRecaptchaService) {
    $scope.submitted = false;
    $scope.processForm = function () {
        if ($scope.popupContact.$invalid) { // check if all fields are valid
            $scope.submitted = true;
            $event.preventDefault(); // stop form submit if any of the fields have an $invalid state
        } else {
            $scope.submitted = true;
            $http({
                method: 'POST',
                url: 'nebrewerylocator/Home/Submit',
                data: $.param($scope.contactfields),  // pass in data as strings
                headers: { 'Content-Type': 'application/x-www-form-urlencoded' }  // set the headers as form data
            })
            .then(function successCallback(response) {               
                $(".contactform").hide();
                $scope.formsending = false;
                responseData = response.data;
                $scope.grecaptcharesponse = null;
                if (!responseData.isMessageSent) { // check if email was sent
                    $scope.submiterrors = responseData.submitErrors
                    $scope.messagesent = responseData.isMessageSent;
                    $scope.submitmessage = $sce.trustAsHtml(responseData.submitMessage);
                    $scope.grecaptcharesponse = responseData.gCaptchaResponse;
                } else {
                    $scope.messagesent = responseData.isMessageSent;
                    $scope.submitmessage = $sce.trustAsHtml(responseData.submitMessage); // treat submitmessage as HTML
                }
            }, function errorCallback(response) {
                $(".contactform").hide();
                console.log("Error message: " + response.status + " " + response.error);
                $scope.submitmessage = $sce.trustAsHtml("<strong>Error!</strong>The server could not be contacted and your message has not been sent. Please check your internet connection and try again later"); // treat submitmessage as HTML
            });
        }
    };
});
