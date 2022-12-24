namespace Claim.Tests.Services

open System.Threading
open Xunit
open Moq
open Amazon.S3
open Amazon.S3.Model
open Microsoft.AspNetCore.Http
open System.ComponentModel
open System.Diagnostics.CodeAnalysis
open Claim.Web.Services
open Claim.Web.Config

[<Collection("AWSS3Service")>]
[<Category("Services")>]
[<ExcludeFromCodeCoverage>]
type AWSS3ServiceTests() =

    [<Fact(DisplayName = "Should return string when process succeeds")>]
    member this.PutObjectAsyncShouldReturnString() =
        async {
            let mockAmazonS3 = Mock<IAmazonS3>()
            let mockFormFile = Mock<IFormFile>()
            let AwsS3Service = AWSS3Service(mockAmazonS3.Object) :> IAWSS3Service
            let key = "proifle.png"
            let claimId = "639f6dee05f54b0dafd29287"

            let! path = AwsS3Service.PutObjectAsync(key, claimId, mockFormFile.Object.OpenReadStream())

            mockAmazonS3.Verify((fun x -> x.PutObjectAsync(It.IsAny<PutObjectRequest>(), It.IsAny<CancellationToken>())), Times.Once)
            let expectedPath = AwsConfig.S3.Endpoint + "/" + AwsConfig.S3.Name + "/" + claimId + "/" + key
            Assert.Equal(expectedPath, path)
        }
