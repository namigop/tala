namespace WcfStorm.Tala

open System
open System.Collections.ObjectModel
open System.Collections.Specialized
open RestSharp
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


type HttpHeaders() =
    inherit ObservableCollection<HttpHeader>()
 
type Cookies() = 
    inherit ObservableCollection<RestResponseCookie>()