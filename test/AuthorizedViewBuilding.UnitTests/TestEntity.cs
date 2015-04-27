using Psns.Common.Persistence.Definitions;
using Psns.Common.Mvc.ViewBuilding.Entities;

using Psns.Common.Mvc.ViewBuilding.Authorized;
using Psns.Common.Mvc.ViewBuilding.Authorized.Attributes;

using System.ComponentModel.DataAnnotations;

namespace AuthorizedViewBuilding.UnitTests
{
    [CrudAuthorize(AccessType.Create, "roleName")]
    [CrudAuthorize(AccessType.Read, "Reader")]
    public  class TestEntity : IIdentifiable, INameable, ISecurable<User, int>
    {
        public int Id       { get; set; }
        public string Name  { get; set; }

        public const string AuthKey = "Authorized";

        [CrudAuthorize(AccessType.Update, "Updater")]
        public string ProtectedProperty { get; set; }

        [CrudAuthorize(AccessType.Create, "Updated")]
        public int UnsupportedProtectedProperty { get; set; }

        [Display(Name = "Labeled Protected Property")]
        [CrudAuthorize(AccessType.Update, "Updater")]
        public string LabeledProtectedProperty { get; set; }

        [CrudAuthorize(AccessType.Read, "Readers")]
        [CrudAuthorize(AccessType.Update, "Updater")]
        public string RestrictedReadProperty { get; set; }

        public bool Demand(User user, AccessType accessType)
        {
            return this.Name.Equals(AuthKey);
        }
    }
}
