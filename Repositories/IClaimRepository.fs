namespace EscortBookClaim.Repositories

open System
open System.Linq.Expressions
open Microsoft.AspNetCore.JsonPatch
open System.Collections.Generic
open System.Threading.Tasks
open MongoDB.Driver
open EscortBookClaim.Models

[<AllowNullLiteral>]
type IClaimRepository =
    abstract member GetAllAsync: Expression<Func<Claim, bool>> * int * int -> Task<List<Claim>>
    abstract member GetOneAsync: Expression<Func<Claim, bool>> -> Task<Claim>
    abstract member CreateAsync: Claim -> Task
    abstract member UpdateOneAsync: string -> Claim -> Task<ReplaceOneResult>
