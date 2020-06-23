using ServiceLayer.DataBox;
using System.Collections.Generic;
using System.Linq;

namespace ServiceLayer
{
    public static class CustomExtensions
    {
        public static IEnumerable<Report> ToReport(this IEnumerable<Data> data)
        {
            return data.Select(entity => new Report(entity));
        }
    }
}
