using Microsoft.VisualStudio.TestTools.UnitTesting;

using Psns.Common.Test.BehaviorDrivenDevelopment;
using Psns.Common.Mvc.ViewBuilding.Authorized;
using Psns.Common.Mvc.ViewBuilding.Authorized.Visitors;
using Psns.Common.Mvc.ViewBuilding.ViewModels;
using Psns.Common.Mvc.ViewBuilding.ViewModels.TableModel;

using System.Threading.Tasks;

using Moq;

namespace AuthorizedViewBuilding.UnitTests.DetailsVisitorTests
{
    public class WhenWorkingWithTheAuthorizedDetailsVisitor : BehaviorDrivenDevelopmentCaseTemplate
    {
        protected AuthorizedDetailsVisitor<User, int> Visitor;
        protected Mock<ICrudUserStore<User, int>> MockUserStore;
        protected DetailsView DetailsView;

        public override void Arrange()
        {
            base.Arrange();

            MockUserStore = new Mock<ICrudUserStore<User, int>>();
            MockUserStore.Setup(u => u.CurrentUser).Returns(new User { Id = 1 });
            MockUserStore.Setup(u => u.FindByIdAsync(It.IsAny<int>())).Returns(Task.FromResult(new User { Id = 1 }));

            DetailsView = new DetailsView();

            Visitor = new AuthorizedDetailsVisitor<User, int>(MockUserStore.Object);
        }

        public override void Act()
        {
            base.Act();

            Visitor.Visit(DetailsView);
        }
    }

    [TestClass]
    public class AndBuildingTheDetailsViewWithoutViewAccessOnASpecificEntity : WhenWorkingWithTheAuthorizedDetailsVisitor
    {
        public override void Arrange()
        {
            base.Arrange();

            MockUserStore.Setup(u => u.IsInRoleAsync(It.IsAny<User>(), It.IsAny<string>())).Returns(Task.FromResult(false));

            DetailsView.Table.Rows.Add(new Row(new TestEntity { Name = TestEntity.AuthKey }));
            DetailsView.Table.Rows.Add(new Row(new TestEntity { Name = "Unauthorized" }));
        }

        public override void Act()
        {
            DetailsView.Table.Accept(Visitor);
        }

        [TestMethod]
        public void ThenTheRestrictedRowShouldNotBePresentInTheDetailsView()
        {
            Assert.AreEqual(1, DetailsView.Table.Rows.Count);
            Assert.AreEqual(TestEntity.AuthKey, (DetailsView.Table.Rows[0].Source as TestEntity).Name);
        }
    }

    [TestClass]
    public class AndBuildingTheDetailsViewWithoutViewAccessOnASpecificEntityProperty : WhenWorkingWithTheAuthorizedDetailsVisitor
    {
        public override void Arrange()
        {
            base.Arrange();

            MockUserStore.Setup(u => u.IsInRoleAsync(It.IsAny<User>(), It.IsAny<string>())).Returns(Task.FromResult(false));

            var entity = new TestEntity { Name = TestEntity.AuthKey };
            var row = new Row(entity);

            row.Columns.Add(new Column(entity.GetType().GetProperty("RestrictedReadProperty")));
            row.Columns.Add(new Column(entity.GetType().GetProperty("LabeledProtectedProperty")) { Value = "value" });

            DetailsView.Table.Rows.Add(row);
        }

        public override void Act()
        {
            DetailsView.Table.Accept(Visitor);
        }

        [TestMethod]
        public void ThenTheRestrictedColumnShouldNotBePresentInTheDetailsView()
        {
            Assert.AreEqual(1, DetailsView.Table.Rows[0].Columns.Count);
            Assert.AreEqual("value", DetailsView.Table.Rows[0].Columns[0].Value);
        }
    }
}
