using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXIAggregateRoots.Exceptions
{
    public class UnableToLoadMapException : Exception
    {

        public UnableToLoadMapException(string message = "Current map couldn't be unpacked and fetched.")
            : base(message)
        { }
    }
}
