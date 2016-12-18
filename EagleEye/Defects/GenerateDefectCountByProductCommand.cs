namespace EagleEye.Defects
{
    public class GenerateDefectCountByProductCommand : ICommand
    {
        private Defects _defects;

        public GenerateDefectCountByProductCommand(Defects defects)
        {
            _defects = defects;
        }

        public void Execute()
        {
            _defects.GenerateDefectCountByProduct();
        }
    }
}
