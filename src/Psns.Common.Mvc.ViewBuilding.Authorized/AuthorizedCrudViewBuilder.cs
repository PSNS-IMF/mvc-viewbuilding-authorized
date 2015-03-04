using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Psns.Common.Mvc.ViewBuilding.ViewBuilders;
using Psns.Common.Mvc.ViewBuilding.ViewModels;
using Psns.Common.Mvc.ViewBuilding.Entities;
using Psns.Common.Persistence.Definitions;

namespace Psns.Common.Mvc.ViewBuilding.Authorized
{
    public class AuthorizedCrudViewBuilder : ICrudViewBuilder
    {
        ICrudViewBuilder _baseBuilder;

        public AuthorizedCrudViewBuilder(ICrudViewBuilder baseBuilder)
        {
            _baseBuilder = baseBuilder;
        }

        public DetailsView BuildDetailsView<T>(int id, params IDetailsViewVisitor[] viewVisitors) 
            where T : class, IIdentifiable, INameable
        {
            return _baseBuilder.BuildDetailsView<T>(id, viewVisitors);
        }

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
            throw new NotImplementedException();
        }

        public UpdateView BuildUpdateView<T>(T model) 
            where T : class, IIdentifiable, INameable
        {
            throw new NotImplementedException();
        }

        public UpdateView BuildUpdateView<T>(int? id) 
            where T : class, IIdentifiable, INameable
        {
            throw new NotImplementedException();
        }

        public IEnumerable<FilterOption> GetIndexFilterOptions<T>() 
            where T : class, IIdentifiable
        {
            throw new NotImplementedException();
        }
    }
}
