// service that allows to work with TWAIN devices
var _twainService = new Vintasoft.Shared.WebServiceControllerJS('http://localhost:25319/api/MyVintasoftTwainApi');

// TWAIN device manager
var _twainDeviceManager = new Vintasoft.Twain.WebTwainDeviceManagerJS(_twainService);
var _initSetting = new Vintasoft.Twain.WebTwainDeviceManagerInitSettingsJS();
// an array of available TWAIN devices
var _devices;

// images acquired from TWAIN device
var _acquiredImages = [];
// sizes of images acquired from TWAIN device
var _acquiredImageSizes = [];
// index of focused acquired image
var _focusedImageIndex = -1;
var _isImageProcessing = false;

// timer for monitoring the image uploading process
var _uploadToHttpTimer;


$(".navigation").on("click", goToImage);
$(".deleting").on("click", removeImage);
$(".processing").on("click", processFocusedImage);
$("#SaveImagedButton").on("click", SaveImage);
$("#HttpUploadButton").on("click", UploadScannedImages);
$("#HttpCancelButton").on("click", CancelUploadingScannedImages);
$("#closeErrorMessageDialog").on("click", __closeErrorMessageDialog);
$(window).on("beforeunload", __pageUnload);

// ================= Getting information about TWAIN devices ========================

// if TWAIN device manager is NOT opened
if (_twainDeviceManager.get_IsOpened() == false) {
    // send a request for opening TWAIN 2.X device manager
    _twainDeviceManager.open(_initSetting, __openDeviceManager_success, __open2XDeviceManager_fail);
}

/**
 TWAIN device manager is opened successfully.
*/
function __openDeviceManager_success() {
    // send a request for getting information about TWAIN devices available in the system
    _twainDeviceManager.getDevices(__getDeviceInfos__success, __showErrorMessage);
}

/**
 TWAIN 2.X device manager is not opened.
*/
function __open2XDeviceManager_fail(data) {
    _initSetting.set_IsTwain2Compatible(false);
    // try to open TWAIN 1.X Manager
    _twainDeviceManager.open(_initSetting, __openDeviceManager_success, __openDeviceManager_fail);
}

/**
 TWAIN device manager is NOT opened.
*/
function __openDeviceManager_fail(data) {
    if (data.status === 0 || data.status === 404) {
        data.errorMessage = 'VintaSoft Web TWAIN service is not found.<br />Please download VintaSoft Web TWAIN Service from this <a href="http://www.vintasoft.com/zip/VSWebTwainService1_0.zip">link</a>, manually install the service on your computer and refresh this web page.';
    }
    __showErrorMessage(data);
}

/**
 Information about TWAIN devices is received successfully.
*/
function __getDeviceInfos__success(data) {
    // get an array of available TWAIN devices
    _devices = data.devices;
    // get index of default TWAIN device
    var defaultDeviceIndex = data.defaultDeviceIndex;
    var objSel = document.getElementById("DevicesSelect");
    objSel.options.length = 0;
    // for each TWAIN device
    for (var i = 0; i < _devices.length; i++) {
        // add the device info to the device list
        objSel.options.length = objSel.options.length + 1;
        objSel.options[i].text = _devices[i].get_DeviceName();
        if (i === defaultDeviceIndex)
            objSel.options[i].selected = true;
    }
    $("#ScanImageButton").prop("disabled", false);
}

// ===========================================================



// ================= Image scanning ========================

/**
 "Scan Image" button is clicked.
*/
function ScanImage() {
    var showUIVal = $("#ShowUI").prop('checked');
    var deviceIndex = $("#DevicesSelect")[0].selectedIndex;
    if (_twainDeviceManager.get_OpenedDevice() == undefined) {
        var twainDevice = _devices[deviceIndex];
        // if scanner's UI must be shown
        if (showUIVal)
            // send a request for opening TWAIN device
            twainDevice.open(showUIVal, __acquireModal, __openDevice_fail);
            // if scanner's UI must NOT be shown
        else
            // send a request for opening TWAIN device
            twainDevice.open(showUIVal, __setDevicePixelTypeCapability, __openDevice_fail);
        // disable the "Scan Image" button
        $("#ScanImageButton").prop("disabled", true);
    }
    return false;
}

