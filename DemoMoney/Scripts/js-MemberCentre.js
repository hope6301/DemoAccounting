
document.addEventListener("DOMContentLoaded", function () {
    // CSS 選擇器 選擇ID為 btn的物件 宣告為 btn
    const btn = document.querySelector("#button-Edit")

    const Cancel = document.querySelector("#button-Cancel")
    const account = document.querySelector("#AccountID")
    const LastName = document.getElementById("Last_nameID")
    const FirsrName = document.getElementById("First_nameID")
    const pas = document.getElementById("pas")

    // 監聽btn 這個物件 是否被點擊
    btn.addEventListener("click", function () {

        console.log(btn.textContent)

        if (btn.textContent === "儲存") {
            var data = {
                Account: account.value,
                Last_name: LastName.value,
                First_name: FirsrName.value
            }
            $.ajax({
                url: "/Users/EditUsers", //要執行的方法位置
                data: data, //要傳送的資料
                type: "post", //傳送的類型
                dataType: 'json', //返回資料的格式是json
                //如果正確傳送，並返回會執行什麼方法
                success: function (response) {
                    if (response.Result) {
                        alert('修改成功');
                        top.location.href = "/DemoMoney/Index"
                    } else {
                        alert('修改成功123');
                    }
                    btn.textContent = "編輯資料"
                    LastName.disabled = true
                    FirsrName.disabled = true
                },
                //如果錯誤，會返回什麼值，執行什麼方法
                error: function (errer) {
                    alert('修改失敗');
                    btn.textContent = "編輯資料"
                    LastName.disabled = true
                    FirsrName.disabled = true
                }
            });

        }
        else
        {
            console.log("按下去")
            btn.textContent = "儲存"
            LastName.disabled = false
            FirsrName.disabled = false
        }

    })

})
