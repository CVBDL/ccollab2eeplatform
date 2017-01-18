using System.Collections.Generic;

namespace EagleEye.GVizApi
{
    /// <summary>
    /// Each cell in the table is described by an object with the following properties:
    /// 
    /// - `v` [Optional]
    ///   The cell value.
    ///   The data type should match the column data type.
    ///   If the cell is null, the v property should be null, though it can still have f and p properties.
    ///   
    /// - `f` [Optional]
    ///   A string version of the `v` value, formatted for display.
    ///   
    /// - `p` [Optional]
    ///   An object that is a map of custom values applied to the cell.
    ///   These values can be of any JavaScript type.
    ///  
    /// Cells in the row array should be in the same order as their column descriptions in cols.
    /// To indicate a null cell, you can specify `null`.
    /// </summary>
    public class Cell
    {
        public object v { get; set; }
        public string f { get; set; }
        public Dictionary<string, object> p { get; set; }

        public Cell(object value = null, string format = null, Dictionary<string, object> props = null)
        {
            v = value;
            f = format;
            p = props;
        }
    }
}
