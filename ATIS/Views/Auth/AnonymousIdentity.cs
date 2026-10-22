namespace ATIS.Views.Auth;

public class AnonymousIdentity : CustomIdentity
{
    public AnonymousIdentity() : base(string.Empty, string.Empty) { }
}
