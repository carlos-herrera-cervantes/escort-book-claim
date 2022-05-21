namespace EscortBookClaim.Attributes

open System.Threading.Tasks
open Microsoft.AspNetCore.Mvc
open Microsoft.AspNetCore.Mvc.Filters
open EscortBookClaim.Repositories
open EscortBookClaim.Constants

type ClaimStatusFilter(claimRepository: IClaimRepository) =

    member this._claimRepository = claimRepository

    interface IAsyncActionFilter with

        member this.OnActionExecutionAsync(context: ActionExecutingContext, next: ActionExecutionDelegate) =
            async {
                let pair = context.ActionArguments.TryGetValue("id")
                let id = snd pair

                let! claim = this._claimRepository.GetOneAsync(fun c -> c.Id = id.ToString()) |> Async.AwaitTask

                if (isNull claim) then
                    context.Result <- NotFoundResult()
                elif claim.Status <> ClaimStatus.InReview then
                    let forbidden = ObjectResult()
                    forbidden.StatusCode <- 403
                    context.Result <- forbidden
                else
                    do! next.Invoke() :> Task |> Async.AwaitTask
            } |> Async.StartAsTask :> Task

type ClaimStatusAttribute() =
    inherit TypeFilterAttribute(typeof<ClaimStatusFilter>)