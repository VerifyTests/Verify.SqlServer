using Verify.SqlServer;
using Xunit;

[GlobalSetUp]
public static class GlobalSetup
{
    public static void Setup()
    {
        #region Enable
        VerifySqlServer.Enable();
        #endregion
    }
}