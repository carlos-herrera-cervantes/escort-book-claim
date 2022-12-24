namespace Claim.Tests.Repositories

open Xunit
open System.ComponentModel
open System.Diagnostics.CodeAnalysis
open MongoDB.Driver
open Claim.Web.Repositories
open Claim.Web.Config
open Claim.Web.Models

[<Collection("DictumRepository")>]
[<Category("Repositories")>]
[<ExcludeFromCodeCoverage>]
type DictumRepositoryTests() =

    [<Fact(DisplayName = "Should return null")>]
    member this.GetOneAsyncShouldReturnNull() =
        async {
            let mongoClient = MongoClient(MongoConfig.Host)
            let dictumRepository = DictumRepository(mongoClient) :> IDictumRepository
            let filter = Builders<Dictum>.Filter.Empty

            let! dictum = dictumRepository.GetOneAsync(filter) |> Async.AwaitTask

            Assert.Null(dictum)
        }