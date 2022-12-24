namespace Claim.Tests.Services

open Xunit
open Moq
open Confluent.Kafka
open System.ComponentModel
open System.Diagnostics.CodeAnalysis
open Claim.Web.Services

[<Collection("KafkaService")>]
[<Category("Services")>]
[<ExcludeFromCodeCoverage>]
type KafkaServiceTests() =

    [<Fact(DisplayName = "Should return true when process succeeds")>]
    member this.SendMessageAsyncShouldReturnTrue() =
        async {
            let mockProducer = Mock<IProducer<Null, string>>()
            let topic = "test-topic"
            let message = Message<Null, string>()

            let kafkaService = KafkaService(mockProducer.Object) :> IKafkaService
            let! result = kafkaService.SendMessageAsync(topic, message)

            mockProducer.Verify((fun x ->
                x.ProduceAsync(
                    It.IsAny<string>(),
                    It.IsAny<Message<Null, string>>())
                ), Times.Once)
            Assert.True(result)
        }
