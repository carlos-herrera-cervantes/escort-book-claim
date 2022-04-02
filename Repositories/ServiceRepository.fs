namespace EscortBookClaim.Repositories

open System
open System.Linq.Expressions
open MongoDB.Driver
open Microsoft.Extensions.Configuration
open EscortBookClaim.Models

type ServiceRepository(mongoClient: MongoClient, configuration: IConfiguration) =

    let database = configuration.GetSection("MongoDB").GetSection("Payment").Value
    
    member this._context = mongoClient.GetDatabase(database).GetCollection<Service>("services")

    interface IServiceRepository with

        member this.GetOneAsync(expression: Expression<Func<Service, bool>>) =
            this._context.Find(expression).FirstOrDefaultAsync()

        member this.UpdateOneAsync(id: string)(service: Service) =
            let filter = Builders<Service>.Filter.Eq((fun entity -> entity.Id), id)
            let options = ReplaceOptions(IsUpsert = true)

            this._context.ReplaceOneAsync(filter, service, options)
