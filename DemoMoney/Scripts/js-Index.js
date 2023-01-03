
document.addEventListener("DOMContentLoaded", function () {

    var today = new Date();

    //今天的年月日
    var todaydd = today.getDate();
    var todaymm = today.getMonth() + 1;
    var todayyyy = today.getFullYear();

    //一個月前的月份
    var MonthAgomm = today.getMonth();

    //一年前的年
    var YearAgoyyyy = today.getFullYear() - 1;





    var dd = today.getDate();
    var mm = today.getMonth() + 1; //January is 0!
    var yyyy = today.getFullYear();

    //var mm = today.getMonth();
    //var yyyyend = today.getFullYear() - 1;

    //if (dd < 10) { dd = '0' + dd } if (mm < 10) { mm = '0' + mm } starttoday = yyyyend + '-' + mm + '-' + dd;
    //if (dd < 10) { dd = '0' + dd } if (mm < 10) { mm = '0' + mm } finishtoday = yyyy + '-' + mm + '-' + dd;

    //一年前的日期
    if (dd < 10) {
        dd = '0' + dd
    }
    if (todaymm < 10) {
        todaymm = '0' + todaymm
    }
    YearAgotoday = YearAgoyyyy + '-' + todaymm + '-' + todaydd;

    //一個月前的日期
    if (dd < 10) { dd = '0' + dd } if (MonthAgomm < 10) { MonthAgomm = '0' + MonthAgomm } MonthAgotoday = todayyyy + '-' + MonthAgomm + '-' + todaydd;

    //現在的日期
    if (dd < 10) { dd = '0' + dd } if (todaymm < 10) { todaymm = '0' + todaymm } today = todayyyy + '-' + todaymm + '-' + todaydd;

    $('#start').attr('MIN', YearAgotoday);
    $('#start').attr('value', MonthAgotoday);
    $('#start').attr('MAX', today);

    $('#finish').attr('MIN', YearAgotoday);
    $('#finish').attr('value', today);
    $('#finish').attr('MAX', today);

    const startdate = document.querySelector('#start')
    const finishdate = document.querySelector('#finish')


    document.querySelector('#btnName').addEventListener('click', function () {
        console.log(startdate.value)
        console.log(finishdate.value)
        var data = {
            startdatevalue: startdate.value,
            finishdatevalue: finishdate.value,
        };

        $.ajax({
            url: "/DemoMoney/Index", //要執行的方法位置
            data: data, //要傳送的資料
            type: "post", //傳送的類型
            dataType: 'json', //返回資料的格式是json
            success: function () {
            },
            //如果錯誤，會返回什麼值，執行什麼方法
            error: function () {

            }
        });

    })

    //下載按鈕被點擊，下載檔案
    document.querySelector('#download_excel').addEventListener('click', function () {
        $.ajax({
            url: '/DemoMoney/Button_Click',
            type: 'post',
            dataType: 'json',
            data: { sender: 'John', location: 123 },
            success: function (response) {
                if (response.Result) {
                    alert('Download Success!!' + response.Message);
                } else {
                    alert('DownloadError!!' + response.Message);
                }
            },
            error: function (errer) {
                alert('DownloadError!!');
            }
        });
    })

    // 先略過，測試用按鈕
    document.querySelector('#text_api').addEventListener('click', function () {

        console.log("text_api")
        $.ajax({
            url: '/DemoMoney/textapi',
            type: 'post',
            dataType: 'json',
            data: { sender: 'John', location: 123 },
            success: function (response) {
                if (response.Result) {
                    alert('Download Success!!' + response.Message);
                } else {
                    alert('DownloadError!!' + response.Message);
                }
            },
            error: function (errer) {
                alert('DownloadError!!');
            }
        });
    })

})
