open Saturn
open Giraffe
open Microsoft.AspNetCore.Http
open Microsoft.Extensions.DependencyInjection
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Authentication.JwtBearer

let getClaims next (ctx:HttpContext) =
    let claims =
        ctx.User.Claims
        |> Seq.map(fun c -> {| Type = c.Type; Value = c.Value |})
        |> Seq.toArray
    json claims next ctx

let unauthorisedHandler _ ctx =
    Response.unauthorized ctx "Bearer" "Realm" "eff off"

let myRoutes = router {
    get "/identity" (requiresAuthentication unauthorisedHandler >=> getClaims)
}

let configureJwt (options:JwtBearerOptions) =
    options.Authority <- "http://localhost:5000"
    options.RequireHttpsMetadata <- false
    options.Audience <- "api1"

let app = application {
    url "http://localhost:5002"
    use_jwt_authentication_with_config configureJwt
    use_router myRoutes
}

run app