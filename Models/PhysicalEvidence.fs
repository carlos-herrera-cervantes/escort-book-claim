namespace EscortBookClaim.Models

type PhysicalEvidence () =
    inherit BaseEntity()

    member val ClaimId: string = null with get, set

    member val Path: string = null with get, set
