namespace WcfStorm.Tala

open Xceed.Wpf.Toolkit
open System.Collections.ObjectModel
open RestSharp
open System.Threading
open FsXaml
open System
open System.Net
open System.Configuration

type SettingsWindowViewModel() =
    inherit NotifyBase()

    let updateAuth (source:BasicAuthentation) (target:BasicAuthentation) =
        target.Username <- source.Username
        target.Password <- source.Password
        target.Domain <- source.Domain
        target
 
    let updateSettings (source:GeneralSettings) (target:GeneralSettings) =
        target.FollowRedirects <- source.FollowRedirects
        target.MaxRedirects <- source.MaxRedirects
        target.Timeout <- source.Timeout
        target

    let mutable basicAuth = updateAuth Config.basicAuth (BasicAuthentation())
    let mutable genSettings = updateSettings Config.genSettings (GeneralSettings())
     
    let mutable close = fun () -> ()

    member this.Close
        with get() = close
        and set v = close <- v

    member this.General
        with get() = genSettings
        and set v = this.RaiseAndSetIfChanged(&genSettings, v, "General")

    member this.Authentication 
        with get() = basicAuth
        and set v = this.RaiseAndSetIfChanged(&basicAuth, v, "Authentication")

    member this.CancelCommand =
        let canRun arg = true
        Command.create 
            canRun
            (fun arg -> this.Close())
        
    member this.OkCommand =
        let canRun arg = true
        Command.create 
            canRun
            (fun arg -> 
                (updateAuth basicAuth Config.basicAuth) |> ignore
                (updateSettings genSettings Config.genSettings) |> ignore
                this.Close()
            )

         

