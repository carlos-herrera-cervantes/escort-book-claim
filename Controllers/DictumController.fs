namespace EscortBookClaim.Controllers

open Microsoft.AspNetCore.JsonPatch
open Microsoft.AspNetCore.Mvc
open EscortBookClaim.Repositories
open EscortBookClaim.Models
open EscortBookClaim.Handlers
open EscortBookClaim.Types
open EscortBookClaim.Common

[<Route("api/v1/claims/{id}/dictum")>]
[<Produces("application/json")>]
[<ApiController>]
type DictumController (dictumRepository: IDictumRepository, operationHandler: IOperationHandler<ClaimStatusEvent>) =
    inherit ControllerBase()

    member this._dictumRepository = dictumRepository

    member this._operationHandler = operationHandler

    [<HttpPost>]
    member this.CreateAsync([<FromRoute>] id: string, [<FromBody>] dictum: Dictum) =
        async {
            dictum.ClaimId <- id
            let! _ = this._dictumRepository.CreateAsync(dictum) |> Async.AwaitTask

            let claimStatusEvent = new ClaimStatusEvent(ClaimId = id, Status = dictum.Status)
            Emitter<ClaimStatusEvent>.EmitMessage(this._operationHandler, claimStatusEvent)

            return this.Created("", dictum) :> IActionResult
        }

    [<HttpPatch>]
    member this.UpdateOneAsync([<FromRoute>] id: string, [<FromBody>] partialDictum: JsonPatchDocument<Dictum>) =
        async {
            let! dictum = this._dictumRepository.GetOneAsync(fun d -> d.ClaimId = id) |> Async.AwaitTask

            match box dictum with
            | null ->
                return this.NotFound() :> IActionResult
            | _ ->
                let! _ = this._dictumRepository.UpdateOneAsync(id)(dictum)(partialDictum) |> Async.AwaitTask
                let claimStatusEvent = new ClaimStatusEvent(ClaimId = id, Status = dictum.Status)
                
                Emitter<ClaimStatusEvent>.EmitMessage(this._operationHandler, claimStatusEvent)
                return dictum |> this.Ok :> IActionResult
        }