/**
 Device is NOT opened.
*/
function __openDevice_fail(data) {
    __showErrorMessage(data);
    $("#ScanImageButton").prop("disabled", false);
}

/**
 Changes the pixel type of device.
*/
function __setDevicePixelTypeCapability(data) {
    var device = _twainDeviceManager.get_OpenedDevice();
    if (device != undefined) {
        var pixelType = GetSelectedRadioValueInGroup("ScanPixelType");
        var pixelTypeCapId = new Vintasoft.Twain.WebDeviceCapabilityIdJS("IPixelType");
        // send a request for setting the IPixelType capability of TWAIN device
        device.setDeviceCapability(pixelTypeCapId, pixelType, __setDeviceADFCapability, __acquireModal);
    }
}

/**
 Enables/disables the device feeder.
*/
function __setDeviceADFCapability() {
    var device = _twainDeviceManager.get_OpenedDevice();
    if (device != undefined) {
        var useADF = $("#UseADF").prop("checked");
        var feederEnabledCapId = new Vintasoft.Twain.WebDeviceCapabilityIdJS("FeederEnabled");
        // send a request for setting the FeederEnabled capability of TWAIN device
        device.setDeviceCapability(feederEnabledCapId, useADF, __setDeviceDuplexCapability, __acquireModal);
    }
}

/**
 Enables/disables the duplex functionality of the device feeder.
*/
function __setDeviceDuplexCapability() {
    var device = _twainDeviceManager.get_OpenedDevice();
    if (device != undefined) {
        var useDuplex = $("#UseDuplex").prop("checked");
        var duplexEnabledCapId = new Vintasoft.Twain.WebDeviceCapabilityIdJS("DuplexEnabled");
        // send a request for setting the DuplexEnabled capability of TWAIN device
        device.setDeviceCapability(duplexEnabledCapId, useDuplex, __acquireModal, __acquireModal);
    }
}


/**
 Sends a request for acquiring images from TWAIN device.
*/
function __acquireModal() {
    // get opened TWAIN device
    var device = _twainDeviceManager.get_OpenedDevice();
    // if device is found
    if (device != undefined) {
        // send a request for acquiring images from TWAIN device
        device.acquireModal(__acquireModal_success, __showErrorMessage);
    }

}

/**
 One step of synchronous image acquisition is executed successfully.
*/
function __acquireModal_success(data) {
    var response = data;
    // get state of image acquisition
    var acquireModalState = response.acquireModalState;
    var acquireModalStateAsString = __convertAcquireModalStateToString(acquireModalState);
    switch (acquireModalState) {
        case 2:   // image is acquired
            // get image as Base64 string
            var base64 = "data:image/bmp; base64," + response.imageId;
            // add image to the collection of acquired images
            _acquiredImages.push(base64);
            _acquiredImageSizes.push({ width: response.width, height: response.height });
            // set image as focused image
            _focusedImageIndex = _acquiredImages.length - 1;
            // update image preview
            __updateImagePrieview();
            break;

        case 4:   // scan is failed
            __showErrorMessage(data);
            break;

        case 8:   // image acquiring progress is changed
            break;

        case 9:   // scan is finished
            break;
    }

    // if image acquisition must be continued
    if (acquireModalState !== 0)
        // send a request for acquiring images from TWAIN device
        __acquireModal();
        // if image acquisition must be finished
    else {
        // get opened TWAIN device
        var device = _twainDeviceManager.get_OpenedDevice();
        // if device is found
        if (device != undefined)
            // close the device
            device.close(__twainDeviceClose__success, __showErrorMessage);
    }
}

