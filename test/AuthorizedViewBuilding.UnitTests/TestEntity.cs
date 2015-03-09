using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Psns.Common.Persistence.Definitions;
using Psns.Common.Mvc.ViewBuilding.Entities;

using Psns.Common.Mvc.ViewBuilding.Authorized;
using Psns.Common.Mvc.ViewBuilding.Authorized.Attributes;

using System.Security.Permissions;

namespace AuthorizedViewBuilding.UnitTests
{
    [CrudAuthorize(AccessType.Create, "roleName")]
    public  class TestEntity : IIdentifiable, INameable, ISecurable<User, int>
    {
        public int Id       { get; set; }
        public string Name  { get; set; }

        public bool Demand(User user, AccessType accessType)
        {
            return user.UserName.Equals("Authorized");
        }
    }
}
