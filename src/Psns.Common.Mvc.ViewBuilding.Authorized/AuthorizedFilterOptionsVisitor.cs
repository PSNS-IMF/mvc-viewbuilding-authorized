using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.AspNet.Identity;

using Psns.Common.Mvc.ViewBuilding.ViewModels;
using Psns.Common.Persistence.Definitions;

namespace Psns.Common.Mvc.ViewBuilding.Authorized
{
    public class AuthorizedFilterOptionsVisitor<TUser, TKey> : IFilterOptionVisitor
        where TUser : class, IUser<TKey>
        where TKey : IEquatable<TKey>
    {
        ICrudUserStore<TUser, TKey> _userStore;

        public AuthorizedFilterOptionsVisitor(ICrudUserStore<TUser, TKey> userStore)
        {
            _userStore = userStore;
        }

        public IIdentifiable Visit(IIdentifiable item)
        {
            if(item.PermissionDenied(_userStore, AccessType.Read))
                return null;
            else
                return item;
        }
    }
}
