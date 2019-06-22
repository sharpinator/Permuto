using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using Gremlin.Net.Driver;
using Permuto.CxC.Exchange.Data;
using Permuto.CxC.Exchange.Entities;
using System.Linq;

namespace Permuto.CxC.Exchange.Data.Tests
{
    /// <summary>
    /// Summary description for CommodityIntegrationTests
    /// </summary>
    [TestClass]
    public class CommodityIntegrationTests
    {
        public CommodityIntegrationTests()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public async Task IntegrationTest_AddCommodity()
        {
            GremlinServer server = new GremlinServer("permuto.gremlin.cosmos.azure.com", 443, true, "/dbs/CxC/colls/Exchange", "");
            GremlinClientFactory factory = new GremlinClientFactory(server);
            CommodityDataAdapter da = new CommodityDataAdapter(factory);
            Commodity c = new Commodity() { id = Guid.NewGuid().ToString(), Name = "Aluminum", Symbol = "AL" };
            await da.Add(c);
            var newC = await da.Get(c.id);
            Assert.IsNotNull(newC);
        }
        [TestMethod]
        public async Task IntegrationTest_GetCommodities()
        {
            GremlinServer server = new GremlinServer("permuto.gremlin.cosmos.azure.com", 443, true, "/dbs/CxC/colls/Exchange", "");
            GremlinClientFactory factory = new GremlinClientFactory(server);
            CommodityDataAdapter da = new CommodityDataAdapter(factory);
            var commodities = await da.GetAll();
            Assert.AreNotEqual(0, commodities.Count());
        }
    }
}
