﻿namespace EscortBookClaim.Repositories

open Microsoft.EntityFrameworkCore
open EscortBookClaim.Contexts

type EscortProfileRepository(context: EscortProfileContext) =
    
    member this._context = context

    interface IEscortProfileRepository with

        member this.GetByIdAsync(id: string) =
            this._context.Profiles.AsNoTracking().FirstOrDefaultAsync(fun p -> p.EscortId = id)