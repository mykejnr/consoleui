using ConsoleUI.Views.Components;
using ConsoleUI.Views.Extensions;

namespace ConsoleUI.Views;

/// <summary>
/// Constructs and renders a list of <see cref="ITableItem"/> in a
/// table to the console
/// to the console
/// </summary>
/// <typeparam name="T">
/// A type that implements <see cref="ITableItem"/>
/// </typeparam>
public class TableView<T>  where T : ITableItem
{
    /// <summary>
    /// Get a list of <see cref="TableColumn"/>s
    /// </summary>
    public List<TableColumn> Columns { get; private set; } = [];
    /// <summary>
    /// Get <see cref="TableBuilderOptions"/> used for configuring the table
    /// </summary>
    public TableBuilderOptions Options { get; private set; }
    /// <summary>
    /// Get a list of <see cref="ITableItem"/>s of type <typeparamref name="T"/>
    /// </summary>
    public List<T> Items { get; private set; }
    /// <summary>
    /// The computed table width
    /// </summary>
    public int TableWidth { get; private set; }
    /// <summary>
    /// List of table rows. Each row is a list of raw string representation of the cells
    /// </summary>
    public List<List<string>> Rows { get; private set; }

    private string HorizontalBorder { get; }
    private readonly List<string> _footer = [];
    private readonly string MARGIN_LEFT_STR;

    /// <summary>
    /// Create an instance of <see cref="TableView{T}"/>
    /// </summary>
    /// <param name="items"></param>
    /// <param name="options"></param>
    public TableView(List<T> items, TableBuilderOptions options)
    {
        Options = options;
        SetDefaultOptions();
        MARGIN_LEFT_STR = ' '.Repeat(options.Margin.Left);
        Items = items;
        Columns = GenerateColumns();
        Rows = GenerateRows();
        HorizontalBorder = DrawHorizontalBorder();
    }

    private void SetDefaultOptions()
    {
        Options.Title ??= "Untitled Table";
        Options.Margin ??= new() { Left = 2, Right = 2 };
        Options.CellPadding ??= new() { Left = 1, Right = 1 };
    }

    private List<TableColumn> GenerateColumns()
    {
        return Options.Columns.Select(name => new TableColumn
        { 
            Name = name,
            Width = name.Length + Options.CellPadding.Right + Options.CellPadding.Left,
        }).ToList();
    }

    /// <summary>
    /// Set alignment for a table column
    /// </summary>
    /// <param name="columnAt">Index of column</param>
    /// <param name="align">alignment of type <see cref="ColumnAlign"/></param>
    /// <returns>This object</returns>
    public TableView<T> Align(int columnAt, ColumnAlign align)
    {
        Columns.ElementAt(columnAt).Align = align;
        return this;
    }

    /// <summary>
    /// Add a text to be rendered at the bottom of the table
    /// <para>
    /// This method can be called multiple times. Items will be rendered
    /// in the order in which they were added
    /// </para>
    /// </summary>
    /// <param name="footerString"></param>
    /// <returns></returns>
    public TableView<T> AddFooter(string footerString)
    {
        _footer.Add(footerString);
        return this;
    }

    private List<List<string>> GenerateRows()
    {
        // Generate a list of "list of strings".
        // Each "list of strings" in the main list represent a table row
        // generate from "Items"
        // We also take advantage of the loop to compute column widths
        TableColumn col;
        var padding_x = Options.CellPadding.Left + Options.CellPadding.Right;
        var rows = new List<List<string>> { Capacity = Columns.Count };

        foreach (var item in Items)
        {
            var row = item.ToTableRow(); // Get a list of strings
            rows.Add(row);

            // For each item in the row, determine the new column widths
            for (int i = 0; i < row.Count; i++)
            {
                col = Columns[i];
                col.Width = Math.Max(col.Width,  row[i].Length + padding_x);
            }
        }
        // Compute total width of the table by summing up the column widths.
        // Outer table borders = +2
        // Inner table borders = Columns.Count - 1
        // :. +2 -1 = +1
        TableWidth = Columns.Sum(c => c.Width) + Columns.Count + 1;

        // what if the title itself is longer than all the column widths combined?
        // 1. we could try and spread the extra space over the individual column width or
        // 2. we just add the extra space to the last column
        // ...Let's go easy on ourself, and choose option 2
        var titleWidth = Options.Title.Length + padding_x;
        if (titleWidth > TableWidth)
        { 
            Columns.Last().Width += titleWidth - TableWidth;
            TableWidth = titleWidth;
        }

        return rows;
    }

