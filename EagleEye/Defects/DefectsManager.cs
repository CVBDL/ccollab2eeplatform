namespace EagleEye.Defects
{
    public class DefectsManager
    {
        ICommand _generateReviewCountByMonthCommand;

        public DefectsManager(ICommand generateReviewCountByMonthCommand)
        {
            _generateReviewCountByMonthCommand = generateReviewCountByMonthCommand;
        }

        public void GenerateReviewCountByMonth()
        {
            _generateReviewCountByMonthCommand.Execute();
        }
    }
}
