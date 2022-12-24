namespace Claim.Tests.Repositories

open Xunit
open System.ComponentModel
open System.Diagnostics.CodeAnalysis
open Microsoft.EntityFrameworkCore
open Claim.Web.Repositories
open Claim.Web.Contexts
open Claim.Web.Config

[<Collection("EscortProfileRepository")>]
[<Category("Repositories")>]
[<ExcludeFromCodeCoverage>]
type EscortProfileRepositoryTests() =

    member this._contextOptions =
        DbContextOptionsBuilder<EscortProfileContext>().UseNpgsql(PostgresConfig.EscortDb).Options

    [<Fact(DisplayName = "GetByIdAsync should return null")>]
    member this.GetByIdAsyncShouldReturnNull() =
        async {
            use context = new EscortProfileContext(this._contextOptions)
            let escortProfileRepository = EscortProfileRepository(context) :> IEscortProfileRepository
            let id = "86619170-959b-46e8-b6d0-773157120594"

            let! escortProfile = escortProfileRepository.GetByIdAsync(id) |> Async.AwaitTask

            Assert.Null(escortProfile)
        }
