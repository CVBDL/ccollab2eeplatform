namespace EagleEye.Defects
{
    public class DefectCountBySeverityCommand : ICommand
    {
        private Defects _defects;

        public DefectCountBySeverityCommand(Defects defects)
        {
            _defects = defects;
        }

        public void Execute()
        {
            _defects.GenerateDefectCountBySeverity();
        }
    }
}
