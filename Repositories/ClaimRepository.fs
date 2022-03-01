namespace EscortBookClaim.Repositories

open System
open System.Collections.Generic
open Microsoft.Extensions.Configuration
open Microsoft.Azure.Cosmos
open System.Linq
open EscortBookClaim.Models

type ClaimRepository (client: CosmosClient, configuration: IConfiguration) =

    member this._database = configuration.GetSection("CosmosDB").GetSection("Database").Value

    member this._container = client.GetContainer(this._database, "Claims")

    interface IClaimRepository with

        member this.GetAllAsync(offset: int, limit: int) =
            async {
                let query = this._container.GetItemQueryIterator<Claim>()
                let results = List<Claim>()

                while (query.HasMoreResults) do
                    let! response = query.ReadNextAsync() |> Async.AwaitTask
                    results.AddRange(response.ToList())

                return results :> IEnumerable<Claim>
            }

        member this.GetOneAsync(id: string) =
            async {
                let! response = this._container.ReadItemAsync<Claim>(id, new PartitionKey(id)) |> Async.AwaitTask
                return response.Resource
            }

        member this.CreateAsync(claim: Claim) =
            async {
                let partitionKey = new PartitionKey(claim.Id)
                let! result = this._container.CreateItemAsync<Claim>(claim, Nullable partitionKey) |> Async.AwaitTask

                return result
            }

        member this.UpdateOneAsync(id: string)(claim: Claim) =
            async {
                let partitionKey = new PartitionKey(id)
                let! result = this._container.UpsertItemAsync<Claim>(claim, Nullable partitionKey)
                            |> Async.AwaitTask

                return result
            }
