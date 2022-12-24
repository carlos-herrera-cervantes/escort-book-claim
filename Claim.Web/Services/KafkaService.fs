namespace Claim.Web.Services

open Confluent.Kafka

type KafkaService(producer: IProducer<Null, string>) =

    interface IKafkaService with

        member this.SendMessageAsync(topic: string, message: Message<Null, string>) =
            async {
                let! _ = producer.ProduceAsync(topic, message) |> Async.AwaitTask
                return true
            }
