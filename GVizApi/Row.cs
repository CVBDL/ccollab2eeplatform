using System.Collections.Generic;

namespace GVizApi
{
    /// <summary>
    /// Each row object has one required property called c, which is
    /// an array of cells in that row. It also has an optional p
    /// property that defines a map of arbitrary custom values to
    /// assign to the whole row.
    /// </summary>
    public class Row
    {
        public List<Cell> c { get; set; } = new List<Cell>();

        public Row(params object[] values)
        {
            foreach (var val in values)
            {
                Cell cell = new Cell(val);

                c.Add(cell);
            }
        }

        public void AddCell(Cell cell)
        {
            c.Add(cell);
        }
    }
}
