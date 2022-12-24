namespace Claim.Web.Repositories

open System.Threading.Tasks
open Microsoft.AspNetCore.JsonPatch
open MongoDB.Driver
open Claim.Web.Models
open Claim.Web.Config

type DictumRepository (client: IMongoClient) =
    
    member this._context =
        client
            .GetDatabase(MongoConfig.Databases.ClaimDb)
            .GetCollection<Dictum>("dictum")

    interface IDictumRepository with

        member this.GetOneAsync(filter: FilterDefinition<Dictum>): Task<Dictum> =
            this._context.FindAsync(filter).Result.FirstOrDefaultAsync()

        member this.CreateAsync(dictum: Dictum): Task =
            this._context.InsertOneAsync dictum

        member this.UpdateOneAsync(id: string, newDictum: Dictum, currentDictum: JsonPatchDocument<Dictum>): Task =
            currentDictum.ApplyTo(newDictum)

            let filter = Builders<Dictum>.Filter.Eq((fun entity -> entity.Id), id)
            let options = ReplaceOptions(IsUpsert = true)

            this._context.ReplaceOneAsync(filter, newDictum, options) |> ignore
            Task.CompletedTask
