using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.AspNet.Identity;

namespace Psns.Common.Mvc.ViewBuilding.Authorized
{
    /// <summary>
    /// Defines a user store that provides a CurrentUser object
    /// </summary>
    /// <typeparam name="TUser"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    public interface ICrudUserStore<TUser, in TKey> : 
        IUserStore<TUser, TKey>, 
        IUserRoleStore<TUser, TKey> 
            where TUser : class, IUser<TKey> 
    {
        /// <summary>
        /// The TUser in context
        /// </summary>
        TUser CurrentUser { get; }
    }
}
