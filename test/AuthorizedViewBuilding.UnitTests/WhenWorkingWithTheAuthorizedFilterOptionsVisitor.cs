using Microsoft.VisualStudio.TestTools.UnitTesting;

using Psns.Common.Test.BehaviorDrivenDevelopment;
using Psns.Common.Mvc.ViewBuilding.Authorized;
using Psns.Common.Mvc.ViewBuilding.Authorized.Visitors;

using System.Reflection;
using System.Threading.Tasks;

using Moq;

namespace AuthorizedViewBuilding.UnitTests.FilterOptionsVisitoTests
{
    public class WhenWorkingWithTheFilterOptionsVisitor : BehaviorDrivenDevelopmentCaseTemplate
    {
        protected AuthorizedFilterOptionsVisitor<User, int> Visitor;
        protected Mock<ICrudUserStore<User, int>> MockUserStore;
        protected TestEntity Subject;

        public override void Arrange()
        {
            base.Arrange();

            MockUserStore = new Mock<ICrudUserStore<User, int>>();
            MockUserStore.Setup(u => u.CurrentUser).Returns(new User { Id = 1 });

            Visitor = new AuthorizedFilterOptionsVisitor<User, int>(MockUserStore.Object);
        }

        public override void Act()
        {
            base.Act();

            Subject = Visitor.Visit(Subject) as TestEntity;
        }
    }

    [TestClass]
    public class AndVisitingAnItemWithPermissions : WhenWorkingWithTheFilterOptionsVisitor
    {
        public override void Arrange()
        {
            base.Arrange();

            Subject = new TestEntity { Id = 1, Name = "Authorized" };
        }

        [TestMethod]
        public void ThenTheOriginalItemShouldBeReturned()
        {
            Assert.AreEqual(1, Subject.Id);
        }
    }

    [TestClass]
    public class AndVisitingAnItemWithoutPermissions : WhenWorkingWithTheFilterOptionsVisitor
    {
        public override void Arrange()
        {
            base.Arrange();

            Subject = new TestEntity { Id = 1, Name = "Unauthorized" };
        }

        [TestMethod]
        public void ThenTheSubjectShouldBeNull()
        {
            Assert.IsNull(Subject);
        }
    }

    [TestClass]
    public class AndVisitingAPropertyOfAnItemWithoutReadAccess : WhenWorkingWithTheFilterOptionsVisitor
    {
        PropertyInfo _result;

        public override void Arrange()
        {
            base.Arrange();

            MockUserStore.Setup(s => s.FindByIdAsync(It.IsAny<int>())).Returns(Task.FromResult(new User { Id = 1 }));
            MockUserStore.Setup(s => s.IsInRoleAsync(It.IsAny<User>(), It.IsAny<string>())).Returns(Task.FromResult(false));
        }

        public override void Act()
        {
            base.Act();

            _result = Visitor.Visit(new TestEntity().GetType().GetProperty("RestrictedReadProperty"));
        }

        [TestMethod]
        public void ThenNullShouldBeReturned()
        {
            Assert.IsNull(_result);
        }
    }

    [TestClass]
    public class AndVisitingAPropertyOfAnItemWithReadAccess : WhenWorkingWithTheFilterOptionsVisitor
    {
        PropertyInfo _result;

        public override void Arrange()
        {
            base.Arrange();

            MockUserStore.Setup(s => s.FindByIdAsync(It.IsAny<int>())).Returns(Task.FromResult(new User { Id = 1 }));
            MockUserStore.Setup(s => s.IsInRoleAsync(It.IsAny<User>(), It.IsAny<string>())).Returns(Task.FromResult(true));
        }

        public override void Act()
        {
            base.Act();

            _result = Visitor.Visit(new TestEntity().GetType().GetProperty("RestrictedReadProperty"));
        }

        [TestMethod]
        public void ThenTheOriginalPropertyShouldBeReturned()
        {
            Assert.AreEqual("RestrictedReadProperty", _result.Name);
        }
    }
}
