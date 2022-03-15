namespace EscortBookClaim.Repositories

open EscortBookClaim.Models
open System.Threading.Tasks
open System.Collections.Generic

type IPhysicalEvidenceRepository =
    abstract member GetAllAsync: string -> Task<List<PhysicalEvidence>>
    abstract member ValidateEvidenceNumber: string -> Async<bool>
    abstract member CreateAsync: PhysicalEvidence -> Task