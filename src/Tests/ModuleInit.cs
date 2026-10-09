public static class ModuleInit
{
    #region Enable

    [ModuleInitializer]
    public static void Init() =>
        VerifySqlServer.Initialize();

    #endregion

    [ModuleInitializer]
    public static void InitOther()
    {
        #region UseSsimForPng

        VerifierSettings.UseSsimForPng();

        #endregion

        VerifierSettings.InitializePlugins();
    }
}