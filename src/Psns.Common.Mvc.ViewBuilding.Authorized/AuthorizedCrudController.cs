using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Psns.Common.Mvc.ViewBuilding.Controllers;
using Psns.Common.Mvc.ViewBuilding.Entities;
using Psns.Common.Persistence.Definitions;

using Microsoft.AspNet.Identity;

namespace Psns.Common.Mvc.ViewBuilding.Authorized
{
    /// <summary>
    /// Derives from CrudController to enable permission checking
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TUser"></typeparam>
    /// <typeparam name="TUserKey"></typeparam>
    public class AuthorizedCrudController<TEntity, TUser, TUserKey> : CrudController<TEntity>
        where TEntity : class, INameable, IIdentifiable
        where TUser : class, IUser<TUserKey>
        where TUserKey : IEquatable<TUserKey>
    {
        public AuthorizedCrudController(IAuthorizedCrudViewBuilder<TUser, TUserKey> viewBuilder, IRepositoryFactory factory)
            : base(viewBuilder, factory) { }
    }
}
