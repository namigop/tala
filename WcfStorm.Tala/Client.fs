namespace WcfStorm.Tala

open System
open RestSharp
open System.Threading

type Request =
| GET  of Uri * IRestRequest
| POST of Uri * IRestRequest
//| DEL  of Uri * IRestRequest
//| PUT  of Uri * IRestRequest

type Response(resp : IRestResponse) =   
    member x.Content = resp.Content

type IClient =
    abstract Run : Request -> Async<Response * CancellationTokenSource>

module Client =
    let create (url:string) =
        let runAsync restReq =  async {
            let client = new RestClient(url)
            let cancellationTokenSource = new CancellationTokenSource()
            let! restResponse =  Async.AwaitTask( client.ExecuteTaskAsync(restReq, cancellationTokenSource.Token) )
            return (Response(restResponse), cancellationTokenSource)
        }

        
        {
            new IClient with
                member x.Run req =
                    match req with
                    | GET(uri, restReq) ->
                        restReq.Method <- Method.GET
                        runAsync restReq
                    | POST(uri, restReq) ->
                        restReq.Method <- Method.POST
                        runAsync restReq
                     
                
        }
       

                
