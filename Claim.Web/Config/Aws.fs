namespace Claim.Web.Config

open System

module AwsConfig =

    module S3 =

        let AccessKey = Environment.GetEnvironmentVariable("S3_ACCESS_KEY")

        let SecretKey = Environment.GetEnvironmentVariable("S3_SECRET_KEY")

        let Endpoint = Environment.GetEnvironmentVariable("S3_ENDPOINT")

        let Region = Environment.GetEnvironmentVariable("S3_REGION")

        let Name = Environment.GetEnvironmentVariable("S3_NAME")
