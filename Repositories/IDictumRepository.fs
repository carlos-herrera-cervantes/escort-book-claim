namespace EscortBookClaim.Repositories

open System
open Microsoft.AspNetCore.JsonPatch
open System.Linq.Expressions
open System.Threading.Tasks
open MongoDB.Driver
open EscortBookClaim.Models

type IDictumRepository =
    abstract member GetOneAsync: Expression<Func<Dictum, bool>> -> Task<Dictum>
    abstract member CreateAsync: Dictum -> Task
    abstract member UpdateOneAsync: string -> Dictum -> JsonPatchDocument<Dictum> -> Task<ReplaceOneResult>