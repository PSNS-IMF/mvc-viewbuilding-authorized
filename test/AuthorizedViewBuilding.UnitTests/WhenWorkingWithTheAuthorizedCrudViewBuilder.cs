using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Psns.Common.Test.BehaviorDrivenDevelopment;
using Psns.Common.Mvc.ViewBuilding.Authorized;
using Psns.Common.Mvc.ViewBuilding.ViewBuilders;
using Psns.Common.Mvc.ViewBuilding.ViewModels;

using Microsoft.AspNet.Identity;

using System.Threading.Tasks;

using Moq;

namespace AuthorizedViewBuilding.UnitTests
{
    public class WhenWorkingWithTheAuthorizedCrudViewBuilder : BehaviorDrivenDevelopmentCaseTemplate
    {
        protected AuthorizedCrudViewBuilder<User, int> ViewBuilder;
        protected Mock<ICrudViewBuilder> MockBaseBuilder;
        protected Mock<ICrudUserStore<User, int>> MockUserStore;

        public override void Arrange()
        {
            base.Arrange();

            MockBaseBuilder = new Mock<ICrudViewBuilder>();
            MockUserStore = new Mock<ICrudUserStore<User, int>>();

            ViewBuilder = new AuthorizedCrudViewBuilder<User, int>(MockBaseBuilder.Object, MockUserStore.Object);
        }
    }

    [TestClass]
    public class AndBuildingTheDetailsView : WhenWorkingWithTheAuthorizedCrudViewBuilder
    {
        public override void Act()
        {
            base.Act();

            ViewBuilder.BuildDetailsView<TestEntity>(1, null);
        }

        [TestMethod]
        public void ThenTheBaseBuilderBuildDetailsViewShouldBeCalledOnce()
        {
            MockBaseBuilder.Verify(b => b.BuildDetailsView<TestEntity>(1, null), Times.Once());
        }
    }

    [TestClass]
    public class AndBuildingTheIndexViewWithCreateAccess : WhenWorkingWithTheAuthorizedCrudViewBuilder
    {
        IndexView _result;

        public override void Arrange()
        {
            base.Arrange();

            MockUserStore.Setup(u => u.CurrentUser).Returns(new User { Id = 1 });
            MockUserStore.Setup(u => u.FindByIdAsync(It.IsAny<int>())).Returns(Task.FromResult(new User { Id = 1 }));
            MockUserStore.Setup(u => u.IsInRoleAsync(It.IsAny<User>(), It.IsAny<string>())).Returns(Task.FromResult(true));

            var view = new IndexView("TestEntity");
            view.CreateButton = new ActionModel();

            MockBaseBuilder.Setup(b => b.BuildIndexView<TestEntity>(null, null, null, null, null, null, null, null)).Returns(view);
        }

        public override void Act()
        {
            base.Act();

            _result = ViewBuilder.BuildIndexView<TestEntity>(null, null, null, null, null, null, null, null);
        }

        [TestMethod]
        public void ThenTheCreateButtonShouldExist()
        {
            Assert.IsNotNull(_result.CreateButton);
        }
    }

    [TestClass]
    public class AndBuildingTheIndexViewWithoutCreateAccess : WhenWorkingWithTheAuthorizedCrudViewBuilder
    {
        IndexView _result;

        public override void Arrange()
        {
            base.Arrange();

            MockUserStore.Setup(u => u.CurrentUser).Returns(new User { Id = 1 });
            MockUserStore.Setup(u => u.FindByIdAsync(It.IsAny<int>())).Returns(Task.FromResult(new User { Id = 1 }));
            MockUserStore.Setup(u => u.IsInRoleAsync(It.IsAny<User>(), It.IsAny<string>())).Returns(Task.FromResult(false));

            var view = new IndexView("TestEntity");
            view.CreateButton = new ActionModel();

            MockBaseBuilder.Setup(b => b.BuildIndexView<TestEntity>(null, null, null, null, null, null, null, null)).Returns(view);
        }

        public override void Act()
        {
            base.Act();

            _result = ViewBuilder.BuildIndexView<TestEntity>(null, null, null, null, null, null, null, null);
        }

        [TestMethod]
        public void ThenTheCreateButtonShouldNotExistInContextItems()
        {
            Assert.IsNull(_result.CreateButton);
        }
    }

    [TestClass]
    public class AndBuildingTheUpdateViewBasedOnId : WhenWorkingWithTheAuthorizedCrudViewBuilder
    {
        public override void Act()
        {
            base.Act();

            ViewBuilder.BuildUpdateView<TestEntity>(1);
        }

        [TestMethod]
        public void ThenTheBaseBuilderBuildUpdateViewShouldBeCalledOnce()
        {
            MockBaseBuilder.Verify(b => b.BuildUpdateView<TestEntity>(1), Times.Once());
        }
    }

    [TestClass]
    public class AndBuildingTheUpdateView : WhenWorkingWithTheAuthorizedCrudViewBuilder
    {
        public override void Act()
        {
            base.Act();

            ViewBuilder.BuildUpdateView<TestEntity>(new TestEntity { Id = 1 });
        }

        [TestMethod]
        public void ThenTheBaseBuilderBuildUpdateViewShouldBeCalledOnce()
        {
            MockBaseBuilder.Verify(b => b.BuildUpdateView<TestEntity>(It.Is<TestEntity>(e => e.Id == 1)), Times.Once());
        }
    }

    [TestClass]
    public class AndBuildingIndexFilterOptions : WhenWorkingWithTheAuthorizedCrudViewBuilder
    {
        public override void Act()
        {
            base.Act();

            ViewBuilder.GetIndexFilterOptions<TestEntity>();
        }

        [TestMethod]
        public void ThenTheBaseBuilderGetIndexFilterOptionsShouldBeCalledOnce()
        {
            MockBaseBuilder.Verify(b => b.GetIndexFilterOptions<TestEntity>(), Times.Once());
        }
    }
}
