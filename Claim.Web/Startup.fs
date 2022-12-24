namespace Claim.Web

open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Hosting
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

type Startup() =

    member this.ConfigureServices(services: IServiceCollection) =
        services.AddControllers().AddNewtonsoftJson() |> ignore
        services.AddDbContext<EscortBookClaimContext>(fun options ->
            options.UseNpgsql(PostgresConfig.CustomerDb) |> ignore) |> ignore
        services.AddDbContext<EscortProfileContext>(fun options ->
            options.UseNpgsql(PostgresConfig.EscortDb) |> ignore) |> ignore
        services.AddMongoDBClient() |> ignore
        services.AddS3Client() |> ignore
        services.AddKafkaProducer() |> ignore 
        services.AddTransient<IAWSS3Service, AWSS3Service>() |> ignore
        services.AddTransient<IClaimRepository, ClaimRepository>() |> ignore
        services.AddTransient<IPhysicalEvidenceRepository, PhysicalEvidenceRepository>() |> ignore
        services.AddTransient<IDictumRepository, DictumRepository>() |> ignore
        services.AddTransient<ICustomerProfileRepository, CustomerProfileRepository>() |> ignore
        services.AddTransient<IEscortProfileRepository, EscortProfileRepository>() |> ignore
        services.AddTransient<IServiceRepository, ServiceRepository>() |> ignore
        services.AddTransient<IKafkaService, KafkaService>() |> ignore
        services.AddSingleton(typedefof<IOperationHandler<_>>, typedefof<OperationHandler<_>>) |> ignore
        services.AddHostedService<ClaimStatusConsumer>() |> ignore
        services.AddHostedService<ServiceStatusConsumer>() |> ignore
        services.AddHostedService<ClaimCreatedConsumer>() |> ignore

    member this.Configure(app: IApplicationBuilder, env: IWebHostEnvironment) =
        if env.IsDevelopment() then
            app.UseDeveloperExceptionPage() |> ignore

        app.UseRouting() |> ignore
        app.UseEndpoints(fun endpoints -> endpoints.MapControllers() |> ignore) |> ignore
