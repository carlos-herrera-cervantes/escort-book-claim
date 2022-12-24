namespace Claim.Web.Backgrounds

open System
open System.Threading.Tasks
open System.Threading
open Microsoft.Extensions.Hosting
open Microsoft.Extensions.DependencyInjection
open Newtonsoft.Json
open Confluent.Kafka
open Claim.Web.Types
open Claim.Web.Handlers
open Claim.Web.Services
open Claim.Web.Config

type ClaimCreatedConsumer(factory: IServiceScopeFactory, operationHandler: IOperationHandler<ClaimCreatedEvent>) =
    inherit BackgroundService()

    member this._operationHandler = operationHandler

    member this._kafkaService = factory.CreateScope().ServiceProvider.GetRequiredService<IKafkaService>()

    override this.ExecuteAsync(stoppingToken: CancellationToken) =
        this._operationHandler.Subscribe("ClaimCreatedConsumer")(Action<ClaimCreatedEvent>(fun claimCreatedEvent ->
            async {
                let bytes = JsonConvert.SerializeObject(claimCreatedEvent)
                let message = Message<Null, string>()
                message.Value <- bytes

                let! _ = this._kafkaService.SendMessageAsync(KafkaConfig.Topics.OperationStatistics, message)

                return true
            } |> Async.StartAsTask |> ignore
        ))
        Task.CompletedTask
