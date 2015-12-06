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
        temp.Add(Feature(Description = "Feature1", IsInTala=true, IsInRest = true))
        temp.Add(Feature(Description = "Feature2", IsInTala=false, IsInRest = true))
        temp.Add(Feature(Description = "Feature3", IsInTala=true, IsInRest = false))
        temp

    member this.Features =features


