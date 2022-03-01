namespace EscortBookClaim.Handlers

open System

[<AllowNullLiteral>]
type IOperationHandler<'a> =
    abstract member Publish: 'a -> unit
    abstract member Subscribe: string -> Action<'a> -> unit