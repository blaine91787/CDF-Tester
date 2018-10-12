using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDFTester
{
    public class DateModChangeException : Exception
    {
        public DateModChangeException()
            : base() { }

        public DateModChangeException(string message)
            : base(message) { }

        public DateModChangeException(string format, params object[] args)
            : base(string.Format(format, args)) { }

        public DateModChangeException(string message, Exception innerException)
            : base(message, innerException) { }

        public DateModChangeException(string format, Exception innerException, params object[] args)
            : base(string.Format(format, args), innerException) { }
    }
}
