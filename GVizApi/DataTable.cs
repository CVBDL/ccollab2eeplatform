using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace GVizApi
{
    /// <summary>
    /// The data object consists of two required top-level properties,
    /// `cols` and `rows`, and an optional `p` property that is a map of 
    /// arbitrary values.
    /// </summary>
    public class DataTable
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(DataTable));

        public List<Column> cols { get; set; } = new List<Column>();
        public List<Row> rows { get; set; } = new List<Row>();
        
        /// <summary>
        /// Adds a new column to the data table, and returns the index of the new column.
        /// </summary>
        /// <param name="col"></param>
        /// <returns>Index of the new column</returns>
        public int AddColumn(Column col)
        {
            cols.Add(col);

            return cols.IndexOf(col);
        }

        public int AddRow(Row row)
        {
            rows.Add(row);

            return rows.IndexOf(row);
        }

        public string ToJSON()
        {
            string json = string.Empty;
            try
            {
                json = JsonConvert.SerializeObject(this);
            }
            catch (Exception e)
            {
                log.Error(string.Format("Unable to serialize data table to JSON. Message: {0}", e.Message));
            }

            return json;
        }
    }
}
