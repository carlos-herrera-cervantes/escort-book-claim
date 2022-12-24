namespace Claim.Tests.Repositories

open Xunit
open System.ComponentModel
open System.Diagnostics.CodeAnalysis
open MongoDB.Driver
open Claim.Web.Repositories
open Claim.Web.Config
open Claim.Web.Models

[<Collection("ClaimRepository")>]
[<Category("Repositories")>]
[<ExcludeFromCodeCoverage>]
type ClaimRepositoryTests() =

    [<Fact(DisplayName = "Should return an empty list when process succeeds")>]
    member this.GetAllAsyncShouldReturnEmptyList() =
        async {
            let mongoClient = MongoClient(MongoConfig.Host)
            let claimRepository = ClaimRepository(mongoClient) :> IClaimRepository
            let offset = 0
            let limit = 10

            let! result = claimRepository.GetAllAsync(offset, limit)

            Assert.Empty(result)
        }

    [<Fact(DisplayName = "Should return an empty list when process succeeds")>]
    member this.GetAllByFilterAsyncShouldReturnEmptyList() =
        async {
            let mongoClient = MongoClient(MongoConfig.Host)
            let claimRepository = ClaimRepository(mongoClient) :> IClaimRepository
            let offset = 0
            let limit = 10
            let filter = Builders<Claim>.Filter.Eq((fun c -> c.Id), "63a0051738746ba00913a5b7")

            let! result = claimRepository.GetAllByFilterAsync(filter, offset, limit)

            Assert.Empty(result)
        }

    [<Fact(DisplayName = "Should return null when document does not exists")>]
    member this.GetOneAsyncShouldReturnNull() =
        async {
            let mongoClient = MongoClient(MongoConfig.Host)
            let claimRepository = ClaimRepository(mongoClient) :> IClaimRepository
            let filter = Builders<Claim>.Filter.Eq((fun c -> c.Id), "63a0051738746ba00913a5b7")

            let! result = claimRepository.GetOneAsync(filter) |> Async.AwaitTask

            Assert.Null(result)
        }
