using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Psns.Common.Test.BehaviorDrivenDevelopment;
using Psns.Common.Mvc.ViewBuilding.Authorized;
using Psns.Common.Mvc.ViewBuilding.ViewBuilders;
using Psns.Common.Mvc.ViewBuilding.ViewModels;
using Psns.Common.Mvc.ViewBuilding.ViewModels.TableModel;

using Microsoft.AspNet.Identity;

using System.Threading.Tasks;

using Moq;

namespace AuthorizedViewBuilding.UnitTests.VisitorTests
{
    public class WhenWorkingWithTheAuthorizedIndexVisitor : BehaviorDrivenDevelopmentCaseTemplate
    {
        protected AuthorizedIndexVisitor<User, int, TestEntity> Visitor;
        protected Mock<ICrudUserStore<User, int>> MockUserStore;

        public override void Arrange()
        {
            base.Arrange();

            MockUserStore = new Mock<ICrudUserStore<User, int>>();

            Visitor = new AuthorizedIndexVisitor<User, int, TestEntity>(MockUserStore.Object, new UserManager<User, int>(MockUserStore.Object));
        }
    }

    public class AndBuildingTheIndexView : WhenWorkingWithTheAuthorizedIndexVisitor
    {
        protected IndexView IndexView;

        public override void Arrange()
        {
            base.Arrange();

            MockUserStore.Setup(u => u.CurrentUser).Returns(new User { Id = 1 });
            MockUserStore.Setup(u => u.FindByIdAsync(It.IsAny<int>())).Returns(Task.FromResult(new User { Id = 1 }));
            MockUserStore.Setup(u => u.IsInRoleAsync(It.IsAny<User>(), It.IsAny<string>())).Returns(Task.FromResult(true));

            IndexView = new IndexView("model");
            IndexView.CreateButton = new ActionModel();
        }

        public override void Act()
        {
            base.Act();

            Visitor.Visit(IndexView);
        }
    }

    [TestClass]
    public class AndBuildingTheIndexViewWithCreateAccess : AndBuildingTheIndexView
    {
        [TestMethod]
        public void ThenTheCreateButtonShouldExist()
        {
            Assert.IsNotNull(IndexView.CreateButton);
        }
    }

    [TestClass]
    public class AndBuildingTheIndexViewWithoutCreateAccess : AndBuildingTheIndexView
    {
        public override void Arrange()
        {
            base.Arrange();

            MockUserStore.Setup(u => u.IsInRoleAsync(It.IsAny<User>(), It.IsAny<string>())).Returns(Task.FromResult(false));
        }

        [TestMethod]
        public void ThenTheCreateButtonShouldNotExistInContextItems()
        {
            Assert.IsNull(IndexView.CreateButton);
        }
    }

    [TestClass]
    public class AndBuildingTheIndexViewWithoutViewAccessOnASpecificEntity : AndBuildingTheIndexView
    {
        public override void Arrange()
        {
            base.Arrange();

            //MockUserStore.Setup(u => u.IsInRoleAsync(It.IsAny<User>(), It.IsAny<string>())).Returns(Task.FromResult(false));
            MockUserStore.Setup(u => u.CurrentUser).Returns(new User { Id = 1, UserName = "Randowm" });

            IndexView = new IndexView("modelName");
            var row = new Row(new TestEntity { Name = "Authorized" });
            IndexView.Table.Rows.Add(row);

            row = new Row(new TestEntity { Name = "Random" });
            IndexView.Table.Rows.Add(row);
        }

        public override void Act()
        {
            foreach(var row in IndexView.Table.Rows)
                Visitor.Visit(row);
        }

        [TestMethod]
        public void ThenTheEntityShouldNotBeInTheIndexViewsTable()
        {
            Assert.AreEqual(1, IndexView.Table.Rows.Count);
        }
    }
}
