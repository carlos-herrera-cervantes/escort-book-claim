namespace Claim.Web.Repositories

open System.Threading.Tasks
open System.Collections.Generic
open MongoDB.Driver
open Claim.Web.Models

type IPhysicalEvidenceRepository =
    abstract member GetAllAsync: FilterDefinition<PhysicalEvidence> -> Async<IEnumerable<PhysicalEvidence>>
    abstract member ValidateEvidenceNumber: string * string -> Async<bool>
    abstract member CreateAsync: PhysicalEvidence -> Task
