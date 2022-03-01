namespace EscortBookClaim.Repositories

open System.Threading.Tasks
open EscortBookClaim.Models

type IServiceRepository =
    abstract member GetByIdAsync: string -> Task<Service>
