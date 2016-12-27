namespace EagleEye
{
    public class DensityStatistics
    {
        /// <summary>
        /// Comment count.
        /// </summary>
        public long TotalComments { get; set; } = 0;

        /// <summary>
        /// Defect count.
        /// </summary>
        public long TotalDefects { get; set; } = 0;

        /// <summary>
        /// "LOC" for short.
        /// </summary>
        public long LineOfCode { get; set; } = 0;

        /// <summary>
        /// "LOCC" for short.
        /// </summary>
        public long LineOfCodeChanged { get; set; } = 0;
    }
}
