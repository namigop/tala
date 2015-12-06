namespace WcfStorm.Tala

open FsXaml
open System.IO
open RestSharp
open System
open System.Collections.ObjectModel
open System.Net
open System.Threading
open Xceed.Wpf.Toolkit
open System.Diagnostics
open Newtonsoft.Json

/// <summary>
/// Wrapper for the HTTP Response
/// </summary>
type ProcessedResponse =
    { ResponseCode : string
      ResponseStatus : RestSharp.ResponseStatus
      Elapsed : TimeSpan
      HttpCallFailed : bool
      HttpContentType : HttpContentType
      RawResponseText : string
      Cookies : RestSharp.RestResponseCookie seq
      Headers : WcfStorm.Tala.HttpHeader seq }

module Core =

    /// <summary>
    /// Checks if the response failed or returned an HTTP respons that indicates a failure
    /// </summary>
    let isFailed (rawResponse : IRestResponse) =
        rawResponse.ResponseStatus = ResponseStatus.Aborted || 
        rawResponse.ResponseStatus = ResponseStatus.Error || 
        rawResponse.ResponseStatus = ResponseStatus.TimedOut ||
        rawResponse.ErrorException <> null ||
        (Convert.ToInt32(rawResponse.StatusCode)) >= 300

    /// <summary>
    /// Get the content type
    /// </summary>
    let getContentType rawContentType = HttpContentType.Other("").Parse(rawContentType)

    /// <summary>
    /// Read the RestResponse and create a wrapper for it
    /// </summary>
    let processRestResp (rawResponse : IRestResponse) (elapsed:TimeSpan) =
        let respCode = "HTTP " + Convert.ToInt32(rawResponse.StatusCode).ToString() + " " + rawResponse.StatusDescription
        let rawRespText =
            if (rawResponse.ErrorException = null) then rawResponse.Content
            else rawResponse.ErrorException.ToString()
        let respHeaders = rawResponse.Headers |> Seq.map (fun i -> WcfStorm.Tala.HttpHeader(Key = i.Name, Value = i.Value.ToString()))
        { ResponseCode = respCode
          ResponseStatus = rawResponse.ResponseStatus
          Elapsed = elapsed
          HttpCallFailed = isFailed rawResponse
          HttpContentType = getContentType(rawResponse.ContentType)
          RawResponseText = rawRespText
          Cookies = seq { for c in rawResponse.Cookies do yield c } 
          Headers = respHeaders }
     
    /// <summary>
    /// Setup the HTTP Request Body
    /// </summary>
    let setupBody reqBody (req:IRestRequest)  =            
        if  req.RequestFormat = DataFormat.Xml then
                req.AddParameter(new Parameter( Name="application/xml", Value=reqBody, Type = ParameterType.RequestBody)) |>ignore  
        elif  req.RequestFormat = DataFormat.Json then
            req.AddParameter(new Parameter( Name="application/json", Value=reqBody, Type = ParameterType.RequestBody)) |>ignore
        else 
            req.AddParameter(new Parameter( Name="text/plain", Value=reqBody, Type = ParameterType.RequestBody)) |>ignore
        
    /// <summary>
    /// Get the data format (XML or JSON) by checking the HTTP headers
    /// </summary>
    let getDataFormat (httpHeaders: HttpHeaders) =
        match (httpHeaders |> Seq.tryFind(fun d -> d.Key.ToLowerInvariant().Trim() = "content-type" ) ) with
        | Some(header) -> 
            let headerKey = header.Key.ToLowerInvariant().Trim()
            if (headerKey.EndsWith("xml")) then
                Some(DataFormat.Xml)
            else if (headerKey.EndsWith("json")) then
                Some(DataFormat.Json)
            else
                None
        | None -> None

    /// <summary>
    /// Create an HTTP RestRequest
    /// </summary>
    let createRequest  (verb:Method) (httpParams: HttpParams) (httpHeaders: HttpHeaders) = 
        let req = RestRequest(verb) 
        let tryAssignDataFormat (restReq:RestRequest) allHeaders =
            match getDataFormat allHeaders with
            | Some(format) -> restReq.RequestFormat <- format
            | None -> ()
       
        //tryAssignDataFormat req httpHeaders
        do httpParams 
            |> Seq.filter (fun pr -> String.IsNullOrWhiteSpace(pr.Name) |> not) 
            |> Seq.iter (fun pr -> req.AddParameter(pr.Name, pr.Value, pr.ParameterType) |> ignore)
       
        do httpHeaders 
            |> Seq.filter (fun pr -> String.IsNullOrWhiteSpace(pr.Key) |> not) 
            |> Seq.iter (fun pr -> req.AddHeader(pr.Key, pr.Value) |> ignore)

        req
        
 
    /// <summary>
    /// Run an HTTP Call asynchronously
    /// </summary>
    let runAsync url (resource:string) (httpParams : HttpParams) (httpHeaders: HttpHeaders) (verb:Method) (reqBody:string)= 
        let client = Client.create url (httpHeaders |> Seq.tryFind(fun t -> t.Key.ToLowerInvariant().Trim() = "user-agent"))

        let req = 
            match verb with
            | Method.GET -> GET_Req(Guid.NewGuid(), createRequest Method.GET httpParams httpHeaders)
            | Method.DELETE -> DELETE_Req(Guid.NewGuid(), createRequest Method.DELETE httpParams httpHeaders)
            | Method.OPTIONS -> OPTIONS_Req(Guid.NewGuid(), createRequest Method.OPTIONS httpParams httpHeaders)
            | Method.HEAD -> HEAD_Req(Guid.NewGuid(), createRequest Method.HEAD httpParams httpHeaders)
            | Method.POST -> 
                let postReq = createRequest Method.POST httpParams httpHeaders
                if not (String.IsNullOrWhiteSpace(reqBody)) then
                    setupBody reqBody postReq
                POST_Req(Guid.NewGuid(), postReq)
            | Method.PUT -> 
                let putReq = createRequest Method.PUT httpParams httpHeaders
                if not (String.IsNullOrWhiteSpace(reqBody)) then
                    setupBody reqBody putReq
                PUT_Req(Guid.NewGuid(), putReq)
            | _ -> failwith "Not yet supported"


        let cancel = new CancellationTokenSource()
        let execute() = async {    
            match client.Run cancel req with
            | GET_Resp(id, respTask) | DELETE_Resp(id, respTask) | OPTIONS_Resp(id, respTask) | HEAD_Resp(id, respTask) ->
                let sw = Stopwatch.StartNew()
                let! rawResponse = Async.AwaitTask(respTask)      
                sw.Stop()        
                return  rawResponse, sw.Elapsed
            | POST_Resp(id, respTask) | PUT_Resp(id, respTask) -> 
                let sw = Stopwatch.StartNew()
                let! rawResponse = Async.AwaitTask(respTask)
                sw.Stop()     
                return rawResponse, sw.Elapsed
            }
        execute()

    /// <summary>
    /// Gets a Uri instance
    /// </summary>
    let getUri rawUrl =
        try
            if (not(String.IsNullOrWhiteSpace(rawUrl))) then
                let url = if rawUrl.StartsWith("http://") then rawUrl else "http://" + rawUrl
                Some(new Uri(url))
            else
                None
        with
        | _ -> None

    /// <summary>
    /// Opens a file dialog and saves the Tala test 
    /// </summary>
    let saveTestData(testReq:TestRequest) =
        let dlg = new Microsoft.Win32.SaveFileDialog();
        dlg.FileName <- "Data"
        dlg.DefaultExt <- ".tala"
        dlg.Filter <- "Tala (*.tala)|*.tala"

        // Show save file dialog box
        let result = dlg.ShowDialog()
        if (result.HasValue && result.Value) then    
            let filename = dlg.FileName
            File.WriteAllText(filename, JsonConvert.SerializeObject(testReq) |> EditorOptions.prettyPrintJson)
     
    /// <summary>
    /// Opens a dialog and loads a saved Tala Test
    /// </summary>
    let openTestData() =
        let dlg = new Microsoft.Win32.OpenFileDialog();
        dlg.DefaultExt <- ".tala" 
        dlg.Filter <- "Tala (.tala)|*.tala" 
        let result = dlg.ShowDialog()
        if (result.HasValue && result.Value) then    
            let filename = dlg.FileName
            let text = File.ReadAllText(filename)
            let testReq = JsonConvert.DeserializeObject<TestRequest>(text)
            Some(testReq)
        else
            None
         
