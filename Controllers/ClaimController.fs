namespace EscortBookClaim.Controllers

open Microsoft.AspNetCore.Mvc
open EscortBookClaim.Repositories
open EscortBookClaim.Models
open EscortBookClaim.Types
open EscortBookClaim.Handlers
open EscortBookClaim.Common

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
        operationHandler: IOperationHandler<string>
    ) =
    inherit ControllerBase()

    member this._claimRepository = claimRepository

    member this._customerProfileRepository = customerProfileRepository

    member this._dictumRepository = dictumRepository

    member this._escortProfileRepository = escortProfileRepository

    member this._serviceRepository = serviceRepository

    member this._operationHandler = operationHandler

    [<HttpGet>]
    member this.GetAllAsync([<FromBody>] payload: Payload) =
        async {
            let! claims = this._claimRepository.GetAllAsync(0)(10)("CustomerId=" + payload.User.Id) |> Async.AwaitTask
            return claims |> this.Ok :> IActionResult
        }

    [<HttpGet("{id}")>]
    member this.GetOneAsync([<FromRoute>] id: string) =
        async {
            let! claim = this._claimRepository.GetOneAsync(fun c -> c.Id = id) |> Async.AwaitTask

            match box claim with
            | null ->
                return NotFoundResult() :> IActionResult
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

    [<HttpPost>]
    member this.CreateAsync([<FromBody>] createClaimDTO: CreateClaimDTO) =
        async {
            let newClaim = Claim()
            newClaim.ServiceId <- createClaimDTO.ServiceId
            newClaim.Comment <- createClaimDTO.Comment

            if createClaimDTO.User.Type = "Escort" then
                newClaim.EscortId <- createClaimDTO.User.Id
                newClaim.CustomerId <- createClaimDTO.CustomerId

            if createClaimDTO.User.Type = "Customer" then
                newClaim.CustomerId <- createClaimDTO.User.Id
                newClaim.EscortId <- createClaimDTO.EscortId

            let! _ = this._claimRepository.CreateAsync(newClaim) |> Async.AwaitTask
            Emitter<string>.EmitMessage(this._operationHandler, createClaimDTO.ServiceId)
            return this.Created("", newClaim) :> IActionResult
        }
