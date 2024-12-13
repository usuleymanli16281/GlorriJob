﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlorriJob.Persistence.Exceptions
{
    public class PageOutOfRangeException : Exception
    {
        public PageOutOfRangeException(string message) : base(message)
        {

        }
    }
}