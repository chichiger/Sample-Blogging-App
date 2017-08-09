using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Web1.Controllers
{
    [TestClass]
    public class Test
    {
        [TestInitialize]
        public void SetUp()
        {
            
        }

        [TestCleanup]
        public void Cleanup()
        {
            
        }

        [TestMethod]
        public void Test1()
        {
            Assert.IsTrue(true);
            Console.WriteLine(("testing"));
            // Assert.IsTrue((boolean check), "message")
        }
    }
}
