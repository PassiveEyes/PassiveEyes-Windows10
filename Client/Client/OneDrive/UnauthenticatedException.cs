namespace Client.OneDrive
{
    using System;

    public class UnauthenticatedException : Exception
    {
        public UnauthenticatedException() : base("Do you need to get a new authorization token?") { }
    }
}
