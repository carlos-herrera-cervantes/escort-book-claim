namespace Claim.Web.Common

open Claim.Web.Handlers

[<AbstractClass; Sealed>]
type Emitter<'a> =
    
    static member EmitMessage(operationHandler: IOperationHandler<'a>, message: 'a) =
        operationHandler.Publish(message)
