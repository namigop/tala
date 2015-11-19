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