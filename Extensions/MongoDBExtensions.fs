namespace EscortBookClaim.Extensions

open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Configuration
open MongoDB.Driver

module MongoDBExtensions =

    type IServiceCollection with

        member this.AddMongoDBClient(configuration: IConfiguration) =
            let connectionString = configuration
                                    .GetSection("MongoDB")
                                    .GetSection("host").Value
            let client = MongoClient(connectionString)

            this.AddSingleton<MongoClient>(fun _ -> client) |> ignore
            this
