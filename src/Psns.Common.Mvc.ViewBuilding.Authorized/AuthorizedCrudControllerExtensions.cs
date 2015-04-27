using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

using Psns.Common.Mvc.ViewBuilding.Controllers;
using Psns.Common.Mvc.ViewBuilding.Entities;
using Psns.Common.Persistence.Definitions;

using Microsoft.AspNet.Identity;

namespace Psns.Common.Mvc.ViewBuilding.Authorized
{
    public static class AuthorizedCrudControllerExtensions
    {
        public static ActionResult UnauthorizedResult<TEntity, TUser, TUserKey>(this AuthorizedCrudController<TEntity, TUser, TUserKey> controller,
            AccessType accessType)
            where TEntity : class, INameable, IIdentifiable
            where TUser : class, IUser<TUserKey>
            where TUserKey : IEquatable<TUserKey>
        {
            return new HttpUnauthorizedResult(string.Format("You do not have {0} access to {1}", accessType, typeof(TEntity).Name));
        }
    }
}
