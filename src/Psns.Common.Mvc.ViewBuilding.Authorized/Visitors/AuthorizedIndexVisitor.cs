using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Psns.Common.Mvc.ViewBuilding.ViewModels;
using Psns.Common.Mvc.ViewBuilding.ViewModels.TableModel;

using Microsoft.AspNet.Identity;

using Psns.Common.Mvc.ViewBuilding.Authorized.Attributes;

namespace Psns.Common.Mvc.ViewBuilding.Authorized.Visitors
{
    /// <summary>
    /// A visitor for the IndexView model
    /// </summary>
    /// <typeparam name="TUser"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TEntity"></typeparam>
    public class AuthorizedIndexVisitor<TUser, TKey, TEntity> : IIndexViewVisitor
        where TUser : class, IUser<TKey>
        where TKey : IEquatable<TKey>
    {
        ICrudUserStore<TUser, TKey> _userStore;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="userStore"></param>
        /// <param name="userManager"></param>
        public AuthorizedIndexVisitor(ICrudUserStore<TUser, TKey> userStore)
        {
            _userStore = userStore;
        }

        /// <summary>
        /// If TEntity is decorated with CrudAuthorizeAttribute and AccessType.Create is specified and Current User is not in
        /// the given Roles, then the CreateButton of the view is set to null.
        /// </summary>
        /// <param name="view"></param>
        public void Visit(IndexView view)
        {
            var authorizeAttribute = (typeof(TEntity).GetCustomAttributes(typeof(CrudAuthorizeAttribute), false) as CrudAuthorizeAttribute[])
                .Where(attribute => attribute.AccessType == AccessType.Create).SingleOrDefault();

            if(authorizeAttribute != null)
            {
                using(var userManager = new UserManager<TUser, TKey>(_userStore))
                {
                    foreach(var roleName in authorizeAttribute.RoleNames)
                    {
                        if(userManager.IsInRole(_userStore.CurrentUser.Id, roleName))
                            return;
                    }
                }
            }

            view.CreateButton = null;
        }

        /// <summary>
        /// No checking
        /// </summary>
        /// <param name="column"></param>
        public void Visit(Column column)
        {
            return;
        }

        /// <summary>
        /// No checking
        /// </summary>
        /// <param name="row"></param>
        public void Visit(Row row)
        {
            return;
        }

        /// <summary>
        /// Removes any rows in the table whose Source object returns permission denied for AccessType.Read.
        /// </summary>
        /// <param name="table"></param>
        public void Visit(Table table)
        {
            this.AuthorizeRows(_userStore, table);
        }
    }
}
