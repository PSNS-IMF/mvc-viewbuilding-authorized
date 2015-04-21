using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Psns.Common.Mvc.ViewBuilding.Authorized.Visitors;

using Psns.Common.Mvc.ViewBuilding.ViewBuilders;
using Psns.Common.Mvc.ViewBuilding.ViewModels;
using Psns.Common.Mvc.ViewBuilding.Entities;
using Psns.Common.Persistence.Definitions;

using Psns.Common.Mvc.ViewBuilding.Authorized.Attributes;

using Microsoft.AspNet.Identity;

namespace Psns.Common.Mvc.ViewBuilding.Authorized
{
    /// <summary>
    /// Defines a CrudViewBuilder for authorization purposes
    /// </summary>
    /// <typeparam name="TUser"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    public interface IAuthorizedCrudViewBuilder<TUser, in TKey> : ICrudViewBuilder 
        where TUser : class, IUser<TKey>
        where TKey : IEquatable<TKey> { }

    /// <summary>
    /// A View Builder that provides the permission-checking Visitors
    /// </summary>
    /// <typeparam name="TUser"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    public class AuthorizedCrudViewBuilder<TUser, TKey> : IAuthorizedCrudViewBuilder<TUser, TKey>
        where TUser : class, IUser<TKey>
        where TKey : IEquatable<TKey>
    {
        ICrudViewBuilder _baseBuilder;
        ICrudUserStore<TUser, TKey> _userStore;

        public AuthorizedCrudViewBuilder(ICrudViewBuilder baseBuilder, ICrudUserStore<TUser, TKey> userStore)
        {
            _baseBuilder = baseBuilder;
            _userStore = userStore;
        }

        /// <summary>
        /// Passes the AuthorizedIndexVisitor to the base view builder
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
                new AuthorizedIndexVisitor<TUser, TKey, T>(_userStore) 
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

        /// <summary>
        /// Passes the AuthorizedUpdateVisitor to the base view builder
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <param name="viewVisitors"></param>
        /// <returns></returns>
        public UpdateView BuildUpdateView<T>(T model, params IUpdateViewVisitor[] viewVisitors) 
            where T : class, IIdentifiable, INameable
        {
            return _baseBuilder.BuildUpdateView<T>(model, SetVisitors(viewVisitors));
        }

        /// <summary>
        /// Passes the AuthorizedUpdateVisitor to the base view builder
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <param name="viewVisitors"></param>
        /// <returns></returns>
        public UpdateView BuildUpdateView<T>(int? id, params IUpdateViewVisitor[] viewVisitors) 
            where T : class, IIdentifiable, INameable
        {
            return _baseBuilder.BuildUpdateView<T>(id, SetVisitors(viewVisitors));
        }

        /// <summary>
        /// Passes the AuthorizedDetailsVisitor to the base view builder
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <param name="viewVisitors"></param>
        /// <returns></returns>
        public DetailsView BuildDetailsView<T>(int id, params IDetailsViewVisitor[] viewVisitors)
            where T : class, IIdentifiable, INameable
        {
            var visitors = viewVisitors ?? new IDetailsViewVisitor[0];

            visitors = visitors.Concat(new IDetailsViewVisitor[] 
            { 
                new AuthorizedDetailsVisitor<TUser, TKey>(_userStore)
            }).ToArray();

            return _baseBuilder.BuildDetailsView<T>(id, visitors);
        }

        /// <summary>
        /// Passes the AuthorizedFilterOptionsVisitor to the base view builder
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filterOptionVisitors"></param>
        /// <returns></returns>
        public IEnumerable<FilterOption> GetIndexFilterOptions<T>(params IFilterOptionVisitor[] filterOptionVisitors) 
            where T : class, IIdentifiable
        {
            var visitors = filterOptionVisitors ?? new IFilterOptionVisitor[0];

            visitors = visitors.Concat(new IFilterOptionVisitor[] 
            { 
                new AuthorizedFilterOptionsVisitor<TUser, TKey>(_userStore)
            }).ToArray();

            return _baseBuilder.GetIndexFilterOptions<T>(visitors);
        }

        private IUpdateViewVisitor[] SetVisitors(params IUpdateViewVisitor[] viewVisitors)
        {
            var visitors = viewVisitors ?? new IUpdateViewVisitor[0];

            visitors = visitors.Concat(new IUpdateViewVisitor[] 
            { 
                new AuthorizedUpdateVisitor<TUser, TKey>(_userStore)
            }).ToArray();

            return visitors;
        }
    }
}
