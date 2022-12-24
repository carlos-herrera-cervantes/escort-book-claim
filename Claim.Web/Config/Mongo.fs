namespace Claim.Web.Config

open System

module MongoConfig =

    let Host = Environment.GetEnvironmentVariable("MONGO_HOST")

    module Databases =

        let PaymentDb = Environment.GetEnvironmentVariable("PAYMENT_DB")

        let ClaimDb = Environment.GetEnvironmentVariable("CLAIM_DB")
