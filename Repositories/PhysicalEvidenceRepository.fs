namespace EscortBookClaim.Repositories

open Microsoft.Extensions.Configuration
open MongoDB.Driver
open EscortBookClaim.Models
open EscortBookClaim.Common

type PhysicalEvidenceRepository (client: MongoClient, configuration: IConfiguration) =
    
    let database = configuration.GetSection("MongoDB").GetSection("Default").Value

    member this._maxEvidenceNumber = configuration.GetSection("Validators").GetSection("MaxEvidenceNumber").Value |> int64
    
    member this._context = client.GetDatabase(database).GetCollection<PhysicalEvidence>("physical-evidence")

    interface IPhysicalEvidenceRepository with

        member this.GetAllAsync(filters: string) =
            let buildedFilter = MongoDBDefinitions<PhysicalEvidence>.BuildFilter filters
            this._context.Find(buildedFilter).ToListAsync()

        member this.CreateAsync(physicalEvidence: PhysicalEvidence) = this._context.InsertOneAsync physicalEvidence

        member this.ValidateEvidenceNumber(claimId: string) =
            async {
                let! result = this._context.CountDocumentsAsync(fun e -> e.ClaimId = claimId) |> Async.AwaitTask
                return result < this._maxEvidenceNumber
            }
