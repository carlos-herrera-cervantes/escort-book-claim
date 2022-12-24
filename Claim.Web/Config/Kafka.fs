namespace Claim.Web.Config

open System

module KafkaConfig =

    let Servers = Environment.GetEnvironmentVariable("KAFKA_SERVERS")

    let ClientId = Environment.GetEnvironmentVariable("KAFKA_CLIENT_ID")

    module Topics =

        [<Literal>]
        let OperationStatistics = "operations-statistics"
