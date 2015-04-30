using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

using System.Web.Mvc;

using Psns.Common.Test.BehaviorDrivenDevelopment;
using Psns.Common.Persistence.Definitions;

using Psns.Common.Mvc.ViewBuilding.Authorized;
using Psns.Common.Mvc.ViewBuilding.ViewModels;
using Psns.Common.Web.Adapters;

using Moq;

namespace AuthorizedViewBuilding.UnitTests
{
    public class WhenWorkingWithTheAuthorizedCrudController : BehaviorDrivenDevelopmentCaseTemplate
    {
        protected AuthorizedCrudController<TestEntity, User, int> Controller;

        protected Mock<IAuthorizedCrudViewBuilder<User, int>> MockViewBuilder;
        protected Mock<ICrudUserStore<User, int>> MockUserStore;
        protected Mock<IRepositoryFactory> MockRepositoryFactory;
        protected Mock<IRepository<TestEntity>> MockRepository;

        protected ActionResult Result;

        public override void Arrange()
        {
            base.Arrange();

            MockViewBuilder = new Mock<IAuthorizedCrudViewBuilder<User,int>>();
            MockUserStore = new Mock<ICrudUserStore<User, int>>();
            MockRepositoryFactory = new Mock<IRepositoryFactory>();
            MockRepository = new Mock<IRepository<TestEntity>>();

            MockRepositoryFactory.Setup(f => f.Get<TestEntity>()).Returns(MockRepository.Object);

            MockUserStore.Setup(store => store.CurrentUser).Returns(new User());
            MockUserStore.Setup(store => store.FindByIdAsync(It.IsAny<int>())).Returns(Task.FromResult(new User()));

            Controller = new AuthorizedCrudController<TestEntity, User, int>(MockViewBuilder.Object,
                MockUserStore.Object,
                MockRepositoryFactory.Object);
        }

        protected void AssertUnauthorized(AccessType accessType, string entityName)
        {
            var result = (Result as HttpUnauthorizedResult);

            Assert.AreEqual(401, result.StatusCode);
            Assert.AreEqual(string.Format("You do not have {0} access to {1}", accessType.ToString(), entityName), result.StatusDescription);
        }
    }

    public class AndTheUserIsNotInTheRoles : WhenWorkingWithTheAuthorizedCrudController
    {
        protected Func<ActionResult> ControllerAction { get; set; }

        public override void Arrange()
        {
            base.Arrange();

            MockUserStore.Setup(store => store.IsInRoleAsync(It.IsAny<User>(), It.IsAny<string>())).Returns(Task.FromResult(false));
        }

        public override void Act()
        {
            base.Act();

            Result = ControllerAction();
        }
    }

    public class AndTheUserIsInTheRoles : WhenWorkingWithTheAuthorizedCrudController
    {
        protected Func<ActionResult> ControllerAction { get; set; }

        public override void Arrange()
        {
            base.Arrange();

            MockUserStore.Setup(store => store.IsInRoleAsync(It.IsAny<User>(), It.IsAny<string>())).Returns(Task.FromResult(true));
        }

        public override void Act()
        {
            base.Act();

            Result = ControllerAction();
        }

        protected void AssertCommon()
        {
            Assert.IsInstanceOfType(Result, typeof(ViewResult));
        }
    }

    public class ButDemandDenies : AndTheUserIsInTheRoles
    {
        public override void Arrange()
        {
            base.Arrange();

            MockRepository.Setup(r => r.Find(It.IsAny<object[]>())).Returns(new TestEntity { Name = "Unauthorized" });
        }
    }

    #region Update
    [TestClass]
    public class AndAccessingUpdateIdGetAsAUserNotInACreatorRole : AndTheUserIsNotInTheRoles
    {
        public override void Arrange()
        {
            base.Arrange();

            ControllerAction = () => Controller.Update(id: null);
        }

        [TestMethod]
        public void ThenUnAuthorizedShouldBeReturned()
        {
            AssertUnauthorized(AccessType.Create, typeof(TestEntity).Name);
        }
    }

    [TestClass]
    public class AndAccessingUpdateIdGetAsAUserNotInAnUpdaterRole : AndTheUserIsNotInTheRoles
    {
        public override void Arrange()
        {
            base.Arrange();

            ControllerAction = () => Controller.Update(1);
        }

        [TestMethod]
        public void ThenUnAuthorizedShouldBeReturned()
        {
            AssertUnauthorized(AccessType.Update, typeof(TestEntity).Name);
        }
    }

