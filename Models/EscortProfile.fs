namespace EscortBookClaim.Models

open System
open System.ComponentModel.DataAnnotations.Schema

[<Table("profiles", Schema = "public")>]
type EscortProfile () =

    [<Column("id")>]
    member val ID: string = null with get, set

    [<Column("escortid")>]
    member val EscortId: string = null with get, set

    [<Column("firstname")>]
    member val FirstName: string = null with get, set

    [<Column("lastname")>]
    member val LastName: string = null with get, set

    [<Column("email")>]
    member val Email: string = null with get, set

    [<Column("phonenumber")>]
    member val PhoneNumber: string = null with get, set

    [<Column("gender")>]
    member val Gender: string = null with get, set

    [<Column("nationalityid")>]
    member val NationalityId: string = null with get, set

    [<Column("birthdate")>]
    member val Birthdate: DateTime = DateTime.UtcNow with get, set

    [<Column("createdat")>]
    member val CreatedAt: DateTime = DateTime.UtcNow with get, set

    [<Column("updatedat")>]
    member val UpdatedAt: DateTime = DateTime.UtcNow with get, set