/**
 TWAIN device is closed successfully.
*/
function __twainDeviceClose__success() {
    $("#ScanImageButton").prop("disabled", false);
}

// ===========================================================



// ================= Image navigation ========================

/**
 Image navigation.
*/
function goToImage(event) {
    if (_acquiredImages.length !== 0 && _focusedImageIndex !== -1) {
        switch (event.target.id) {
            case "GoToFirstImageButton":
                if (_focusedImageIndex === 0)
                    return false;
                _focusedImageIndex = 0;
                break;
            case "GoToPreviousImageButton":
                if (_focusedImageIndex === 0)
                    return false;
                _focusedImageIndex = _focusedImageIndex - 1;
                break;
            case "GoToNextImageButton":
                if (_focusedImageIndex === _acquiredImages.length - 1)
                    return false;
                _focusedImageIndex = _focusedImageIndex + 1;
                break;
            case "GoToLastImageButton":
                if (_focusedImageIndex === _acquiredImages.length - 1)
                    return false;
                _focusedImageIndex = _acquiredImages.length - 1;
                break;
        }
        __updateImagePrieview();
    }
    return false;
}

// ===========================================================



// ================= Image removing ========================

/**
 Removes one or all images from local storage.
*/
function removeImage(event) {
    if (_acquiredImages.length !== 0 && _focusedImageIndex !== -1) {
        switch (event.target.id) {
            case "DeleteImageButton":
                var ajaxParams = {
                    type: 'POST',
                    data: {
                        twainSessionId: _twainDeviceManager.get_TwainSessionID(),
                        imageIndex: _focusedImageIndex
                    }
                }
                var request = new Vintasoft.Shared.WebRequestJS("DeleteImage", __deleteImage_success, __showErrorMessage, ajaxParams);
                // send a request for deleting the image from local storage
                _twainService.sendRequest(request);
                break;

            case "DeleteAllImagesButton":
                var ajaxParams = {
                    type: 'POST',
                    data: { twainSessionId: _twainDeviceManager.get_TwainSessionID() }
                }
                var request = new Vintasoft.Shared.WebRequestJS("DeleteAllImages", __deleteAllImages_success, __showErrorMessage, ajaxParams);
                // send a request for deleting all images from local storage
                _twainService.sendRequest(request);
                break;
        }
    }
    return false;
}

/**
 Image, from local storage, is deleted successfully.
*/
function __deleteImage_success(data) {
    if (_acquiredImages.length !== 0 && _focusedImageIndex !== -1) {
        var removedIndex = data.imageIndex;
        _acquiredImages.splice(removedIndex, 1);
        _acquiredImageSizes.splice(removedIndex, 1);
        _focusedImageIndex = 0;
        if (_acquiredImages.length === 0)
            _focusedImageIndex = -1;
        __updateImagePrieview();
    }
}

/**
 All images, from local storage, are deleted successfully.
*/
function __deleteAllImages_success() {
    _acquiredImages = [];
    _acquiredImageSizes = [];
    _focusedImageIndex = -1;
    __updateImagePrieview();
}
// ===========================================================



// ========================== Image processing ==================================

