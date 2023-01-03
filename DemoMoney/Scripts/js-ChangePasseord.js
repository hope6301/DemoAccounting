
document.addEventListener("DOMContentLoaded", function () {
    // CSS 選擇器 選擇ID為 btn的物件 宣告為 btn

    const currentpsw = document.getElementById("CurrentPasswordID")
    const newpsw = document.getElementById("NewpasswordID")
    const repeatnewpsw = document.getElementById("RepeatNewPasswordID")
    const btnpsw = document.getElementById("btnpsw")
    const mseeagecurrent = document.getElementById("messageCurrent")
    const mseeageNew = document.getElementById("messageNew")
    const mseeageRepeatNew = document.getElementById("messageRepeatNew")


    const rule_1 = document.getElementById("rule_1")
    const rule_2 = document.getElementById("rule_2")
    const rule_3 = document.getElementById("rule_3")

    var checkCurrentpsw = false
    var checkNewpsw = false
    var checkRepeatNewpsw = false

    currentpsw.addEventListener("blur", function () {
        console.log("離開目前")
        let PSW_regex = new RegExp(/^(?=.*\d)(?=.*[a-z])(?=.*[A-Z]).{6,16}$/)

        if (PSW_regex.test(currentpsw.value)) {
            mseeagecurrent.textContent = "正確"
            checkCurrentpsw = true
        } else {
            mseeagecurrent.textContent = "錯誤"
            checkCurrentpsw = false
        }

    })

    newpsw.addEventListener("blur", function () {

        // 6 ~ 16 字中英文組成
        // 至少包含 1 個大寫英文 與 小寫英文
        // 至少包含 1 個數字
        let PSW_regex = new RegExp(/^(?=.*\d)(?=.*[a-z])(?=.*[A-Z]).{6,16}$/)

        //6-16 字組成
        let PSW_regex_1 = new RegExp(/.{6,16}/)
        //包含大小寫英文
        let PSW_regex_2 = new RegExp(/(?=.*[a-z])(?=.*[A-Z])/)
        //是不是有包含數字
        let PSW_regex_3 = new RegExp(/(?=.*\d)/)


        //驗證 新密碼是否符合格式 規則1 (6-16 字組成)
        if (PSW_regex_1.test(newpsw.value)) {
            rule_1.textContent = "1. 6-16字英文組成：正確"
        } else {
            rule_1.textContent = "1. 6-16字英文組成：錯誤"
        }

        //驗證 新密碼是否符合格式 規則2 (包含大小寫英文)
        if (PSW_regex_2.test(newpsw.value)) {
            rule_2.textContent = "2. 至少包含1個大寫英文與小寫英文：正確"
        } else {
            rule_2.textContent = "2. 至少包含1個大寫英文與小寫英文：錯誤"
        }
        //驗證 新密碼是否符合格式 規則 (是不是有包含數字)
        if (PSW_regex_3.test(newpsw.value)) {
            rule_3.textContent = "3. 至少包含1個數字：正確"
        } else {
            rule_3.textContent = "3. 至少包含1個數字：錯誤"
        }

        //新密碼符合格式 check 改為 true
        if (PSW_regex.test(newpsw.value)) {
            mseeageNew.textContent = null
            checkNewpsw = true
        }
        else {
            mseeageNew.textContent = "不符合資格"
            checkNewpsw = false
        }



        //驗證 確認新密碼 是否與 新密碼符合
        if (PSW_regex.test(repeatnewpsw.value)) {
            if (repeatnewpsw.value === newpsw.value) {
                mseeageRepeatNew.textContent = "確認新密碼：正確"
                checkRepeatNewpsw = true
            }
            else {
                mseeageRepeatNew.textContent = "確認新密碼：錯誤"
                checkRepeatNewpsw = false
            }
        }
        else {
            mseeageRepeatNew.textContent = "格式錯誤或不能空白"
            checkRepeatNewpsw = false
        }

    })

    //確認新密碼的 input 光標離開
    repeatnewpsw.addEventListener("blur", function () {
        let PSW_regex = new RegExp(/^(?=.*\d)(?=.*[a-z])(?=.*[A-Z]).{6,16}$/)

        if (PSW_regex.test(repeatnewpsw.value)) {
            if (repeatnewpsw.value === newpsw.value) {
                mseeageRepeatNew.textContent = "確認新密碼：正確"
                checkRepeatNewpsw = true
            }
            else {
                mseeageRepeatNew.textContent = "確認新密碼：錯誤"
                checkRepeatNewpsw = false
            }
        }
        else {
            mseeageRepeatNew.textContent = "格式錯誤或不能空白"
            checkRepeatNewpsw = false
        }
    })



    btnpsw.addEventListener("click", function () {


        console.log(checkCurrentpsw)
        console.log(checkNewpsw)
        console.log(checkRepeatNewpsw)

        if (checkCurrentpsw && checkNewpsw && checkRepeatNewpsw) {
            console.log("通過可傳送資料到後端")
            var data = {
                OldPassword: currentpsw.value
                , NewPassword: newpsw.value
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

                        localStorage.setItem("data", "kevin");
                        sessionStorage.setItem("data", "kevin");

                        top.location.href = "/DemoMoney/Index"
                    } else {
                        alert('密碼修改失敗，' + response.Message);
                    }
                },
                //如果錯誤，會返回什麼值，執行什麼方法
                error: function (errer) {
                    alert('修改失敗' + response.Message);
                }
            });

        } else {
            console.log("失敗 不可傳送")
        }
    })
})