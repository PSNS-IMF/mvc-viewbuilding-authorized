using System;
using System.Linq;

using System.ComponentModel.DataAnnotations;

using Psns.Common.Mvc.ViewBuilding.ViewModels;
using Psns.Common.Mvc.ViewBuilding.Authorized.Attributes;

using Microsoft.AspNet.Identity;

namespace Psns.Common.Mvc.ViewBuilding.Authorized.Visitors
{
    /// <summary>
    /// A Visitor for the UpdateView model
    /// </summary>
    /// <typeparam name="TUser"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    public class AuthorizedUpdateVisitor<TUser, TKey> : IUpdateViewVisitor
        where TUser : class, IUser<TKey>
        where TKey : IEquatable<TKey>
    {
        ICrudUserStore<TUser, TKey> _userStore;

        public AuthorizedUpdateVisitor(ICrudUserStore<TUser, TKey> userStore)
        {
            _userStore = userStore;
        }

        /// <summary>
        /// Returns null if InputProperty represents a property decorated with CrudAuthorizeAuthorizeAttribute for
        /// the Update AccessType and Current User is not in the specified roles; otherwise, the InputProperty is returned.
        /// </summary>
        /// <param name="inputProperty"></param>
        /// <returns></returns>
        public InputProperty Visit(InputProperty inputProperty)
        {
            var propertyInfo = inputProperty.Source.GetType().GetProperty(inputProperty.Label);

            if(propertyInfo == null)
            {
                foreach(var property in inputProperty.Source.GetType().GetProperties())
                {
                    var displayAttribute = (property.GetCustomAttributes(typeof(DisplayAttribute), false) as DisplayAttribute[])
                        .Where(a => a.Name.Equals(inputProperty.Label))
                        .SingleOrDefault();

                    if(displayAttribute == null)
                        return inputProperty;

                    propertyInfo = property;
                }
            }

            if(!propertyInfo.CurrentUserHasAccess(AccessType.Update, _userStore))
                return null;

            return inputProperty;
        }
    }
}