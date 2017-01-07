namespace EagleEye.Defects
{
    public class DefectSeverityCountByProductCommand : ICommand
    {
        private Defects _defects;

        public DefectSeverityCountByProductCommand(Defects defects)
        {
            _defects = defects;
        }

        public void Execute()
        {
            _defects.GenerateDefectSeverityCountByProduct("DefectCountBySeverity");
        }
    }
}