/**
 Processes the focused scanned image.
*/
function processFocusedImage(event) {
    if (_focusedImageIndex >= 0 && _focusedImageIndex < _acquiredImages.length) {
        var id = event.target.id;
        var twainSessionId = _twainDeviceManager.get_TwainSessionID();
        var request;
        switch (id) {

            case "IsImageBlankButton":
                var ajaxParams = {
                    type: 'POST',
                    data: {
                        twainSessionId: twainSessionId,
                        imageIndex: _focusedImageIndex,
                        maxNoiseLevel: 0.01
                    }
                }
                request = new Vintasoft.Shared.WebRequestJS("IsImageBlank", __isImageBlank_success, __processing_fail, ajaxParams);
                break;

            case "InvertButton":
                var ajaxParams = {
                    type: 'POST',
                    data: { twainSessionId: twainSessionId, imageIndex: _focusedImageIndex }
                }
                request = new Vintasoft.Shared.WebRequestJS("InvertImage", __modifiedImage_success, __processing_fail, ajaxParams);
                break;

            case "IncreaseBrightnessButton":
            case "DecreaseBrightnessButton":
                var brightness = 5;
                if (id === "DecreaseBrightnessButton") brightness = -5;
                var ajaxParams = {
                    type: 'POST',
                    data: {
                        twainSessionId: twainSessionId,
                        imageIndex: _focusedImageIndex,
                        brightness: brightness
                    }
                }
                request = new Vintasoft.Shared.WebRequestJS("ChangeImageBrightness", __modifiedImage_success, __processing_fail, ajaxParams);
                break;

            case "IncreaseContrastButton":
            case "DecreaseContrastButton":
                var contrast = 5;
                if (id === "DecreaseContrastButton") contrast = -5;
                var ajaxParams = {
                    type: 'POST',
                    data: {
                        twainSessionId: twainSessionId,
                        imageIndex: _focusedImageIndex,
                        contrast: contrast
                    }
                }
                request = new Vintasoft.Shared.WebRequestJS("ChangeImageContrast", __modifiedImage_success, __processing_fail, ajaxParams);
                break;

            case "CropButton":
                var imageSize = _acquiredImageSizes[_focusedImageIndex];
                if (imageSize.width > 20 && imageSize.height > 20) {
                    var ajaxParams = {
                        type: 'POST',
                        data: {
                            twainSessionId: twainSessionId,
                            imageIndex: _focusedImageIndex,
                            x: 10, y: 10,
                            width: imageSize.width - 20,
                            height: imageSize.height - 20
                        }
                    }
                    request = new Vintasoft.Shared.WebRequestJS("CropImage", __modifiedImage_success, __processing_fail, ajaxParams);
                }
                break;

            case "IncreaseCanvasButton":
                var imageSize = _acquiredImageSizes[_focusedImageIndex];
                var ajaxParams = {
                    type: 'POST',
                    data: {
                        twainSessionId: twainSessionId,
                        imageIndex: _focusedImageIndex,
                        canvasWidth: imageSize.width + 20,
                        canvasHeight: imageSize.height + 20,
                        canvasColor: 0,
                        imageXPosition: 10, imageYPosition: 10
                    }
                }
                request = new Vintasoft.Shared.WebRequestJS("ResizeCanvas", __modifiedImage_success, __processing_fail, ajaxParams);
                break;

            case "Rotate90Button":
            case "Rotate180Button":
            case "Rotate270Button":
                var angle = 90;
                if (id === "Rotate180Button") angle = 180;
                else if (id === "Rotate270Button") angle = 270;
                var ajaxParams = {
                    type: 'POST',
                    data: {
                        twainSessionId: twainSessionId,
                        imageIndex: _focusedImageIndex,
                        angle: angle
                    }
                }
                request = new Vintasoft.Shared.WebRequestJS("RotateImage", __modifiedImage_success, __processing_fail, ajaxParams);
                break;

            case "DespeckleButton":
                var ajaxParams = {
                    type: 'POST',
                    data: {
                        twainSessionId: twainSessionId,
                        imageIndex: _focusedImageIndex,
                        level1: 8, level2: 25,
                        radius: 30, level3: 400
                    }
                }
                request = new Vintasoft.Shared.WebRequestJS("DespeckleImage", __modifiedImage_success, __processing_fail, ajaxParams);
                break;

            case "DetectImageBorderButton":
                var ajaxParams = {
                    type: 'POST',
                    data: {
                        twainSessionId: twainSessionId,
                        imageIndex: _focusedImageIndex,
                        brightness: brightness
                    }
                }
                request = new Vintasoft.Shared.WebRequestJS("DetectBorder", __modifiedImage_success, __processing_fail, ajaxParams);
                break;

            case "DeskewButton":
                var ajaxParams = {
                    type: 'POST',
                    data: {
                        twainSessionId: twainSessionId,
                        imageIndex: _focusedImageIndex,
                        borderColor: 0,
                        scanIntervalX: 5, scanIntervalY: 5
                    }
                }
                request = new Vintasoft.Shared.WebRequestJS("DeskewImage", __modifiedImage_success, __processing_fail, ajaxParams);
                break;
        }
        if (request != undefined && !_isImageProcessing) {
            _isImageProcessing = true;
            _twainService.sendRequest(request);
        }
    }
    return false;
}

