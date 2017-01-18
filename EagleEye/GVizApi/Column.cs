using System;
using System.Collections.Generic;

namespace EagleEye.GVizApi
{
    /// <summary>
    /// - `type` [Required]
    ///   Data type of the data in the column. Available values:
    ///   
    ///     * 'boolean'
    ///     * 'number'
    ///     * 'string'
    ///     * 'date'
    ///     * 'datetime'
    ///     * 'timeofday'
    ///     
    /// - `label` [Optional]
    ///   String value that some visualizations display for this column.
    ///   Example: `label:'Height'`
    ///   
    /// - `pattern` [Optional]
    ///   String pattern that was used by a data source to format numeric,
    ///   date, or time column values. 
    ///   
    /// - `id` [Optional]
    ///   String ID of the column. Must be unique in the table.
    ///   
    /// - `p` [Optional]
    ///   An object that is a map of custom values applied to the cell.
    /// </summary>
    public class Column
    {
        private readonly List<string> types = new List<string>
        {
            "string",
            "number",
            "boolean",
            "date",
            "datetime",
            "timeofday"
        };

        public string type { get; set; }
        public string label { get; set; }
        public string pattern { get; set; }
        public string id { get; set; }
        public Dictionary<string, object> p { get; set; }

        public Column(string type = "string", string label = null, string pattern = null, string id = null, Dictionary<string, object> props = null)
        {
            if (types.IndexOf(type) < 0)
            {
                throw new Exception(string.Format("Given data table column type is invalid: {0}", type));
            }

            this.type = type;
            this.label = label;
            this.pattern = pattern;
            this.id = id;
            this.p = props;
        }
    }
}
