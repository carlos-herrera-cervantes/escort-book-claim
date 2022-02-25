namespace EscortBookClaim.Repositories

open Microsoft.Azure.Cosmos
open Microsoft.Extensions.Configuration
open System.Collections.Generic
open System.Linq
open System
open EscortBookClaim.Models

type PhysicalEvidenceRepository (client: CosmosClient, configuration: IConfiguration) =
    
    member this._database = configuration.GetSection("CosmosDB").GetSection("Database").Value
    
    member this._container = client.GetContainer(this._database, "PhysicalEvidence")

    member this._maxEvidenceNumber = configuration["Validators:MaxEvidenceNumber"] |> int

    interface IPhysicalEvidenceRepository with

        member this.GetAllAsync() =
            async {
                let query = this._container.GetItemQueryIterator<PhysicalEvidence>()
                let results = List<PhysicalEvidence>()

                while (query.HasMoreResults) do
                    let! response = query.ReadNextAsync() |> Async.AwaitTask
                    results.AddRange(response.ToList())

                return results :> IEnumerable<PhysicalEvidence>
            }

        member this.CreateAsync(physicalEvidence: PhysicalEvidence) =
            async {
                let partitionKey = new PartitionKey(physicalEvidence.ClaimId)
                let! result = this._container.CreateItemAsync<PhysicalEvidence>(physicalEvidence, Nullable partitionKey)
                            |> Async.AwaitTask

                return result
            }

        member this.ValidateEvidenceNumber(claimId: string) =
            async {
                let! result = this._container.Scripts.ExecuteStoredProcedureAsync<int>("CountEvidence", new PartitionKey(claimId), [||])
                            |> Async.AwaitTask
                
                if result.Resource >= this._maxEvidenceNumber then
                    return false
                else
                    return true
            }
