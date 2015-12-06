namespace WcfStorm.Tala

open FsXaml
open Newtonsoft.Json
open RestSharp
open System
open System.Collections.ObjectModel
open System.Net
open System.Threading
open System.Windows.Input
open Xceed.Wpf.Toolkit
open System.Reflection


type SettingsWindow = XAML< "SettingsWindow.xaml" >
type InfoWindow = XAML< "InfoWindow.xaml" >

/// <summary>
/// DataContext of the MainWindow
/// </summary>
type MainWindowViewModel() =
    inherit NotifyBase()
    let requestPayload = HttpPayload()
    let responsePayload = HttpPayload()
    let mutable isCallInProgress = false
    let mutable httpCallFailed = false
    let mutable respTime = 0.0
    let mutable selectedTabIndex = 0
    let mutable isParametersChecked = true
    let mutable isRawBodyChecked = false
    let mutable selectedResponseCookie = RestResponseCookie()
    let verbs =
        let temp = ObservableCollection<Method>()
        temp.Add(Method.GET)
        temp.Add(Method.POST)
        temp.Add(Method.PUT)
        temp.Add(Method.DELETE)
        temp.Add(Method.HEAD)
        temp.Add(Method.OPTIONS)
        temp

    let mutable selectedVerb = Method.GET
    let title =
        let version = Assembly.GetExecutingAssembly().GetName().Version
        String.Format("WcfStorm.Tala v{0}.{1}",version.Major, version.Minor)

    let urls =
        let temp = ObservableCollection<string>()
        temp.Add("http://www.reddit.com/r/programming.json")
        temp.Add("http://www.google.com")
        temp.Add("http://www.microsoft.com")
        temp

    let mutable targetUrl = urls.Item(0)
    let respHeaders = HttpHeaders()
    let respCookies = Cookies()

    let reqParams =
        let temp = HttpParams()
        temp.Add(HttpParam())
        temp

    let headers =
        let temp = HttpHeaders()
        temp.Add(WcfStorm.Tala.HttpHeader(Key = "User-Agent", Value = "WcfStorm.Rest/2.2.0 (.NET4.0);support@wcfstorm.com"))
        temp.Add(WcfStorm.Tala.HttpHeader(Key = "Content-Type", Value = "application/json"))
        temp.Add(WcfStorm.Tala.HttpHeader(Key = "", Value = ""))
        temp

    let parameterTypeSelection =
        let temp = new ObservableCollection<ParameterType>()
        temp.Add(ParameterType.UrlSegment)
        temp.Add(ParameterType.GetOrPost)
        temp

    let mutable statusCode = ""

    /// <summary>
    /// Gets or sets the selected Url
    /// </summary>
    member this.TargetUrl
        with get () = targetUrl
        and set v = this.RaiseAndSetIfChanged(&targetUrl, v, "TargetUrl")

    /// <summary>
    /// Gets the application Title
    /// </summary>
    member this.Title = title

    /// <summary>
    /// Gets the response cookies
    /// </summary>
    member this.ResponseCookies = respCookies

    /// <summary>
    /// Gets the collection of http request parameters
    /// </summary>
    member this.RequestParameters = reqParams

     /// <summary>
    /// Gets the collection of http request headers
    /// </summary>
    member this.RequestHeaders = headers

    /// <summary>
    /// Gets the collection of http response headers
    /// </summary>
    member this.ResponseHeaders = respHeaders

    /// <summary>
    /// Gets the request payload.
    /// </summary>
    member this.Request = requestPayload

    /// <summary>
    /// Gets the response payload
    /// </summary>
    member this.Response = responsePayload

    /// <summary>
    /// Gets a collection Of Urls that the user can select from
    /// </summary>
    member this.Urls = urls

    /// <summary>
    /// Gets a collectionHTTP Vebs (GET, PUT, POST) that the user can select from
    /// </summary>
    member this.Verbs = verbs

    /// <summary>
    /// Gets the collection of parametertypes (RawBody, Parameters, Cookies)
    /// </summary>
    member this.ParameterTypeSelection = parameterTypeSelection

    /// <summary>
    /// Gets or sets a value indicating whether the call failed or not
    /// </summary>
    member this.HttpCallFailed
        with get () = httpCallFailed
        and set v = this.RaiseAndSetIfChanged(&httpCallFailed, v, "HttpCallFailed")

    /// <summary>
    /// Gets or sets the selected HTTP verb
    /// </summary>
    member this.SelectedVerb
        with get () = selectedVerb
        and set v = this.RaiseAndSetIfChanged(&selectedVerb, v, "SelectedVerb")

    /// <summary>
    /// Gets or sets a value indicating whether an HTTP call is in progress
    /// </summary>
    member this.IsCallInProgress
        with get () = isCallInProgress
        and set v = this.RaiseAndSetIfChanged(&isCallInProgress, v, "IsCallInProgress")

    /// <summary>
    /// Gets or sets the HTTP status code of the response
    /// </summary>
    member this.ResponseStatusCode
        with get () = statusCode
        and set v = this.RaiseAndSetIfChanged(&statusCode, v, "ResponseStatusCode")

    /// <summary>
    /// Gets or sets the selected  response cookie
    /// </summary>
    member this.SelectedResponseCookie
        with get () = selectedResponseCookie
        and set v = this.RaiseAndSetIfChanged(&selectedResponseCookie, v, "SelectedResponseCookie")

    /// <summary>
    /// Gets the command to add a request header
    /// </summary>
    member this.AddRequestHeaderCommand = 
        Command.create 
            (fun arg -> not this.IsCallInProgress) 
            (fun arg -> headers.Add(WcfStorm.Tala.HttpHeader(Key = "", Value = "")))

    /// <summary>
    /// Gets the command to add a request parameter
    /// </summary>
    member this.AddRequestParameterCommand = 
        Command.create 
            (fun arg -> not this.IsCallInProgress) 
            (fun arg -> 
                this.Request.SelectedTabIndex <- 1
                this.RequestParameters.Add(HttpParam(Name = "", Value = "")))

    /// <summary>
    /// Gets the command to remove an HTTP header
    /// </summary>
    member this.RemoveHeaderCommand =
        let onRun (arg : obj) =
            match Cast.convert<WcfStorm.Tala.HttpHeader> (arg) with
            | Some(reqHeader) ->
                this.RequestHeaders.Remove(reqHeader) |> ignore
                if (this.RequestHeaders.Count = 0) then (this.AddRequestHeaderCommand :> ICommand).Execute(null)
            | None -> ()
        Command.create (fun arg -> not this.IsCallInProgress) onRun

    /// <summary>
    /// Gets the command to remove an HTTP request
    /// </summary>
    member this.RemoveRequestParameterCommand =
        let onRun (arg : obj) =
            match Cast.convert<WcfStorm.Tala.HttpParam> (arg) with
            | Some(reqParam) ->
                this.RequestParameters.Remove(reqParam) |> ignore
                if (this.RequestParameters.Count = 0) then (this.AddRequestParameterCommand :> ICommand).Execute(null)
            | None -> ()
        Command.create (fun arg -> not this.IsCallInProgress) onRun

    /// <summary>
    /// Gets the command to save the Tala test
    /// </summary>
    member this.SaveCommand =
        let canRun arg = not this.IsCallInProgress
        Command.create canRun (fun arg ->
            let testReq = TestRequest(Url = this.TargetUrl, Verb = this.SelectedVerb, RequestText = "")
            testReq.RequestHeaders <- this.RequestHeaders
                                      |> Seq.filter (fun t -> not (String.IsNullOrWhiteSpace(t.Key)))
                                      |> Seq.toArray
            testReq.RequestParameters <- this.RequestParameters
                                         |> Seq.filter (fun t -> not (String.IsNullOrWhiteSpace(t.Name)))
                                         |> Seq.toArray
            testReq.RequestText <- this.Request.Doc.Text
            Core.saveTestData testReq)

    /// <summary>
    /// Gets the command to load the saved Talatest
    /// </summary>
    member this.OpenCommand =
        let canRun arg = not this.IsCallInProgress
        Command.create canRun (fun arg ->
            match Core.openTestData() with
            | Some(testReq) ->
                this.TargetUrl <- testReq.Url
                this.SelectedVerb <- testReq.Verb
                this.Request.Doc.Text <- testReq.RequestText
                this.RequestHeaders.Clear()
                testReq.RequestHeaders
                |> Seq.filter (fun t -> not (String.IsNullOrWhiteSpace(t.Key)))
                |> Seq.iter (fun h -> this.RequestHeaders.Add h)
                testReq.RequestParameters
                |> Seq.filter (fun t -> not (String.IsNullOrWhiteSpace(t.Name)))
                |> Seq.iter (fun h -> this.RequestParameters.Add h)
            | None -> ())

    /// <summary>
    /// Gets the command to showa feature comparision to WcfStorm.REST
    /// </summary>
    member this.FeatureComparisonCommand = 
        let canRun arg =true
        Command.create canRun (fun arg ->
            let win = InfoWindow()
                  
            win.Root.Owner <- System.Windows.Application.Current.MainWindow
            win.Root.ShowDialog() |> ignore)

    /// <summary>
    /// Gets the command to open the setting window
    /// </summary>
    member this.SettingsCommand =
        let canRun arg = not this.IsCallInProgress
        Command.create canRun (fun arg ->
            let win = SettingsWindow()
            let vm = win.Root.DataContext :?> SettingsWindowViewModel
            vm.Close <- win.Root.Close
            win.Root.Owner <- System.Windows.Application.Current.MainWindow
            win.Root.ShowDialog() |> ignore)

    /// <summary>
    /// Gets the time taken for the HTTP call
    /// </summary>
    member this.ResponseTimeInSec
        with get () = respTime
        and set v = this.RaiseAndSetIfChanged(&respTime, v, "ResponseTimeInSec")

    /// <summary>
    /// Gets the command to send the HTTP Request
    /// </summary>
    member this.SendCommand =
        let processResp (rawResponse : IRestResponse) elapsed =
            let processed = Core.processRestResp rawResponse elapsed
            this.ResponseStatusCode <- processed.ResponseCode
            this.HttpCallFailed <- processed.HttpCallFailed
            this.Response.SetText(processed.RawResponseText, processed.HttpContentType)
            this.Response.Mode <- processed.HttpContentType
            this.IsCallInProgress <- false
            this.ResponseTimeInSec <- processed.Elapsed.TotalSeconds

            respCookies.Clear()
            for c in processed.Cookies do
                respCookies.Add c
            
            if respCookies.Count > 0 then
                this.SelectedResponseCookie <- respCookies.Item(0);  

            respHeaders.Clear()
            for h in processed.Headers do
                respHeaders.Add h

        let canRun arg = not this.IsCallInProgress
        let onOk (resp, elapsed) = processResp resp elapsed

        let onError exc =
            this.IsCallInProgress <- false
            this.Response.Doc.Text <- exc.ToString()

        let onCancel arg = this.Response.Doc.Text <- "Operation canceled."

        let getTargetUrl() =
            match Core.getUri this.TargetUrl with
            | Some(uri) ->
                let trimmed = uri.AbsoluteUri.TrimEnd('/')
                if not (urls.Contains(trimmed)) then urls.Add(trimmed)
                trimmed
            | None -> ""
        Command.create canRun (fun arg ->
            this.IsCallInProgress <- true
            this.HttpCallFailed <- false
            Async.StartWithContinuations(Core.runAsync (getTargetUrl()) "/" this.RequestParameters this.RequestHeaders this.SelectedVerb this.Request.Doc.Text, onOk, onError, onCancel))