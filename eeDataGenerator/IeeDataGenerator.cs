using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eeDataGenerator
{
    public interface IeeDataGenerator
    {
        /// <summary>
        /// The raw data will be processed.
        /// It could be a path to a file or string in JSON format.
        /// The detail format is decided by concrete implementation of eeDataGenerator class
        /// </summary>
        List<string[]> ReviewsRawData { get; }

        /// <summary>
        /// The raw data will be processed.
        /// It could be a path to a file or string in JSON format.
        /// The detail format is decided by concrete implementation of eeDataGenerator class
        /// </summary>
        List<string[]> DefectsRawData { get; }


        /// <summary>
        /// Execute the process
        /// </summary>
        /// <param name="rawData">The raw data will be processed.
        /// It could be a path to a file or string in JSON format.
        /// The detail format is decided by concrete implementation of eeDataGenerator class</param>
        /// <returns></returns>
        bool Execute(List<string> rawData);
    }
}
