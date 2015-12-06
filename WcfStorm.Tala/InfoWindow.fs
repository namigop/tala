namespace WcfStorm.Tala

open Xceed.Wpf.Toolkit
open System.Collections.ObjectModel
open RestSharp
open System.Threading
open FsXaml
open System
open System.Net
open System.Configuration

type Feature() =
    inherit NotifyBase()

    let mutable description = ""
    let mutable isInTala = false
    let mutable isInRest = false

    member this.Description 
        with get() = description
        and set v = this.RaiseAndSetIfChanged(&description, v, "Description")

    
    member this.IsInTala 
        with get() = isInTala
        and set v = this.RaiseAndSetIfChanged(&isInTala, v, "IsInTala")

    member this.IsInRest 
        with get() = isInRest
        and set v = this.RaiseAndSetIfChanged(&isInRest, v, "IsInRest")

type InfoWindowViewModel() =
    inherit NotifyBase()

    let features =
        let temp = new ObservableCollection<Feature>()
        temp.Add(Feature(Description = "Send GET, PUT, POST, DELETE, HEAD and OPTIONS HTTP Requests ", IsInTala=true, IsInRest = true))
        temp.Add(Feature(Description = "Support Basic Authentication", IsInTala=true, IsInRest = true))
        temp.Add(Feature(Description = "Supoprt Windows Authentication", IsInTala=true, IsInRest = true))
        temp.Add(Feature(Description = "Syntax Highlighting for JSON", IsInTala=true, IsInRest = true))
        temp.Add(Feature(Description = "Syntax Highlighting for XML", IsInTala=true, IsInRest = true))
        temp.Add(Feature(Description = "View response cookies", IsInTala=true, IsInRest = true))
        temp.Add(Feature(Description = "Save HTTP Request", IsInTala=true, IsInRest = true))
        temp.Add(Feature(Description = "Load HTTP Request", IsInTala=true, IsInRest = true))
        temp.Add(Feature(Description = "Create and load test projects", IsInTala=false, IsInRest = true))
        temp.Add(Feature(Description = "Support for functional test cases", IsInTala=false, IsInRest = true))
        temp.Add(Feature(Description = "Line-by-line comparison of expected and actual responses", IsInTala=false, IsInRest = true))
        temp.Add(Feature(Description = "Support for custom validation rules on actual responses", IsInTala=false, IsInRest = true))
        temp.Add(Feature(Description = "Support for dynamic requests via Python scripting", IsInTala=false, IsInRest = true))
        temp.Add(Feature(Description = "Support X509 certificates", IsInTala=false, IsInRest = true))
        temp.Add(Feature(Description = "Send cookies in the HTTP Request", IsInTala=false, IsInRest = true))
      
        temp.Add(Feature(Description = "Support for performance tests", IsInTala=false, IsInRest = true))
        temp.Add(Feature(Description = "Support for distributed/networked performance tests tests", IsInTala=false, IsInRest = true))
       
        temp

    member this.Features = features

    member this.OpenWcfStormCommand =
        let canRun arg = true
        Command.create 
            canRun
            (fun arg -> System.Diagnostics.Process.Start("http://www.wcfstorm.com/wcf/learn-more-rest.aspx") |> ignore)


