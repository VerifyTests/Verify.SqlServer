namespace VerifyTests.SqlServer;

/// <summary>
/// The files produced when a schema is verified as a diagram. Can be combined.
/// </summary>
[Flags]
public enum DiagramFormat
{
    /// <summary>
    /// A Mermaid ER diagram in a markdown code block. Uses the `md` extension.
    /// </summary>
    Markdown = 1,

    /// <summary>
    /// The diagram rendered to an image. Uses the `svg` extension.
    /// </summary>
    Svg = 2,

    /// <summary>
    /// The diagram rendered to an image. Uses the `png` extension.
    /// </summary>
    Png = 4,

    All = Markdown | Svg | Png
}
