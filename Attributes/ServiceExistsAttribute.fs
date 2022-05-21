namespace EscortBookClaim.Attributes

open Microsoft.AspNetCore.Http
open Newtonsoft.Json
open System.Threading.Tasks
open Microsoft.AspNetCore.Mvc
open Microsoft.AspNetCore.Mvc.Filters
open EscortBookClaim.Repositories
open EscortBookClaim.Models

type ServiceExistsFilter(serviceRepository: IServiceRepository) =

    member this._serviceRepository = serviceRepository

    interface IAsyncActionFilter with

        member this.OnActionExecutionAsync(context: ActionExecutingContext, next: ActionExecutionDelegate) =
            async {
                let claim = context.ActionArguments["claim"] :?> Claim
                let! service = this._serviceRepository.GetOneAsync(fun s -> s.Id = claim.ServiceId) |> Async.AwaitTask

                if (isNull service) then
                    context.Result <- NotFoundResult()
                elif service.Status = "returned" || service.Status = "reclaimed" then
                    context.Result <- ConflictResult()
                else
                    do! next.Invoke() :> Task |> Async.AwaitTask
            } |> Async.StartAsTask :> Task

type ServiceExistsAttribute() =
    inherit TypeFilterAttribute(typeof<ServiceExistsFilter>)