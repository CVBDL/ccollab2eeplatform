namespace EagleEye.Defects
{
    public class DefectCountOfSeverityByProductCommand : ICommand
    {
        private Defects _defects;

        public DefectCountOfSeverityByProductCommand(Defects defects)
        {
            _defects = defects;
        }

        public void Execute()
        {
            _defects.GenerateDefectCountOfSeverityByProduct("DefectCountBySeverity");
        }
    }
}
