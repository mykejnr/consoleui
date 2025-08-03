namespace ConsoleUI.Views.Components;

/// <summary>
/// Represents paddings for a ui element
/// </summary>
public class Padding
{
    /// <summary>
    /// Number of white spaces to be used to the left
    /// </summary>
    public int Left { get; set; }
    /// <summary>
    /// Number of white spaces to be used to the right
    /// </summary>
    public int Right { get; set; }
    /// <summary>
    /// Number of white spaces to be used to the top
    /// </summary>
    public int Top { get; set; }
    /// <summary>
    /// Number of white spaces to be used to the bottom
    /// </summary>
    public int Bottom { get; set; }
}

/// <summary>
/// Represent margins for ui element
/// </summary>
public class Margin : Padding { }
