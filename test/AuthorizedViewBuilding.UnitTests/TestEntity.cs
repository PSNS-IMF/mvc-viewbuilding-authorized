using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Psns.Common.Persistence.Definitions;
using Psns.Common.Mvc.ViewBuilding.Entities;

namespace AuthorizedViewBuilding.UnitTests
{
    internal class TestEntity : IIdentifiable, INameable
    {
        public int Id       { get; set; }
        public string Name  { get; set; }
    }
}
