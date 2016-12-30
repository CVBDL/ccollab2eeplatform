namespace Ccollab
{
    /// <summary>
    /// Ref to ccollab-cmd.json
    /// </summary>
    public class CcollabCmd
    {
        public string Id { get; set; }
        public string CmdName { get; set; }
        public string Args { get; set; }
        public string ReviewsCreationDateLow { get; set; }
        public string ReviewsCreationDateHigh { get; set; }
        public string RelUrl { get; set; }
    }
}
