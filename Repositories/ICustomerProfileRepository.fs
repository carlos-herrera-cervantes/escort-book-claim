namespace EscortBookClaim.Repositories

open System.Threading.Tasks
open EscortBookClaim.Models

type ICustomerProfileRepository =
    abstract member GetByIdAsync: string -> Task<CustomerProfile>
