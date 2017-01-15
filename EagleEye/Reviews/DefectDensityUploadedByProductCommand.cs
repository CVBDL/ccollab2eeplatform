namespace EagleEye.Reviews
{
    public class DefectDensityUploadedByProductCommand : ICommand
    {
        private Reviews _reviews;

        public DefectDensityUploadedByProductCommand(Reviews reviews)
        {
            _reviews = reviews;
        }

        public void Execute()
        {
            _reviews.GenerateDefectDensityUploadedByProduct();
        }
    }
}
