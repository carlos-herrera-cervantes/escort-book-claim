namespace EscortBookClaim.Controllers

open Microsoft.AspNetCore.Mvc
open EscortBookClaim.Repositories
open EscortBookClaim.Models

[<Route("api/v1/claims")>]
[<Produces("application/json")>]
[<ApiController>]
type ClaimController (claimRepository: IClaimRepository) =
    inherit ControllerBase()

    member this._claimRepository = claimRepository

    [<HttpGet>]
    member this.GetAllAsync() =
        async {
            let! claims = this._claimRepository.GetAllAsync(0, 10)
            return claims |> this.Ok :> IActionResult
        }

    [<HttpGet("{id}")>]
    member this.GetOneAsync([<FromRoute>] id: string) =
        async {
            let! claim = this._claimRepository.GetOneAsync(id)

            match box claim with
            | null ->
                return NotFoundResult() :> IActionResult
            | _ ->
                return claim |> this.Ok :> IActionResult
        }

    [<HttpPost>]
    member this.CreateAsync([<FromBody>] claim: Claim) =
        async {
            let! _ = this._claimRepository.CreateAsync(claim)
            return this.Created("", claim) :> IActionResult
        }