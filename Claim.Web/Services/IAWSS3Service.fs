namespace Claim.Web.Services

open System.IO

[<AllowNullLiteral>]
type IAWSS3Service =
    abstract member PutObjectAsync: string * string * Stream -> Async<string>
