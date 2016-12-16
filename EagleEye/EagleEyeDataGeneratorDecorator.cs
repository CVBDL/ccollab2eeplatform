using Ccollab;
using System.Collections.Generic;

namespace EagleEye
{
    public abstract class EagleEyeDataGeneratorDecorator: ICcollabDataSource
    {
        private ICcollabDataSource _ccollabDataGenerator;

        public EagleEyeDataGeneratorDecorator(ICcollabDataSource ccollabDataGenerator)
        {
            _ccollabDataGenerator = ccollabDataGenerator;
        }

        public List<string[]> GetReviewsRawData()
        {
            return _ccollabDataGenerator.GetReviewsRawData();
        }

        public List<string[]> GetDefectsRawData()
        {
            return _ccollabDataGenerator.GetDefectsRawData();
        }
    }
}
