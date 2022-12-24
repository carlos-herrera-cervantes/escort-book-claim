namespace Claim.Web.Repositories

open System.Collections.Generic
open System.Threading.Tasks
open MongoDB.Driver
open Claim.Web.Models
open Claim.Web.Config

type PhysicalEvidenceRepository (client: IMongoClient) =
    
    member this._context =
        client
            .GetDatabase(MongoConfig.Databases.ClaimDb)
            .GetCollection<PhysicalEvidence>("physical-evidence")

    interface IPhysicalEvidenceRepository with

        member this.GetAllAsync(filter: FilterDefinition<PhysicalEvidence>): Async<IEnumerable<PhysicalEvidence>> =
            async {
                let! result = this._context.FindAsync(filter).Result.ToListAsync() |> Async.AwaitTask
                return result :> IEnumerable<PhysicalEvidence>
            }

        member this.CreateAsync(physicalEvidence: PhysicalEvidence): Task =
            this._context.InsertOneAsync physicalEvidence

        member this.ValidateEvidenceNumber(claimId: string, userId: string): Async<bool> =
            async {
                let! totalEvidence =
                    this._context.CountDocumentsAsync(
                        fun e -> e.ClaimId = claimId && e.UserId = userId
                    ) |> Async.AwaitTask
                let maxEvidence  = int64(5)
                return totalEvidence < maxEvidence
            }
