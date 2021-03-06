﻿using Microsoft.VisualStudio.TestTools.UnitTesting;

using Psns.Common.Test.BehaviorDrivenDevelopment;
using Psns.Common.Mvc.ViewBuilding.Authorized;
using Psns.Common.Mvc.ViewBuilding.Authorized.Visitors;
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

            Visitor = new AuthorizedIndexVisitor<User, int, TestEntity>(MockUserStore.Object);
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

            MockUserStore.Setup(u => u.CurrentUser).Returns(new User { Id = 1, UserName = "Random" });

            var row = new Row(new TestEntity { Name = "Authorized" });
            IndexView.Table.Rows.Add(row);

            row = new Row(new TestEntity { Name = "Random" });
            IndexView.Table.Rows.Add(row);
        }

        public override void Act()
        {
            IndexView.Table.Accept(Visitor);
        }

        [TestMethod]
        public void ThenTheEntityShouldNotBeInTheIndexViewsTable()
        {
            Assert.AreEqual(1, IndexView.Table.Rows.Count);
            Assert.AreEqual("Authorized", (IndexView.Table.Rows[0].Source as TestEntity).Name);
        }
    }

    [TestClass]
    public class AndBuildingTheIndexViewWithoutReadAccessOnASpecificProperty : AndBuildingTheIndexView
    {
        public override void Arrange()
        {
            base.Arrange();

            MockUserStore.Setup(u => u.IsInRoleAsync(It.IsAny<User>(), It.IsAny<string>())).Returns(Task.FromResult(false));

            var entity = new TestEntity { RestrictedReadProperty = "Restricted", Name = TestEntity.AuthKey };
            var row = new Row(entity);

            row.Columns.Add(new Column(entity.GetType().GetProperty("RestrictedReadProperty")));
            row.Columns.Add(new Column(entity.GetType().GetProperty("LabeledProtectedProperty")) { Value = "Value" });

            IndexView.Table.Rows.Add(row);
        }

        public override void Act()
        {
            Visitor.Visit(IndexView.Table);
        }

        [TestMethod]
        public void ThenTheColumnForThePropertyShouldNotBePresentInTheViewModel()
        {
            Assert.AreEqual(1, IndexView.Table.Rows[0].Columns.Count);
            Assert.AreEqual("Value", IndexView.Table.Rows[0].Columns[0].Value);
        }
    }
}
