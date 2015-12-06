namespace WcfStorm.Tala

open Xceed.Wpf.Toolkit
open System.Collections.ObjectModel
open RestSharp
open System.Threading
open FsXaml
open System
open System.Net

/// <summary>
/// Holds the data to make an HTTP request.  This needs to be serialized so everything is mutable
/// </summary>
type TestRequest() =
    let mutable url = ""
    let mutable verb = Method.GET
    let mutable reqText = ""
    let mutable headers : WcfStorm.Tala.HttpHeader array = Array.empty
    let mutable reqParams : HttpParam array = Array.empty

    /// <summary>
    /// Gets or sets the target url
    /// </summary>
    member x.Url 
        with get() = url
        and set v = url <- v

    /// <summary>
    /// Gets or sets the Http verb (GET, PUT, POST etc.)
    /// </summary>
    member x.Verb 
        with get() = verb
        and set v = verb <- v

    /// <summary>
    /// Gets or sets the collection of HTTP headers
    /// </summary>
    member x.RequestHeaders 
        with get() = headers
        and set v = headers <- v

    /// <summary>
    /// Gets or sets the collection of HTTP Request Parameters
    /// </summary>
    member x.RequestParameters
        with get() = reqParams
        and set v = reqParams <- v

    /// <summary>
    /// Gets or sets the HTTP Request text
    /// </summary>
    member x.RequestText 
        with get() = reqText
        and set v = reqText <- v
