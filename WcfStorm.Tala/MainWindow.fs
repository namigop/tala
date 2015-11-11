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
   
    let urls = 
        let temp = ObservableCollection<TargetUrl>()
        temp.Add(TargetUrl(Url="http://www.google.com", IsCallInProgress=false))
        temp.Add(TargetUrl(Url="http://www.microsoft.com", IsCallInProgress=false))
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
    
    member this.ResponseStatusCode 
        with get() = statusCode
        and set v = this.RaiseAndSetIfChanged(&statusCode, v, "ResponseStatusCode")

    member this.AddRequestHeaderCommand =
        Command.create 
            (fun arg -> true) 
            (fun arg -> headers.Add(WcfStorm.Tala.HttpHeader(Key="", Value="")))
        
    member this.SendCommand = 
        let run() =
            let client = Client.create this.TargetUrl.Url
            let req = GET(Uri("http://www.google.com"), new RestRequest("/"))
            Async.RunSynchronously (client.Run req)
           
        let cmd = 
            Command.create 
                (fun arg -> true) 
                (fun arg -> 
                    let res, cancelToken = run()
                    this.ResponseStatusCode <- res.RestResponse.StatusCode.ToString() + " " + res.RestResponse.StatusDescription
                    this.Response.Doc.Text <-  res.Content
                    respHeaders.Clear()
                    res.RestResponse.Headers
                    |> Seq.fold(fun (acc:HttpHeaders) i -> 
                        acc.Add(WcfStorm.Tala.HttpHeader(Key=i.Name, Value=i.Value.ToString()))
                        acc) respHeaders
                    |> ignore
                    )
                    
        cmd