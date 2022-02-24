namespace EscortBookClaim.Models

type Claim () =
    inherit BaseEntity()
    
    member val ServiceId: string = null with get, set

    member val customerId: string = null with get, set

    member val escortId: string = null with get, set

    member val status: string = null with get, set

    member val comment: string = null with get, set
