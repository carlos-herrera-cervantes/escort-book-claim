﻿namespace EscortBookClaim.Controllers

open Microsoft.AspNetCore.Mvc
open Microsoft.AspNetCore.Http
open Microsoft.Extensions.Configuration
open System.Linq
open EscortBookClaim.Repositories
open EscortBookClaim.Services
open EscortBookClaim.Models
open EscortBookClaim.Attributes

[<Route("api/v1/claims/{id}/physical-evidence")>]
[<Produces("application/json")>]
[<ApiController>]
type PhysicalEvidenceController
    (
        physicalEvidenceRepository: IPhysicalEvidenceRepository,
        s3Service: IAWSS3Service,
        configuration: IConfiguration
    ) =
    inherit ControllerBase()
    
    member this._physicalEvidenceRepository = physicalEvidenceRepository

    member this._s3Service = s3Service

    member this._configuration = configuration

    [<HttpGet>]
    member this.GetAllAsync
        (
            [<FromRoute>] id: string,
            [<FromHeader(Name = "user-id")>] userId: string
        ) =
        async {
            let! physicalEvidence = this._physicalEvidenceRepository.GetAllAsync(fun p -> p.ClaimId = id && p.UserId = userId)
                                    |> Async.AwaitTask
            let endpoint = this._configuration
                            .GetSection("AWS")
                            .GetSection("S3")
                            .GetSection("Endpoint").Value
            
            let bucketName = this._configuration
                                .GetSection("AWS")
                                .GetSection("S3")
                                .GetSection("Name").Value

            let evidence = physicalEvidence.Select(fun p -> p.Path <- endpoint + "/" + bucketName + "/" + p.Path; p)

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
        ) =
        async {
            let! existsSpace = this._physicalEvidenceRepository.ValidateEvidenceNumber(id)(userId)

            match existsSpace with
            | true ->
                let imageStream = image.OpenReadStream()
                let! url = this._s3Service.PutObjectAsync(image.FileName)(id)(imageStream)
                    
                let newPhysicalEvidence = PhysicalEvidence()
                newPhysicalEvidence.ClaimId <- id
                newPhysicalEvidence.Path <- id + "/" + image.FileName
                newPhysicalEvidence.UserId <- userId

                let! _ = this._physicalEvidenceRepository.CreateAsync(newPhysicalEvidence) |> Async.AwaitTask
                newPhysicalEvidence.Path <- url

                return this.Created("", newPhysicalEvidence) :> IActionResult
            | false ->
                return this.BadRequest() :> IActionResult
        }