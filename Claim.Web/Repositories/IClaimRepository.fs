namespace Claim.Web.Repositories

open System.Collections.Generic
open System.Threading.Tasks
open MongoDB.Driver
open Claim.Web.Models

[<AllowNullLiteral>]
type IClaimRepository =
    abstract member GetAllAsync: int * int -> Async<IEnumerable<Claim>>
    abstract member GetAllByFilterAsync: FilterDefinition<Claim> * int * int -> Async<IEnumerable<Claim>>
    abstract member GetOneAsync: FilterDefinition<Claim> -> Task<Claim>
    abstract member CreateAsync: Claim -> Task
    abstract member UpdateOneAsync: string * Claim -> Task
