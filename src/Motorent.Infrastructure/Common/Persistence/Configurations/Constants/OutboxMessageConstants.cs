namespace Motorent.Infrastructure.Common.Persistence.Configurations.Constants;

internal static class OutboxMessageConstants
{
    public const string TableName = "outbox_messages";
    
    public const int TypeMaxLength = 256;
    
    public const int DataMaxLength = 8192;
    
    public const int ErrorTypeMaxLength = 50;
    
    public const int ErrorMessageMaxLength = 100;
    
    public const int ErrorDetailsMaxLength = 2048;
    
    public const int StatusMaxLength = 20;
}