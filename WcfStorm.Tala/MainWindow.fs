namespace WcfStorm.Tala

open Xceed.Wpf.Toolkit
open System.Collections.ObjectModel
open RestSharp
open System.Threading
open FsXaml
open System
open System.Net

type MainWindow = XAML<"MainWindow.xaml">
 
   

type MainWindowViewModel() =
    inherit NotifyBase()
    let requestPayload = HttpPayload()
    let responsePayload = HttpPayload()
    let mutable isCallInProgress = false
    let mutable httpCallFailed= false

    let urls = 
        let temp = ObservableCollection<TargetUrl>()
        temp.Add(TargetUrl(Url="http://www.google.com", IsCallInProgress=false))
        temp.Add(TargetUrl(Url="http://www.microsoft.com", IsCallInProgress=false))
        temp.Add(TargetUrl(Url="http://www.reddit.com/r/programming.json", IsCallInProgress=false))
        temp
    let mutable targetUrl = urls.Item(0)
    
    let respHeaders = HttpHeaders()
    let headers = 
        let temp = HttpHeaders()
        temp.Add(WcfStorm.Tala.HttpHeader(Key="User-Agent", Value="WcfStorm.Rest/2.2.0 (.NET4.0);support@wcfstorm.com"))
        temp.Add(WcfStorm.Tala.HttpHeader(Key="Content-Type", Value="application/json"))
        temp

    let mutable statusCode = ""

    member this.TargetUrl 
        with get() = targetUrl
        and set v = this.RaiseAndSetIfChanged(&targetUrl, v, "TargetUrl")
  
    member this.Headers  = headers
    member this.ResponseHeaders = respHeaders
    member this.Request  = requestPayload
    member this.Response = responsePayload
    member this.Urls     = urls
    
    member this.HttpCallFailed 
        with get() = httpCallFailed
        and set v = this.RaiseAndSetIfChanged(&httpCallFailed, v, "HttpCallFailed")
  
    member this.IsCallInProgress 
        with get() = isCallInProgress
        and set v = this.RaiseAndSetIfChanged(&isCallInProgress, v, "IsCallInProgress")
  
    member this.ResponseStatusCode 
        with get() = statusCode
        and set v = this.RaiseAndSetIfChanged(&statusCode, v, "ResponseStatusCode")

    member this.AddRequestHeaderCommand =
        Command.create 
            (fun arg -> true) 
            (fun arg -> headers.Add(WcfStorm.Tala.HttpHeader(Key="", Value="")))
        
    member this.RemoveHeaderCommand =
        let onRun (arg:obj) =
            match Cast.convert<WcfStorm.Tala.HttpHeader>(arg) with
            | Some(reqHeader) -> this.Headers.Remove(reqHeader) |> ignore
            | None -> ()
        Command.create (fun arg -> true) onRun

    member this.SendCommand =
        let processResp (rawResponse:IRestResponse) =
            let processed = Core.processRestResp rawResponse
            this.ResponseStatusCode <- processed.ResponseCode
            this.HttpCallFailed <- processed.HttpCallFailed
            this.Response.SetText(processed.RawResponseText, processed.HttpContentType)
            this.Response.Mode <- processed.HttpContentType
            this.IsCallInProgress <- false
            respHeaders.Clear()
            for h in processed.Headers do
                respHeaders.Add h

        let run resp = async {    
            match resp with
            | GET_Resp(id, respTask) ->
                let! rawResponse = Async.AwaitTask(respTask)              
                return rawResponse
            | POST_Resp(id, respTask) -> 
                let! rawResponse = Async.AwaitTask(respTask)
                return rawResponse
        } 
           
        let setup() =                
            let client = Client.create this.TargetUrl.Url
            let req = GET_Req(Guid.NewGuid(), new RestRequest("/"))
            let cancel = new CancellationTokenSource()
            (client.Run cancel req)
             
        let cmd = 
            Command.create 
                (fun arg -> not this.IsCallInProgress) 
                (fun arg -> 
                    this.IsCallInProgress <- true
                    this.HttpCallFailed <- false
                    Async.StartWithContinuations( 
                        run (setup()),
                        (fun r -> processResp r),
                        (fun _ -> this.Response.Doc.Text <- "Operation failed."),
                        (fun _ -> this.Response.Doc.Text <- "Operation canceled."))
                )               
        cmd