namespace EscortBookClaim.Models

open System

type BaseEntity () =

    member val Id: string = null with get, set

    member val CreatedAt: DateTime = DateTime.UtcNow with get, set

    member val UpdatedAt: DateTime = DateTime.UtcNow with get, set
