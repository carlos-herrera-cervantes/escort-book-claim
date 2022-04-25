namespace EscortBookClaim.Attributes

open System.Threading.Tasks
open Microsoft.AspNetCore.Mvc
open Microsoft.AspNetCore.Mvc.Filters
open EscortBookClaim.Repositories

type ServiceExistsFilter(serviceRepository: IServiceRepository) =

    member this._serviceRepository = serviceRepository

    interface IAsyncActionFilter with

        member this.OnActionExecutionAsync(context: ActionExecutingContext, next: ActionExecutionDelegate) =
            async {
                let pair = context.ActionArguments.TryGetValue("id")
                let id = snd pair

                let! service = this._serviceRepository.GetOneAsync(fun s -> s.Id = id.ToString()) |> Async.AwaitTask

                if (isNull service) then
                    context.Result <- NotFoundResult()
                else
                    do! next.Invoke() :> Task |> Async.AwaitTask
            } |> Async.StartAsTask :> Task

type ServiceExistsAttribute() =
    inherit TypeFilterAttribute(typeof<ServiceExistsFilter>)