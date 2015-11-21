namespace WcfStorm.Tala

open FsXaml
open RestSharp
open System
open System.Collections.ObjectModel
open System.Net
open System.Threading
open Xceed.Wpf.Toolkit

type ProcessedResponse =
    { ResponseCode : string
      HttpCallFailed : bool
      HttpContentType : HttpContentType
      RawResponseText : string
      Headers : WcfStorm.Tala.HttpHeader seq }

module Core =
    let getContentType rawContentType = HttpContentType.Other("").Parse(rawContentType)
    let processRestResp (rawResponse : IRestResponse) =
        let respCode = "HTTP " + Convert.ToInt32(rawResponse.StatusCode).ToString() + " " + rawResponse.StatusDescription

        let rawRespText =
            if (rawResponse.ErrorException = null) then rawResponse.Content
            else rawResponse.ErrorException.ToString()

        let respHeaders = rawResponse.Headers |> Seq.map (fun i -> WcfStorm.Tala.HttpHeader(Key = i.Name, Value = i.Value.ToString()))
        { ResponseCode = respCode
          HttpCallFailed = rawResponse.ErrorException <> null
          HttpContentType = getContentType(rawResponse.ContentType)
          RawResponseText = rawRespText
          Headers = respHeaders }
    
    let createRequest  (verb:Method) (httpParams: HttpParam seq)= 
        let req = RestRequest(verb)
        for pr in httpParams do
            req.AddParameter(pr.Name, pr.Value, pr.ParameterType) |> ignore
        req
        

    let createGetRequest = createRequest Method.GET
  //  let createPutRequest = createRequest Method.PUT

    let runAsync url (resource:string) (httpParams : HttpParam seq)= 
        let client = Client.create url
        let req = GET_Req(Guid.NewGuid(), createGetRequest httpParams)
        let cancel = new CancellationTokenSource()
        let execute() = async {    
            match client.Run cancel req with
            | GET_Resp(id, respTask) ->
                let! rawResponse = Async.AwaitTask(respTask)              
                return rawResponse
            | POST_Resp(id, respTask) -> 
                let! rawResponse = Async.AwaitTask(respTask)
                return rawResponse
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

       