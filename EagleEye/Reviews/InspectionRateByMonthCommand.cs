namespace EagleEye.Reviews
{
    public class InspectionRateByMonthCommand : ICommand
    {
        private Reviews _reviews;

        public InspectionRateByMonthCommand(Reviews reviews)
        {
            _reviews = reviews;
        }

        public void Execute()
        {
            _reviews.GenerateAllInspectionRateByMonth("AllInspectionRateByMonth");
        }
    }
}
