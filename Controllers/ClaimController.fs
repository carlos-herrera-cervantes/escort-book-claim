namespace EscortBookClaim.Controllers

open Microsoft.AspNetCore.Mvc
open EscortBookClaim.Repositories
open EscortBookClaim.Models
open EscortBookClaim.Types
open EscortBookClaim.Handlers
open EscortBookClaim.Common
open EscortBookClaim.Attributes
open EscortBookClaim.Constants

[<Route("api/v1/claims")>]
[<Produces("application/json")>]
[<ApiController>]
type ClaimController
    (
        claimRepository: IClaimRepository,
        customerProfileRepository: ICustomerProfileRepository,
        dictumRepository: IDictumRepository,
        escortProfileRepository: IEscortProfileRepository,
        serviceRepository: IServiceRepository,
        operationHandler: IOperationHandler<ServiceStatusEvent>
    ) =
    inherit ControllerBase()

    member this._claimRepository = claimRepository

    member this._customerProfileRepository = customerProfileRepository

    member this._dictumRepository = dictumRepository

    member this._escortProfileRepository = escortProfileRepository

    member this._serviceRepository = serviceRepository

    member this._operationHandler = operationHandler

    [<HttpGet>]
    member this.GetByExternal([<FromQuery>] pager: Pager) =
        async {
            let! claims = this._claimRepository.GetAllAsync(pager.Page, pager.PageSize)
                        |> Async.AwaitTask
            return claims |> this.Ok :> IActionResult
        }

    [<HttpGet("profile")>]
    member this.GetAllAsync
        (
            [<FromHeader(Name = "user-id")>] userId: string,
            [<FromHeader(Name = "user-type")>] userType: string,
            [<FromQuery>] pager: Pager
        ) =
        async {
            let! claims =
                match userType with
                | "Customer" -> this._claimRepository.GetAllByFilterAsync((fun c -> c.CustomerId = userId), pager.Page, pager.PageSize)
                                |> Async.AwaitTask
                | _ -> this._claimRepository.GetAllByFilterAsync((fun c -> c.EscortId = userId), pager.Page, pager.PageSize)
                    |> Async.AwaitTask

            return claims |> this.Ok :> IActionResult
        }

    [<HttpGet("{id}")>]
    member this.GetOneAsync([<FromRoute>] id: string) =
        async {
            let! claim = this._claimRepository.GetOneAsync(fun c -> c.Id = id) |> Async.AwaitTask

            match box claim with
            | null -> return NotFoundResult() :> IActionResult
            | _ ->
                let! customerProfile = this._customerProfileRepository.GetByIdAsync(claim.CustomerId)
                                    |> Async.AwaitTask
                let! escortProfile = this._escortProfileRepository.GetByIdAsync(claim.EscortId)
                                    |> Async.AwaitTask
                let! service = this._serviceRepository.GetOneAsync(fun s -> s.Id = claim.ServiceId)
                                    |> Async.AwaitTask
                let! dictum = this._dictumRepository.GetOneAsync(fun d -> d.ClaimId = id) |> Async.AwaitTask

                let claimDetail = ClaimDetailDTO()
                claimDetail.Id <- claim.Id
                claimDetail.Customer <- customerProfile.FirstName + " " + customerProfile.LastName
                claimDetail.Escort <- escortProfile.FirstName + " " + escortProfile.LastName
                claimDetail.Comment <- claim.Comment
                claimDetail.Status <- claim.Status
                claimDetail.CreatedAt <- claim.CreatedAt
                claimDetail.UpdatedAt <- claim.UpdatedAt
                
                if dictum <> null then claimDetail.Dictum <- dictum.Response

                claimDetail.Price <- service.Price
                claimDetail.Time <- service.TimeQuatity
                claimDetail.TimeMeasurementUnit <- service.TimeMeasurementUnit

                return claimDetail |> this.Ok :> IActionResult
        }

    [<HttpPost("profile")>]
    [<ServiceExists>]
    member this.CreateAsync
        (
            [<FromBody>] claim: Claim,
            [<FromHeader(Name = "user-id")>] userId: string,
            [<FromHeader(Name = "user-type")>] userType: string
        ) =
        async {
            if userType = "Escort" then
                claim.EscortId <- userId
            else
                claim.CustomerId <- userId

            claim.Owner <- userType

            let! _ = this._claimRepository.CreateAsync claim |> Async.AwaitTask
            let serviceStatusEvent = ServiceStatusEvent(ServiceId = claim.ServiceId, Status = claim.Status)
            Emitter<ServiceStatusEvent>.EmitMessage(this._operationHandler, serviceStatusEvent)

            return this.Created("", claim) :> IActionResult
        }

    [<HttpPatch("{id}/argument")>]
    member this.ArgueAsync
        (
            [<FromHeader(Name = "user-id")>] userId: string,
            [<FromHeader(Name = "user-type")>] userType: string,
            [<FromRoute>] id: string,
            [<FromBody>] argument: ClaimArgumentDTO
        ) =
        async {
            let! claim =
                match userType with
                | "Escort" -> this._claimRepository.GetOneAsync(fun c -> c.Id = id && c.EscortId = userId) |> Async.AwaitTask
                | _ -> this._claimRepository.GetOneAsync(fun c -> c.Id = id && c.CustomerId = userId) |> Async.AwaitTask

            match claim with
            | null -> return NotFoundResult() :> IActionResult
            | _ ->
                claim.Argument <- argument.Argument
                let! _ = this._claimRepository.UpdateOneAsync(id)(claim) |> Async.AwaitTask
                return claim |> this.Ok :> IActionResult
        }

    [<HttpPost("{id}/cancel")>]
    member this.CancelAsync
        (
            [<FromHeader(Name = "user-id")>] userId: string,
            [<FromHeader(Name = "user-type")>] userType: string,
            [<FromRoute>] id: string
        ) =
        async {
            let! claim =
                match userType with
                | "Escort" -> this._claimRepository.GetOneAsync(fun c -> c.Id = id && c.EscortId = userId && c.Owner = userType)
                            |> Async.AwaitTask
                | _ -> this._claimRepository.GetOneAsync(fun c -> c.Id = id && c.CustomerId = userId && c.Owner = userType)
                    |> Async.AwaitTask

            match claim with
            | null -> return NotFoundResult() :> IActionResult
            | _ ->
                claim.Status <- ClaimStatus.Cancelled
                let! _ = this._claimRepository.UpdateOneAsync(id)(claim) |> Async.AwaitTask
                let serviceStatusEvent = ServiceStatusEvent(ServiceId = claim.ServiceId, Status = ClaimStatus.Cancelled)
                Emitter<ServiceStatusEvent>.EmitMessage(this._operationHandler, serviceStatusEvent)
                return claim |> this.Ok :> IActionResult
        }
