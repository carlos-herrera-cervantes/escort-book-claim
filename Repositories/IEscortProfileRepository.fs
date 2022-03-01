namespace EscortBookClaim.Repositories

open System.Threading.Tasks
open EscortBookClaim.Models

type IEscortProfileRepository =
    abstract member GetByIdAsync: string -> Task<EscortProfile>
