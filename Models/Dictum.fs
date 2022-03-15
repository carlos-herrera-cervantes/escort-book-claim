namespace EscortBookClaim.Models

open MongoDB.Bson
open MongoDB.Bson.Serialization.Attributes
open Newtonsoft.Json

type Dictum () =
    inherit BaseEntity()

    [<BsonElement("serviceId")>]
    [<BsonRepresentation(BsonType.ObjectId)>]
    member val ClaimId: string = null with get, set

    [<BsonElement("response")>]
    member val Response: string = null with get, set

    [<BsonElement("userId")>]
    [<BsonRepresentation(BsonType.ObjectId)>]
    member val UserId: string = null with get, set

    [<BsonElement("status")>]
    member val Status: string = null with get, set
