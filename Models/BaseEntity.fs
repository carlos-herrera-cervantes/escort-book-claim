namespace EscortBookClaim.Models

open System
open Newtonsoft.Json
open MongoDB.Bson
open MongoDB.Bson.Serialization.Attributes

[<AllowNullLiteral>]
type BaseEntity () =

    [<BsonElement("_id")>]
    [<BsonId>]
    [<BsonRepresentation(BsonType.ObjectId)>]
    member val Id: string = null with get, set

    [<BsonElement("createdAt")>]
    [<BsonRepresentation(BsonType.DateTime)>]
    member val CreatedAt: DateTime = DateTime.UtcNow with get, set

    [<BsonElement("updatedAt")>]
    [<BsonRepresentation(BsonType.DateTime)>]
    member val UpdatedAt: DateTime = DateTime.UtcNow with get, set
