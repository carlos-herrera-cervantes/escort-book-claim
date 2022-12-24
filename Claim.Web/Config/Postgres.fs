namespace Claim.Web.Config

open System

module PostgresConfig =

    let CustomerDb = Environment.GetEnvironmentVariable("CUSTOMER_DB")

    let EscortDb = Environment.GetEnvironmentVariable("ESCORT_DB")