/**
 Image processing is failed.
*/
function __processing_fail(data) {
    __showErrorMessage(data);
    _isImageProcessing = false;
}

/**
 Image is processed successfully.
*/
function __modifiedImage_success(data) {
    var index = data.imageIndex;
    if (index < _acquiredImages.length) {
        _acquiredImages[index] = "data:image/png; base64," + data.imageAsBase64String;
        if (data.width != undefined && data.height != undefined)
            _acquiredImageSizes[index] = { width: data.width, height: data.height };
        if (index === _focusedImageIndex) {
            __updateImagePrieview();
        }
    }
    _isImageProcessing = false;
}

/**
 IsImageBlank command is executed successfully.
*/
function __isImageBlank_success(data) {
    if (data.isBlank)
        alert("Image is blank.");
    else
        alert("Image is NOT blank.");
    _isImageProcessing = false;
}

// ===========================================================



// ================= Image saving ===========================

/**
 Saves scanned image(s) to a file.
*/
function SaveImage() {
    // get image saving parameters from UI

    var filepath = $("#SaveImagePath").val();
    if (filepath == "") {
        alert('Image file path is not specified.');
        return;
    }

    var saveAllImages = $("#SaveAllImages").prop("checked");
    var indices = []
    if (saveAllImages)
        for (var i = 0; i < _acquiredImages.length; i++)
            indices.push(i);
    else
        indices.push(_focusedImageIndex);

    filepath = filepath + GetSelectedRadioValueInGroup('SaveImageFormat');

    var createNewFile = $("#CreateNewFile").prop("checked");

    // create a request for saving images to a file

    var ajaxParams = {
        type: 'POST',
        data: {
            twainSessionId: _twainDeviceManager.get_TwainSessionID(),
            filePath: filepath,
            createNewFile: createNewFile,
            imageIndices: indices
        }
    }
    var request = new Vintasoft.Shared.WebRequestJS("SaveImages", __saveImages_success, __showErrorMessage, ajaxParams);

    // send a request for saving images to a file
    _twainService.sendRequest(request);

}

/**
 Images are saved successully.
*/
function __saveImages_success(data) {
    alert("Images are saved successully.");
}

// ===========================================================



// ============================ Image uploading to HTTP ==================

/**
 Uploads scanned image(s) to HTTP server.
*/
function UploadScannedImages() {

    // get image uploading parameters from UI

    var httpUrl = $("#HttpUrlTextBox").val();
    if (httpUrl == "") {
        alert('HTTP url is not specified.');
        return;
    }

    var fieldName = $("#HttpFileFieldTextBox").val();
    if (fieldName == "") {
        alert('Field name is not specified.');
        return;
    }

    var fileName = $("#HttpFileFieldValueTextBox").val();
    if (fileName == "") {
        alert('File name is not specified.');
        return;
    }

    var uploadAllImages = $("#UploadAllImagesToHttp").prop("checked");
    var indices = []
    if (uploadAllImages)
        for (var i = 0; i < _acquiredImages.length; i++)
            indices.push(i);
    else
        indices.push(_focusedImageIndex);

    fileName = fileName + GetSelectedRadioValueInGroup('HttpUploadImageFileFormat');

    var textFields = [];
    var firstFieldName = $("#HttpTextField1TextBox").val();
    var firstFieldValue = $("#HttpTextField1ValueTextBox").val();
    textFields.push({
        fieldName: firstFieldName,
        fieldValue: firstFieldValue
    });
    var secondFieldName = $("#HttpTextField2TextBox").val();
    var secondFieldValue = $("#HttpTextField2ValueTextBox").val();
    textFields.push({
        fieldName: secondFieldName,
        fieldValue: secondFieldValue
    });


    // create a request for uploading images to HTTP server

    var ajaxParams = {
        type: 'POST',
        data: {
            fileFields: [
                    {
                        fieldName: fieldName,
                        fieldValue: fileName,
                        imageIndices: indices
                    }
            ],
            textFields: textFields,
            twainSessionId: _twainDeviceManager.get_TwainSessionID(),
            httpUrl: httpUrl
        }
    }

    var request = new Vintasoft.Shared.WebRequestJS("UploadImagesToHttp", __uploadToHttpSuccess, __uploadToHttpError, ajaxParams);

    // send a request for starting an image uploading process
    _twainService.sendRequest(request);

    $("#HttpUploadButton").prop("disabled", true);
    $("#HttpCancelButton").prop("disabled", false);
}

