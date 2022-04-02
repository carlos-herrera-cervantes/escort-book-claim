namespace EscortBookClaim.Repositories

open System
open System.Threading.Tasks
open System.Linq.Expressions
open EscortBookClaim.Models
open MongoDB.Driver

type IServiceRepository =
    abstract member GetOneAsync: Expression<Func<Service, bool>> -> Task<Service>
    abstract member UpdateOneAsync: string -> Service -> Task<ReplaceOneResult>
