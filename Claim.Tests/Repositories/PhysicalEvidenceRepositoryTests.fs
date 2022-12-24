namespace Claim.Tests.Repositories

open Xunit
open System.ComponentModel
open System.Diagnostics.CodeAnalysis
open MongoDB.Driver
open Claim.Web.Repositories
open Claim.Web.Config
open Claim.Web.Models

[<Collection("PhysicalEvidenceRepository")>]
[<Category("Repositories")>]
[<ExcludeFromCodeCoverage>]
type PhysicalEvidenceRepositoryTests() =

    [<Fact(DisplayName = "Should return empty enumerable")>]
    member this.GetAllAsyncShouldReturnEmptyEnumerable() =
        async {
            let mongoClient = MongoClient(MongoConfig.Host)
            let physicalEvidenceRepository = PhysicalEvidenceRepository(mongoClient) :> IPhysicalEvidenceRepository
            let filter = Builders<PhysicalEvidence>.Filter.Empty

            let! evidences = physicalEvidenceRepository.GetAllAsync(filter)

            Assert.Empty(evidences)
        }

    member this.ValidateEvidenceNumberShouldReturnTrue() =
        async {
            let mongoClient = MongoClient(MongoConfig.Host)
            let physicalEvidenceRepository = PhysicalEvidenceRepository(mongoClient) :> IPhysicalEvidenceRepository
            let claimId = "63a0a7d3b7f8d0b95b3b6b60"
            let userId = "63a0a7d98eb16ac3f10c5998"

            let! limitNotExceeded = physicalEvidenceRepository.ValidateEvidenceNumber(claimId, userId)

            Assert.True(limitNotExceeded)
        }
