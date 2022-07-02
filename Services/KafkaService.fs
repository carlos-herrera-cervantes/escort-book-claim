namespace EscortBookClaim.Services

open Microsoft.Extensions.Configuration
open Confluent.Kafka

type KafkaService(configuration: IConfiguration) =

    member this._configuration = configuration

    interface IKafkaService with

        member this.SendMessageAsync(topic: string)(message: Message<Null, string>) =
            async {
                let kafkaSection = configuration.GetSection("Kafka")
                let bootstrapServers = kafkaSection.GetSection("Servers").Value
                let clientId = kafkaSection.GetSection("ClientId").Value
                
                let config = ProducerConfig()
                config.BootstrapServers <- bootstrapServers
                config.ClientId <- clientId

                use producer = ProducerBuilder<Null, string>(config).Build()
                let! _ = producer.ProduceAsync(topic, message) |> Async.AwaitTask

                return true
            }
