namespace EscortBookClaim.Models

open Newtonsoft.Json

type Claim () =
    inherit BaseEntity()
    
    [<JsonProperty("serviceId")>]
    member val ServiceId: string = null with get, set

    [<JsonProperty("customerId")>]
    member val CustomerId: string = null with get, set

    [<JsonProperty("escortId")>]
    member val EscortId: string = null with get, set

    [<JsonProperty("status")>]
    member val Status: string = null with get, set

    [<JsonProperty("comment")>]
    member val Comment: string = null with get, set
