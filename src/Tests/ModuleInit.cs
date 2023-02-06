public static class ModuleInit
{
    #region enable

    [ModuleInitializer]
    public static void Init() =>
        VerifySqlServer.Initialize();

    #endregion

    [ModuleInitializer]
    public static void InitOther() =>
        VerifierSettings.InitializePlugins();
}