namespace EscortBookClaim.Attributes

open System.Threading.Tasks
open Microsoft.AspNetCore.Mvc
open Microsoft.AspNetCore.Mvc.Filters
open EscortBookClaim.Repositories

type ClaimExistsFilter(claimRepository: IClaimRepository) =

    member this._claimRepository = claimRepository

    interface IAsyncActionFilter with

        member this.OnActionExecutionAsync(context: ActionExecutingContext, next: ActionExecutionDelegate) =
            async {
                let pair = context.ActionArguments.TryGetValue("id")
                let id = snd pair

                let! claim = this._claimRepository.GetOneAsync(fun c -> c.Id = id.ToString()) |> Async.AwaitTask

                if (isNull claim) then
                    context.Result <- NotFoundResult()
                else
                    do! next.Invoke() :> Task |> Async.AwaitTask
            } |> Async.StartAsTask :> Task

type ClaimExistsAttribute() =
    inherit TypeFilterAttribute(typeof<ClaimExistsFilter>)