using System;

namespace ServiceLayer.DataBox
{
    public class Report
    {
        public Report(Data data)
        {
            Data = data;
            date = DateTime.Now;
        }
        private DateTime date;
        public Data Data { get; set; }
        public string Date { get { return $"{date.ToShortDateString()} ({date.ToShortTimeString()})"; } }
    }
}
