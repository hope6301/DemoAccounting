using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoMoney.Services
{
    public class ServiceResult<T>
    {
        private string _message;
        public Exception Exception { get; set; }
        public string KeyValue { get; set; }
        public string Message
        {
            get { return _message; }
            set
            {
                if ((value ?? "").Length > 255)
                {
                    value = value.Substring(0, 255);
                }
                _message = value;
            }
        }
        public T Result { get; set; }
        public string SessionID { get; set; }
        public ServiceStatus Status { get; set; }
        public int Total { get; set; }
    }
}
