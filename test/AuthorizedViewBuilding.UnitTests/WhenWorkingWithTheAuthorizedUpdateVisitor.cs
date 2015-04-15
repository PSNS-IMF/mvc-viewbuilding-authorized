using Microsoft.VisualStudio.TestTools.UnitTesting;

using Psns.Common.Mvc.ViewBuilding.Authorized;
using Psns.Common.Mvc.ViewBuilding.Authorized.Attributes;
using Psns.Common.Mvc.ViewBuilding.Authorized.Visitors;
using Psns.Common.Mvc.ViewBuilding.ViewModels;

using Psns.Common.Test.BehaviorDrivenDevelopment;

using Moq;

using Microsoft.AspNet.Identity;
using System.Threading.Tasks;

namespace AuthorizedViewBuilding.UnitTests
{
    public class WhenWorkingWithTheAuthorizedUpdateVisitor : BehaviorDrivenDevelopmentCaseTemplate
    {
        protected AuthorizedUpdateVisitor<User, int> Visitor;
        protected Mock<ICrudUserStore<User, int>> MockUserStore;
        protected InputProperty ToVisit;
        protected InputProperty Visited;

        public override void Arrange()
        {
            base.Arrange();

            MockUserStore = new Mock<ICrudUserStore<User, int>>();
            MockUserStore.Setup(s => s.CurrentUser).Returns(new User { Id = 1 });
            MockUserStore.Setup(u => u.FindByIdAsync(It.IsAny<int>())).Returns(Task.FromResult(new User { Id = 1 }));

            Visitor = new AuthorizedUpdateVisitor<User, int>(MockUserStore.Object);
        }

        public override void Act()
        {
            base.Act();

            Visited = Visitor.Visit(ToVisit);
        }
    }

    [TestClass]
    public class AndAccessingAProtectedFieldAsAUserInTheRequiredRoles : WhenWorkingWithTheAuthorizedUpdateVisitor
    {
        public override void Arrange()
        {
            base.Arrange();

            MockUserStore.Setup(s => s.IsInRoleAsync(It.IsAny<User>(), It.IsAny<string>())).Returns(Task.FromResult(true));

            ToVisit = new InputProperty
            {
                Source = new TestEntity(),
                Label = "ProtectedProperty"
            };
        }

        [TestMethod]
        public void ThenTheInputPropertyShouldBeReturned()
        {
            Assert.AreEqual(Visited.Label, ToVisit.Label);
        }
    }

    [TestClass]
    public class AndAccessingAProtectedFieldAsAUserNotInTheRequireRoles : WhenWorkingWithTheAuthorizedUpdateVisitor
    {
        public override void Arrange()
        {
            base.Arrange();

            MockUserStore.Setup(s => s.IsInRoleAsync(It.IsAny<User>(), It.IsAny<string>())).Returns(Task.FromResult(false));

            ToVisit = new InputProperty
            {
                Source = new TestEntity(),
                Label = "ProtectedProperty"
            };
        }

        [TestMethod]
        public void ThenTheInputPropertyShouldBeReturnedNull()
        {
            Assert.IsNull(Visited);
        }
    }

    [TestClass]
    public class AndAccessingANonProtectedField : WhenWorkingWithTheAuthorizedUpdateVisitor
    {
        public override void Arrange()
        {
            base.Arrange();

            ToVisit = new InputProperty
            {
                Source = new TestEntity(),
                Label = "Name"
            };
        }

        [TestMethod]
        public void ThenTheInputPropertyShouldBeReturned()
        {
            Assert.AreEqual(Visited.Label, ToVisit.Label);
        }
    }

    [TestClass]
    public class AndAccessingANonSupportedProtectedField : WhenWorkingWithTheAuthorizedUpdateVisitor
    {
        public override void Arrange()
        {
            base.Arrange();

            ToVisit = new InputProperty
            {
                Source = new TestEntity(),
                Label = "UnsupportedProtectedProperty"
            };
        }

        [TestMethod]
        public void ThenTheInputPropertyShouldBeReturned()
        {
            Assert.AreEqual(Visited.Label, ToVisit.Label);
        }
    }

    [TestClass]
    public class AndAccessingAProtectedFieldWithADisplayAttributeAsAUserInTheRequiredRoles : WhenWorkingWithTheAuthorizedUpdateVisitor
    {
        public override void Arrange()
        {
            base.Arrange();

            MockUserStore.Setup(s => s.IsInRoleAsync(It.IsAny<User>(), It.IsAny<string>())).Returns(Task.FromResult(true));

            ToVisit = new InputProperty
            {
                Source = new TestEntity(),
                Label = "Labeled Protected Property"
            };
        }

        [TestMethod]
        public void ThenTheInputPropertyShouldBeReturned()
        {
            Assert.AreEqual(Visited.Label, ToVisit.Label);
        }
    }

    [TestClass]
    public class AndAccessingAProtectedFieldWithANonMatchingDisplayAttributeAsAUserInTheRequiredRoles : WhenWorkingWithTheAuthorizedUpdateVisitor
    {
        public override void Arrange()
        {
            base.Arrange();

            MockUserStore.Setup(s => s.IsInRoleAsync(It.IsAny<User>(), It.IsAny<string>())).Returns(Task.FromResult(true));

            ToVisit = new InputProperty
            {
                Source = new TestEntity(),
                Label = "Non Existing Label Name"
            };
        }

        [TestMethod]
        public void ThenTheInputPropertyShouldBeReturned()
        {
            Assert.AreEqual(Visited.Label, ToVisit.Label);
        }
    }
}
