using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.AspNet.Identity;

namespace Psns.Common.Mvc.ViewBuilding.Authorized
{
    public interface ISecurable<TUser, in TKey> where TUser : IUser<TKey>
    {
        bool Demand(TUser user, AccessType accessType);
    }
}
