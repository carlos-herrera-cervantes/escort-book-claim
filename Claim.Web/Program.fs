open Microsoft.AspNetCore.Builder
open Microsoft.Extensions.Hosting
open Microsoft.Extensions.DependencyInjection
open Microsoft.EntityFrameworkCore
open Claim.Web.Extensions.MongoDBExtensions
open Claim.Web.Extensions.AwsExtensions
open Claim.Web.Extensions.KafkaExtensions
open Claim.Web.Repositories
open Claim.Web.Services
open Claim.Web.Backgrounds
open Claim.Web.Handlers
open Claim.Web.Contexts
open Claim.Web.Config

[<EntryPoint>]
let main args =
    let builder = WebApplication.CreateBuilder args
    builder.Services.AddControllers().AddNewtonsoftJson() |> ignore
    builder.Services.AddDbContext<EscortBookClaimContext>(fun options ->
        options.UseNpgsql(PostgresConfig.CustomerDb) |> ignore) |> ignore
    builder.Services.AddDbContext<EscortProfileContext>(fun options ->
        options.UseNpgsql(PostgresConfig.EscortDb) |> ignore) |> ignore
    builder.Services.AddMongoDBClient() |> ignore
    builder.Services.AddS3Client() |> ignore
    builder.Services.AddKafkaProducer() |> ignore 
    builder.Services.AddTransient<IAWSS3Service, AWSS3Service>() |> ignore
    builder.Services.AddTransient<IClaimRepository, ClaimRepository>() |> ignore
    builder.Services.AddTransient<IPhysicalEvidenceRepository, PhysicalEvidenceRepository>() |> ignore
    builder.Services.AddTransient<IDictumRepository, DictumRepository>() |> ignore
    builder.Services.AddTransient<ICustomerProfileRepository, CustomerProfileRepository>() |> ignore
    builder.Services.AddTransient<IEscortProfileRepository, EscortProfileRepository>() |> ignore
    builder.Services.AddTransient<IServiceRepository, ServiceRepository>() |> ignore
    builder.Services.AddTransient<IKafkaService, KafkaService>() |> ignore
    builder.Services.AddSingleton(typedefof<IOperationHandler<_>>, typedefof<OperationHandler<_>>) |> ignore
    builder.Services.AddHostedService<ClaimStatusConsumer>() |> ignore
    builder.Services.AddHostedService<ServiceStatusConsumer>() |> ignore
    builder.Services.AddHostedService<ClaimCreatedConsumer>() |> ignore

    let app = builder.Build()

    app.UseHttpLogging() |> ignore
    app.UseRouting() |> ignore
    app.Run()

    0
