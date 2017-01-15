namespace EagleEye.Defects
{
    public class DefectCountOfTypeByProductCommand : ICommand
    {
        private Defects _defects;

        public DefectCountOfTypeByProductCommand(Defects defects)
        {
            _defects = defects;
        }

        public void Execute()
        {
            _defects.GenerateDefectCountOfTypeByProduct();
        }
    }
}
