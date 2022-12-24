namespace Claim.Web.Repositories

open System
open System.Collections.Generic
open System.Threading.Tasks
open MongoDB.Driver
open Claim.Web.Models
open Claim.Web.Config

type ClaimRepository (client: IMongoClient) =
    
    member this._context =
        client
            .GetDatabase(MongoConfig.Databases.ClaimDb)
            .GetCollection<Claim>("claim")

    interface IClaimRepository with

        member this.GetAllAsync(offset: int, limit: int): Async<IEnumerable<Claim>> =
            async {
                let page = if offset <= 1 then 0 else offset - 1
                let! result =
                    this._context
                        .Find(Builders<Claim>.Filter.Empty)
                        .Skip(Nullable (page * limit))
                        .Limit(Nullable limit)
                        .ToListAsync() |> Async.AwaitTask
                return result :> IEnumerable<Claim>
            }

        member this.GetAllByFilterAsync
            (
                filter: FilterDefinition<Claim>,
                offset: int,
                limit: int
            ): Async<IEnumerable<Claim>> =
            async {
                let page = if offset <= 1 then 0 else offset - 1
                let! result =
                    this._context
                        .Find(filter)
                        .Skip(Nullable (page * limit))
                        .Limit(Nullable limit)
                        .ToListAsync() |> Async.AwaitTask
                return result :> IEnumerable<Claim>
            }

        member this.GetOneAsync(filter: FilterDefinition<Claim>): Task<Claim> =
            this._context.FindAsync(filter).Result.FirstOrDefaultAsync()

        member this.CreateAsync(claim: Claim): Task =
            this._context.InsertOneAsync claim

        member this.UpdateOneAsync(id: string, newClaim: Claim): Task =
            let filter = Builders<Claim>.Filter.Eq((fun entity -> entity.Id), id)
            let options = ReplaceOptions(IsUpsert = true)
            
            this._context.ReplaceOneAsync(filter, newClaim, options) |> ignore
            Task.CompletedTask
