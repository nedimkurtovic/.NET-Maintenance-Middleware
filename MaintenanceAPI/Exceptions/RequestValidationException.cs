﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceAPI.Exceptions
{
    public class RequestValidationException : Exception
    {
        public RequestValidationException()
        {

        }
        public RequestValidationException(string message) : base(message)
        {

        }
        public RequestValidationException(string message, Exception ex) : base(message, ex)
        {

        }
    }
}
