namespace EscortBookClaim.Models

type Dictum () =
    inherit BaseEntity()

    member val ClaimId: string = null with get, set

    member val Response: string = null with get, set

    member val UserId: string = null with get, set
