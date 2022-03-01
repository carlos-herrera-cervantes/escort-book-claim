namespace EscortBookClaim.Controllers

open Microsoft.AspNetCore.Mvc
open EscortBookClaim.Repositories
open EscortBookClaim.Models

[<Route("api/v1/claims")>]
[<Produces("application/json")>]
[<ApiController>]
type ClaimController
    (
        claimRepository: IClaimRepository,
        customerProfileRepository: ICustomerProfileRepository,
        dictumRepository: IDictumRepository,
        escortProfileRepository: IEscortProfileRepository,
        serviceRepository: IServiceRepository
    ) =
    inherit ControllerBase()

    member this._claimRepository = claimRepository

    member this._customerProfileRepository = customerProfileRepository

    member this._dictumRepository = dictumRepository

    member this._escortProfileRepository = escortProfileRepository

    member this._serviceRepository = serviceRepository

    [<HttpGet>]
    member this.GetAllAsync() =
        async {
            let! claims = this._claimRepository.GetAllAsync(0, 10)
            return claims |> this.Ok :> IActionResult
        }

    [<HttpGet("{id}")>]
    member this.GetOneAsync([<FromRoute>] id: string) =
        async {
            let! claim = this._claimRepository.GetOneAsync(id)

            match box claim with
            | null ->
                return NotFoundResult() :> IActionResult
            | _ ->
                let! customerProfile = this._customerProfileRepository.GetByIdAsync(claim.CustomerId)
                                    |> Async.AwaitTask
                let! escortProfile = this._escortProfileRepository.GetByIdAsync(claim.EscortId)
                                    |> Async.AwaitTask
                let! service = this._serviceRepository.GetByIdAsync(claim.ServiceId)
                                    |> Async.AwaitTask
                let! dictum = this._dictumRepository.GetOneAsync(id)

                let claimDetail = ClaimDetailDTO()
                claimDetail.Id <- claim.Id
                claimDetail.Customer <- customerProfile.FirstName + " " + customerProfile.LastName
                claimDetail.Escort <- escortProfile.FirstName + " " + escortProfile.LastName
                claimDetail.Comment <- claim.Comment
                claimDetail.Status <- claim.Status
                claimDetail.CreatedAt <- claim.CreatedAt
                claimDetail.UpdatedAt <- claim.UpdatedAt
                claimDetail.Dictum <- dictum.Response
                claimDetail.Price <- service.Price
                claimDetail.Time <- service.TimeQuatity
                claimDetail.TimeMeasurementUnit <- service.TimeMeasurementUnit

                return claimDetail |> this.Ok :> IActionResult
        }

    [<HttpPost>]
    member this.CreateAsync([<FromBody>] claim: Claim) =
        async {
            let! _ = this._claimRepository.CreateAsync(claim)
            return this.Created("", claim) :> IActionResult
        }
