open FSharp.Control.Tasks
open System.Net.Http
open IdentityModel.Client
open System.Threading.Tasks
open Newtonsoft.Json.Linq

let block (t:_ Task) = t.Wait()

[<EntryPoint>]
let main _ =
    task {
        use client = new HttpClient()
        let! disco = client.GetDiscoveryDocumentAsync "http://localhost:5000"
        if disco.IsError then printfn "%s" disco.Error

        let! tokenResponse =
            client.RequestClientCredentialsTokenAsync(
                ClientCredentialsTokenRequest(
                    Address = disco.TokenEndpoint,
                    ClientId = "client",
                    ClientSecret = "secret",
                    Scope = "api1"))

        if tokenResponse.IsError then
            printfn "%s" tokenResponse.Error

        printfn "%O" tokenResponse.Json

        use client = new HttpClient()
        client.SetBearerToken tokenResponse.AccessToken

        let! response = client.GetAsync "http://localhost:5002/identity"
        if (not response.IsSuccessStatusCode) then
            printfn "%A" response.StatusCode
        else
            let! content = response.Content.ReadAsStringAsync()
            printfn "%O" (JArray.Parse content)

    } |> block
    0