using System.Collections.Generic;
using ServiceLayer.DataBox;

namespace ServiceLayer
{
    public interface IGetData
    {
        public IEnumerable<Data> GetData();
    }
}
