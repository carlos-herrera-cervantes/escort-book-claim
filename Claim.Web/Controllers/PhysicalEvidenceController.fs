namespace Claim.Web.Controllers

open System
open System.Linq
open Microsoft.AspNetCore.Mvc
open Microsoft.AspNetCore.Http
open MongoDB.Driver
open Claim.Web.Repositories
open Claim.Web.Services
open Claim.Web.Models
open Claim.Web.Attributes
open Claim.Web.Config

[<Route("api/v1/claims/{id}/physical-evidence")>]
[<Produces("application/json")>]
[<ApiController>]
type PhysicalEvidenceController
    (
        physicalEvidenceRepository: IPhysicalEvidenceRepository,
        s3Service: IAWSS3Service
    ) =
    inherit ControllerBase()
    
    member this._physicalEvidenceRepository = physicalEvidenceRepository

    member this._s3Service = s3Service

    [<HttpGet>]
    member this.GetAllAsync([<FromRoute>] id: string, [<FromHeader(Name = "user-id")>] userId: string): Async<IActionResult> =
        async {
            let! physicalEvidence =
                this._physicalEvidenceRepository
                    .GetAllAsync(
                        Builders<PhysicalEvidence>.Filter.And(
                            Builders<PhysicalEvidence>.Filter.Eq((fun p -> p.ClaimId), id),
                            Builders<PhysicalEvidence>.Filter.Eq((fun p -> p.UserId), userId)
                        )
                    )
            let selector =
                Func<PhysicalEvidence, PhysicalEvidence>(
                    fun p -> p.Path <- AwsConfig.S3.Endpoint + "/" + AwsConfig.S3.Name + "/" + p.Path; p
                )
            let evidence = physicalEvidence.Select(selector)

            return evidence |> this.Ok :> IActionResult
        }

    [<HttpPost>]
    [<ClaimExists>]
    [<RequestSizeLimit(2000000L)>]
    member this.CreateAsync
        (
            [<FromRoute>] id: string,
            [<FromForm>] image: IFormFile,
            [<FromHeader(Name = "user-id")>] userId: string
        ): Async<IActionResult> =
        async {
            let! existsSpace = this._physicalEvidenceRepository.ValidateEvidenceNumber(id, userId)

            match existsSpace with
            | true ->
                let imageStream = image.OpenReadStream()
                let! url = this._s3Service.PutObjectAsync(image.FileName, id, imageStream)

                let newPhysicalEvidence = PhysicalEvidence()
                newPhysicalEvidence.ClaimId <- id
                newPhysicalEvidence.Path <- id + "/" + image.FileName
                newPhysicalEvidence.UserId <- userId

                do! this._physicalEvidenceRepository.CreateAsync(newPhysicalEvidence) |> Async.AwaitTask
                newPhysicalEvidence.Path <- url

                return this.Created("", newPhysicalEvidence) :> IActionResult
            | false ->
                return this.BadRequest() :> IActionResult
        }
