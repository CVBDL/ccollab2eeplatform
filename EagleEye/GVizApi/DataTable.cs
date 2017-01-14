using System.Collections.Generic;

namespace EagleEye.GVizApi
{
    public class DataTable
    {
        private List<Column> cols = new List<Column>();
        private List<Row> rows = new List<Row>();

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

        /// <summary>
        /// [
        ///     ["Product", "Count"],
        ///     ["Team1", 20],
        ///     ["Team2", 10]
        /// ]
        /// </summary>
        /// <returns></returns>
        public List<List<object>> GetData()
        {
            List<List<object>> dataTable = new List<List<object>>();

            List<object> header = new List<object>();
            foreach (Column col in cols)
            {
                header.Add(col.Label);
            }

            dataTable.Add(header);

            foreach (Row row in rows)
            {
                dataTable.Add(row.Cells);
            }

            return dataTable;
        }
    }
}
