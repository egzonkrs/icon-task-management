using FluentResults;

namespace Icon.SharedKernel.Common;

public class CustomFluentError : Error
{
    private const string MetadataCodeKey = "Code";
    private const string MetadataReasonKey = "Reason";
    public string Code { get; }

    public CustomFluentError(string code, string message) : base(message)
    {
        Code = code;
        Metadata.Add(MetadataCodeKey, code);
        Metadata.Add(MetadataReasonKey, message);
    }

    public static string GetErrorCode(IError error)
    {
        return error.Metadata.TryGetValue(MetadataCodeKey, out var code) ? code.ToString()! : "UNKNOWN";
    }

    public static string GetErrorReason(IError error)
    {
        return error.Metadata.TryGetValue(MetadataReasonKey, out var reason) ? reason.ToString()! : error.Message;
    }
}
