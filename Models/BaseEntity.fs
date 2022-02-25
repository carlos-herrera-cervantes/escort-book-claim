namespace EscortBookClaim.Models

open System
open Newtonsoft.Json

[<AllowNullLiteral>]
type BaseEntity () =

    [<JsonProperty("id")>]
    member val Id: string = Guid.NewGuid().ToString() with get, set

    [<JsonProperty("createdAt")>]
    member val CreatedAt: DateTime = DateTime.UtcNow with get, set

    [<JsonProperty("updatedAt")>]
    member val UpdatedAt: DateTime = DateTime.UtcNow with get, set
