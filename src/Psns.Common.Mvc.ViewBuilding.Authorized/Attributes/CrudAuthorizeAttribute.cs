using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Psns.Common.Mvc.ViewBuilding.Authorized.Attributes
{
    /// <summary>
    /// Used to decorate protected classes and properties
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property, 
        AllowMultiple = true, 
        Inherited = false)]
    public class CrudAuthorizeAttribute : Attribute
    {
        /// <summary>
        /// The AccessType being controlled
        /// </summary>
        public readonly AccessType AccessType;

        /// <summary>
        /// The RoleNames permitted to the AccessType
        /// </summary>
        public readonly string[] RoleNames;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="accessType"></param>
        /// <param name="roleNames"></param>
        public CrudAuthorizeAttribute(AccessType accessType, params string[] roleNames)
        {
            AccessType = accessType;
            RoleNames = roleNames;
        }
    }
}
