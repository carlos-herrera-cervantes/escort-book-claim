namespace EscortBookClaim.Repositories

open System
open Microsoft.AspNetCore.JsonPatch
open System.Linq.Expressions
open MongoDB.Driver
open Microsoft.Extensions.Configuration
open EscortBookClaim.Models

type DictumRepository (client: MongoClient, configuration: IConfiguration) =
    
    let database = configuration.GetSection("MongoDB").GetSection("Default").Value
    
    member this._context = client.GetDatabase(database).GetCollection<Dictum>("dictum")

    interface IDictumRepository with

        member this.GetOneAsync(expression: Expression<Func<Dictum, bool>>) =
            this._context.Find(expression).FirstOrDefaultAsync()

        member this.CreateAsync(dictum: Dictum) = this._context.InsertOneAsync dictum

        member this.UpdateOneAsync(id: string)(newDictum: Dictum)(currentDictum: JsonPatchDocument<Dictum>) =
            currentDictum.ApplyTo(newDictum)

            let filter = Builders<Dictum>.Filter.Eq((fun entity -> entity.Id), id)
            let options = ReplaceOptions(IsUpsert = true)

            this._context.ReplaceOneAsync(filter, newDictum, options)
