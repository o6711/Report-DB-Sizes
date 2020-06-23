using System;
using System.Collections.Generic;
using System.Text;
using ServiceLayer.DataBox;

namespace ServiceLayer
{
    public interface IGetData
    {
        public IEnumerable<Data> GetData();
    }
}
