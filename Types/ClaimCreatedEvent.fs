namespace EscortBookClaim.Types

open Newtonsoft.Json

type ClaimCreatedEvent() =

    [<JsonProperty("operation")>]
    member val Operation: string = null with get, set

    [<JsonProperty("customerId")>]
    member val CustomerId: string = null with get, set

    [<JsonProperty("escortId")>]
    member val EscortId: string = null with get, set

    [<JsonProperty("to")>]
    member val To: string = null with get, set
