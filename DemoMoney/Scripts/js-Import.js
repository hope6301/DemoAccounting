document.addEventListener("DOMContentLoaded", function () {

    document.querySelector('#btnSave').addEventListener('click', function () {
        console.log("123")
        if (window.FormData !== undefined) {
            var fileUpload = $("#uploadBtn").get(0);
            var files = fileUpload.files;
            var fileData = new FormData();

            // 取得上傳的檔案
            for (var i = 0; i < files.length; i++) {
                fileData.append(files[i].name, files[i]);
            }

            // 將檔案POST至後端
            $.ajax({
                url: '/DemoMoney/UploadUserList',
                type: "POST",
                contentType: false, // Not to set any content header
                processData: false, // Not to process data
                data: fileData,
                success: function (result) {
                    alert(result);
                },
                error: function (err) {
                    alert(err.statusText);
                }
            });
        } else {
            alert("FormData is not supported.");
        }
    })

});