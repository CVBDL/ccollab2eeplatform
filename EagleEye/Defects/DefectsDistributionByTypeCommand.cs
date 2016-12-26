namespace EagleEye.Defects
{
    public class DefectsDistributionByTypeCommand : ICommand
    {
        private Defects _defects;

        public DefectsDistributionByTypeCommand(Defects defects)
        {
            _defects = defects;
        }

        public void Execute()
        {
            _defects.GenerateDefectsDistributionByType("DefectsDistributionByType");
        }
    }
}
