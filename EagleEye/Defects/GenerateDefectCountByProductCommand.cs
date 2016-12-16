namespace EagleEye.Defects
{
    public class GenerateDefectCountByProductCommand : ICommand
    {
        private Defects _defectsDataGenerator;

        public GenerateDefectCountByProductCommand(Defects reviewsDataGenerator)
        {
            _defectsDataGenerator = reviewsDataGenerator;
        }

        public void Execute()
        {
            _defectsDataGenerator.GenerateDefectCountByProduct();
        }
    }
}
