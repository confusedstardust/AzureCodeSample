using Azure.Identity;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Fluent;
using Microsoft.Extensions.Logging;
using System;

namespace AzureCodeSample.Service
{
    public class CosmosService
    {
        public CosmosClient GetClient(ILogger logger)
        {
            try
            {
                var options = new CosmosClientOptions() { ConnectionMode = ConnectionMode.Gateway };
                var cosmosDbEndpoint = Environment.GetEnvironmentVariable("CosmosDBEndpoint");
              
              //Method1 generate CosmosClient with accessKey
                string accessKey = Environment.GetEnvironmentVariable("CosmosDBAccessKey");
                CosmosClientBuilder cosmosClientBuilder = new CosmosClientBuilder(cosmosDbEndpoint, accessKey);
                return cosmosClientBuilder.WithConnectionModeGateway().Build();
              
              //Method2 generate CosmosClient with Managed Idenity
                var client = new CosmosClient(cosmosDbEndpoint, new DefaultAzureCredential(), options);
                return client;
            }
            catch (Exception ex)
            {
                logger.LogInformation($"info:{ex}");
                return null;
            }
        }

        public async GetCosmosItemBySQL(string databaseID,string containerID)
        {
            string sqlStatement="";
            Container container = cosmosClient.GetContainer(databaseId, containerId);
            var iterator = container.GetItemQueryIterator<MyModel>(queryText: sqlStatement);
            while (iterator.HasMoreResults)
                {
                    foreach (var val in await iterator.ReadNextAsync())
                    {
                      //add some needed logic
                    }
                }
        }

        public async GetCosmosItemByLinq(string databaseID,string containerID)
        {
            string ID="";
            Container container = cosmosClient.GetContainer(databaseId, containerID);
            var iterator=container.GetItemLinqQueryable<MyModel>()
                .Where(x=>x.Val.startWith(ID))
                .Select(x => new
                {
                    //what I want to select
                })
                .ToFeedIterator();
            while (iterator.HasMoreResults)
                {
                    foreach (var val in await iterator.ReadNextAsync())
                    {
                      //add some needed logic
                    }
                }
        }
        public async Task<bool> UpdateCosmosItem(string databaseID,string containerID)
        {
            string ID="";
            Container container = cosmosClient.GetContainer(databaseId, containerID);
            MyModel modelItem= new MyModel
            {
              //add attrs
            };
          //add
            var result= await container.CreateItemAsync<MyModel>(modelItem, new PartitionKey(modelItem.ID));
          //update
            var result = await container.ReplaceItemAsync(item: modelItem, id: modelItem.id, partitionKey: new PartitionKey(modelItem.ID));
          //update partilly
            var result = await container.PatchItemAsync<MyModel>(
            id: modelItem.id,
            partitionKey: new PartitionKey(modelItem.ID),
            patchOperations: new[]{
              //replace single attr
              PatchOperation.Replace($"/Attr1", "2"),
              //add a object
              PatchOperation.Add($"/Attr2", new MyModel{
                            //attr here
              })
            });
            return result.StatusCode==HttpStatusCode.OK?true:false;
        }
}
