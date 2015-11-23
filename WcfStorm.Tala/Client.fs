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

//| DEL  of Uri * IRestRequest
//| PUT  of Uri * IRestRequest
type IClient =
    abstract Run : CancellationTokenSource -> Request -> Response

module Client =
    open RestSharp.Authenticators

    let private setupConfig (userAgentHeader : WcfStorm.Tala.HttpHeader option) (client : RestClient) =
        client.FollowRedirects <- Config.genSettings.FollowRedirects
        client.Timeout <-  Config.genSettings.Timeout
        client.MaxRedirects <- Nullable(Config.genSettings.MaxRedirects)
        if (userAgentHeader.IsSome) then client.UserAgent <- userAgentHeader.Value.Value
        client

        TODO
    let private setupAuth  (client : RestClient) =
       client.Authenticator <-  HttpBasicAuthenticator(cfg.BasicAuthentication.Username, (StringEncryptionOps.encryptor.Decrypt cfg.BasicAuthentication.Password))
     
        //TODO
        client

    let create (url : string) (userAgentHeader : WcfStorm.Tala.HttpHeader option) =
        let runAsync (cancellationTokenSource : CancellationTokenSource) restReq =
            let client =
                new RestClient(url)
                |> setupConfig userAgentHeader
                |> setupAuth
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