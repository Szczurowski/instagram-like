using System;
using System.Collections.Generic;
using System.Text;

namespace Insta.Common
{
    public interface ILogger
    {
        void Error(Exception exception);
    }
}
