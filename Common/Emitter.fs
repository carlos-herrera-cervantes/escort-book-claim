namespace EscortBookClaim.Common

open EscortBookClaim.Handlers

[<AbstractClass; Sealed>]
type Emitter<'a> =
    
    static member EmitMessage(operationHandler: IOperationHandler<'a>, message: 'a) =
        operationHandler.Publish(message)
