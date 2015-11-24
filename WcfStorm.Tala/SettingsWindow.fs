namespace WcfStorm.Tala

open Xceed.Wpf.Toolkit
open System.Collections.ObjectModel
open RestSharp
open System.Threading
open FsXaml
open System
open System.Net
open System.Configuration

type SettingsWindowViewModel() as this =
    inherit NotifyBase()

    let mutable isWindowsDefaultCredChecked = true
    let mutable isWindowsNetworkCredChecked = false
    let mutable isAnonymousAuthChecked = true
    let mutable isBasicAuthChecked = false
    let mutable isWindowsAuthChecked = false
    let canEnterCredentials() = isBasicAuthChecked ||(isWindowsAuthChecked && isWindowsNetworkCredChecked)

    let updateAuth (source:BasicAuthentation) (target:BasicAuthentation) =       
        if not (isAnonymousAuthChecked) then
            target.Username <- source.Username
            target.Password <- source.Password
            target.Domain <- source.Domain
            if (isBasicAuthChecked) then
                target.AuthMode <- AuthMode.Basic
            if (isWindowsNetworkCredChecked) then
                target.AuthMode <- AuthMode.Windows

        else
            target.AuthMode <- AuthMode.Anonymous

        target
 
    let updateSettings (source:GeneralSettings) (target:GeneralSettings) =
        target.FollowRedirects <- source.FollowRedirects
        target.MaxRedirects <- source.MaxRedirects
        target.Timeout <- source.Timeout
        target
    
    let mutable basicAuth = updateAuth Config.basicAuth (BasicAuthentation())
    let mutable genSettings = updateSettings Config.genSettings (GeneralSettings())

    let mutable close = fun () -> ()

    member this.CanEnterCredentials = canEnterCredentials()
    member this.IsWindowsDefaultCredChecked 
        with get() = isWindowsDefaultCredChecked
        and set v = this.RaiseAndSetIfChanged(&isWindowsDefaultCredChecked, v, "IsWindowsDefaultCredChecked")
   
    member this.IsWindowsNetworkCredChecked 
        with get() = isWindowsNetworkCredChecked
        and set v = 
            this.RaiseAndSetIfChanged(&isWindowsNetworkCredChecked, v, "IsWindowsNetworkCredChecked")
            this.OnPropertyChanged("CanEnterCredentials")
    
    member this.IsAnonymousAuthChecked 
        with get() = isAnonymousAuthChecked
        and set v = this.RaiseAndSetIfChanged(&isAnonymousAuthChecked, v, "IsAnonymousAuthChecked")
    
    member this.IsBasicAuthChecked 
        with get() = isBasicAuthChecked
        and set v = 
            this.RaiseAndSetIfChanged(&isBasicAuthChecked, v, "IsBasicAuthChecked")
            this.OnPropertyChanged("CanEnterCredentials")
    
    member this.IsWindowsAuthChecked 
        with get() = isWindowsAuthChecked
        and set v = this.RaiseAndSetIfChanged(&isWindowsAuthChecked, v, "IsWindowsAuthChecked")

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

         

