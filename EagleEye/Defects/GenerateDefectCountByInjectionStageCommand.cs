namespace EagleEye.Defects
{
    public class GenerateDefectCountByInjectionStageCommand: ICommand
    {
        private Defects _defects;

        public GenerateDefectCountByInjectionStageCommand(Defects defects)
        {
            _defects = defects;
        }

        public void Execute()
        {
            _defects.GenerateDefectCountByInjectionStage();
        }
    }
}
