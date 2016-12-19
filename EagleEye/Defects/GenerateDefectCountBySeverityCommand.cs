namespace EagleEye.Defects
{
    public class GenerateDefectCountBySeverityCommand : ICommand
    {
        private Defects _defects;

        public GenerateDefectCountBySeverityCommand(Defects defects)
        {
            _defects = defects;
        }

        public void Execute()
        {
            _defects.GenerateDefectCountBySeverity();
        }
    }
}
