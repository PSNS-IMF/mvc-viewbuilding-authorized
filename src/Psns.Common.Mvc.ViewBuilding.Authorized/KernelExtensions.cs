using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Ninject;

using Microsoft.AspNet.Identity;

using Psns.Common.Mvc.ViewBuilding.ViewBuilders;

namespace Psns.Common.Mvc.ViewBuilding.Authorized
{
    /// <summary>
    /// Extensions for the Ninject.IKernel
    /// </summary>
    public static class KernelExtensions
    {
        /// <summary>
        /// Binds ICrudViewBuilder to CrudViewBuilder when injected into AuthorizedCrudViewBuilder.
        /// Binds ICrudViewBuilder (otherwise) and IAuthorizedCrudViewBuilder to AuthorizedCrudViewBuilder.
        /// </summary>
        /// <typeparam name="TUser">Microsoft.AspNet.Identity.IUser of TKey</typeparam>
        /// <typeparam name="TKey">IEquatable of TKey</typeparam>
        /// <param name="kernel">A Ninject IKernel</param>
        public static void BindAuthorizedComponents<TUser, TKey>(this IKernel kernel)
            where TUser : class, IUser<TKey>
            where TKey : IEquatable<TKey>
        {
            kernel.Bind<ICrudViewBuilder>().To<CrudViewBuilder>().WhenInjectedInto<AuthorizedCrudViewBuilder<TUser, TKey>>();
            kernel.Bind(typeof(ICrudViewBuilder), typeof(IAuthorizedCrudViewBuilder<TUser, TKey>)).To<AuthorizedCrudViewBuilder<TUser, TKey>>();
        }
    }
}
