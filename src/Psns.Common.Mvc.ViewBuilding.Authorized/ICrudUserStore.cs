using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.AspNet.Identity;

namespace Psns.Common.Mvc.ViewBuilding.Authorized
{
    public interface ICrudUserStore<TUser, in TKey> : 
        IUserStore<TUser, TKey>, 
        IUserRoleStore<TUser, TKey> 
            where TUser : class, IUser<TKey> 
    {
        TUser CurrentUser { get; }
    }
}
