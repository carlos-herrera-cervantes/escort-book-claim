namespace EscortBookClaim.Services

open Microsoft.Extensions.Configuration
open System.IO
open Amazon.Runtime
open Amazon.S3
open Amazon.S3.Model

type AWSS3Service (configuration: IConfiguration) =
    
    let s3Section = configuration.GetSection("AWS").GetSection("S3")
    let accessKey = s3Section.GetSection("AccessKey").Value
    let secretKey = s3Section.GetSection("SecretKey").Value
    let credentials = BasicAWSCredentials(accessKey, secretKey)
    let s3Config = AmazonS3Config(
        ServiceURL = s3Section.GetSection("Endpoint").Value,
        UseHttp = true,
        ForcePathStyle = true,
        AuthenticationRegion = s3Section.GetSection("Region").Value
    )

    member this._s3Client = new AmazonS3Client(credentials, s3Config)

    member this._bucketName = s3Section.GetSection("Name").Value

    member this._endpoint = s3Section.GetSection("Endpoint").Value

    interface IAWSS3Service with

        member this.PutObjectAsync(key: string) (claimId: string) (imageStream: Stream) =
            async {
                let request = PutObjectRequest(
                    InputStream = imageStream,
                    BucketName = this._bucketName,
                    Key = claimId + "/" + key
                )

                let! _ = this._s3Client.PutObjectAsync(request) |> Async.AwaitTask
                return this._endpoint + "/" + this._bucketName + "/" + claimId + "/" + key
            }
