namespace Claim.Web.Services

open System.IO
open Amazon.S3
open Amazon.S3.Model
open Claim.Web.Config

type AWSS3Service(s3Client: IAmazonS3) =

    interface IAWSS3Service with

        member this.PutObjectAsync(key: string, claimId: string, imageStream: Stream) =
            async {
                let request =
                    PutObjectRequest(
                        InputStream = imageStream,
                        BucketName = AwsConfig.S3.Name,
                        Key = claimId + "/" + key
                    )
                let! _ = s3Client.PutObjectAsync(request) |> Async.AwaitTask
                return AwsConfig.S3.Endpoint +
                    "/" + AwsConfig.S3.Name +
                    "/" + claimId + "/" + key
            }
