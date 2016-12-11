using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EagleEye
{
    public interface IEagleEyeDataGenerator
    {
        /// <summary>
        /// The raw data will be processed.
        /// It could be a path to a file or string in JSON format.
        /// The detail format is decided by concrete implementation of EagleEyeDataGenerator class
        /// </summary>
        List<string[]> ReviewsRawData { get; }

        /// <summary>
        /// The raw data will be processed.
        /// It could be a path to a file or string in JSON format.
        /// The detail format is decided by concrete implementation of EagleEyeDataGenerator class
        /// </summary>
        List<string[]> DefectsRawData { get; }


        /// <summary>
        /// Execute the process
        /// </summary>
        /// <returns></returns>
        bool Execute();
    }
}
