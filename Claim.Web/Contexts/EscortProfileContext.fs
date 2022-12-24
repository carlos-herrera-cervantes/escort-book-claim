namespace Claim.Web.Contexts

open Microsoft.EntityFrameworkCore
open Claim.Web.Models

type public EscortProfileContext(options: DbContextOptions<EscortProfileContext>) =
    inherit DbContext(options)

    [<DefaultValue>]
    val mutable profiles: DbSet<EscortProfile>

    member public this.Profiles with get() = this.profiles and set v = this.profiles <- v
