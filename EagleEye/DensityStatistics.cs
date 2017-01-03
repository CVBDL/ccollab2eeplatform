namespace EagleEye
{
    public class DensityStatistics
    {
        /// <summary>
        /// Comment count.
        /// </summary>
        public long TotalCommentCount { get; set; } = 0;

        /// <summary>
        /// Defect count.
        /// </summary>
        public long TotalDefectCount { get; set; } = 0;

        /// <summary>
        /// "LOC" for short.
        /// </summary>
        public long TotalLOC { get; set; } = 0;

        /// <summary>
        /// "LOCC" for short.
        /// </summary>
        public long TotalLOCChanged { get; set; } = 0;
    }
}
