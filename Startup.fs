namespace EscortBookClaim

open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Hosting
open Microsoft.Extensions.Configuration
open Microsoft.EntityFrameworkCore
open EscortBookClaim.Extensions.MongoDBExtensions
open EscortBookClaim.Repositories
open EscortBookClaim.Services
open EscortBookClaim.Backgrounds
open EscortBookClaim.Handlers
open EscortBookClaim.Contexts

type Startup private () =

    member val Configuration: IConfiguration = null with get, set

    new (configuration: IConfiguration) as this = Startup() then
        this.Configuration <- configuration

    member this.ConfigureServices(services: IServiceCollection) =
        services.AddControllers() |> ignore
        services.AddDbContext<EscortBookClaimContext>(fun options ->
            options.UseSqlServer(this.Configuration
            .GetSection("SqlServer").GetSection("Default").Value) |> ignore) |> ignore
        services.AddDbContext<EscortProfileContext>(fun options ->
            options.UseNpgsql(this.Configuration
            .GetSection("Postgres").GetSection("Default").Value) |> ignore) |> ignore
        services.AddMongoDBClient(this.Configuration) |> ignore
        services.AddTransient<IAWSS3Service, AWSS3Service>() |> ignore
        services.AddTransient<IClaimRepository, ClaimRepository>() |> ignore
        services.AddTransient<IPhysicalEvidenceRepository, PhysicalEvidenceRepository>() |> ignore
        services.AddTransient<IDictumRepository, DictumRepository>() |> ignore
        services.AddTransient<ICustomerProfileRepository, CustomerProfileRepository>() |> ignore
        services.AddTransient<IEscortProfileRepository, EscortProfileRepository>() |> ignore
        services.AddTransient<IServiceRepository, ServiceRepository>() |> ignore
        services.AddSingleton(typedefof<IOperationHandler<_>>, typedefof<OperationHandler<_>>) |> ignore
        services.AddHostedService<ClaimStatusConsumer>() |> ignore
        services.AddHostedService<ServiceStatusConsumer>() |> ignore

    member this.Configure(app: IApplicationBuilder, env: IWebHostEnvironment) =
        if env.IsDevelopment() then
            app.UseDeveloperExceptionPage() |> ignore

        app.UseRouting() |> ignore
        app.UseEndpoints(fun endpoints -> endpoints.MapControllers() |> ignore) |> ignore
