namespace EscortBookClaim.Backgrounds

open System
open System.Linq.Expressions
open Microsoft.AspNetCore.JsonPatch
open System.Threading.Tasks
open System.Threading
open Microsoft.Extensions.Hosting
open Microsoft.Extensions.DependencyInjection
open EscortBookClaim.Handlers
open EscortBookClaim.Types
open EscortBookClaim.Repositories
open EscortBookClaim.Models

type ClaimStatusConsumer (factory: IServiceScopeFactory, operationHandler: IOperationHandler<ClaimStatusEvent>) =
    inherit BackgroundService()

    member this._operationHandler = operationHandler

    member this._claimRepository = factory.CreateScope().ServiceProvider.GetRequiredService<IClaimRepository>()

    override this.ExecuteAsync(stoppingToken: CancellationToken) =
        this._operationHandler.Subscribe("ClaimStatusConsumer")(Action<ClaimStatusEvent>(fun dictum ->
            async {
                let! claim = this._claimRepository.GetOneAsync(fun c -> c.Id = dictum.ClaimId)
                            |> Async.AwaitTask
                claim.Status <- dictum.Status

                let! _ = this._claimRepository.UpdateOneAsync(claim.Id)(claim)
                        |> Async.AwaitTask
                
                return true
            } |> Async.StartAsTask |> ignore
        ))
        Task.CompletedTask