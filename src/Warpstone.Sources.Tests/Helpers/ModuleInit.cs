namespace Warpstone.Sources.Tests.Helpers;

internal static class ModuleInit
{
    [ModuleInitializer]
    public static void Init()
    {
        VerifierSettings.UseUtf8NoBom();
        VerifySourceGenerators.Initialize();
    }
}
