using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EagleEye
{
    public abstract class EagleEyeDataGeneratorDecorator: IEagleEyeDataGenerator
    {
        private IEagleEyeDataGenerator _ccollabDataGenerator;

        public List<string[]>  ReviewsRawData
        {
            get
            {
                return _ccollabDataGenerator.ReviewsRawData;
            }
        }

        public List<string[]> DefectsRawData
        {
            get
            {
                return _ccollabDataGenerator.DefectsRawData;
            }
        }

        public EagleEyeDataGeneratorDecorator(IEagleEyeDataGenerator ccollabDataGenerator)
        {
            _ccollabDataGenerator = ccollabDataGenerator;
        }

        public bool Execute()
        {
            return _ccollabDataGenerator.Execute();
        }


    }
}
