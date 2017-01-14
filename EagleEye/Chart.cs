using System.Collections.Generic;
using EagleEye.GVizApi;

namespace EagleEye
{
    /// <summary>
    /// JSON payload format used to update chart datatable to EagleEye platform.
    /// https://github.com/CVBDL/EagleEye-Docs/blob/master/rest-api/rest-api.md#edit-data-table
    /// {
    ///     "datatable": [
    ///         ["Month", "Count"],
    ///         ["2016-01", 20],
    ///         ["2016-02", 30],
    ///         ["2016-03", 25]
    ///     ]
    /// }
    /// </summary>
    internal class Chart
    {
        public List<List<object>> datatable { get; set; }

        public Chart(List<List<object>> dt = null)
        {
            datatable = dt;
        }

        public Chart(DataTable dt)
        {
            datatable = dt.GetData();
        }
    }
}
