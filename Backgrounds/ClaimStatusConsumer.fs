namespace EscortBookClaim.Backgrounds

open System
open System.Threading.Tasks
open System.Threading
open Microsoft.Extensions.Hosting
open Microsoft.Extensions.DependencyInjection
open EscortBookClaim.Handlers
open EscortBookClaim.Types
open EscortBookClaim.Repositories

type ClaimStatusConsumer (factory: IServiceScopeFactory, operationHandler: IOperationHandler<ClaimStatusEvent>) =
    inherit BackgroundService()

    member this._operationHandler = operationHandler

    member this._claimRepository = factory.CreateScope().ServiceProvider.GetRequiredService<IClaimRepository>()

    override this.ExecuteAsync(stoppingToken: CancellationToken) =
        this._operationHandler.Subscribe("ClaimStatusConsumer")(Action<ClaimStatusEvent>(fun dictum ->
            async {
                let! claim = this._claimRepository.GetOneAsync(dictum.ClaimId)
                claim.Status <- dictum.Status
                let! _ = this._claimRepository.UpdateOneAsync(claim.Id)(claim)
                
                return true
            } |> Async.StartAsTask |> ignore
        ))
        Task.CompletedTask