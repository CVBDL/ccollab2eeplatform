namespace EagleEye.Defects
{
    public class DefectCountOfSeverityByCreatorCommand : ICommand
    {
        private Defects _defects;

        public DefectCountOfSeverityByCreatorCommand(Defects defects)
        {
            _defects = defects;
        }

        public void Execute()
        {
            _defects.GenerateDefectCountOfSeverityByCreatorFromProduct();
        }
    }
}