    /// <summary>
    /// Render the table with the list of <see cref="ITableItem"/> provided
    /// </summary>
    /// <returns>
    /// The index of the selected row. Returns -1 if the table is closed
    /// without selecting any row.
    /// </returns>
    public int Render()
    {
        ConsoleUIApp.ClearScreen();
        DrawTableHead();

        // Save cursor positions, we will reset cursor position
        // on each key down/up press
        var left = Console.CursorLeft;
        var top = Console.CursorTop;
        var selection = 0;

        while (true)
        {
            Console.SetCursorPosition(left, top);
            DrawTableBody(selection);

            var key = Console.ReadKey(true).Key;
            switch (key)
            {
                case ConsoleKey.Enter:
                    // If there are no items in the table just close table
                    return Items.Count == 0 ? -1 : selection;
                case ConsoleKey.DownArrow:
                    selection += 1;
                    if (selection >= Items.Count)
                        selection = 0;
                    break;
                case ConsoleKey.UpArrow:
                    selection -= 1;
                    selection = Math.Max(0, selection);
                    break;
                case ConsoleKey.X:
                    return -1;
            }
        }
    }

    private void DrawTableHead()
    {
        var value = "[x] Close";
        Console.WriteLine(MARGIN_LEFT_STR + value.AlignRight(TableWidth));
        RenderTableHeader();
    }

    private void DrawTableBody(int selection)
    {
        DrawTableRows(selection);
        if (Items.Count > 0)
            Console.WriteLine(HorizontalBorder);

        RenderFooter();
    }

    private void RenderFooter()
    {
        foreach (var footer in _footer)
            Console.WriteLine(footer.PadLeft(Options.Margin.Left + TableWidth));

        Console.Write($"\n{MARGIN_LEFT_STR}*Number or records = {Items.Count} ");
    }

    private List<string>? _cachedRows;
    private void DrawTableRows(int selected)
    {
        _cachedRows ??= GenerateTableRows();
        var consoleForeColor = Console.ForegroundColor;
        string selectionMarker;

        foreach (var (row, idx) in _cachedRows.Select((x, i) => (x, i)))
        {
            selectionMarker = "  ";
            if (selected == idx)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                selectionMarker = " <";
            }
            Console.WriteLine(row + selectionMarker);
            Console.ForegroundColor = consoleForeColor;
        }
    }

    private List<string> GenerateTableRows()
    {
        return Rows.Select(row =>
        {
            var str = MARGIN_LEFT_STR + "|"; // Draw left margin + left border of the row
            for (var i = 0; i < row.Count; i++)
            {
                str += AlignCell(row[i], Columns[i]) + "|";
            }
            return str;
        }).ToList();
    }

    private void RenderTableHeader()
    {
        RenderTableTitle();
        Console.WriteLine(HorizontalBorder);

        var str = MARGIN_LEFT_STR + "|";

        foreach (var col in Columns)
            str += AlignCell(col.Name, col) + "|";

        Console.WriteLine(str); // Column Names
        Console.WriteLine(HorizontalBorder); // Bottom Bord
    }

    private void RenderTableTitle()
    {
        // Draw a straight line on top of the title bar
        // eg +--------------------------------+
        // -2 = left and right border (+ ... +)
        // because TableWidth accounts for the left and right border width
        var topBorder = $"{MARGIN_LEFT_STR}+{'-'.Repeat(TableWidth - 2)}+";
        Console.WriteLine(topBorder);

        var titleStr = Options.Title.AlignCenter(TableWidth - 2);
        titleStr = $"{MARGIN_LEFT_STR}|{titleStr}|";
        Console.WriteLine(titleStr);
    }

    private string DrawHorizontalBorder()
    {
        // Draw a horizontal line with column markings
        // eg. +-----+------+--------+
        var border = MARGIN_LEFT_STR + "+";
        foreach (var col in Columns)
            border += '-'.Repeat(col.Width) + "+" ;
        return border;
    }

    private string AlignCell(string value, TableColumn col)
    {
        return AlignCell(value, col.Width, col.Align);
    }

    private string AlignCell(string value, int width, ColumnAlign alignTo)
    {
        value = ' '.Repeat(Options.CellPadding.Left)
            + value + ' '.Repeat(Options.CellPadding.Right);

        return alignTo switch
        {
            ColumnAlign.Left => value.AlignLeft(width),
            ColumnAlign.Right => value.AlignRight(width),
            _ => value.AlignCenter(width),
        };
    }
}