/**
 Image uploading is started successully.
*/
function __uploadToHttpSuccess(data) {
    // create timer for monitoring the image uploading process
    _uploadToHttpTimer = setInterval(getHttpUploadStatus, 100);
}

/**
Image uploading is failed.
*/
function __uploadToHttpError(data) {
    $("#HttpUploadButton").prop("disabled", false);
    $("#HttpCancelButton").prop("disabled", true);
    __showErrorMessage(data);
}

/**
 Timer function, which monitors the image uploading process.
*/
function getHttpUploadStatus() {
    var ajaxParams = {
        type: 'POST',
        data: {
            twainSessionId: _twainDeviceManager.get_TwainSessionID()
        }
    }
    var request = new Vintasoft.Shared.WebRequestJS("GetHttpUploadStatus", __getHttpUploadStatusSuccess, __getHttpUploadStatusError, ajaxParams);
    // send a request for getting status of image uploading process
    _twainService.sendRequest(request);
}

/**
 Status of image uploading process is received successfully.
*/
function __getHttpUploadStatusSuccess(data) {
    console.log("Uploaded :" + data.bytesUploaded + "/" + data.bytesTotal + ". Status:" + data.statusString);
    // if images are uploaded successfully
    if (data.statusCode === 5) {
        if (_uploadToHttpTimer != undefined) {
            clearInterval(_uploadToHttpTimer);
            _uploadToHttpTimer = undefined;
        }
        $("#HttpUploadButton").prop("disabled", false);
        $("#HttpCancelButton").prop("disabled", true);
        alert("Images are uploaded successfully.");
    }
}

/**
 Status of image uploading process is NOT received.
*/
function __getHttpUploadStatusError(data) {
    $("#HttpUploadButton").prop("disabled", false);
    $("#HttpCancelButton").prop("disabled", true);
    __showErrorMessage(data);
    // cancel image uploading
    CancelUploadingScannedImages();
}

/**
 Cancels the image uploading process.
*/
function CancelUploadingScannedImages() {
    if (_uploadToHttpTimer != undefined) {
        clearInterval(_uploadToHttpTimer);
        _uploadToHttpTimer = undefined;
    }

    var ajaxParams = {
        type: 'POST',
        data: {
            twainSessionId: _twainDeviceManager.get_TwainSessionID()
        }
    }
    var request = new Vintasoft.Shared.WebRequestJS("CancelHttpUpload", __cancelHttpUploadSuccess, __showErrorMessage, ajaxParams);
    // send a request for cancelling the image uploading process
    _twainService.sendRequest(request);
}

/**
 Image uploading is cancelled successfully.
*/
function __cancelHttpUploadSuccess(data) {
    $("#HttpUploadButton").prop("disabled", false);
    $("#HttpCancelButton").prop("disabled", true);
}

// ===========================================================



// ========================== Utils ==================================

