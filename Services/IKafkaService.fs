namespace EscortBookClaim.Services

open Confluent.Kafka

[<AllowNullLiteral>]
type IKafkaService =
    abstract member SendMessageAsync: string -> Message<Null, string> -> Async<bool>
