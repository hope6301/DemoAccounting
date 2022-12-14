//------------------------------------------------------------------------------
// <auto-generated>
//     這個程式碼是由範本產生。
//
//     對這個檔案進行手動變更可能導致您的應用程式產生未預期的行為。
//     如果重新產生程式碼，將會覆寫對這個檔案的手動變更。
// </auto-generated>
//------------------------------------------------------------------------------

namespace DemoMoney.Models.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;

    public class DemoMoneyTable
    {
        public int ID { get; set; }

        [DisplayName("日期")]
        [Display(Name ="日期")]
        [Required(ErrorMessage = "必填欄位")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy/MM/dd}")]
        [DataType(DataType.Date)]
        //[DefaultValue(typeof(DateTime), DateTime.Now.ToString("yyyy/MM/dd"))]
        public System.DateTime date { get; set; } = DateTime.Now;

        [DisplayName("類別")]
        [Required(ErrorMessage = "必填欄位")]
        public string category { get; set; }

        [DisplayName("金額")]
        [Required(ErrorMessage = "必填欄位")]
        public int money { get; set; }

        [DisplayName("備註")]
        public string remark { get; set; }

        [DisplayName("收入或支出")]
        [Required(ErrorMessage = "必填欄位")]
        public string InAndOut { get; set; }

        [DisplayName("是否刪除")]
        public string DeleteOrNot { get; set; }

        [DisplayName("使用者名稱")]
        public string users { get; set; }
    }

    public class LietDemoMoneyTable : DemoMoneyTable
    {
        public List<DemoMoneyTable> listdemoMoneyTables { get; set; }
    }
}
