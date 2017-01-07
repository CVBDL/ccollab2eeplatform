namespace EagleEye.Defects
{
    public class DefectCountByCreatorCommand : ICommand
    {
        private Defects _defects;

        public DefectCountByCreatorCommand(Defects defects)
        {
            _defects = defects;
        }

        public void Execute()
        {
            _defects.GenerateDefectCountByCreatorFromProduct();
        }
    }
}
