namespace EagleEye.Defects
{
    public class DefectCountOfTypeByProduct : ICommand
    {
        private Defects _defects;

        public DefectCountOfTypeByProduct(Defects defects)
        {
            _defects = defects;
        }

        public void Execute()
        {
            _defects.GenerateDefectCountOfTypeByProduct("DefectsDistributionByType");
        }
    }
}
