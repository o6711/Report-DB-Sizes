namespace ServiceLayer.DataBox
{
    public class Data
    {
        /// <summary>
        /// Pure example: a server
        /// </summary>
        public string Allocation { get; set; }

        /// <summary>
        /// Pure example: a data base
        /// </summary>
        public string SourceFileName { get; set; }

        /// <summary>
        /// Size of SourceFileName. Expected some double digit in GigaBytes
        /// </summary>
        public string Size { get; set; }
    }
}
