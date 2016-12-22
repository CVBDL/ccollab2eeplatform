namespace EagleEye.Defects
{
    public class GenerateDefectCountByTypeCommand : ICommand
    {
        private Defects _defects;

        public GenerateDefectCountByTypeCommand(Defects defects)
        {
            _defects = defects;
        }

        public void Execute()
        {
            _defects.GenerateDefectCountByType();
        }
    }
}
