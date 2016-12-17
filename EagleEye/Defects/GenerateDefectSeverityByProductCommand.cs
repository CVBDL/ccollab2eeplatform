using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EagleEye.Defects
{
    public class GenerateDefectSeverityByProductCommand : ICommand
    {
        private Defects _defectsDataGenerator;

        public GenerateDefectSeverityByProductCommand(Defects reviewsDataGenerator)
        {
            _defectsDataGenerator = reviewsDataGenerator;
        }

        public void Execute()
        {
            _defectsDataGenerator.GenerateDefectSeverityByProduct();
        }
    }
}
