using ATIS.Strings;

namespace ATIS.Views;

public class SavingException : Exception
{
    public SavingException() : base(StringsRes.ErrorSaving)
    {
    }

    public SavingException(string message) : base(message)
    {
    }

    public SavingException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
public class DeletionException : Exception
{
    public DeletionException() : base(StringsRes.ErrorDeleting)
    {
    }

    public DeletionException(string message) : base(message)
    {
    }

    public DeletionException(string message,
        Exception innerException) : base(message, innerException)
    {
    }
}



