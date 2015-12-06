namespace WcfStorm.Tala

open RestSharp
open System.Collections.ObjectModel

/// <summary>
/// HTTP Request Paramaters
/// </summary>
type HttpParam() =
    inherit NotifyBase()
    let mutable name = ""
    let mutable value = ""
    let mutable paramType = ParameterType.GetOrPost

    /// <summary>
    /// Name of the parameter
    /// </summary>
    member this.Name 
        with get () = name
        and set v = this.RaiseAndSetIfChanged(&name, v, "Name")
    
    /// <summary>
    /// Value of the parameter
    /// </summary>
    member this.Value
        with get () = value
        and set v = this.RaiseAndSetIfChanged(&value, v, "Value")

    /// <summary>
    /// Type of the parameter
    /// </summary>
    member this.ParameterType
        with get () = paramType
        and set v = this.RaiseAndSetIfChanged(&paramType, v, "ParameterType")

/// <summary>
/// Collection of HttpParams instances
/// </summary>
type HttpParams() =
    inherit ObservableCollection<HttpParam>()