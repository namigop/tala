namespace WcfStorm.Tala

open RestSharp
open System
open System.Configuration
open System.Threading
open System.Threading.Tasks

type Request =
    | GET_Req of Guid * IRestRequest
    | DELETE_Req of Guid * IRestRequest
    | HEAD_Req of Guid * IRestRequest
    | OPTIONS_Req of Guid * IRestRequest
    | POST_Req of Guid * IRestRequest
    | PUT_Req of Guid * IRestRequest

type Response =
    | GET_Resp of Guid * Task<IRestResponse>
    | DELETE_Resp of Guid * Task<IRestResponse>
    | HEAD_Resp of Guid * Task<IRestResponse>
    | OPTIONS_Resp of Guid * Task<IRestResponse>
    | POST_Resp of Guid * Task<IRestResponse>
    | PUT_Resp of Guid * Task<IRestResponse>

type IClient =
    abstract Run : CancellationTokenSource -> Request -> Response

module Client =
    open RestSharp.Authenticators
    open System.Net

    let private setupConfig (userAgentHeader : WcfStorm.Tala.HttpHeader option) (client : RestClient) =
        client.FollowRedirects <- Config.genSettings.FollowRedirects
        client.Timeout <-  Config.genSettings.Timeout
        client.MaxRedirects <- Nullable(Config.genSettings.MaxRedirects)
        if (userAgentHeader.IsSome) then client.UserAgent <- userAgentHeader.Value.Value
        client

      
    let private setupAuth (auth:BasicAuthentation) (restReq:IRestRequest) (client : RestClient) =
        match auth.AuthMode  with
        | AuthMode.Anonymous -> ()
        | AuthMode.Basic ->
            client.Authenticator <- HttpBasicAuthenticator(auth.Username, auth.Password)
        | AuthMode.Windows ->
            match auth.WinCredType with
            | WindowsCredentialsType.Default ->
                client.Authenticator <- NtlmAuthenticator() // use default credentials!
            | WindowsCredentialsType.Network ->
                let cred = new NetworkCredential(auth.Username, auth.Password, auth.Domain)
                restReq.Credentials <- cred   

        client

    let create (url : string) (userAgentHeader : WcfStorm.Tala.HttpHeader option) =
        let runAsync (cancellationTokenSource : CancellationTokenSource) restReq =
            let client =
                new RestClient(url)
                |> setupConfig userAgentHeader
                |> setupAuth Config.basicAuth restReq
            client.ExecuteTaskAsync(restReq, cancellationTokenSource.Token)

        let run verb id cancellationTokenSource (restReq : IRestRequest) (createResp : Guid * Task<IRestResponse> -> 'a) =
            restReq.Method <- verb
            let resp = runAsync cancellationTokenSource restReq
            createResp (id, resp)

        { new IClient with
              member x.Run cancellationTokenSource req =
                  match req with
                    | GET_Req(id, restReq) -> 
                        run 
                            Method.GET 
                            id 
                            cancellationTokenSource 
                            restReq 
                            (fun (id2, resp2) -> GET_Resp(id2, resp2))
                    | DELETE_Req(id, restReq) -> 
                        run 
                            Method.DELETE 
                            id 
                            cancellationTokenSource 
                            restReq 
                            (fun (id2, resp2) -> DELETE_Resp(id2, resp2))
                    | HEAD_Req(id, restReq) -> 
                        run 
                            Method.HEAD 
                            id 
                            cancellationTokenSource 
                            restReq 
                            (fun (id2, resp2) -> HEAD_Resp(id2, resp2))
                    | OPTIONS_Req(id, restReq) -> 
                        run 
                            Method.OPTIONS 
                            id 
                            cancellationTokenSource 
                            restReq 
                            (fun (id2, resp2) -> OPTIONS_Resp(id2, resp2))
                    | PUT_Req(id, restReq) -> 
                        run 
                            Method.PUT 
                            id 
                            cancellationTokenSource 
                            restReq 
                            (fun (id2, resp2) -> PUT_Resp(id2, resp2))
                    | POST_Req(id, restReq) ->
                        run 
                            Method.POST 
                            id 
                            cancellationTokenSource 
                            restReq 
                            (fun (id2, resp2) -> POST_Resp(id2, resp2))
        }