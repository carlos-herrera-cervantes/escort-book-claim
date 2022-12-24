namespace Claim.Web.Backgrounds

open System
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Hosting
open System.Threading.Tasks
open System.Threading
open MongoDB.Driver
open Claim.Web.Handlers
open Claim.Web.Repositories
open Claim.Web.Types
open Claim.Web.Models

type ServiceStatusConsumer (factory: IServiceScopeFactory, operationHandler: IOperationHandler<ServiceStatusEvent>) =
    inherit BackgroundService()

    member this._operationHandler = operationHandler

    member this._serviceRepository = factory.CreateScope().ServiceProvider.GetRequiredService<IServiceRepository>()

    override this.ExecuteAsync(stoppingToken: CancellationToken) =
        this._operationHandler.Subscribe("ServiceStatusConsumer")(Action<ServiceStatusEvent>(fun serviceStatusEvent ->
            async {
                let! service =
                    this._serviceRepository.GetOneAsync(
                        Builders<Service>.Filter.Eq((fun s -> s.Id), serviceStatusEvent.ServiceId)
                    ) |> Async.AwaitTask
                service.Status <- serviceStatusEvent.ToService()

                let! _ = this._serviceRepository.UpdateOneAsync(serviceStatusEvent.ServiceId, service)
                        |> Async.AwaitTask
                
                return true
            } |> Async.StartAsTask |> ignore
        ))
        Task.CompletedTask
