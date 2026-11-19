namespace Warpstone.Tests.Parsers;

public static class StringParser
{
    [TestCaseSource(typeof(TestOptions), nameof(TestOptions.All))]
    public static void Is_case_sensitive_by_default(ParseOptions options)
        => String("abc").TryParse("ABC", options).Success.Should().BeFalse();

    // In Turkish, 'I' is the upper case of 'ı', not of 'i'. An ordinal comparison doesn't care about the current culture.
    [SetCulture("tr-TR")]
    [TestCaseSource(typeof(TestOptions), nameof(TestOptions.All))]
    public static void Ignores_case_regardless_of_the_current_culture(ParseOptions options)
        => String("i", ignoreCase: true).Parse("I", options).Should().Be("I");

    [TestCaseSource(typeof(TestOptions), nameof(TestOptions.All))]
    public static void Culture_overload_still_compares_by_culture(ParseOptions options)
        => String("i", ignoreCase: true, CultureInfo.GetCultureInfo("tr-TR")).TryParse("I", options).Success.Should().BeFalse();
}
