using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Roniz.WCF.Discovery.Extenstions.WcfService
{
    public class Service2 : IService1, IService2
    {
        public string GetData(int value)
        {
            return string.Format("You entered: {0}", value);
        }

        public CompositeType GetDataUsingDataContract(CompositeType composite)
        {
            if (composite == null)
            {
                throw new ArgumentNullException("composite");
            }
            if (composite.BoolValue)
            {
                composite.StringValue += "Suffix";
            }
            return composite;
        }

        public string GetData2(int value)
        {
            return value.ToString();
        }
    }
}