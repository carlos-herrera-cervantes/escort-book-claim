namespace Claim.Tests.Repositories

open Xunit
open System.ComponentModel
open System.Diagnostics.CodeAnalysis
open Microsoft.EntityFrameworkCore
open Claim.Web.Repositories
open Claim.Web.Contexts
open Claim.Web.Config

[<Collection("CustomerProfileRepository")>]
[<Category("Repositories")>]
[<ExcludeFromCodeCoverage>]
type CustomerProfileRepositoryTests() =

    member this._contextOptions =
        DbContextOptionsBuilder<EscortBookClaimContext>().UseNpgsql(PostgresConfig.CustomerDb).Options

    [<Fact(DisplayName = "Should return null")>]
    member this.GetByIdAsyncShouldReturnNull() =
        async {
            use context = new EscortBookClaimContext(this._contextOptions)
            let customerProfileRepository = CustomerProfileRepository(context) :> ICustomerProfileRepository
            let id = "943227dc-fefa-4d50-9eee-20e9de70c575"

            let! customerProfile = customerProfileRepository.GetByIdAsync(id) |> Async.AwaitTask

            Assert.Null(customerProfile)
        }
