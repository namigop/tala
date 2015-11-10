namespace WcfStorm.Tala

open Xceed.Wpf.Toolkit
open System.Collections.ObjectModel
open RestSharp
open System.Threading
open FsXaml
open System

type MainWindow = XAML<"MainWindow.xaml">
 
   

type MainWindowViewModel() =
    let requestPayload = HttpPayload()
    let responsePayload = HttpPayload()
    let urls = 
        let temp = ObservableCollection<TargetUrl>()
        temp.Add(TargetUrl(Url="http://www.google.com", IsCallInProgress=false))
        temp.Add(TargetUrl(Url="http://www.microsoft.com", IsCallInProgress=false))
        temp
    let headers = 
        let temp = HttpHeaders()
        temp.Add(WcfStorm.Tala.HttpHeader(Key="wer", Value="werwer"))
        temp

    member this.Headers 
        with get() = headers

    member this.Request = requestPayload
    member this.Response = responsePayload
    member x.Urls = urls

    member this.SendCommand = 
        let run() =
            let client = Client.create "http://www.google.com"
            let req = GET(Uri("http://www.google.com"), new RestRequest("/"))
            Async.RunSynchronously (client.Run req)
           
        let cmd = 
            Command.create 
                (fun arg -> true) 
                (fun arg -> 
                    let res, cancelToken = run()
                    this.Response.Doc.Text <-  res.Content)
        cmd