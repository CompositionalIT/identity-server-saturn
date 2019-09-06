open Saturn
open Microsoft.AspNetCore.Builder
open Microsoft.Extensions.DependencyInjection

module Config =
    open IdentityServer4.Models
    let getIdentityResources() = [ IdentityResources.OpenId() :> IdentityResource ]
    let getApis() = [ ApiResource("api1", "My API") ]
    let getClients() =
        [ Client(ClientId = "client",
                 AllowedGrantTypes = GrantTypes.ClientCredentials,
                 ClientSecrets = ResizeArray [ Secret("secret".Sha256()) ],
                 AllowedScopes = ResizeArray [ "api1" ]) ]

let configIdentity (app:IApplicationBuilder) = 
    app.UseIdentityServer()

let configServices (services:IServiceCollection) =
    services
        .AddIdentityServer()
        .AddInMemoryIdentityResources(Config.getIdentityResources())
        .AddInMemoryApiResources(Config.getApis())
        .AddInMemoryClients(Config.getClients())
        .AddDeveloperSigningCredential()
        .Services

let app = application {
    no_router
    use_developer_exceptions
    app_config configIdentity
    service_config configServices
}

run app