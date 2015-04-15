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

using System.ComponentModel.DataAnnotations;

namespace AuthorizedViewBuilding.UnitTests
{
    [CrudAuthorize(AccessType.Create, "roleName")]
    public  class TestEntity : IIdentifiable, INameable, ISecurable<User, int>
    {
        public int Id       { get; set; }
        public string Name  { get; set; }

        [CrudAuthorize(AccessType.Update, "Updater")]
        public string ProtectedProperty { get; set; }

        [CrudAuthorize(AccessType.Create, "Updated")]
        public int UnsupportedProtectedProperty { get; set; }

        [Display(Name = "Labeled Protected Property")]
        [CrudAuthorize(AccessType.Update, "Updater")]
        public string LabeledProtectedProperty { get; set; }

        public bool Demand(User user, AccessType accessType)
        {
            if(accessType == AccessType.Read)
                return this.Name.Equals("Authorized");
            else
                return false;
        }
    }
}
