namespace WcfStorm.Tala

open ICSharpCode.AvalonEdit.Highlighting
open ICSharpCode.AvalonEdit
open System.IO
open System.Resources
open System.Xml
open ICSharpCode.AvalonEdit.Document

module Resource =
    open ICSharpCode.AvalonEdit.Highlighting.Xshd

    let jsonHighlightingMode =
        let res = ResourceManager("Tala", System.Reflection.Assembly.GetExecutingAssembly())
        let stream = res.GetObject("Json") :?> byte array
        let ms = new MemoryStream(stream)
        HighlightingLoader.Load(new XmlTextReader(ms), HighlightingManager.Instance);
        
type HttpPayload()=
    inherit NotifyBase()
    let mutable highlighting : IHighlightingDefinition =
        Resource.jsonHighlightingMode
        // Unchecked.defaultof<IHighlightingDefinition>
    let mutable text = TextDocument()


    member this.Highlighting 
        with get () = highlighting
        and set v = this.RaiseAndSetIfChanged(&highlighting, v, "Highlighting")
    
    member this.Text 
        with get () = text
        and set v = this.RaiseAndSetIfChanged(&text, v, "Text")

    