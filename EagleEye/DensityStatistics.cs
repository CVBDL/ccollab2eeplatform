namespace EagleEye
{
    public class DensityStatistics
    {
        /// <summary>
        /// Comment count.
        /// </summary>
        public int TotalCommentCount { get; set; } = 0;

        /// <summary>
        /// Defect count.
        /// </summary>
        public int TotalDefectCount { get; set; } = 0;

        /// <summary>
        /// "LOC" for short.
        /// </summary>
        public int TotalLOC { get; set; } = 0;

        /// <summary>
        /// "LOCC" for short.
        /// </summary>
        public int TotalLOCChanged { get; set; } = 0;

        /// <summary>
        /// Total Person-Time
        /// </summary>
        public double TotalPersonTimeInSecond { get; set; } = 0;
    }
}
