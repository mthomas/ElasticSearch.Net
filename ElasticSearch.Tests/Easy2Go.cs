using System;
using System.Collections.Generic;
using ElasticSearch;
using ElasticSearch.Client;
using ElasticSearch.Client.Config;
using ElasticSearch.Client.Domain;
using ElasticSearch.Client.QueryDSL;
using NUnit.Framework;

namespace Tests
{
    public class GameDataForList : GameData
    {
        public int Game { get; set; }
    }

    public class GameData
    {
        public string GameId { get; set; }

        public int CurrentTurn { get; set; }
        public int CurrentPlayerId { get; set; }

        public int Status { get; set; }
        public DateTime StartDate { get; set; }

        public DateTime? LastTurnDate { get; set; }
        public string LastTurnDescription { get; set; }

        public int? ResigningPlayerId { get; set; }
        public int? WinningPlayerId { get; set; }

        public DateTime? EndDate { get; set; }

        public int StartContext { get; set; }

        public int Version { get; set; }
        public int? TutorialType { get; set; }
    }

    [TestFixture]
    public class ScopelyIntegration
    {
        [Test]
        public void CanIndexGetGemsGame()
        {
            var id = Guid.NewGuid().ToString();

            var client = new ElasticSearchClient("ec2-107-22-42-34.compute-1.amazonaws.com", 9500, TransportType.Thrift);
            var data = new Dictionary<string, object> {{"GameId", "asdf"}};

            var bulkObject = new BulkObject("gems","gameindex" , id, data);

            client.Bulk(new List<BulkObject> {bulkObject});

            var loaded = client.Get("gems", "gameindex", id);

            Assert.IsNotNull(loaded);
            Assert.IsNotNull(loaded.GetFields());
            Assert.AreEqual("asdf", loaded.GetFields()["GameId"]);
        }

        [Test]
        public void CanScanDiceGames()
        {
            var client = new ElasticSearchClient("ec2-107-22-42-34.compute-1.amazonaws.com", 9500, TransportType.Thrift);

            var startDate = DateTime.UtcNow.AddYears(-10).ToString("o");
            var endDate = DateTime.UtcNow.ToString("o");

            var lastTurnDate = new RangeQuery("LastTurnDate", startDate, endDate, true, true);

            var results = client.Scan<GameDataForList>("dice", new[] {"gameindex"}, lastTurnDate, null, 10, "1m");

            int i = 0;

            foreach (var result in results)
            {
                i++;
                Console.WriteLine("{0}: {1}", i, result.GameId);

                Assert.IsNotNull(result.GameId);

                if(i == 100)
                {
                    return;
                }
            }
        }
    }

	[TestFixture]
	public class Easy2Go
	{
		[Test]
		public void SimpleTests()
		{
			var indexName = "myindex_" + Guid.NewGuid();
			var indexType = "type";

			var client = new ElasticSearchClient("localhost");

			var result = client.Index(indexName, indexType, "testkey", "{\"a\":\"b\",\"c\":\"d\"}");
			Assert.AreEqual(true, result.Success);

			client.Refresh(indexName);

			var doc = client.Search(indexName, indexType, "c:d");
			Console.WriteLine(doc.Response);
			Assert.AreEqual(1, doc.GetHits().Hits.Count);
			Assert.AreEqual("b", doc.GetHits().Hits[0].Source["a"]);

			client.Delete(indexName, indexType, "testkey");

			client.Refresh(indexName);

			var doc1 = client.Get(indexName, indexType, "testkey");
			Assert.AreEqual(null,doc1);

			client.DeleteIndex(indexName);
		}
	}
}
