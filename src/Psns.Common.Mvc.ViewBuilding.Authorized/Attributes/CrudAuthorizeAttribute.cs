using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Psns.Common.Mvc.ViewBuilding.Authorized.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property, 
        AllowMultiple = true, 
        Inherited = false)]
    public class CrudAuthorizeAttribute : Attribute
    {
        public readonly AccessType AccessType;
        public readonly string[] RoleNames;

        public CrudAuthorizeAttribute(AccessType accessType, params string[] roleNames)
        {
            AccessType = accessType;
            RoleNames = roleNames;
        }
    }
}
