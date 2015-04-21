using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Psns.Common.Mvc.ViewBuilding.ViewModels;
using Psns.Common.Mvc.ViewBuilding.ViewModels.TableModel;

using Microsoft.AspNet.Identity;

namespace Psns.Common.Mvc.ViewBuilding.Authorized.Visitors
{
    /// <summary>
    /// A visitor for the Details view model.
    /// </summary>
    /// <typeparam name="TUser"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    public class AuthorizedDetailsVisitor<TUser, TKey> : IDetailsViewVisitor
        where TUser : class, IUser<TKey>
        where TKey : IEquatable<TKey>
    {
        ICrudUserStore<TUser, TKey> _userStore;

        public AuthorizedDetailsVisitor(ICrudUserStore<TUser, TKey> userStore)
        {
            _userStore = userStore;
        }

        public void Visit(DetailsView view)
        {
            return;
        }

        public void Visit(Column column)
        {
            return;
        }

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
