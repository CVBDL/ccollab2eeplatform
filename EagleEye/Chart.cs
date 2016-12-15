using System.Collections.Generic;

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
    public class Chart
    {
        public List<List<object>> datatable { get; set; }
    }
}
