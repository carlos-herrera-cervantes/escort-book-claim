namespace Claim.Web.Repositories

open System.Threading.Tasks
open Microsoft.EntityFrameworkCore
open Claim.Web.Contexts
open Claim.Web.Models

type CustomerProfileRepository(context: EscortBookClaimContext) =

    member this._context = context

    interface ICustomerProfileRepository with

        member this.GetByIdAsync(id: string): Task<CustomerProfile> =
            this._context
                .Profiles
                .AsNoTracking().
                FirstOrDefaultAsync(fun p -> p.CustomerID = id)
