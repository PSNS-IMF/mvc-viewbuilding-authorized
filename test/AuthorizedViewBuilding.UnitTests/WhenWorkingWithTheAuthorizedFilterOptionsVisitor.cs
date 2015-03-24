using Microsoft.VisualStudio.TestTools.UnitTesting;

using Psns.Common.Test.BehaviorDrivenDevelopment;
using Psns.Common.Mvc.ViewBuilding.Authorized;

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
}
