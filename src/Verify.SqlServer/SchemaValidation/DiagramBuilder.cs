#if NET10_0_OR_GREATER
using DbToMermaid;

static class DiagramBuilder
{
    public static async Task<ConversionResult> Build(SqlConnection connection, DiagramFormat formats)
    {
        var targets = new List<Target>();

        if (formats.HasFlag(DiagramFormat.Markdown))
        {
            var markdown = await SqlServerToMermaid.RenderMarkdown(connection);
            targets.Add(new("md", markdown));
        }

        if (formats.HasFlag(DiagramFormat.Svg))
        {
            var svg = await SqlServerToMermaid.RenderSvg(connection);
            targets.Add(new("svg", svg));
        }

        if (formats.HasFlag(DiagramFormat.Png))
        {
            var png = await SqlServerToMermaid.RenderPng(connection);
            targets.Add(new("png", new MemoryStream(png)));
        }

        return new(null, targets);
    }
}
#endif
