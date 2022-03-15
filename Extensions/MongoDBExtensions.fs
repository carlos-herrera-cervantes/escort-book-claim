namespace EscortBookClaim.Extensions

open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Configuration
open MongoDB.Driver

module MongoDBExtensions =

    type IServiceCollection with

        member this.AddMongoDBClient(configuration: IConfiguration) =
            let connectionString = configuration
                                    .GetSection("MongoDB")
                                    .GetSection("ConnectionString").Value
            let client = MongoClient(connectionString)

            this.AddSingleton<MongoClient>(fun _ -> client) |> ignore
            this
