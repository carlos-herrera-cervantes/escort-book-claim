namespace EscortBookClaim.Types

type ClaimStatusEvent () =
    
    member val ClaimId: string = null with get, set

    member val Status: string = null with get, set
