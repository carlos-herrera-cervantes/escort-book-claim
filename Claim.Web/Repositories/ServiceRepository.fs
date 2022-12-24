namespace Claim.Web.Repositories

open System.Threading.Tasks
open MongoDB.Driver
open Claim.Web.Models
open Claim.Web.Config

type ServiceRepository(mongoClient: IMongoClient) =
    
    member this._context =
        mongoClient
            .GetDatabase(MongoConfig.Databases.PaymentDb)
            .GetCollection<Service>("services")

    interface IServiceRepository with

        member this.GetOneAsync(filter: FilterDefinition<Service>): Task<Service> =
            this._context.FindAsync(filter).Result.FirstOrDefaultAsync()

        member this.UpdateOneAsync(id: string, service: Service): Task<ReplaceOneResult> =
            let filter = Builders<Service>.Filter.Eq((fun entity -> entity.Id), id)
            let options = ReplaceOptions(IsUpsert = true)

            this._context.ReplaceOneAsync(filter, service, options)
