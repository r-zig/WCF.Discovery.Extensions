using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WCF.Discovery.Extensions.UnitTest.Mock
{
    class MockService : IMockServiceContract
    {
        #region IMockServiceContract Members

        public string Echo(string input)
        {
            return "echo " + input;
        }

        #endregion
    }
}
