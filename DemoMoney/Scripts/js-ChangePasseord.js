
document.addEventListener("DOMContentLoaded", function () {
    // CSS 選擇器 選擇ID為 btn的物件 宣告為 btn

    const currentpsw = document.getElementById("CurrentPasswordID")
    const newpsw = document.getElementById("NewpasswordID")
    const repeatnewpsw = document.getElementById("RepeatNewPasswordID")
    const btnpsw = document.getElementById("btnpsw")

    btnpsw.addEventListener("click", function () {

        var data = {
            currentpsw : currentpsw.value
            ,newpsw : newpsw.value
            ,repeatnewpsw : repeatnewpsw.value
        }
        $.ajax({
            url: "/Users/ChangePassword", //要執行的方法位置
            data: data, //要傳送的資料
            type: "post", //傳送的類型
            dataType: 'json', //返回資料的格式是json
            //如果正確傳送，並返回會執行什麼方法
            success: function (response) {
                if (response.Result) {
                    alert('密碼修改成功');
                    //top.location.href = "/DemoMoney/Index"
                } else {
                    alert('密碼修改失敗，'+response.Message);
                }
            },
            //如果錯誤，會返回什麼值，執行什麼方法
            error: function (errer) {
                alert('修改失敗' + response.Message);
            }
        });
    })
})