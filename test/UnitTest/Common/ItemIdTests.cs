using Microsoft.VisualStudio.TestTools.UnitTesting;
using ServiceFabricContrib;
using System;

namespace UnitTest.Common
{
    [TestClass]
    public class ItemIdTests
    {
        [TestMethod]
        public void TestEqual()
        {
            ItemId target = null;
            Assert.IsTrue(target == null);
            var id = Guid.NewGuid();
            target = new ItemId(id);
            Assert.IsTrue(target != null);
            var target2 = new ItemId(id);
            Assert.AreEqual(target ,target2);
        }
    }
}
