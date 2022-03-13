namespace EscortBookClaim.Models

open MongoDB.Bson
open MongoDB.Bson.Serialization.Attributes
open Newtonsoft.Json

type PhysicalEvidence () =
    inherit BaseEntity()

    [<BsonElement("claimId")>]
    [<BsonRepresentation(BsonType.ObjectId)>]
    member val ClaimId: string = null with get, set

    [<BsonElement("path")>]
    member val Path: string = null with get, set
