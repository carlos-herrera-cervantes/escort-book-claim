namespace EscortBookClaim.Repositories

open Microsoft.EntityFrameworkCore
open EscortBookClaim.Contexts

type CustomerProfileRepository(context: EscortBookClaimContext) =

    member this._context = context

    interface ICustomerProfileRepository with

        member this.GetByIdAsync(id: string) =
            this._context.Profiles.AsNoTracking().FirstOrDefaultAsync(fun p -> p.CustomerID = id)
