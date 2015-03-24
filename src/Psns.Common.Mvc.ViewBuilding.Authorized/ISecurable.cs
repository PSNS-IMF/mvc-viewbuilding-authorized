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

    public static class ISecurableExtensions
    {
        public static bool PermissionDenied<TUser, TKey>(this object subject,
            ICrudUserStore<TUser, TKey> userStore,
            AccessType accessType)
            where TUser : class, IUser<TKey>
            where TKey : IEquatable<TKey>
        {
            return (subject is ISecurable<TUser, TKey> &&
                !(subject as ISecurable<TUser, TKey>).Demand(userStore.CurrentUser, accessType));
        }
    }
}
