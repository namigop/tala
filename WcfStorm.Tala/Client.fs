namespace WcfStorm.Tala

open System
open RestSharp
open System.Threading
open System.Threading.Tasks

type Request =
| GET_Req  of Guid * IRestRequest
| POST_Req of Guid * IRestRequest


type Response =
| GET_Resp  of Guid * Task<IRestResponse>
| POST_Resp of Guid * Task<IRestResponse>

//| DEL  of Uri * IRestRequest
//| PUT  of Uri * IRestRequest
 
type IClient =
    abstract Run : CancellationTokenSource -> Request -> Response

module Client =
    let create (url:string) =
        let runAsync (cancellationTokenSource:CancellationTokenSource) restReq = 
            let client = new RestClient(url) 
            client.ExecuteTaskAsync(restReq, cancellationTokenSource.Token)
        
        {
            new IClient with
                member x.Run cancellationTokenSource req =
                    match req with
                    | GET_Req(id, restReq) ->
                        restReq.Method <- Method.GET
                        let resp = runAsync cancellationTokenSource restReq
                        GET_Resp(id, resp)
                    | POST_Req(id, restReq) ->
                        restReq.Method <- Method.POST
                        let resp = runAsync cancellationTokenSource restReq
                        POST_Resp(id, resp)
                
        }
    
       

                
