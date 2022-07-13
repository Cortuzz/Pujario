namespace Pujario.Exceptions.Core
{
    public class UnknownIdentifier : System.Exception
    {
        UnknownIdentifier(string message = null) : base(message) {}
    }
}