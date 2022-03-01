namespace EscortBookClaim.Models

open Newtonsoft.Json

type PhysicalEvidence () =
    inherit BaseEntity()

    [<JsonProperty("claimId")>]
    member val ClaimId: string = null with get, set

    [<JsonProperty("path")>]
    member val Path: string = null with get, set