    [TestClass]
    public class AndAccessingUpdateIdForISecurableAsAUserInAUpdaterRole : ButDemandDenies
    {
        public override void Arrange()
        {
            base.Arrange();

            ControllerAction = () => Controller.Update(1);
        }

        [TestMethod]
        public void ThenUnAuthorizedShouldBeReturned()
        {
            AssertUnauthorized(AccessType.Update, "Unauthorized");
        }
    }

    [TestClass]
    public class AndAccessingUpdateIdGetAsAUserInACreatorRole : AndTheUserIsInTheRoles
    {
        public override void Arrange()
        {
            base.Arrange();

            ControllerAction = () => Controller.Update(id: null);
        }

        [TestMethod]
        public void ThenAViewResultShouldBeReturned()
        {
            Assert.IsNotNull((Result as ViewResult));
        }
    }

    [TestClass]
    public class AndAccessingUpdateIdGetAsAUserInAnUpdaterRole : AndTheUserIsInTheRoles
    {
        public override void Arrange()
        {
            base.Arrange();

            AntiForgeryHelperAdapter.ValidationFunction = () => { return; };

            MockRepository.Setup(r => r.Find(It.IsAny<object[]>())).Returns(new TestEntity { Id = 1, Name = TestEntity.AuthKey });

            MockRepository.Setup(r => r.Update(It.IsAny<TestEntity>(), It.IsAny<string[]>()))
                .Returns(new TestEntity { Id = 1, Name = TestEntity.AuthKey });

            ControllerAction = () => Controller.Update(1);
        }

        [TestMethod]
        public void ThenAViewResultShouldBeReturned()
        {
            Assert.IsInstanceOfType(Result, typeof(RedirectToRouteResult));
        }
    }

    [TestClass]
    public class AndAccessingUpdateModelPostAsAUserNotInACreatorRole : AndTheUserIsNotInTheRoles
    {
        public override void Arrange()
        {
            base.Arrange();

            ControllerAction = () => Controller.Update(new TestEntity());
        }

        [TestMethod]
        public void ThenUnAuthorizedShouldBeReturned()
        {
            AssertUnauthorized(AccessType.Create, typeof(TestEntity).Name);
        }
    }

    [TestClass]
    public class AndAccessingUpdateModelPostForISecurableAsAUserInACreatorRole : ButDemandDenies
    {
        public override void Arrange()
        {
            base.Arrange();

            AntiForgeryHelperAdapter.ValidationFunction = () => { return; };

            ControllerAction = () => Controller.Update(new TestEntity { Name = "Unauthorized" });
        }

        [TestMethod]
        public void ThenUnAuthorizedShouldBeReturned()
        {
            AssertUnauthorized(AccessType.Create, "Unauthorized");
        }
    }

    [TestClass]
    public class AndAccessingUpdateModelPostAsAUserNotInAnUpdaterRole : AndTheUserIsNotInTheRoles
    {
        public override void Arrange()
        {
            base.Arrange();

            ControllerAction = () => Controller.Update(new TestEntity { Id = 1 });
        }

        [TestMethod]
        public void ThenUnAuthorizedShouldBeReturned()
        {
            AssertUnauthorized(AccessType.Update, typeof(TestEntity).Name);
        }
    }

    [TestClass]
    public class AndAccessingUpdateModelPostForISecurableAsAUserInAUpdaterRole : ButDemandDenies
    {
        public override void Arrange()
        {
            base.Arrange();

            AntiForgeryHelperAdapter.ValidationFunction = () => { return; };

            ControllerAction = () => Controller.Update(new TestEntity { Id = 1, Name = "Unauthorized" });
        }

        [TestMethod]
        public void ThenUnAuthorizedShouldBeReturned()
        {
            AssertUnauthorized(AccessType.Update, "Unauthorized");
        }
    }

    [TestClass]
    public class AndAccessingUpdateModelPostAsAUserInACreatorRole : AndTheUserIsInTheRoles
    {
        public override void Arrange()
        {
            base.Arrange();

            AntiForgeryHelperAdapter.ValidationFunction = () => { return; };

            MockViewBuilder.Setup(b => b.BuildUpdateView<TestEntity>(It.IsAny<TestEntity>())).Returns(new UpdateView());

            MockRepository.Setup(r => r.Create(It.IsAny<TestEntity>())).Returns(new TestEntity());

            ControllerAction = () => Controller.Update(new TestEntity { Name = TestEntity.AuthKey });
        }

        [TestMethod]
        public void ThenARedirectToRouteResultShouldBeReturned()
        {
            Assert.IsNotNull((Result as RedirectToRouteResult));
        }
    }

