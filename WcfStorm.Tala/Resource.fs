namespace WcfStorm.Tala

open System
open ICSharpCode.AvalonEdit.Highlighting
open ICSharpCode.AvalonEdit
open System.IO
open System.Resources
open System.Xml
open ICSharpCode.AvalonEdit.Document
open ICSharpCode.AvalonEdit.Highlighting.Xshd
open System.Configuration

type BasicAuthentation() =
    inherit NotifyBase()
    let mutable username = ""
    let mutable password = ""
    let mutable domain = ""

    member this.Username
        with get() = username
        and set v = this.RaiseAndSetIfChanged(&username, v, "Username")

    member this.Password
        with get() = password
        and set v = this.RaiseAndSetIfChanged(&password, v, "Password")

    member this.Domain
        with get() = domain
        and set v = this.RaiseAndSetIfChanged(&domain, v, "Domain")

type GeneralSettings() =
    inherit NotifyBase()

    let mutable followRedirect = 
        ConfigurationManager.AppSettings.Item("followRedirects")
        |> Convert.ToBoolean

    let mutable timeoutInMsec = 
        ConfigurationManager.AppSettings.Item("timeoutInMsec")
        |> Convert.ToInt32

    let mutable maxRedirects = 
        ConfigurationManager.AppSettings.Item("maxRedirects")
        |> Convert.ToInt32

    member this.FollowRedirects
        with get() = followRedirect
        and set v = this.RaiseAndSetIfChanged(&followRedirect, v, "FollowRedirects")

    member this.MaxRedirects
        with get() = maxRedirects
        and set v = this.RaiseAndSetIfChanged(&maxRedirects, v, "MaxRedirects")

    member this.Timeout
        with get() = timeoutInMsec
        and set v = this.RaiseAndSetIfChanged(&timeoutInMsec, v, "Timeout")

module Resource =
    let jsonHighlightingMode =
        let res = ResourceManager("Tala", System.Reflection.Assembly.GetExecutingAssembly())
        let stream = res.GetObject("Json") :?> byte array
        let ms = new MemoryStream(stream)
        HighlightingLoader.Load(new XmlTextReader(ms), HighlightingManager.Instance);
     
 module Cast =
    let convert<'T> (o:obj) = 
        match o with
        | :? 'T as res -> Some(res)
        | _ -> None

module Config =

    let basicAuth = new BasicAuthentation()
    let genSettings = GeneralSettings()
//
//   
//    let followRedirect = 
//        ConfigurationManager.AppSettings.Item("followRedirects")
//        |> Convert.ToBoolean
//
//    let timeoutInMsec = 
//        ConfigurationManager.AppSettings.Item("timeoutInMsec")
//        |> Convert.ToInt32
//
//    let maxRedirects = 
//        ConfigurationManager.AppSettings.Item("maxRedirects")
//        |> Convert.ToInt32
