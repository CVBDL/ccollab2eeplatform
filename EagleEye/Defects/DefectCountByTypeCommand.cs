namespace EagleEye.Defects
{
    public class DefectCountByTypeCommand : ICommand
    {
        private Defects _defects;

        public DefectCountByTypeCommand(Defects defects)
        {
            _defects = defects;
        }

        public void Execute()
        {
            _defects.GenerateDefectCountByTypeFromProduct();
        }
    }
}
