using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlorriJob.Persistence.Exceptions
{
    public class InvalidPageArgumentException : Exception
    {
        public InvalidPageArgumentException(string message) : base(message)
        {
            
        }
    }
}
