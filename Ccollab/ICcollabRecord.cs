namespace Ccollab
{
    public interface ICcollabRecord
    {
        string CreatorLogin { get; }
        string CreatorFullName { get; }

        /// <summary>
        /// A generated value. Reference to employee settings.
        /// For example, "ViewPoint".
        /// </summary>
        string CreatorProductName { get; }
        string ReviewCreationDate { get; }

        /// <summary>
        /// A generated value.
        /// E.g., raw creation date is "2016-09-30 23:33 UTC", expected value is "2016".
        /// </summary>
        string ReviewCreationYear { get; }

        /// <summary>
        /// A generated value.
        /// E.g., raw creation date is "2016-09-30 23:33 UTC", expected value is "09".
        /// </summary>
        string ReviewCreationMonth { get; }

        /// <summary>
        /// A generated value.
        /// E.g., raw creation date is "2016-09-30 23:33 UTC", expected value is "30".
        /// </summary>
        string ReviewCreationDay { get; }
    }
}
