namespace Claim.Web.Extensions

open Microsoft.Extensions.DependencyInjection
open MongoDB.Driver
open Claim.Web.Config

module MongoDBExtensions =

    type IServiceCollection with

        member this.AddMongoDBClient() =
            let client = MongoClient(MongoConfig.Host)
            this.AddSingleton<IMongoClient>(fun _ -> client :> IMongoClient) |> ignore
            this