/**
 Returns string representation of acquire modal state.
*/
function __convertAcquireModalStateToString(acquireModalState) {
    switch (acquireModalState) {
        case 0:
            return 'None';
        case 1:
            return 'ImageAcquiring';
        case 2:
            return 'ImageAcquired';
        case 3:
            return 'ScanCompleted';
        case 4:
            return 'ScanFailed';
        case 5:
            return 'ScanCanceled';
        case 6:
            return 'UserInterfaceClosed';
        case 7:
            return 'DeviceEvent';
        case 8:
            return 'ImageAcquiringProgress';
        case 9:
            return 'ScanFinished';
        default:
            throw 'Not valid AcquireModalState';
    };
}

/**
 Updates an image preview and information about image.
*/
function __updateImagePrieview() {
    // if focused image is specified
    if (_focusedImageIndex !== -1 && _acquiredImages.length > _focusedImageIndex) {

        var disableFirstAndPrevButtons = (_acquiredImages.length <= 1) || (_focusedImageIndex === 0);
        var disableLastAndNextButtons = (_acquiredImages.length <= 1) || (_focusedImageIndex === (_acquiredImages.length - 1));
        $("#GoToFirstImageButton").prop("disabled", disableFirstAndPrevButtons);
        $("#GoToPreviousImageButton").prop("disabled", disableFirstAndPrevButtons);
        $("#GoToNextImageButton").prop("disabled", disableLastAndNextButtons);
        $("#GoToLastImageButton").prop("disabled", disableLastAndNextButtons);

        $(".deleting").prop("disabled", false);
        $(".processing").prop("disabled", false);
        $("#HttpUploadButton").prop("disabled", false);
        $("#SaveImagedButton").prop("disabled", false);

        var size = _acquiredImageSizes[_focusedImageIndex];
        $("#PreviewImage").prop("src", "");
        $("#PreviewImage").prop("src", _acquiredImages[_focusedImageIndex]);
        $("#ImagesInfo").html("Image # " + (_focusedImageIndex + 1) + " from " + _acquiredImages.length + " images<br/>" + size.width + "x" + size.height);
    }
        // if there is no focused image
    else {

        $(".navigation").prop("disabled", true);
        $(".deleting").prop("disabled", true);
        $(".processing").prop("disabled", true);
        $("#HttpUploadButton").prop("disabled", true);
        $("#HttpCancelButton").prop("disabled", true);
        $("#SaveImagedButton").prop("disabled", true);
        $("#PreviewImage").prop("src", "");
        $("#ImagesInfo").html("No Image");
    }
}

/**
 Returns value of selected radio button from the specified group.
*/
function GetSelectedRadioValueInGroup(radioGroupName) {
    return $("[name='" + radioGroupName + "']:checked").val();
}

/**
 Shows an error message.
*/
function __showErrorMessage(data) {
    var markup = "";
    if (data.errorMessage)
        markup = data.errorMessage;
    else {
        if (data.status === 0 || data.status === 404) {
            markup = 'VintaSoft Web TWAIN service is not found.<br />Please download VintaSoft Web TWAIN Service from this <a href="http://www.vintasoft.com/zip/VSWebTwainService1_0.zip">link</a>, manually install the service on your computer and refresh this web page.';
        }
        else if (data.responseJSON != undefined) {
            markup = data.responseJSON.ExceptionMessage;
        }
        else {
            markup = data.responseText;
        }
    }
    $("#errorMessageDialogContent").html(markup);
    $("#errorMessageDialog").css("display", "block");
    if (_devices != undefined)
        $("#ScanImageButton").prop("disabled", false);
}

/**
 Closes the dialog with error message.
*/
function __closeErrorMessageDialog() {
    $("#errorMessageDialog").css("display", "none");
    $("#errorMessageDialogContent").html("");
}

// ===========================================================


function __pageUnload(e) {
    if (_twainDeviceManager != undefined && _twainDeviceManager.get_IsOpened())
        _twainDeviceManager.close();
}
