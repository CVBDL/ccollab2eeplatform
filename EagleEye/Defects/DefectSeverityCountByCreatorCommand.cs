namespace EagleEye.Defects
{
    public class DefectSeverityCountByCreatorCommand : ICommand
    {
        private Defects _defects;

        public DefectSeverityCountByCreatorCommand(Defects defects)
        {
            _defects = defects;
        }

        public void Execute()
        {
            _defects.GenerateDefectSeverityCountByCreatorFromProduct();
        }
    }
}
