namespace EscortBookClaim.Repositories

open System
open Microsoft.Extensions.Configuration
open Microsoft.Azure.Cosmos
open EscortBookClaim.Models

type DictumRepository (client: CosmosClient, configuration: IConfiguration) =
    
    member this._database = configuration.GetSection("CosmosDB").GetSection("Database").Value
    
    member this._container = client.GetContainer(this._database, "Dictums")

    interface IDictumRepository with

        member this.GetOneAsync(id: string) =
            async {
                let! response = this._container.ReadItemAsync<Dictum>(id, new PartitionKey(id))
                             |> Async.AwaitTask
                return response.Resource
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