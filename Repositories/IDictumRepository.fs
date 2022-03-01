namespace EscortBookClaim.Repositories

open Microsoft.Azure.Cosmos
open EscortBookClaim.Models

type IDictumRepository =
    abstract member GetOneAsync: string -> Async<Dictum>
    abstract member CreateAsync: Dictum -> Async<ItemResponse<Dictum>>
    abstract member UpdateOneAsync: string -> Dictum -> Async<ItemResponse<Dictum>>