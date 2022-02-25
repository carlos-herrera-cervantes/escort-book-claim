namespace EscortBookClaim.Repositories

open System.Collections.Generic
open Microsoft.Azure.Cosmos
open EscortBookClaim.Models

[<AllowNullLiteral>]
type IClaimRepository =
    abstract member GetAllAsync: int * int -> Async<IEnumerable<Claim>>
    abstract member GetOneAsync: string -> Async<Claim>
    abstract member CreateAsync: Claim -> Async<ItemResponse<Claim>>
    abstract member UpdateOneAsync: string -> Claim -> Async<ItemResponse<Claim>>
