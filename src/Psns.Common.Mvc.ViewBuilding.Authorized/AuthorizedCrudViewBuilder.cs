using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Psns.Common.Mvc.ViewBuilding.ViewBuilders;
using Psns.Common.Mvc.ViewBuilding.ViewModels;
using Psns.Common.Mvc.ViewBuilding.Entities;
using Psns.Common.Persistence.Definitions;

using Psns.Common.Mvc.ViewBuilding.Authorized.Attributes;

using Microsoft.AspNet.Identity;

namespace Psns.Common.Mvc.ViewBuilding.Authorized
{
    public interface IAuthorizedCrudViewBuilder<TUser, in TKey> : ICrudViewBuilder 
        where TUser : class, IUser<TKey>
        where TKey : IEquatable<TKey> { }

    public class AuthorizedCrudViewBuilder<TUser, TKey> : IAuthorizedCrudViewBuilder<TUser, TKey>
        where TUser : class, IUser<TKey>
        where TKey : IEquatable<TKey>
    {
        ICrudViewBuilder _baseBuilder;
        ICrudUserStore<TUser, TKey> _userStore;
        UserManager<TUser, TKey> _userManager;

        public AuthorizedCrudViewBuilder(ICrudViewBuilder baseBuilder, ICrudUserStore<TUser, TKey> userStore)
        {
            _baseBuilder = baseBuilder;

            _userStore = userStore;
            _userManager = new UserManager<TUser, TKey>(_userStore);
        }

        public DetailsView BuildDetailsView<T>(int id, params IDetailsViewVisitor[] viewVisitors) 
            where T : class, IIdentifiable, INameable
        {
            return _baseBuilder.BuildDetailsView<T>(id, viewVisitors);
        }

        /// <summary>
        /// If T isn't decorated with a CrudAuthorizeAttribute where the AccessType is set to Create or
        /// the current user isn't in the Role listed in the RolesNames of the CrudAuthorizeAttribute, 
        /// then the IndexView.CreateButton returned from baseBuilder is set to null.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="sortKey"></param>
        /// <param name="sortDirection"></param>
        /// <param name="filterKeys"></param>
        /// <param name="filterValues"></param>
        /// <param name="searchQuery"></param>
        /// <param name="viewVisitors"></param>
        /// <returns></returns>
        public IndexView BuildIndexView<T>(int? page = null, 
            int? pageSize = null, 
            string sortKey = null, 
            string sortDirection = null, 
            IEnumerable<string> filterKeys = null, 
            IEnumerable<string> filterValues = null, 
            string searchQuery = null, 
            params IIndexViewVisitor[] viewVisitors) 
            where T : class, IIdentifiable
        {
            var visitors = viewVisitors ?? new IIndexViewVisitor[0];

            visitors = visitors.Concat(new IIndexViewVisitor[] 
            { 
                new AuthorizedIndexVisitor<TUser, TKey, T>(_userStore, _userManager) 
            }).ToArray();

            return _baseBuilder.BuildIndexView<T>(page, 
                pageSize, 
                sortKey, 
                sortDirection, 
                filterKeys, 
                filterValues, 
                searchQuery, 
                visitors);
        }

        public UpdateView BuildUpdateView<T>(T model) 
            where T : class, IIdentifiable, INameable
        {
            return _baseBuilder.BuildUpdateView<T>(model);
        }

        public UpdateView BuildUpdateView<T>(int? id) 
            where T : class, IIdentifiable, INameable
        {
            return _baseBuilder.BuildUpdateView<T>(id);
        }

        public IEnumerable<FilterOption> GetIndexFilterOptions<T>() 
            where T : class, IIdentifiable
        {
            return _baseBuilder.GetIndexFilterOptions<T>();
        }
    }
}
