namespace Claim.Web.Repositories

open System.Threading.Tasks
open MongoDB.Driver
open Claim.Web.Models

type IServiceRepository =
    abstract member GetOneAsync: FilterDefinition<Service> -> Task<Service>
    abstract member UpdateOneAsync: string * Service -> Task<ReplaceOneResult>
