namespace EagleEye.Defects
{
    public class DefectCountByProductCommand : ICommand
    {
        private Defects _defects;

        public DefectCountByProductCommand(Defects defects)
        {
            _defects = defects;
        }

        public void Execute()
        {
            _defects.GenerateDefectCountByProduct("DefectCountByProduct");
        }
    }
}
