namespace EscortBookClaim.Repositories

open EscortBookClaim.Models
open System.Threading.Tasks
open System.Collections.Generic
open System.Linq.Expressions
open System

type IPhysicalEvidenceRepository =
    abstract member GetAllAsync: Expression<Func<PhysicalEvidence, bool>> -> Task<List<PhysicalEvidence>>
    abstract member ValidateEvidenceNumber: string -> string -> Async<bool>
    abstract member CreateAsync: PhysicalEvidence -> Task