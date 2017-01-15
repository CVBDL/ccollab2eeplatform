namespace EagleEye.Reviews
{
    public class DefectDensityChangedByProductCommand : ICommand
    {
        private Reviews _reviews;

        public DefectDensityChangedByProductCommand(Reviews reviews)
        {
            _reviews = reviews;
        }

        public void Execute()
        {
            _reviews.GenerateDefectDensityChangedByProduct();
        }
    }
}
