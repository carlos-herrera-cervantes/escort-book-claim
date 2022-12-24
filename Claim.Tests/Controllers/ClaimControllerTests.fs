namespace Claim.Tests.Controllers

open Xunit
open System.ComponentModel
open System.Diagnostics.CodeAnalysis
open System.Collections.Generic
open System.Threading.Tasks
open Microsoft.AspNetCore.Mvc
open Moq
open MongoDB.Driver
open Claim.Web.Controllers
open Claim.Web.Repositories
open Claim.Web.Handlers
open Claim.Web.Types
open Claim.Web.Models

[<Collection("ClaimController")>]
[<Category("Controllers")>]
[<ExcludeFromCodeCoverage>]
type ClaimControllerTests() =

    [<Fact(DisplayName = "GetByExternal - should return 200 when process succeeds")>]
    member this.GetByExternalShouldReturn200() =
        async {
            let mockClaimRepository = Mock<IClaimRepository>()
            let mockCustomerProfileRepository = Mock<ICustomerProfileRepository>()
            let mockDictumRepository = Mock<IDictumRepository>()
            let mockEscortProfileRepository = Mock<IEscortProfileRepository>()
            let mockServiceRepository = Mock<IServiceRepository>()
            let mockServiceStatusEmitter = Mock<IOperationHandler<ServiceStatusEvent>>()
            let mockClaimCreatedEmitter = Mock<IOperationHandler<ClaimCreatedEvent>>()

            mockClaimRepository
                .Setup(fun x -> x.GetAllAsync(It.IsAny<int>(), It.IsAny<int>()))
                .Returns(async { return [||] :> IEnumerable<Claim> }) |> ignore

            let claimController =
                ClaimController(
                    mockClaimRepository.Object,
                    mockCustomerProfileRepository.Object,
                    mockDictumRepository.Object,
                    mockEscortProfileRepository.Object,
                    mockServiceRepository.Object,
                    mockServiceStatusEmitter.Object,
                    mockClaimCreatedEmitter.Object
                )

            let! response = claimController.GetByExternal(Pager())

            mockClaimRepository
                .Verify((fun x -> x.GetAllAsync(It.IsAny<int>(), It.IsAny<int>())), Times.Once)

            Assert.IsType<OkObjectResult>(response) |> ignore
        }

    [<Fact(DisplayName = "GetAllAsync - Should return 200 when process succeeds")>]
    member this.GetAllAsyncShouldReturn200() =
        async {
            let mockClaimRepository = Mock<IClaimRepository>()
            let mockCustomerProfileRepository = Mock<ICustomerProfileRepository>()
            let mockDictumRepository = Mock<IDictumRepository>()
            let mockEscortProfileRepository = Mock<IEscortProfileRepository>()
            let mockServiceRepository = Mock<IServiceRepository>()
            let mockServiceStatusEmitter = Mock<IOperationHandler<ServiceStatusEvent>>()
            let mockClaimCreatedEmitter = Mock<IOperationHandler<ClaimCreatedEvent>>()

            mockClaimRepository
                .Setup(fun x ->
                    x.GetAllByFilterAsync(
                        It.IsAny<FilterDefinition<Claim>>(),
                        It.IsAny<int>(),
                        It.IsAny<int>()
                    )
                ).Returns(async { return [||] :> IEnumerable<Claim> }) |> ignore

            let claimController =
                ClaimController(
                    mockClaimRepository.Object,
                    mockCustomerProfileRepository.Object,
                    mockDictumRepository.Object,
                    mockEscortProfileRepository.Object,
                    mockServiceRepository.Object,
                    mockServiceStatusEmitter.Object,
                    mockClaimCreatedEmitter.Object
                )

            let! responseForCustomer = claimController.GetAllAsync("63a5173e9f111e3fdda1aedc", "Customer", Pager())
            let! responseForEmployee = claimController.GetAllAsync("63a5173e9f111e3fdda1aedc", "Employee", Pager())

            mockClaimRepository.Verify((fun x ->
                x.GetAllByFilterAsync(
                    It.IsAny<FilterDefinition<Claim>>(),
                    It.IsAny<int>(),
                    It.IsAny<int>()
                )), Times.Exactly(2))

            Assert.IsType<OkObjectResult>(responseForCustomer) |> ignore
            Assert.IsType<OkObjectResult>(responseForEmployee) |> ignore
        }

    [<Fact(DisplayName = "GetOneAsync - Should return 404 when claim does not exists")>]
    member this.GetOneAsyncShouldReturn404() =
        async {
            let mockClaimRepository = Mock<IClaimRepository>()
            let mockCustomerProfileRepository = Mock<ICustomerProfileRepository>()
            let mockDictumRepository = Mock<IDictumRepository>()
            let mockEscortProfileRepository = Mock<IEscortProfileRepository>()
            let mockServiceRepository = Mock<IServiceRepository>()
            let mockServiceStatusEmitter = Mock<IOperationHandler<ServiceStatusEvent>>()
            let mockClaimCreatedEmitter = Mock<IOperationHandler<ClaimCreatedEvent>>()

            mockClaimRepository
                .Setup(fun x -> x.GetOneAsync(It.IsAny<FilterDefinition<Claim>>()))
                .ReturnsAsync(fun () -> null) |> ignore

            let claimController =
                ClaimController(
                    mockClaimRepository.Object,
                    mockCustomerProfileRepository.Object,
                    mockDictumRepository.Object,
                    mockEscortProfileRepository.Object,
                    mockServiceRepository.Object,
                    mockServiceStatusEmitter.Object,
                    mockClaimCreatedEmitter.Object
                )

            let! response = claimController.GetOneAsync("63a51a354a2429e978854554")

            mockClaimRepository
                .Verify((fun x -> x.GetOneAsync(It.IsAny<FilterDefinition<Claim>>())), Times.Once)

            Assert.IsType<NotFoundResult>(response) |> ignore
        }

    [<Fact(DisplayName = "GetOneAsync - Should return 200 when process succeeds")>]
    member this.GetOneAsyncShouldReturn200() =
        async {
            let mockClaimRepository = Mock<IClaimRepository>()
            let mockCustomerProfileRepository = Mock<ICustomerProfileRepository>()
            let mockDictumRepository = Mock<IDictumRepository>()
            let mockEscortProfileRepository = Mock<IEscortProfileRepository>()
            let mockServiceRepository = Mock<IServiceRepository>()
            let mockServiceStatusEmitter = Mock<IOperationHandler<ServiceStatusEvent>>()
            let mockClaimCreatedEmitter = Mock<IOperationHandler<ClaimCreatedEvent>>()

            mockClaimRepository
                .Setup(fun x -> x.GetOneAsync(It.IsAny<FilterDefinition<Claim>>()))
                .ReturnsAsync(Claim()) |> ignore
            mockCustomerProfileRepository
                .Setup(fun x -> x.GetByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(CustomerProfile()) |> ignore
            mockEscortProfileRepository
                .Setup(fun x -> x.GetByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(EscortProfile()) |> ignore
            mockServiceRepository
                .Setup(fun x -> x.GetOneAsync(It.IsAny<FilterDefinition<Service>>()))
                .ReturnsAsync(Service()) |> ignore
            mockDictumRepository
                .Setup(fun x -> x.GetOneAsync(It.IsAny<FilterDefinition<Dictum>>()))
                .ReturnsAsync(Dictum()) |> ignore

            let claimController =
                ClaimController(
                    mockClaimRepository.Object,
                    mockCustomerProfileRepository.Object,
                    mockDictumRepository.Object,
                    mockEscortProfileRepository.Object,
                    mockServiceRepository.Object,
                    mockServiceStatusEmitter.Object,
                    mockClaimCreatedEmitter.Object
                )

            let! response = claimController.GetOneAsync("63a51ec2962abe77ba4dbea6")

            mockClaimRepository
                .Verify((fun x -> x.GetOneAsync(It.IsAny<FilterDefinition<Claim>>())), Times.Once)
            mockCustomerProfileRepository
                .Verify((fun x -> x.GetByIdAsync(It.IsAny<string>())), Times.Once)
            mockEscortProfileRepository
                .Verify((fun x -> x.GetByIdAsync(It.IsAny<string>())), Times.Once)
            mockServiceRepository
                .Verify((fun x -> x.GetOneAsync(It.IsAny<FilterDefinition<Service>>())), Times.Once)
            mockDictumRepository
                .Verify((fun x -> x.GetOneAsync(It.IsAny<FilterDefinition<Dictum>>())), Times.Once)

            Assert.IsType<OkObjectResult>(response) |> ignore
        }

    [<Fact(DisplayName = "CreateAsync - Should return 201 when process succeeds")>]
    member this.CreateAsyncShouldReturn201() =
        async {
            let mockClaimRepository = Mock<IClaimRepository>()
            let mockCustomerProfileRepository = Mock<ICustomerProfileRepository>()
            let mockDictumRepository = Mock<IDictumRepository>()
            let mockEscortProfileRepository = Mock<IEscortProfileRepository>()
            let mockServiceRepository = Mock<IServiceRepository>()
            let mockServiceStatusEmitter = Mock<IOperationHandler<ServiceStatusEvent>>()
            let mockClaimCreatedEmitter = Mock<IOperationHandler<ClaimCreatedEvent>>()

            mockClaimRepository
                .Setup(fun x -> x.CreateAsync(It.IsAny<Claim>()))
                .Returns(Task.CompletedTask) |> ignore
            mockServiceStatusEmitter
                .Setup(fun x -> x.Publish(It.IsAny<ServiceStatusEvent>())) |> ignore
            mockClaimCreatedEmitter
                .Setup(fun x -> x.Publish(It.IsAny<ClaimCreatedEvent>())) |> ignore

            let claimController =
                ClaimController(
                    mockClaimRepository.Object,
                    mockCustomerProfileRepository.Object,
                    mockDictumRepository.Object,
                    mockEscortProfileRepository.Object,
                    mockServiceRepository.Object,
                    mockServiceStatusEmitter.Object,
                    mockClaimCreatedEmitter.Object
                )

            let! response = claimController.CreateAsync(Claim(), "63a523138f07dc0f921d7e52", "Customer")

            mockClaimRepository.Verify((fun x -> x.CreateAsync(It.IsAny<Claim>())), Times.Once)
            mockServiceStatusEmitter.Verify((fun x -> x.Publish(It.IsAny<ServiceStatusEvent>())), Times.Once)
            mockClaimCreatedEmitter.Verify((fun x -> x.Publish(It.IsAny<ClaimCreatedEvent>())), Times.Once)

            Assert.IsType<CreatedResult>(response) |> ignore
        }

    [<Fact(DisplayName = "ArgueAsync - Should return 404 when claim does not exists")>]
    member this.ArgueAsyncShouldReturn404() =
        async {
            let mockClaimRepository = Mock<IClaimRepository>()
            let mockCustomerProfileRepository = Mock<ICustomerProfileRepository>()
            let mockDictumRepository = Mock<IDictumRepository>()
            let mockEscortProfileRepository = Mock<IEscortProfileRepository>()
            let mockServiceRepository = Mock<IServiceRepository>()
            let mockServiceStatusEmitter = Mock<IOperationHandler<ServiceStatusEvent>>()
            let mockClaimCreatedEmitter = Mock<IOperationHandler<ClaimCreatedEvent>>()

            mockClaimRepository
                .Setup(fun x -> x.GetOneAsync(It.IsAny<FilterDefinition<Claim>>()))
                .ReturnsAsync(fun () -> null) |> ignore

            let claimController =
                ClaimController(
                    mockClaimRepository.Object,
                    mockCustomerProfileRepository.Object,
                    mockDictumRepository.Object,
                    mockEscortProfileRepository.Object,
                    mockServiceRepository.Object,
                    mockServiceStatusEmitter.Object,
                    mockClaimCreatedEmitter.Object
                )

            let! response =
                claimController.ArgueAsync(
                    "63a523138f07dc0f921d7e52",
                    "Escort",
                    "63a527668ab55ca84e0dfd7f",
                    ClaimArgumentDTO()
                )

            mockClaimRepository
                .Verify((fun x -> x.GetOneAsync(It.IsAny<FilterDefinition<Claim>>())), Times.Once)

            Assert.IsType<NotFoundResult>(response) |> ignore
        }

    [<Fact(DisplayName = "ArgueAsync - Should return 200 when process succeeds")>]
    member this.ArgueAsyncShouldReturn200() =
        async {
            let mockClaimRepository = Mock<IClaimRepository>()
            let mockCustomerProfileRepository = Mock<ICustomerProfileRepository>()
            let mockDictumRepository = Mock<IDictumRepository>()
            let mockEscortProfileRepository = Mock<IEscortProfileRepository>()
            let mockServiceRepository = Mock<IServiceRepository>()
            let mockServiceStatusEmitter = Mock<IOperationHandler<ServiceStatusEvent>>()
            let mockClaimCreatedEmitter = Mock<IOperationHandler<ClaimCreatedEvent>>()

            mockClaimRepository
                .Setup(fun x -> x.GetOneAsync(It.IsAny<FilterDefinition<Claim>>()))
                .ReturnsAsync(Claim()) |> ignore
            mockClaimRepository
                .Setup(fun x -> x.UpdateOneAsync(It.IsAny<string>(), It.IsAny<Claim>()))
                .Returns(Task.CompletedTask) |> ignore

            let claimController =
                ClaimController(
                    mockClaimRepository.Object,
                    mockCustomerProfileRepository.Object,
                    mockDictumRepository.Object,
                    mockEscortProfileRepository.Object,
                    mockServiceRepository.Object,
                    mockServiceStatusEmitter.Object,
                    mockClaimCreatedEmitter.Object
                )

            let! response =
                claimController.ArgueAsync(
                    "63a523138f07dc0f921d7e52",
                    "Escort",
                    "63a527668ab55ca84e0dfd7f",
                    ClaimArgumentDTO()
                )

            mockClaimRepository
                .Verify((fun x -> x.GetOneAsync(It.IsAny<FilterDefinition<Claim>>())), Times.Once)
            mockClaimRepository
                .Verify((fun x -> x.UpdateOneAsync(It.IsAny<string>(), It.IsAny<Claim>())), Times.Once)

            Assert.IsType<OkObjectResult>(response) |> ignore
        }

    [<Fact(DisplayName = "CancelAsync - Should return 404 when claim does not exists")>]
    member this.CancelAsyncShouldReturn404() =
        async {
            let mockClaimRepository = Mock<IClaimRepository>()
            let mockCustomerProfileRepository = Mock<ICustomerProfileRepository>()
            let mockDictumRepository = Mock<IDictumRepository>()
            let mockEscortProfileRepository = Mock<IEscortProfileRepository>()
            let mockServiceRepository = Mock<IServiceRepository>()
            let mockServiceStatusEmitter = Mock<IOperationHandler<ServiceStatusEvent>>()
            let mockClaimCreatedEmitter = Mock<IOperationHandler<ClaimCreatedEvent>>()

            mockClaimRepository
                .Setup(fun x -> x.GetOneAsync(It.IsAny<FilterDefinition<Claim>>()))
                .ReturnsAsync(fun () -> null) |> ignore

            let claimController =
                ClaimController(
                    mockClaimRepository.Object,
                    mockCustomerProfileRepository.Object,
                    mockDictumRepository.Object,
                    mockEscortProfileRepository.Object,
                    mockServiceRepository.Object,
                    mockServiceStatusEmitter.Object,
                    mockClaimCreatedEmitter.Object
                )

            let! response =
                claimController.CancelAsync(
                    "63a52e9ba04aaf9a3645bf4c",
                    "Escort",
                    "63a52ead5a87dbf962b9664d"
                )

            mockClaimRepository
                .Verify((fun x -> x.GetOneAsync(It.IsAny<FilterDefinition<Claim>>())), Times.Once)

            Assert.IsType<NotFoundResult>(response) |> ignore
        }

    [<Fact(DisplayName = "CancelAsync - Should return 200 when process succeeds")>]
    member this.CancelAsyncShouldReturn200() =
        async {
            let mockClaimRepository = Mock<IClaimRepository>()
            let mockCustomerProfileRepository = Mock<ICustomerProfileRepository>()
            let mockDictumRepository = Mock<IDictumRepository>()
            let mockEscortProfileRepository = Mock<IEscortProfileRepository>()
            let mockServiceRepository = Mock<IServiceRepository>()
            let mockServiceStatusEmitter = Mock<IOperationHandler<ServiceStatusEvent>>()
            let mockClaimCreatedEmitter = Mock<IOperationHandler<ClaimCreatedEvent>>()

            mockClaimRepository
                .Setup(fun x -> x.GetOneAsync(It.IsAny<FilterDefinition<Claim>>()))
                .ReturnsAsync(Claim()) |> ignore
            mockClaimRepository
                .Setup(fun x -> x.UpdateOneAsync(It.IsAny<string>(), It.IsAny<Claim>()))
                .Returns(Task.CompletedTask) |> ignore
            mockServiceStatusEmitter
                .Setup(fun x -> x.Publish(It.IsAny<ServiceStatusEvent>())) |> ignore

            let claimController =
                ClaimController(
                    mockClaimRepository.Object,
                    mockCustomerProfileRepository.Object,
                    mockDictumRepository.Object,
                    mockEscortProfileRepository.Object,
                    mockServiceRepository.Object,
                    mockServiceStatusEmitter.Object,
                    mockClaimCreatedEmitter.Object
                )

            let! response =
                claimController.CancelAsync(
                    "63a52e9ba04aaf9a3645bf4c",
                    "Escort",
                    "63a52ead5a87dbf962b9664d"
                )

            mockClaimRepository
                .Verify((fun x -> x.GetOneAsync(It.IsAny<FilterDefinition<Claim>>())), Times.Once)
            mockClaimRepository
                .Verify((fun x -> x.UpdateOneAsync(It.IsAny<string>(), It.IsAny<Claim>())), Times.Once)
            mockServiceStatusEmitter
                .Verify((fun x -> x.Publish(It.IsAny<ServiceStatusEvent>())), Times.Once)

            Assert.IsType<OkObjectResult>(response) |> ignore
        }
