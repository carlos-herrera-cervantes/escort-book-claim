namespace EscortBookClaim.Contexts

open Microsoft.EntityFrameworkCore
open EscortBookClaim.Models

type public EscortBookClaimContext(options: DbContextOptions<EscortBookClaimContext>) =
    inherit DbContext(options)

    [<DefaultValue>]
    val mutable profiles: DbSet<CustomerProfile>

    member public this.Profiles with get() = this.profiles and set v = this.profiles <- v
