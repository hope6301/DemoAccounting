using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoMoney.Models.Models
{
    public class ExcelModel
    {
        public int ID { get; set; }

        [DisplayName("日期")]
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
        [Required(ErrorMessage = "必填欄位")]
        public string DeleteOrNot { get; set; }
    }
}