    [TestClass]
    public class AndAccessingUpdateModelPostAsAUserInAnUpdaterRole : AndTheUserIsInTheRoles
    {
        public override void Arrange()
        {
            base.Arrange();

            AntiForgeryHelperAdapter.ValidationFunction = () => { return; };

            MockViewBuilder.Setup(b => b.BuildUpdateView<TestEntity>(It.IsAny<TestEntity>())).Returns(new UpdateView());

            MockRepository.Setup(r => r.Update(It.IsAny<TestEntity>(), It.IsAny<string[]>())).Returns(new TestEntity());

            ControllerAction = () => Controller.Update(new TestEntity { Id = 1, Name = TestEntity.AuthKey });
        }

        [TestMethod]
        public void ThenARedirectToRouteResultShouldBeReturned()
        {
            Assert.IsNotNull((Result as RedirectToRouteResult));
        }
    }
    #endregion

    #region Read
    [TestClass]
    public class AndAccessingIndexAsAUserNotInAReaderRole : AndTheUserIsNotInTheRoles
    {
        public override void Arrange()
        {
            base.Arrange();

            ControllerAction = () => Controller.Index();
        }

        [TestMethod]
        public void ThenUnAuthorizedShouldBeReturned()
        {
            AssertUnauthorized(AccessType.Read, typeof(TestEntity).Name);
        }
    }

    [TestClass]
    public class AndAccessingIndexAsAUserInAReaderRole : AndTheUserIsInTheRoles
    {
        public override void Arrange()
        {
            base.Arrange();

            ControllerAction = () => Controller.Index();
        }

        [TestMethod]
        public void ThenAViewResultShouldBeReturned()
        {
            AssertCommon();
        }
    }

    [TestClass]
    public class AndAccessingDetailsAsAUserNotInAReaderRole : AndTheUserIsNotInTheRoles
    {
        public override void Arrange()
        {
            base.Arrange();

            ControllerAction = () => Controller.Details(1);
        }

        [TestMethod]
        public void ThenUnAuthorizedShouldBeReturned()
        {
            AssertUnauthorized(AccessType.Read, typeof(TestEntity).Name);
        }
    }

    [TestClass]
    public class AndAccessingDetailsForISecurableAsAUserInAReaderRole : ButDemandDenies
    {
        public override void Arrange()
        {
            base.Arrange();

            ControllerAction = () => Controller.Details(1);
        }

        [TestMethod]
        public void ThenUnAuthorizedShouldBeReturned()
        {
            AssertUnauthorized(AccessType.Read, "Unauthorized");
        }
    }

    [TestClass]
    public class AndAccessingDetailsAsAUserInAReaderRole : AndTheUserIsInTheRoles
    {
        public override void Arrange()
        {
            base.Arrange();

            ControllerAction = () => Controller.Details(1);
        }

        [TestMethod]
        public void ThenAViewResultShouldBeReturned()
        {
            AssertCommon();
        }
    }

    #endregion

    #region Delete

    [TestClass]
    public class AndAccessingDeleteAsAUserNotInADeleterRole : AndTheUserIsNotInTheRoles
    {
        public override void Arrange()
        {
            base.Arrange();

            AntiForgeryHelperAdapter.ValidationFunction = () => { return; };

            ControllerAction = () => Controller.Delete(1);
        }

        [TestMethod]
        public void ThenUnAuthorizedShouldBeReturned()
        {
            AssertUnauthorized(AccessType.Delete, typeof(TestEntity).Name);
        }
    }

    [TestClass]
    public class AndAccessingDeleteAsAUserInADeleterRole : AndTheUserIsInTheRoles
    {
        public override void Arrange()
        {
            base.Arrange();

            AntiForgeryHelperAdapter.ValidationFunction = () => { return; };

            ControllerAction = () => Controller.Delete(1);
        }

        [TestMethod]
        public void ThenAViewResultShouldBeReturned()
        {
            Assert.IsInstanceOfType(Result, typeof(RedirectToRouteResult));
        }
    }

    [TestClass]
    public class AndAccessingDeleteForISecurableAsAUserInAReaderRole : ButDemandDenies
    {
        public override void Arrange()
        {
            base.Arrange();

            AntiForgeryHelperAdapter.ValidationFunction = () => { return; };

            ControllerAction = () => Controller.Delete(1);
        }

        [TestMethod]
        public void ThenUnAuthorizedShouldBeReturned()
        {
            AssertUnauthorized(AccessType.Delete, "Unauthorized");
        }
    }

    #endregion
}
