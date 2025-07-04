using Warpstone.Ini;

namespace Warpstone.Tests.Grammars;

public static class Ini
{
    [Fact]
    public static void Simple_parses()
    {
        var content = @"
k1=v1

[section1]
k2=v2
k3=v3

[section2]
k4=v4
";
        var result = IniParser.Parse(content);
    }
}
