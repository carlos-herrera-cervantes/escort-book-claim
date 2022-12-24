namespace Claim.Web.Repositories

open System.Threading.Tasks
open Claim.Web.Models

type IEscortProfileRepository =
    abstract member GetByIdAsync: string -> Task<EscortProfile>
