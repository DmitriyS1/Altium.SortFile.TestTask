namespace Altium.SortingService.Models
{
    /// <summary>
    /// Represents data from file
    /// </summary>
    public class Line : IComparable<Line>
    {
        /// <summary>
        /// Number of the company
        /// </summary>
        public int SerialNumber { get; set; }

        /// <summary>
        /// Name of the company
        /// </summary>
        public string CompanyName { get; set; }

        public int CompareTo(Line? other)
        {
            var nameComparisonResult = this.CompanyName.CompareTo(other?.CompanyName);
            if (nameComparisonResult == 0)
            {
                return this.SerialNumber.CompareTo(other?.SerialNumber);
            }

            return nameComparisonResult;
        }
    }
}
