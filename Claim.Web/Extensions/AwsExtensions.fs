namespace Claim.Web.Extensions

open Microsoft.Extensions.DependencyInjection
open Amazon.Runtime
open Amazon.S3
open Claim.Web.Config

module AwsExtensions =

    type IServiceCollection with

        member this.AddS3Client() =
            let credentials = BasicAWSCredentials(AwsConfig.S3.AccessKey, AwsConfig.S3.SecretKey)
            let s3Config =
                AmazonS3Config(
                    ServiceURL = AwsConfig.S3.Endpoint,
                    UseHttp = true,
                    ForcePathStyle = true,
                    AuthenticationRegion = AwsConfig.S3.Region
                )
            let s3Client = new AmazonS3Client(credentials, s3Config)

            this.AddSingleton<IAmazonS3>(fun _ -> s3Client :> IAmazonS3) |> ignore
            this
