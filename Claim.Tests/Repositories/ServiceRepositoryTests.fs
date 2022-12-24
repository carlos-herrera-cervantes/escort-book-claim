namespace Claim.Tests.Repositories

open Xunit
open System.ComponentModel
open System.Diagnostics.CodeAnalysis
open MongoDB.Driver
open Claim.Web.Repositories
open Claim.Web.Config
open Claim.Web.Models

[<Collection("ServiceRepository")>]
[<Category("Repositories")>]
[<ExcludeFromCodeCoverage>]
type ServiceRepositoryTests() =

    [<Fact(DisplayName = "Should return null")>]
    member this.GetOneAsyncShouldReturnNull() =
        async {
            let mongoClient = MongoClient(MongoConfig.Host)
            let serviceRepository = ServiceRepository(mongoClient) :> IServiceRepository
            let filter = Builders<Service>.Filter.Empty

            let! service = serviceRepository.GetOneAsync(filter) |> Async.AwaitTask

            Assert.Null(service)
        }
