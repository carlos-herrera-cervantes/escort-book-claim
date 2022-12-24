namespace Claim.Web.Backgrounds

open System
open System.Threading.Tasks
open System.Threading
open Microsoft.Extensions.Hosting
open Microsoft.Extensions.DependencyInjection
open MongoDB.Driver
open Claim.Web.Handlers
open Claim.Web.Types
open Claim.Web.Repositories
open Claim.Web.Models

type ClaimStatusConsumer (factory: IServiceScopeFactory, operationHandler: IOperationHandler<ClaimStatusEvent>) =
    inherit BackgroundService()

    member this._operationHandler = operationHandler

    member this._claimRepository = factory.CreateScope().ServiceProvider.GetRequiredService<IClaimRepository>()

    override this.ExecuteAsync(stoppingToken: CancellationToken) =
        this._operationHandler.Subscribe("ClaimStatusConsumer")(Action<ClaimStatusEvent>(fun dictum ->
            async {
                let filter = Builders<Claim>.Filter.Eq((fun c -> c.Id), dictum.ClaimId)
                let! claim = this._claimRepository.GetOneAsync(filter)
                            |> Async.AwaitTask
                claim.Status <- dictum.Status

                let! _ = this._claimRepository.UpdateOneAsync(claim.Id, claim)
                        |> Async.AwaitTask
                
                return true
            } |> Async.StartAsTask |> ignore
        ))
        Task.CompletedTask
