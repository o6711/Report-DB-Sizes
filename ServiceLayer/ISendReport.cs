using System.Collections.Generic;
using ServiceLayer.DataBox;

namespace ServiceLayer
{
    public interface ISendReport
    {
        public void SendReport(IEnumerable<Report> reports);
    }
}
