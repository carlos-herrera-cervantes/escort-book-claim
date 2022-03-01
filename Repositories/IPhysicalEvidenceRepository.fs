namespace EscortBookClaim.Repositories

open EscortBookClaim.Models
open Microsoft.Azure.Cosmos
open System.Collections.Generic

type IPhysicalEvidenceRepository =
    abstract member GetAllAsync: unit -> Async<IEnumerable<PhysicalEvidence>>
    abstract member ValidateEvidenceNumber: string -> Async<bool>
    abstract member CreateAsync: PhysicalEvidence -> Async<ItemResponse<PhysicalEvidence>>