namespace WcfStorm.Tala

open ICSharpCode.AvalonEdit.Highlighting
open ICSharpCode.AvalonEdit
open System.IO
open System.Resources
open System.Xml
open ICSharpCode.AvalonEdit.Document
open ICSharpCode.AvalonEdit.Highlighting.Xshd

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