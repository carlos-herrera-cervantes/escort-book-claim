namespace EscortBookClaim.Repositories

open System
open System.Linq
open Microsoft.Extensions.Configuration
open Microsoft.Azure.Cosmos
open EscortBookClaim.Models

type DictumRepository (client: CosmosClient, configuration: IConfiguration) =
    
    member this._database = configuration.GetSection("CosmosDB").GetSection("Database").Value
    
    member this._container = client.GetContainer(this._database, "Dictums")

    interface IDictumRepository with

        member this.GetOneAsync(partitionKey: string) =
            async {
                let query = this._container.GetItemQueryIterator<Dictum>(
                    "select * from c",
                    null,
                    QueryRequestOptions(PartitionKey = PartitionKey(partitionKey))
                )
                let! dictum = query.ReadNextAsync() |> Async.AwaitTask
                return dictum.Resource.FirstOrDefault()
            }

        member this.CreateAsync(dictum: Dictum) =
            async {
                let partitionKey = new PartitionKey(dictum.ClaimId)
                let! result = this._container.CreateItemAsync<Dictum>(dictum, Nullable partitionKey)
                            |> Async.AwaitTask

                return result
            }

        member this.UpdateOneAsync(id: string)(dictum: Dictum) =
            async {
                let partitionKey = new PartitionKey(id)
                let! result = this._container.UpsertItemAsync<Dictum>(dictum, Nullable partitionKey)
                            |> Async.AwaitTask

                return result
            }