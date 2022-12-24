namespace Claim.Web.Contexts

open Microsoft.EntityFrameworkCore
open Claim.Web.Models

type public EscortBookClaimContext(options: DbContextOptions<EscortBookClaimContext>) =
    inherit DbContext(options)

    [<DefaultValue>]
    val mutable profiles: DbSet<CustomerProfile>

    member public this.Profiles with get() = this.profiles and set v = this.profiles <- v
