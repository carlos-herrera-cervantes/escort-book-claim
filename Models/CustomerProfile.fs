namespace EscortBookClaim.Models

open System
open System.ComponentModel.DataAnnotations.Schema

[<Table("Profiles")>]
type CustomerProfile () =

    member val ID: string = null with get, set

    member val CustomerID: string = null with get, set

    member val FirstName: string = null with get, set

    member val LastName: string = null with get, set

    member val Email: string = null with get, set

    member val PhoneNumber: string = null with get, set

    member val Gender: string = null with get, set

    member val Birthdate: DateTime = DateTime.UtcNow with get, set

    member val CreatedAt: DateTime = DateTime.UtcNow with get, set

    member val UpdatedAt: DateTime = DateTime.UtcNow with get, set
