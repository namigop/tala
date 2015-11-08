namespace WcfStorm.Tala

open Xceed.Wpf.Toolkit
open System.Collections.ObjectModel
open RestSharp
open System.Threading

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
            let temp =  async {
                let client = new RestClient("http://www.google.com")
                let request = new RestRequest("/")
                let cancellationTokenSource = new CancellationTokenSource()
                let! restResponse =  Async.AwaitTask( client.ExecuteTaskAsync(request, cancellationTokenSource.Token) )
                return restResponse          
                }

            Async.RunSynchronously temp
         

        let cmd = 
            Command.create 
                (fun arg -> true) 
                (fun arg -> 
                    let res = run()
                    this.Response.Text.Text <- res.Content )
        cmd