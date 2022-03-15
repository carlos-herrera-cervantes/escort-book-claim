namespace EscortBookClaim.Types

open System.Collections.Generic
open Newtonsoft.Json

[<AllowNullLiteral>]
type DecodedJwt() =

    [<JsonProperty("email")>]
    member val Email: string = null with get, set

    [<JsonProperty("roles")>]
    member val Roles: List<string> = null with get, set

    [<JsonProperty("id")>]
    member val Id: string = null with get, set

    [<JsonProperty("iat")>]
    member val Iat: int = 0 with get, set

    [<JsonProperty("exp")>]
    member val Exp: int = 0 with get, set

    [<JsonProperty("type")>]
    member val Type: string = null with get, set

type Payload() =
    [<JsonProperty("user")>]
    member val User: DecodedJwt = null with get, set