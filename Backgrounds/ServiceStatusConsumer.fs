namespace EscortBookClaim.Backgrounds

open System
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Hosting
open System.Threading.Tasks
open System.Threading
open EscortBookClaim.Handlers
open EscortBookClaim.Repositories

type ServiceStatusConsumer (factory: IServiceScopeFactory, operationHandler: IOperationHandler<string>) =
    inherit BackgroundService()

    member this._operationHandler = operationHandler

    member this._serviceRepository = factory.CreateScope().ServiceProvider.GetRequiredService<IServiceRepository>()

    override this.ExecuteAsync(stoppingToken: CancellationToken) =
        this._operationHandler.Subscribe("ServiceStatusConsumer")(Action<string>(fun id ->
            async {
                let! service = this._serviceRepository.GetOneAsync(fun s -> s.Id = id)
                            |> Async.AwaitTask
                service.Status <- "reclaimed"

                let! _ = this._serviceRepository.UpdateOneAsync(id)(service)
                        |> Async.AwaitTask
                
                return true
            } |> Async.StartAsTask |> ignore
        ))
        Task.CompletedTask