namespace Claim.Web.Extensions

open Microsoft.Extensions.DependencyInjection
open Confluent.Kafka
open Confluent.Kafka.DependencyInjection
open Claim.Web.Services
open Claim.Web.Config

module KafkaExtensions =

    type IServiceCollection with

    member this.AddKafkaProducer() =
        let producerConfig = ProducerConfig()
        producerConfig.BootstrapServers <- KafkaConfig.Servers
        producerConfig.ClientId <- KafkaConfig.ClientId

        this.AddKafkaClient<IKafkaService>(producerConfig) |> ignore
        this
