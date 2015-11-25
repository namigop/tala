namespace WcfStorm.Tala

open Xceed.Wpf.Toolkit
open System.Collections.ObjectModel
open RestSharp
open System.Threading
open FsXaml
open System
open System.Net

//this needs to be serialized so everything is mutable
type TestRequest() =
    let mutable url = ""
    let mutable verb = Method.GET
    let mutable reqText = ""
    let mutable headers : WcfStorm.Tala.HttpHeader array = Array.empty
    let mutable reqParams : HttpParam array = Array.empty

    member x.Url 
        with get() = url
        and set v = url <- v
    member x.Verb 
        with get() = verb
        and set v = verb <- v
    member x.RequestHeaders 
        with get() = headers
        and set v = headers <- v
    member x.RequestParameters
        with get() = reqParams
        and set v = reqParams <- v
    member x.RequestText 
        with get() = reqText
        and set v = reqText <- v
