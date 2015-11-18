namespace WcfStorm.Tala

type HttpContentType =
    | Json of string 
    | Xml of string
    | Html of string
    | Other of string

type HttpContentType with
    member this.Parse(contentType2 : string) =
        let contentType = contentType2.ToLowerInvariant().Trim()
        if contentType.Contains("/json") then
            Json(contentType)
        else if contentType.Contains("/xml") || contentType.Contains("/xhtml") then
            Xml(contentType)
        else if contentType.Contains("/html") then
            Html(contentType)
        else
            Other(contentType)
        


