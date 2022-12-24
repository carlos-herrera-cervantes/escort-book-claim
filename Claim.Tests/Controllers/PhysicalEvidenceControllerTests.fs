namespace Claim.Tests.Controllers

open Xunit
open System.ComponentModel
open System.Diagnostics.CodeAnalysis
open System.Collections.Generic
open System.Linq
open System.IO
open System.Threading.Tasks
open Microsoft.AspNetCore.Http
open Microsoft.AspNetCore.Mvc
open Moq
open MongoDB.Driver
open Claim.Web.Controllers
open Claim.Web.Repositories
open Claim.Web.Services
open Claim.Web.Models
open Claim.Web.Config

[<Collection("PhysicalEvidenceController")>]
[<Category("Controllers")>]
[<ExcludeFromCodeCoverage>]
type PhysicalEvidenceControllerTests() =

    [<Fact(DisplayName = "GetAllAsync - Should return 200 when process succeeds")>]
    member this.GetAllAsyncShouldReturn200(): Async<unit> =
        async {
            let mockPhysicalEvidenceRepository = Mock<IPhysicalEvidenceRepository>()
            let mockS3Service = Mock<IAWSS3Service>()
            let physicalEvidenceController =
                PhysicalEvidenceController(
                    mockPhysicalEvidenceRepository.Object,
                    mockS3Service.Object
                )

            mockPhysicalEvidenceRepository
                .Setup(fun x -> x.GetAllAsync(It.IsAny<FilterDefinition<PhysicalEvidence>>()))
                .Returns(async {
                    let physicalEvidence = PhysicalEvidence()
                    physicalEvidence.Path <- "profile.png"
                    return [| physicalEvidence |] :> IEnumerable<PhysicalEvidence>
                }) |> ignore

            let! response =
                physicalEvidenceController.GetAllAsync(
                    "63a55a904b9e69ab2ce9505a",
                    "63a55a966239899945e68aad"
                )

            mockPhysicalEvidenceRepository
                .Verify((fun x ->
                    x.GetAllAsync(It.IsAny<FilterDefinition<PhysicalEvidence>>())
                ), Times.Once)

            let okObjectResult = response :?> OkObjectResult
            let evidences = okObjectResult.Value :?> IEnumerable<PhysicalEvidence>
            let expectedPath = AwsConfig.S3.Endpoint + "/" + AwsConfig.S3.Name + "/profile.png"

            Assert.IsType<OkObjectResult>(response) |> ignore
            Assert.True(evidences.First().Path = expectedPath)
        }

    [<Fact(DisplayName = "CreateAsync - Should return 400 when the number of evidences is exceeded")>]
    member this.CreateAsyncShouldReturn400() =
        async {
            let mockPhysicalEvidenceRepository = Mock<IPhysicalEvidenceRepository>()
            let mockS3Service = Mock<IAWSS3Service>()
            let mockFormFile = Mock<IFormFile>()
            let physicalEvidenceController =
                PhysicalEvidenceController(
                    mockPhysicalEvidenceRepository.Object,
                    mockS3Service.Object
                )

            mockPhysicalEvidenceRepository
                .Setup(fun x -> x.ValidateEvidenceNumber(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(async { return false }) |> ignore

            let! response =
                physicalEvidenceController.CreateAsync(
                    "63a5f673351af3b36beee01d",
                    mockFormFile.Object,
                    "63a5f68047dd12acee83239a"
                )

            mockPhysicalEvidenceRepository
                .Verify((fun x ->
                    x.ValidateEvidenceNumber(It.IsAny<string>(), It.IsAny<string>())
                ), Times.Once)

            Assert.IsType<BadRequestResult>(response) |> ignore
        }

    [<Fact(DisplayName = "CreateAsync - Should return 201 when process succeeds")>]
    member this.CreateAsyncShouldReturn201() =
        async {
            let mockPhysicalEvidenceRepository = Mock<IPhysicalEvidenceRepository>()
            let mockS3Service = Mock<IAWSS3Service>()
            let mockFormFile = Mock<IFormFile>()
            let physicalEvidenceController =
                PhysicalEvidenceController(
                    mockPhysicalEvidenceRepository.Object,
                    mockS3Service.Object
                )

            mockPhysicalEvidenceRepository
                .Setup(fun x -> x.ValidateEvidenceNumber(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(async { return true }) |> ignore
            mockFormFile.Setup(fun x -> x.FileName).Returns("profile.png") |> ignore
            mockS3Service
                .Setup(fun x -> x.PutObjectAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Stream>()))
                .Returns(async { return AwsConfig.S3.Endpoint + "/" + AwsConfig.S3.Name + "/profile.png" }) |> ignore
            mockPhysicalEvidenceRepository
                .Setup(fun x -> x.CreateAsync(It.IsAny<PhysicalEvidence>()))
                .Returns(Task.CompletedTask) |> ignore

            let! response =
                physicalEvidenceController.CreateAsync(
                    "63a5f673351af3b36beee01d",
                    mockFormFile.Object,
                    "63a5f68047dd12acee83239a"
                )

            mockPhysicalEvidenceRepository
                .Verify((fun x ->
                    x.ValidateEvidenceNumber(It.IsAny<string>(), It.IsAny<string>())
                ), Times.Once)
            mockS3Service
                .Verify((fun x ->
                    x.PutObjectAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Stream>())
                ), Times.Once)
            mockPhysicalEvidenceRepository.Verify((fun x -> x.CreateAsync(It.IsAny<PhysicalEvidence>())), Times.Once)

            let createdResult = response :?> CreatedResult
            let physicalEvidence = createdResult.Value :?> PhysicalEvidence
            let expectedPath = AwsConfig.S3.Endpoint + "/" + AwsConfig.S3.Name + "/profile.png"

            Assert.IsType<CreatedResult>(response) |> ignore
            Assert.True(physicalEvidence.Path = expectedPath)
        }
