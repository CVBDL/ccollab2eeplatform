namespace EagleEye.Reviews
{
    public class InspectionRateByMonthFromProductCommand : ICommand
    {
        private Reviews _reviews;

        public InspectionRateByMonthFromProductCommand(Reviews reviews)
        {
            _reviews = reviews;
        }

        public void Execute()
        {
            _reviews.GenerateInspectionRateByMonthFromProduct();
        }
    }
}
