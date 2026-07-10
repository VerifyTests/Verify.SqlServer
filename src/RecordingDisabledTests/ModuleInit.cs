public static class ModuleInit
{
    [ModuleInitializer]
    public static void Init() =>
        VerifySqlServer.Initialize(recordCommands: false);
}
