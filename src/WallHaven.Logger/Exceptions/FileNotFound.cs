namespace WallHaven.Logger.Exceptions
{
    public class FileNotFound : BaseException
    {
        public FileNotFound(string message = "") : base($"{message} not found")
        {
        }
    }
}
