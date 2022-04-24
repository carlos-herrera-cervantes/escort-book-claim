namespace EscortBookClaim.Repositories

open Microsoft.Extensions.Configuration
open MongoDB.Driver
open System.Linq.Expressions
open System
open EscortBookClaim.Models

type PhysicalEvidenceRepository (client: MongoClient, configuration: IConfiguration) =
    
    let database = configuration.GetSection("MongoDB").GetSection("Default").Value

    member this._maxEvidenceNumber = configuration.GetSection("Validators").GetSection("MaxEvidenceNumber").Value |> int64
    
    member this._context = client.GetDatabase(database).GetCollection<PhysicalEvidence>("physical-evidence")

    interface IPhysicalEvidenceRepository with

        member this.GetAllAsync(expression: Expression<Func<PhysicalEvidence, bool>>) =
            this._context.Find(expression).ToListAsync()

        member this.CreateAsync(physicalEvidence: PhysicalEvidence) = this._context.InsertOneAsync physicalEvidence

        member this.ValidateEvidenceNumber(claimId: string)(userId: string) =
            async {
                let! result = this._context.CountDocumentsAsync(fun e -> e.ClaimId = claimId && e.UserId = userId)
                            |> Async.AwaitTask
                return result < this._maxEvidenceNumber
            }
