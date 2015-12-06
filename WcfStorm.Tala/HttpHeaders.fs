namespace WcfStorm.Tala

open System
open System.Collections.ObjectModel
open System.Collections.Specialized
open RestSharp

/// <summary>
/// Http Header
/// </summary>
type HttpHeader() as this =
    inherit NotifyBase()
    let mutable key = ""
    let mutable value = ""

    member x.Key 
        with get () = key
        and set v = this.RaiseAndSetIfChanged(&key, v, "Key")
    member x.Value
        with get () = value
        and set v = this.RaiseAndSetIfChanged(&value, v, "Value")

/// <summary>
/// Collection of  HTTP Headers
/// </summary>
type HttpHeaders() =
    inherit ObservableCollection<HttpHeader>()

/// <summary>
/// Cookies collection
/// </summary>
type Cookies() = 
    inherit ObservableCollection<RestResponseCookie>()