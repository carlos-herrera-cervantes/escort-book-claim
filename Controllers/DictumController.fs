namespace EscortBookClaim.Controllers

open Microsoft.AspNetCore.JsonPatch
open Microsoft.AspNetCore.Mvc
open EscortBookClaim.Repositories
open EscortBookClaim.Models
open EscortBookClaim.Handlers
open EscortBookClaim.Types
open EscortBookClaim.Common
open EscortBookClaim.Attributes

[<Route("api/v1/claims/{id}/dictum")>]
[<Produces("application/json")>]
[<ApiController>]
type DictumController
    (
        dictumRepository: IDictumRepository,
        claimStatusHandler: IOperationHandler<ClaimStatusEvent>,
        serviceStatusHandler: IOperationHandler<ServiceStatusEvent>,
        claimRepository: IClaimRepository
    ) =
    inherit ControllerBase()

    member this._dictumRepository = dictumRepository

    member this._claimStatusHandler = claimStatusHandler

    member this._serviceStatusHandler = serviceStatusHandler

    member this._claimRepository = claimRepository

    [<HttpPost>]
    member this.CreateAsync
        (
            [<FromRoute>] id: string,
            [<FromBody>] dictum: Dictum,
            [<FromHeader(Name = "user-id")>] userId: string
        ) =
        async {
            let! claim = this._claimRepository.GetOneAsync(fun c -> c.Id = id) |> Async.AwaitTask

            match claim with
            | null -> return this.NotFound() :> IActionResult
            | _ ->
                dictum.UserId <- userId
                dictum.ClaimId <- id

                let! _ = this._dictumRepository.CreateAsync(dictum) |> Async.AwaitTask
                let claimStatusEvent = ClaimStatusEvent(ClaimId = id, Status = dictum.Status)
                let serviceStatusEvent = ServiceStatusEvent(ServiceId = claim.ServiceId, Status = dictum.Status)
                
                Emitter<ClaimStatusEvent>.EmitMessage(this._claimStatusHandler, claimStatusEvent)
                Emitter<ServiceStatusEvent>.EmitMessage(this._serviceStatusHandler, serviceStatusEvent)

                return this.Created("", dictum) :> IActionResult
        }

    [<HttpPatch>]
    [<ClaimStatus>]
    member this.UpdateOneAsync([<FromRoute>] id: string, [<FromBody>] partialDictum: JsonPatchDocument<Dictum>) =
        async {
            let! dictum = this._dictumRepository.GetOneAsync(fun d -> d.ClaimId = id) |> Async.AwaitTask

            match dictum with
            | null -> return this.NotFound() :> IActionResult
            | _ ->
                let! _ = this._dictumRepository.UpdateOneAsync(id)(dictum)(partialDictum) |> Async.AwaitTask
                let claimStatusEvent = ClaimStatusEvent(ClaimId = id, Status = dictum.Status)
                
                Emitter<ClaimStatusEvent>.EmitMessage(this._claimStatusHandler, claimStatusEvent)
                return dictum |> this.Ok :> IActionResult
        }
