using System.Collections.Generic;

namespace Ccollab
{
    public interface ICcollabDataSource
    {
        /// <summary>
        /// The raw data will be processed.
        /// It could be a path to a file or string in JSON format.
        /// The detail format is decided by concrete implementation of EagleEyeDataGenerator class
        /// </summary>
        List<ReviewRecord> GetReviewsRawData();

        /// <summary>
        /// The raw data will be processed.
        /// It could be a path to a file or string in JSON format.
        /// The detail format is decided by concrete implementation of EagleEyeDataGenerator class
        /// </summary>
        List<DefectRecord> GetDefectsRawData();
    }
}
