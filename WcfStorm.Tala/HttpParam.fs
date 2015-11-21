namespace WcfStorm.Tala

open RestSharp
open System.Collections.ObjectModel

type HttpParam() =
    inherit NotifyBase()
    let mutable name = ""
    let mutable value = ""
    let mutable paramType = ParameterType.GetOrPost

    member this.Name 
        with get () = name
        and set v = this.RaiseAndSetIfChanged(&name, v, "Name")
    
    member this.Value
        with get () = value
        and set v = this.RaiseAndSetIfChanged(&value, v, "Value")

    member this.ParameterType
        with get () = paramType
        and set v = this.RaiseAndSetIfChanged(&paramType, v, "ParameterType")

        
type HttpParams() =
    inherit ObservableCollection<HttpParam>()