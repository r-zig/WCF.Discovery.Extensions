using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Discovery;

namespace Roniz.WCF.Discovery.Extenstions.Helpers
{
    static class FindCriteriaHelper
    {
        /// <summary>
        /// Check if 2 instances of FindCriteria are equal or not
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        internal static bool Equals(this FindCriteria first,FindCriteria second)
        {
            return first.GetHashCode().Equals(second.GetHashCode());
        }
    }
}
