
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace somReporter
{
    [TestClass()]
    public class TestRecordIndex
    {

        [TestInitialize()]
        public void Initialize()
        {
            RecordIndex.resetIndex(RecordIndex.INDEX.TestTeamId);
        }

        [TestCleanup()]
        public void Cleanup() { }

        [TestMethod]
        public void testCRUDActions()
        {
            Assert.AreEqual(1, RecordIndex.getNextId(RecordIndex.INDEX.TestTeamId), "Initialize should reset back to one");
            Assert.AreEqual(2, RecordIndex.getNextId(RecordIndex.INDEX.TestTeamId), "The previous call should increment");
            Assert.AreEqual(3, RecordIndex.getNextId(RecordIndex.INDEX.TestTeamId), "The previous call should increment");

            RecordIndex.resetIndex(RecordIndex.INDEX.TestTeamId);
            Assert.AreEqual(1, RecordIndex.getNextId(RecordIndex.INDEX.TestTeamId), "reset back to one");
        }

    }
}
