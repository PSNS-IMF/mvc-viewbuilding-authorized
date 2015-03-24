using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Psns.Common.Test.BehaviorDrivenDevelopment;
using Psns.Common.Mvc.ViewBuilding.Authorized;
using Psns.Common.Mvc.ViewBuilding.ViewBuilders;
using Psns.Common.Mvc.ViewBuilding.ViewModels;

using Microsoft.AspNet.Identity;

using System.Threading.Tasks;

using Moq;

namespace AuthorizedViewBuilding.UnitTests.BuilderTests
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
    public class AndBuildingTheIndexView : WhenWorkingWithTheAuthorizedCrudViewBuilder
    {
        public override void Act()
        {
            base.Act();

            ViewBuilder.BuildIndexView<TestEntity>(null, null, null, null, null, null, null, null);
        }

        [TestMethod]
        public void ThenTheBaseBuilderShouldBeCalledWithTHeAuthorizedIndexViewVisitor()
        {
            MockBaseBuilder.Verify(b => b.BuildIndexView<TestEntity>(null, null, null, null, null, null, null, 
                It.Is<IIndexViewVisitor[]>(array => array[0] is AuthorizedIndexVisitor<User, int, TestEntity>)), Times.Once());
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
            MockBaseBuilder.Verify(b => b.GetIndexFilterOptions<TestEntity>(
                It.Is<IFilterOptionVisitor[]>(array => array[0] is AuthorizedFilterOptionsVisitor<User, int>)), Times.Once()
            );
        }
    }
}
