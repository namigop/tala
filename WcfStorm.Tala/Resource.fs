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
//  <add key="followRedirects" value="false"/>
//  <add key="timeoutInMsec" value="false"/>
// <add key="maxRedirects" value="0"/>
   
    let followRedirect = 
        ConfigurationManager.AppSettings.Item("followRedirects")
        |> Convert.ToBoolean

    let timeoutInMsec = 
        ConfigurationManager.AppSettings.Item("timeoutInMsec")
        |> Convert.ToInt32

    let maxRedirects = 
        ConfigurationManager.AppSettings.Item("maxRedirects")
        |> Convert.ToInt32
