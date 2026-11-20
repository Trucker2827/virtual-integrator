namespace VirtualIntegrator.Lon.OpenLdv;

public class OpenLdvException(LdvErr code, string message) : Exception($"{message} (ldv_err={code})")
{
    public LdvErr Code { get; } = code;

    internal static void ThrowIfError(LdvErr code, string message)
    {
        if (code != LdvErr.Ok)
        {
            throw new OpenLdvException(code, message);
        }
    }
}
