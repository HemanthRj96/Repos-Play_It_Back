namespace FFG.Message.Internal
{
    public enum EMessageExecutionEnd
    {
        DoNothing,
        ExecuteAnotherMessage,
        DestroySelf,
    }
}