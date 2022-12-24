namespace Claim.Web.Models

open System
open System.ComponentModel.DataAnnotations.Schema

[<Table("profile", Schema = "public")>]
type CustomerProfile () =

    [<Column("id")>]
    member val ID: string = null with get, set

    [<Column("customer_id")>]
    member val CustomerID: string = null with get, set

    [<Column("first_name")>]
    member val FirstName: string = null with get, set

    [<Column("last_name")>]
    member val LastName: string = null with get, set

    [<Column("email")>]
    member val Email: string = null with get, set

    [<Column("phone_number")>]
    member val PhoneNumber: string = null with get, set

    [<Column("gender")>]
    member val Gender: string = null with get, set

    [<Column("birthdate")>]
    member val Birthdate: DateTime = DateTime.UtcNow with get, set

    [<Column("created_at")>]
    member val CreatedAt: DateTime = DateTime.UtcNow with get, set

    [<Column("updated_at")>]
    member val UpdatedAt: DateTime = DateTime.UtcNow with get, set
