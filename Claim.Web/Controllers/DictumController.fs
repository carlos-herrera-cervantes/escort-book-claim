namespace Claim.Web.Controllers

open Microsoft.AspNetCore.JsonPatch
open Microsoft.AspNetCore.Mvc
open MongoDB.Driver
open Claim.Web.Repositories
open Claim.Web.Models
open Claim.Web.Handlers
open Claim.Web.Types
open Claim.Web.Common
open Claim.Web.Attributes

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
        ): Async<IActionResult> =
        async {
            let filter = Builders<Claim>.Filter.Eq((fun c -> c.Id), id)
            let! claim = this._claimRepository.GetOneAsync(filter) |> Async.AwaitTask

            match claim with
            | null -> return this.NotFound() :> IActionResult
            | _ ->
                dictum.UserId <- userId
                dictum.ClaimId <- id

                do! this._dictumRepository.CreateAsync(dictum) |> Async.AwaitTask
                let claimStatusEvent = ClaimStatusEvent(ClaimId = id, Status = dictum.Status)
                let serviceStatusEvent = ServiceStatusEvent(ServiceId = claim.ServiceId, Status = dictum.Status)
                
                Emitter<ClaimStatusEvent>.EmitMessage(this._claimStatusHandler, claimStatusEvent)
                Emitter<ServiceStatusEvent>.EmitMessage(this._serviceStatusHandler, serviceStatusEvent)

                return this.Created("", dictum) :> IActionResult
        }

    [<HttpPatch>]
    [<ClaimStatus>]
    member this.UpdateOneAsync
        (
            [<FromRoute>] id: string,
            [<FromBody>] partialDictum: JsonPatchDocument<Dictum>
        ): Async<IActionResult> =
        async {
            let! dictum = this._dictumRepository.GetOneAsync(Builders<Dictum>.Filter.Eq((fun d -> d.ClaimId), id))
                        |> Async.AwaitTask

            match dictum with
            | null -> return this.NotFound() :> IActionResult
            | _ ->
                do! this._dictumRepository.UpdateOneAsync(id, dictum, partialDictum) |> Async.AwaitTask
                let claimStatusEvent = ClaimStatusEvent(ClaimId = id, Status = dictum.Status)
                
                Emitter<ClaimStatusEvent>.EmitMessage(this._claimStatusHandler, claimStatusEvent)
                return this.Ok(dictum) :> IActionResult
        }
