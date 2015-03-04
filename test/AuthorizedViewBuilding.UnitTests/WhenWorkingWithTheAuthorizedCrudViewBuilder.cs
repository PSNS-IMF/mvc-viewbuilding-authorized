using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Psns.Common.Test.BehaviorDrivenDevelopment;
using Psns.Common.Mvc.ViewBuilding.Authorized;
using Psns.Common.Mvc.ViewBuilding.ViewBuilders;
using Psns.Common.Mvc.ViewBuilding.ViewModels;

using Moq;

namespace AuthorizedViewBuilding.UnitTests
{
    public class WhenWorkingWithTheAuthorizedCrudViewBuilder : BehaviorDrivenDevelopmentCaseTemplate
    {
        protected AuthorizedCrudViewBuilder ViewBuilder;
        protected Mock<ICrudViewBuilder> MockBaseBuilder;

        public override void Arrange()
        {
            base.Arrange();

            MockBaseBuilder = new Mock<ICrudViewBuilder>();

            ViewBuilder = new AuthorizedCrudViewBuilder(MockBaseBuilder.Object);
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
}
