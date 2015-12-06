namespace WcfStorm.Tala
open System

/// <summary>
/// Type of the response
/// </summary>
type HttpContentType =
    | Json of string 
    | Xml of string
    | Html of string
    | Other of string

/// <summary>
/// Extension method to HttpContentType that adds the Parse method
/// </summary>
type HttpContentType with
    member this.Parse(contentType2 : string) =
        if (String.IsNullOrWhiteSpace contentType2) then
            HttpContentType.Other("")
        else
            let contentType = contentType2.ToLowerInvariant().Trim()
            if contentType.Contains("/json") then
                Json(contentType)
            else if contentType.Contains("/xml") || contentType.Contains("/xhtml") then
                Xml(contentType)
            else if contentType.Contains("/html") then
                Html(contentType)
            else
                Other(contentType)
        


