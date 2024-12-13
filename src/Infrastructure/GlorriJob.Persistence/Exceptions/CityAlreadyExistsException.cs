namespace GlorriJob.Persistence.Exceptions;

public class CityAlreadyExistsException : Exception
{
    public CityAlreadyExistsException(string message) : base(message)
    {

    }
}
