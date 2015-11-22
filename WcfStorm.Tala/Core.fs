namespace WcfStorm.Tala

open FsXaml
open RestSharp
open System
open System.Collections.ObjectModel
open System.Net
open System.Threading
open Xceed.Wpf.Toolkit
open System.Diagnostics

type ProcessedResponse =
    { ResponseCode : string
      ResponseStatus : RestSharp.ResponseStatus
      Elapsed : TimeSpan
      HttpCallFailed : bool
      HttpContentType : HttpContentType
      RawResponseText : string
      Headers : WcfStorm.Tala.HttpHeader seq }

module Core =
    let isFailed (rawResponse : IRestResponse) =
        rawResponse.ResponseStatus = ResponseStatus.Aborted || 
        rawResponse.ResponseStatus = ResponseStatus.Error || 
        rawResponse.ResponseStatus = ResponseStatus.TimedOut ||
        rawResponse.ErrorException <> null ||
        (Convert.ToInt32(rawResponse.StatusCode)) >= 300

    let getContentType rawContentType = HttpContentType.Other("").Parse(rawContentType)
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
          Headers = respHeaders }
    
    let createRequest  (verb:Method) (httpParams: HttpParams) (httpHeaders: HttpHeaders) = 
        let req = RestRequest(verb) 
        do httpParams 
            |> Seq.filter (fun pr -> String.IsNullOrWhiteSpace(pr.Name) |> not) 
            |> Seq.iter (fun pr -> req.AddParameter(pr.Name, pr.Value, pr.ParameterType) |> ignore)
       
        do httpHeaders 
            |> Seq.filter (fun pr -> String.IsNullOrWhiteSpace(pr.Key) |> not) 
            |> Seq.iter (fun pr -> req.AddParameter(pr.Key, pr.Value) |> ignore)

        req
        

    let createGetRequest = createRequest Method.GET
  //  let createPutRequest = createRequest Method.PUT

    let runAsync url (resource:string) (httpParams : HttpParams) (httpHeaders: HttpHeaders)= 
        let client = Client.create url
        let req = GET_Req(Guid.NewGuid(), createGetRequest httpParams httpHeaders)
        let cancel = new CancellationTokenSource()
        let execute() = async {    
            match client.Run cancel req with
            | GET_Resp(id, respTask) ->
                let sw = Stopwatch.StartNew()
                let! rawResponse = Async.AwaitTask(respTask)      
                sw.Stop()        
                return  rawResponse, sw.Elapsed
            | POST_Resp(id, respTask) -> 
                let sw = Stopwatch.StartNew()
                let! rawResponse = Async.AwaitTask(respTask)
                sw.Stop()     
                return rawResponse, sw.Elapsed
            }
        execute()

    let getUri rawUrl =
        try
            if (not(String.IsNullOrWhiteSpace(rawUrl))) then
                let url = if rawUrl.StartsWith("http://") then rawUrl else "http://" + rawUrl
                Some(new Uri(url))
            else
                None
        with
        | _ -> None

       