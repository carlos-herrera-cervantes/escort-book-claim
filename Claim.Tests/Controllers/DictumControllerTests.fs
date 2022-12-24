namespace Claim.Tests.Controllers

open Xunit
open System.ComponentModel
open System.Diagnostics.CodeAnalysis
open System.Threading.Tasks
open Microsoft.AspNetCore.JsonPatch
open Moq
open MongoDB.Driver
open Claim.Web.Controllers
open Claim.Web.Repositories
open Claim.Web.Handlers
open Claim.Web.Types
open Claim.Web.Models
open Microsoft.AspNetCore.Mvc

[<Collection("DictumController")>]
[<Category("Controllers")>]
[<ExcludeFromCodeCoverage>]
type DictumControllerTests() =

    [<Fact(DisplayName = "CreateAsync - Should return 404 when claim does not exists")>]
    member this.CreateAsyncShouldReturn404() =
        async {
            let mockDictumRepository = Mock<IDictumRepository>()
            let mockClaimStatusEvent = Mock<IOperationHandler<ClaimStatusEvent>>()
            let mockServiceStatusEvent = Mock<IOperationHandler<ServiceStatusEvent>>()
            let mockClaimRepository = Mock<IClaimRepository>()
            let dictumController =
                DictumController(
                    mockDictumRepository.Object,
                    mockClaimStatusEvent.Object,
                    mockServiceStatusEvent.Object,
                    mockClaimRepository.Object
                )

            mockClaimRepository
                .Setup(fun x -> x.GetOneAsync(It.IsAny<FilterDefinition<Claim>>()))
                .ReturnsAsync(fun () -> null) |> ignore

            let! response =
                dictumController.CreateAsync(
                    "63a60ae27f2c325a23efd6f6",
                    Dictum(),
                    "63a60ae7f06e3e541defa6de"
                )

            mockClaimRepository
                .Verify((fun x -> x.GetOneAsync(It.IsAny<FilterDefinition<Claim>>())), Times.Once)

            Assert.IsType<NotFoundResult>(response) |> ignore
        }

    [<Fact(DisplayName = "CreateAsync - Should return 201 when process succeeds")>]
    member this.CreateAsyncShouldReturn201() =
        async {
            let mockDictumRepository = Mock<IDictumRepository>()
            let mockClaimStatusEvent = Mock<IOperationHandler<ClaimStatusEvent>>()
            let mockServiceStatusEvent = Mock<IOperationHandler<ServiceStatusEvent>>()
            let mockClaimRepository = Mock<IClaimRepository>()
            let dictumController =
                DictumController(
                    mockDictumRepository.Object,
                    mockClaimStatusEvent.Object,
                    mockServiceStatusEvent.Object,
                    mockClaimRepository.Object
                )

            mockClaimRepository
                .Setup(fun x -> x.GetOneAsync(It.IsAny<FilterDefinition<Claim>>()))
                .ReturnsAsync(Claim()) |> ignore
            mockDictumRepository
                .Setup(fun x -> x.CreateAsync(It.IsAny<Dictum>()))
                .Returns(Task.CompletedTask) |> ignore

            let! response =
                dictumController.CreateAsync(
                    "63a60ae27f2c325a23efd6f6",
                    Dictum(),
                    "63a60ae7f06e3e541defa6de"
                )

            mockClaimRepository
                .Verify((fun x -> x.GetOneAsync(It.IsAny<FilterDefinition<Claim>>())), Times.Once)
            mockDictumRepository.Verify((fun x -> x.CreateAsync(It.IsAny<Dictum>())), Times.Once)
            mockClaimStatusEvent.Verify((fun x -> x.Publish(It.IsAny<ClaimStatusEvent>())), Times.Once)
            mockServiceStatusEvent.Verify((fun x -> x.Publish(It.IsAny<ServiceStatusEvent>())), Times.Once)

            Assert.IsType<CreatedResult>(response) |> ignore
        }

    [<Fact(DisplayName = "UpdateOneAsync - Should return 404 when dictum does not exists")>]
    member this.UpdateOneAsyncShouldReturn404() =
        async {
            let mockDictumRepository = Mock<IDictumRepository>()
            let mockClaimStatusEvent = Mock<IOperationHandler<ClaimStatusEvent>>()
            let mockServiceStatusEvent = Mock<IOperationHandler<ServiceStatusEvent>>()
            let mockClaimRepository = Mock<IClaimRepository>()
            let dictumController =
                DictumController(
                    mockDictumRepository.Object,
                    mockClaimStatusEvent.Object,
                    mockServiceStatusEvent.Object,
                    mockClaimRepository.Object
                )

            mockDictumRepository
                .Setup(fun x -> x.GetOneAsync(It.IsAny<FilterDefinition<Dictum>>()))
                .ReturnsAsync(fun () -> null) |> ignore

            let! response =
                dictumController.UpdateOneAsync(
                    "63a60ae27f2c325a23efd6f6",
                    JsonPatchDocument<Dictum>()
                )

            mockDictumRepository
                .Verify((fun x -> x.GetOneAsync(It.IsAny<FilterDefinition<Dictum>>())), Times.Once)

            Assert.IsType<NotFoundResult>(response) |> ignore
        }

    [<Fact(DisplayName = "UpdateOneAsync - Should return 200 when process succeeds")>]
    member this.UpdateOneAsyncShouldReturn200() =
        async {
            let mockDictumRepository = Mock<IDictumRepository>()
            let mockClaimStatusEvent = Mock<IOperationHandler<ClaimStatusEvent>>()
            let mockServiceStatusEvent = Mock<IOperationHandler<ServiceStatusEvent>>()
            let mockClaimRepository = Mock<IClaimRepository>()
            let dictumController =
                DictumController(
                    mockDictumRepository.Object,
                    mockClaimStatusEvent.Object,
                    mockServiceStatusEvent.Object,
                    mockClaimRepository.Object
                )

            mockDictumRepository
                .Setup(fun x -> x.GetOneAsync(It.IsAny<FilterDefinition<Dictum>>()))
                .ReturnsAsync(Dictum()) |> ignore
            mockDictumRepository
                .Setup(fun x ->
                    x.UpdateOneAsync(It.IsAny<string>(), It.IsAny<Dictum>(), It.IsAny<JsonPatchDocument<Dictum>>())
                )
                .Returns(Task.CompletedTask) |> ignore

            let! response =
                dictumController.UpdateOneAsync(
                    "63a60ae27f2c325a23efd6f6",
                    JsonPatchDocument<Dictum>()
                )

            mockDictumRepository
                .Verify((fun x -> x.GetOneAsync(It.IsAny<FilterDefinition<Dictum>>())), Times.Once)
            mockDictumRepository.Verify((fun x ->
                x.UpdateOneAsync(It.IsAny<string>(), It.IsAny<Dictum>(), It.IsAny<JsonPatchDocument<Dictum>>())
            ), Times.Once)
            mockClaimStatusEvent.Verify((fun x -> x.Publish(It.IsAny<ClaimStatusEvent>())), Times.Once)

            Assert.IsType<OkObjectResult>(response) |> ignore
        }
