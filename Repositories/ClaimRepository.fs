namespace EscortBookClaim.Repositories

open System
open System.Linq.Expressions
open Microsoft.Extensions.Configuration
open MongoDB.Driver
open EscortBookClaim.Models
open EscortBookClaim.Common

type ClaimRepository (client: MongoClient, configuration: IConfiguration) =

    let database = configuration.GetSection("MongoDB").GetSection("Default").Value
    
    member this._context = client.GetDatabase(database).GetCollection<Claim>("claim")

    interface IClaimRepository with

        member this.GetAllAsync(offset: int)(limit: int)(filters: string) =
            let buildedFilter = MongoDBDefinitions<Claim>.BuildFilter filters
            let page = if offset <= 1 then 0 else offset - 1

            this._context.Find(buildedFilter).Skip(Nullable (page * limit)).Limit(Nullable limit).ToListAsync()

        member this.GetOneAsync(expression: Expression<Func<Claim, bool>>) =
            this._context.Find(expression).FirstOrDefaultAsync()

        member this.CreateAsync(claim: Claim) = this._context.InsertOneAsync claim

        member this.UpdateOneAsync(id: string)(newClaim: Claim) =
            let filter = Builders<Claim>.Filter.Eq((fun entity -> entity.Id), id)
            let options = ReplaceOptions(IsUpsert = true)
            
            this._context.ReplaceOneAsync(filter, newClaim, options)
