namespace Claim.Web.Repositories

open System.Threading.Tasks
open Claim.Web.Models

type ICustomerProfileRepository =
    abstract member GetByIdAsync: string -> Task<CustomerProfile>
