namespace eeDataGenerator
{
    /// <summary>
    /// JSON payload format used to update chart datatable to EagleEye platform.
    /// https://github.com/CVBDL/EagleEye-Docs/blob/master/rest-api/rest-api.md#edit-data-table
    /// </summary>
    class Chart
    {
        public object[][] datatable { get; set; }

        public Chart(object[][] dt)
        {
            datatable = dt;
        }
    }
}
