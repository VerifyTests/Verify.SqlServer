public static class ModuleInit
{
    #region DisableRecording

    [ModuleInitializer]
    public static void Init() =>
        VerifySqlServer.Initialize(recordCommands: false);

    #endregion
}
