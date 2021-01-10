using System;

namespace WallHaven.Logger
{
    public class BaseException : Exception
    {
        public BaseException(string message="") : base(message)
        {

        }
    }
}
