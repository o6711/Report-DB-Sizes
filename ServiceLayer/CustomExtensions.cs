using ServiceLayer.DataBox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

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
