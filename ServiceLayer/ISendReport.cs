using System;
using System.Collections.Generic;
using System.Text;
using ServiceLayer.DataBox;

namespace ServiceLayer
{
    public interface ISendReport
    {
        public void SendReport(IEnumerable<Report> reports);
    }
}
