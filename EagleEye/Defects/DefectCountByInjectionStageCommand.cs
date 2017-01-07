namespace EagleEye.Defects
{
    public class DefectCountByInjectionStageCommand : ICommand
    {
        private Defects _defects;

        public DefectCountByInjectionStageCommand(Defects defects)
        {
            _defects = defects;
        }

        public void Execute()
        {
            _defects.GenerateDefectCountByInjectionStageFromProduct();
        }
    }
}
