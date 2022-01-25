using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceAPI.Exceptions
{
    public class MaintenanceException : Exception
    {
        public MaintenanceException()
        {

        }
        public MaintenanceException(string message) : base(message)
        {

        }
        public MaintenanceException(string message, Exception ex) : base(message, ex)
        {

        }
    }
}
