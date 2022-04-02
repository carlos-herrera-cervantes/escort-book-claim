namespace EscortBookClaim.Types

open EscortBookClaim.Constants

type ServiceStatusEvent() =
    
    member val ServiceId: string = null with get, set

    member val Status: string = null with get, set

    member this.ToService(): string =
        let mapping = dict[
            "Proceeds", "returned"
            "NotApplicable", "completed"
            "Rejected", "completed"
            "InReview", "reclaimed"
            "Cancelled", "completed"
        ]

        mapping.Item(this.Status)