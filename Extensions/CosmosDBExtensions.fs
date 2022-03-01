namespace EscortBookClaim.Extensions

open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Configuration
open Microsoft.Azure.Cosmos

module CosmosDBExtensions =

    type IServiceCollection with

        member this.AddCosmosDBClient(configuration: IConfiguration) =
            let cosmosDBSection = configuration.GetSection("CosmosDB")
            let account = cosmosDBSection.GetSection("Account").Value
            let key = cosmosDBSection.GetSection("Key").Value

            let client = new CosmosClient(account, key)

            this.AddSingleton<CosmosClient>(fun _ -> client) |> ignore
            this