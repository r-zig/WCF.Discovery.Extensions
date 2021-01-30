using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace Roniz.WCF.Discovery.Extenstions.WcfService
{
    [ServiceContract]
    public interface IService2
    {
        [OperationContract]
        string GetData2(int value);
    }
}
