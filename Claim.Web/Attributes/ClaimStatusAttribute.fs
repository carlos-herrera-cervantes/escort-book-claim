namespace Claim.Web.Attributes

open System
open System.Threading.Tasks
open Microsoft.AspNetCore.Mvc
open Microsoft.AspNetCore.Mvc.Filters
open MongoDB.Driver
open Claim.Web.Repositories
open Claim.Web.Constants
open Claim.Web.Models

type ClaimStatusFilter(claimRepository: IClaimRepository) =

    member this._claimRepository = claimRepository

    interface IAsyncActionFilter with

        member this.OnActionExecutionAsync(context: ActionExecutingContext, next: ActionExecutionDelegate) =
            async {
                let pair = context.ActionArguments.TryGetValue("id")
                let id = snd pair
                let filter = Builders<Claim>.Filter.Eq((fun c -> c.Id), id.ToString())
                let! claim = this._claimRepository.GetOneAsync(filter) |> Async.AwaitTask

                if (isNull claim) then
                    context.Result <- NotFoundResult()
                elif claim.Status <> ClaimStatus.InReview then
                    let forbidden = ObjectResult()
                    forbidden.StatusCode <- Nullable 403
                    context.Result <- forbidden
                else
                    do! next.Invoke() :> Task |> Async.AwaitTask
            } |> Async.StartAsTask :> Task

type ClaimStatusAttribute() =
    inherit TypeFilterAttribute(typeof<ClaimStatusFilter>)
