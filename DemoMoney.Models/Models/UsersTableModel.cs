using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.ModelBinding;

namespace DemoMoney.Models.Models
{
    public class UsersTableModel
    {
        public int ID { get; set; }

        [DisplayName("帳號")]
        [Required(ErrorMessage = "必填欄位")]
        [BindRequired]

        public string Account { get; set; }

        [DisplayName("密碼")]
        [Required(ErrorMessage = "必填欄位")]
        [RegularExpression(@"^(?=.*\d)(?=.*[a-z])(?=.*[A-Z]).{6,16}$",ErrorMessage ="密碼包含大小寫英文和數字")]
        public string Password { get; set; }

        [DisplayName("姓")]
        [Required(ErrorMessage = "必填欄位")]
        public string Last_name { get; set; }

        [DisplayName("名")]
        [Required(ErrorMessage = "必填欄位")]
        public string First_name { get; set; }

        [Required(ErrorMessage = "必填欄位")]
        public string DeletOrNotUser { get; set; }

    }
}
