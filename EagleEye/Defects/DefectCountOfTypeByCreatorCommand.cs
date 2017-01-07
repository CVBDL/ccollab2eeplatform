namespace EagleEye.Defects
{
    public class DefectCountOfTypeByCreatorCommand : ICommand
    {
        private Defects _defects;

        public DefectCountOfTypeByCreatorCommand(Defects defects)
        {
            _defects = defects;
        }

        public void Execute()
        {
            _defects.GenerateDefectCountOfTypeByCreatorFromProduct();
        }
    }
}
