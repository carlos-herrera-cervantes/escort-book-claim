namespace EscortBookClaim.Models

open System
open MongoDB.Bson
open MongoDB.Bson.Serialization.Attributes
open System.Collections.Generic

[<AllowNullLiteral>]
type Service() =

    [<BsonElement("_id")>]
    [<BsonId>]
    [<BsonRepresentation(BsonType.ObjectId)>]
    member val Id: string = null with get, set

    [<BsonElement("cardId")>]
    [<BsonRepresentation(BsonType.ObjectId)>]
    member val CardId: string = null with get, set

    [<BsonElement("customerId")>]
    [<BsonRepresentation(BsonType.ObjectId)>]
    member val CustomerId: string = null with get, set

    [<BsonElement("escortId")>]
    [<BsonRepresentation(BsonType.ObjectId)>]
    member val EscortId: string = null with get, set

    [<BsonElement("price")>]
    member val Price: decimal = 0M with get, set

    [<BsonElement("status")>]
    member val Status: string = null with get, set

    [<BsonElement("timeQuantity")>]
    member val TimeQuatity: int = 0 with get, set

    [<BsonElement("timeMeasurementUnit")>]
    member val TimeMeasurementUnit: string = null with get, set

    [<BsonElement("details")>]
    member val Details: List<ObjectId> = null with get, set

    [<BsonElement("createdAt")>]
    [<BsonRepresentation(BsonType.DateTime)>]
    member val CreatedAt: DateTime = DateTime.UtcNow with get, set

    [<BsonElement("updatedAt")>]
    [<BsonRepresentation(BsonType.DateTime)>]
    member val UpdatedAt: DateTime = DateTime.UtcNow with get, set
