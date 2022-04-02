namespace EscortBookClaim.Models

open MongoDB.Bson
open MongoDB.Bson.Serialization.Attributes
open Newtonsoft.Json
open EscortBookClaim.Constants
open EscortBookClaim.Types

[<AllowNullLiteral>]
type Claim() =
    inherit BaseEntity()
    
    [<BsonElement("serviceId")>]
    [<BsonRepresentation(BsonType.ObjectId)>]
    member val ServiceId: string = null with get, set

    [<BsonElement("customerId")>]
    [<BsonRepresentation(BsonType.ObjectId)>]
    member val CustomerId: string = null with get, set

    [<BsonElement("escortId")>]
    [<BsonRepresentation(BsonType.ObjectId)>]
    member val EscortId: string = null with get, set

    [<BsonElement("status")>]
    member val Status: string = ClaimStatus.InReview with get, set

    [<BsonElement("comment")>]
    member val Comment: string = null with get, set

    [<BsonElement("argument")>]
    member val Argument: string = null with get, set

type CreateClaimDTO() =

    [<JsonProperty("serviceId")>]
    member val ServiceId: string = null with get, set

    [<JsonProperty("customerId")>]
    member val CustomerId: string = null with get, set

    [<JsonProperty("escortId")>]
    member val EscortId: string = null with get, set

    [<JsonProperty("comment")>]
    member val Comment: string = null with get, set

    [<JsonProperty("user")>]
    member val User: DecodedJwt = null with get, set

type ClaimDetailDTO() =
    inherit BaseEntity()

    [<JsonProperty("escort")>]
    member val Escort: string = null with get, set

    [<JsonProperty("customer")>]
    member val Customer: string = null with get, set

    [<JsonProperty("comment")>]
    member val Comment: string = null with get, set

    [<JsonProperty("dictum")>]
    member val Dictum: string = null with get, set

    [<JsonProperty("status")>]
    member val Status: string = null with get, set

    [<JsonProperty("price")>]
    member val Price: decimal = 0M with get, set

    [<JsonProperty("time")>]
    member val Time: int = 0 with get, set

    [<JsonProperty("timeMeasurementUnit")>]
    member val TimeMeasurementUnit: string = null with get, set

    [<BsonElement("argument")>]
    member val Argument: string = null with get, set
