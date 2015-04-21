using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.AspNet.Identity;

using Psns.Common.Mvc.ViewBuilding.ViewModels.TableModel;
using Psns.Common.Mvc.ViewBuilding.Authorized.Attributes;

namespace Psns.Common.Mvc.ViewBuilding.Authorized.Visitors
{
    public static class ITableVisitorExtensions
    {
        /// <summary>
        /// Removes any rows in the table whose Source object returns permission denied for AccessType.Read.
        /// </summary>
        /// <typeparam name="TUser"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="visitor"></param>
        /// <param name="userStore"></param>
        /// <param name="table"></param>
        public static void AuthorizeRows<TUser, TKey>(this ITableVisitor visitor, 
            ICrudUserStore<TUser, TKey> userStore,
            Table table)
            where TUser : class, IUser<TKey>
            where TKey : IEquatable<TKey>
        {
            foreach(var row in table.Rows.ToArray())
            {
                if(row.Source.PermissionDenied(userStore, AccessType.Read))
                    table.Rows.Remove(row);
                else
                {
                    foreach(var column in row.Columns.ToArray())
                    {
                        if(!column.Source.CurrentUserHasAccess(AccessType.Read, userStore))
                            row.Columns.Remove(column);
                    }
                }
            }
        }
    }
}