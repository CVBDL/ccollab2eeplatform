namespace EagleEye.Reviews
{
    public class DefectDensityChangedByMonthCommand : ICommand
    {
        private Reviews _reviews;

        public DefectDensityChangedByMonthCommand(Reviews reviews)
        {
            _reviews = reviews;
        }

        public void Execute()
        {
            _reviews.GenerateDefectDensityChangedByMonthFromProduct();
        }
    }
}
