namespace EscortBookClaim.Models

open Newtonsoft.Json

type Dictum () =
    inherit BaseEntity()

    [<JsonProperty("claimId")>]
    member val ClaimId: string = null with get, set

    [<JsonProperty("response")>]
    member val Response: string = null with get, set

    [<JsonProperty("userId")>]
    member val UserId: string = null with get, set

    [<JsonProperty("status")>]
    member val Status: string = null with get, set
