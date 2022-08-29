public static class ModuleInit
{
    #region enable

    [ModuleInitializer]
    public static void Init()
    {
        VerifySqlServer.Enable();

        #endregion

        VerifyDiffPlex.Initialize();
    }
}