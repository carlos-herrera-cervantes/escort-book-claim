namespace EscortBookClaim.Repositories

open MongoDB.Driver
open Microsoft.Extensions.Configuration
open EscortBookClaim.Models

type ServiceRepository(mongoClient: MongoClient, configuration: IConfiguration) =

    let database = configuration.GetSection("MongoDB").GetSection("Database").Value
    
    member this._context = mongoClient.GetDatabase(database).GetCollection<Service>("services")

    interface IServiceRepository with

        member this.GetByIdAsync(id: string) =
            this._context.Find(fun s -> s.Id = id).FirstOrDefaultAsync()
