namespace Claim.Web.Repositories

open Microsoft.AspNetCore.JsonPatch
open System.Threading.Tasks
open MongoDB.Driver
open Claim.Web.Models

type IDictumRepository =
    abstract member GetOneAsync: FilterDefinition<Dictum> -> Task<Dictum>
    abstract member CreateAsync: Dictum -> Task
    abstract member UpdateOneAsync: string * Dictum * JsonPatchDocument<Dictum> -> Task
