using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;

namespace WCF.Discovery.Extensions.UnitTest
{
    [ServiceContract]
    interface IMockServiceContract
    {
        [OperationContract]
        string Echo(string input);
    }
}
