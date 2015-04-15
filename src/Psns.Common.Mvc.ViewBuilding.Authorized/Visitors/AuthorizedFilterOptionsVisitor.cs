using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.AspNet.Identity;

using Psns.Common.Mvc.ViewBuilding.ViewModels;
using Psns.Common.Persistence.Definitions;

namespace Psns.Common.Mvc.ViewBuilding.Authorized.Visitors
{
    /// <summary>
    /// A visitor for IIdentifiable objects used in the FilterOptions on the IndexView model
    /// </summary>
    /// <typeparam name="TUser"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    public class AuthorizedFilterOptionsVisitor<TUser, TKey> : IFilterOptionVisitor
        where TUser : class, IUser<TKey>
        where TKey : IEquatable<TKey>
    {
        ICrudUserStore<TUser, TKey> _userStore;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="userStore"></param>
        public AuthorizedFilterOptionsVisitor(ICrudUserStore<TUser, TKey> userStore)
        {
            _userStore = userStore;
        }

        /// <summary>
        /// Returns null if the item implements ISecurable and Demand returns false for Read access;
        /// else return the item.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public IIdentifiable Visit(IIdentifiable item)
        {
            if(item.PermissionDenied(_userStore, AccessType.Read))
                return null;
            else
                return item;
        }
    }
}
