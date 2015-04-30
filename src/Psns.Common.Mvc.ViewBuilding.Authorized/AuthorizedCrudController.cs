using System;

using System.Web.Mvc;

using Psns.Common.Mvc.ViewBuilding.Controllers;
using Psns.Common.Mvc.ViewBuilding.Entities;
using Psns.Common.Persistence.Definitions;

using Psns.Common.Mvc.ViewBuilding.Authorized.Attributes;

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
        ICrudUserStore<TUser, TUserKey> _userStore;

        public AuthorizedCrudController(IAuthorizedCrudViewBuilder<TUser, TUserKey> viewBuilder,
            ICrudUserStore<TUser, TUserKey> userStore,
            IRepositoryFactory factory)
            : base(viewBuilder, factory) 
        {
            _userStore = userStore;
        }

        public override ActionResult Index()
        {
            if(!typeof(TEntity).CurrentUserHasAccess(AccessType.Read, _userStore))
                return this.UnauthorizedResult(AccessType.Read);
            else
                return base.Index();
        }

        public override ActionResult Details(int id)
        {
            var entity = Repository.Find(id);

            if(!typeof(TEntity).CurrentUserHasAccess(AccessType.Read, _userStore))
                return this.UnauthorizedResult(AccessType.Read);
            else if(entity.PermissionDenied(_userStore, AccessType.Read))
                return this.UnauthorizedResult(AccessType.Read, entity.Name);
            else
                return base.Details(entity);
        }

        public override ActionResult Update(int? id)
        {
            if(!id.HasValue)
            {
                if(!typeof(TEntity).CurrentUserHasAccess(AccessType.Create, _userStore))
                    return this.UnauthorizedResult(AccessType.Create);
                else
                    return base.Update(id);
            }
            else
            {
                var entity = Repository.Find(id);

                if(!typeof(TEntity).CurrentUserHasAccess(AccessType.Update, _userStore))
                    return this.UnauthorizedResult(AccessType.Update);
                else if(entity.PermissionDenied(_userStore, AccessType.Update))
                    return this.UnauthorizedResult(AccessType.Update, entity.Name);
                else
                    return this.Update<TEntity>(entity, HttpVerbs.Get);
            }
        }

        public override ActionResult Update(TEntity model)
        {
            if(this.IsCreate(model))
            {
                if(!typeof(TEntity).CurrentUserHasAccess(AccessType.Create, _userStore))
                    return this.UnauthorizedResult(AccessType.Create);
                else if(model.PermissionDenied(_userStore, AccessType.Create))
                    return this.UnauthorizedResult(AccessType.Create, model.Name);
                else
                    return base.Update(model);
            }
            else if(!typeof(TEntity).CurrentUserHasAccess(AccessType.Update, _userStore))
                return this.UnauthorizedResult(AccessType.Update);
            else if(model.PermissionDenied(_userStore, AccessType.Update))
                return this.UnauthorizedResult(AccessType.Update, model.Name);
            else
                return base.Update(model);
        }

        public override ActionResult Delete(int id)
        {
            var entity = Repository.Find(id);

            if(!typeof(TEntity).CurrentUserHasAccess(AccessType.Delete, _userStore))
                return this.UnauthorizedResult(AccessType.Delete);
            else if(entity.PermissionDenied(_userStore, AccessType.Delete))
                return this.UnauthorizedResult(AccessType.Delete, entity.Name);
            else
                return base.Delete(entity);
        }
    }
}
