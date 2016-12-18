namespace EagleEye.Defects
{
    public class GenerateDefectSeverityByProductCommand : ICommand
    {
        private Defects _defects;

        public GenerateDefectSeverityByProductCommand(Defects defects)
        {
            _defects = defects;
        }

        public void Execute()
        {
            _defects.GenerateDefectSeverityByProduct();
        }
    }
}
