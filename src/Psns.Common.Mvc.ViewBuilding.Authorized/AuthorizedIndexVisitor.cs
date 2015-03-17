using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Psns.Common.Mvc.ViewBuilding.ViewModels;
using Psns.Common.Mvc.ViewBuilding.ViewModels.TableModel;

using Microsoft.AspNet.Identity;

using Psns.Common.Mvc.ViewBuilding.Authorized.Attributes;

namespace Psns.Common.Mvc.ViewBuilding.Authorized
{
    public class AuthorizedIndexVisitor<TUser, TKey, TEntity> : IIndexViewVisitor
        where TUser : class, IUser<TKey>
        where TKey : IEquatable<TKey>
    {
        ICrudUserStore<TUser, TKey> _userStore;
        UserManager<TUser, TKey> _userManager;

        public AuthorizedIndexVisitor(ICrudUserStore<TUser, TKey> userStore, UserManager<TUser, TKey> userManager)
        {
            _userStore = userStore;
            _userManager = userManager;
        }

        public void Visit(IndexView view)
        {
            var authorizeAttribute = (typeof(TEntity).GetCustomAttributes(typeof(CrudAuthorizeAttribute), false) as CrudAuthorizeAttribute[])
                .Where(attribute => attribute.AccessType == AccessType.Create).SingleOrDefault();

            if(authorizeAttribute != null)
            {
                foreach(var roleName in authorizeAttribute.RoleNames)
                {
                    if(_userManager.IsInRole(_userStore.CurrentUser.Id, roleName))
                        return;
                }
            }

            view.CreateButton = null;
        }

        public void Visit(Column column)
        {
            return;
        }

        public void Visit(Row row)
        {
            return;
        }

        public void Visit(Table table)
        {
            for(int i = 0; i < table.Rows.Count; i++)
            {
                var row = table.Rows[i];

                if(row.Source is ISecurable<TUser, TKey> &&
                    !(row.Source as ISecurable<TUser, TKey>).Demand(_userStore.CurrentUser, AccessType.Read))
                {
                    table.Rows.Remove(row);
                }
            }
        }
    }
}
