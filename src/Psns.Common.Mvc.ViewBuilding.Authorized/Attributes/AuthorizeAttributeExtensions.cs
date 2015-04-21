using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Reflection;

using Microsoft.AspNet.Identity;

namespace Psns.Common.Mvc.ViewBuilding.Authorized.Attributes
{
    public static class AuthorizeAttributeExtensions
    {
        public static bool CurrentUserHasAccess<TUser, TKey>(this PropertyInfo propertyInfo,
            AccessType accessType,
            ICrudUserStore<TUser, TKey> userStore)
            where TUser : class, IUser<TKey>
            where TKey : IEquatable<TKey>
        {
            var authorizeAttribute = (propertyInfo.GetCustomAttributes(typeof(CrudAuthorizeAttribute), false) as
                CrudAuthorizeAttribute[])
                    .Where(a => a.AccessType == accessType)
                    .SingleOrDefault();

            if(authorizeAttribute == null)
                return true;

            using(var userManager = new UserManager<TUser, TKey>(userStore))
            {
                foreach(var roleName in authorizeAttribute.RoleNames)
                {
                    if(userManager.IsInRole(userStore.CurrentUser.Id, roleName))
                        return true;
                }
            }

            return false;
        }
    }
}
