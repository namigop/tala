namespace WcfStorm.Tala

open Xceed.Wpf.Toolkit
open System.Collections.ObjectModel
open RestSharp
open System.Threading
open FsXaml
open System

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

    let headers = 
        let temp = HttpHeaders()
        temp.Add(WcfStorm.Tala.HttpHeader(Key="User-Agent", Value="WcfStorm.Rest/2.2.0 (.NET4.0);support@wcfstorm.com"))
        temp.Add(WcfStorm.Tala.HttpHeader(Key="Content-Type", Value="application/json"))
        temp


    member this.TargetUrl 
        with get() = targetUrl
        and set v = this.RaiseAndSetIfChanged(&targetUrl, v, "TargetUrl")
  
    member this.Headers  = headers
    member this.Request  = requestPayload
    member this.Response = responsePayload
    member this.Urls     = urls

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
                    this.Response.Doc.Text <-  res.Content)
        cmd