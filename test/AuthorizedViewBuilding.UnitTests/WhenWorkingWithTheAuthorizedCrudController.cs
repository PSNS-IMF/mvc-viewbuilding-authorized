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

        protected ActionResult Result;

        public override void Arrange()
        {
            base.Arrange();

            MockViewBuilder = new Mock<IAuthorizedCrudViewBuilder<User,int>>();
            MockUserStore = new Mock<ICrudUserStore<User, int>>();
            MockRepositoryFactory = new Mock<IRepositoryFactory>();

            MockUserStore.Setup(store => store.CurrentUser).Returns(new User());
            MockUserStore.Setup(store => store.FindByIdAsync(It.IsAny<int>())).Returns(Task.FromResult(new User()));

            Controller = new AuthorizedCrudController<TestEntity, User, int>(MockViewBuilder.Object,
                MockUserStore.Object,
                MockRepositoryFactory.Object);
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

        protected void AssertCommon(AccessType accessType, string entityName)
        {
            var result = (Result as HttpUnauthorizedResult);

            Assert.AreEqual(401, result.StatusCode);
            Assert.AreEqual(string.Format("You do not have {0} access to {1}", accessType.ToString(), entityName), result.StatusDescription);
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
            Assert.IsNotNull((Result as ViewResult));
        }
    }

    #region Update
    [TestClass]
    public class AndAccessingUpdateIdAsAUserNotInACreatorRole : AndTheUserIsNotInTheRoles
    {
        public override void Arrange()
        {
            base.Arrange();

            ControllerAction = () => Controller.Update(0);
        }

        [TestMethod]
        public void ThenUnAuthorizedShouldBeReturned()
        {
            AssertCommon(AccessType.Create, typeof(TestEntity).Name);
        }
    }

    [TestClass]
    public class AndAccessingUpdateIdAsAUserInACreatorRole : AndTheUserIsInTheRoles
    {
        public override void Arrange()
        {
            base.Arrange();

            ControllerAction = () => Controller.Update(0);
        }

        [TestMethod]
        public void ThenAViewResultShouldBeReturned()
        {
            Assert.IsNotNull((Result as ViewResult));
        }
    }

    [TestClass]
    public class AndAccessingUpdateModelAsAUserNotInACreatorRole : AndTheUserIsNotInTheRoles
    {
        public override void Arrange()
        {
            base.Arrange();

            ControllerAction = () => Controller.Update(new TestEntity());
        }

        [TestMethod]
        public void ThenUnAuthorizedShouldBeReturned()
        {
            AssertCommon(AccessType.Create, typeof(TestEntity).Name);
        }
    }

    [TestClass]
    public class AndAccessingUpdateModelAsAUserInACreatorRole : AndTheUserIsInTheRoles
    {
        bool _validateCalled;

        public override void Arrange()
        {
            base.Arrange();

            AntiForgeryHelperAdapter.ValidationFunction = () => _validateCalled = true;

            MockViewBuilder.Setup(b => b.BuildUpdateView<TestEntity>(It.IsAny<TestEntity>())).Returns(new UpdateView());

            var mockRepository = new Mock<IRepository<TestEntity>>();
            mockRepository.Setup(r => r.Update(It.IsAny<TestEntity>(), It.IsAny<string[]>())).Returns(new TestEntity());

            MockRepositoryFactory.Setup(f => f.Get<TestEntity>()).Returns(mockRepository.Object);

            ControllerAction = () => Controller.Update(new TestEntity { Id = 1 });
        }

        [TestMethod]
        public void ThenAViewResultShouldBeReturned()
        {
            Assert.IsNotNull((Result as ViewResult));
            Assert.IsTrue(_validateCalled);
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
            AssertCommon(AccessType.Read, typeof(TestEntity).Name);
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
    #endregion
}
