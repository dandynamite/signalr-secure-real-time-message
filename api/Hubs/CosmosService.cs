using System.Net;
using Microsoft.Azure.Cosmos;

namespace SignalRTest.Hubs;
public class CosmosService
{
    private readonly Container _container;

    public CosmosService(string connection)
    {
        CosmosClient client = new CosmosClient(connection);
        Database database = client.CreateDatabaseIfNotExistsAsync("test").Result;
        _container = database.GetContainer("connection_id");
    }

    public Task UpsertItem(string connection_id)
    {
        // Create an item
        dynamic testItem = new
        {
            id = "MyTestItemId",
            connection_id = connection_id
        };

        return _container.UpsertItemAsync(testItem);
    }

    public async Task<string> GetItem()
    {
        var res = await _container.ReadItemAsync<dynamic>("MyTestItemId", new PartitionKey("MyTestItemId"));

        if (res.StatusCode.Equals(HttpStatusCode.NotFound))
            return string.Empty;

        return res.Resource.connection_id;
    }
}
