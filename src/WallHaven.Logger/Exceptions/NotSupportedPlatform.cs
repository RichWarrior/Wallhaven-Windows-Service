namespace WallHaven.Logger.Exceptions
{
    public class NotSupportedPlatform : BaseException
    {
        public NotSupportedPlatform(string message = "") : base("Not Supported Platform")
        {
        }
    }
}
