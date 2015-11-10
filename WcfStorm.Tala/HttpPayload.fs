namespace WcfStorm.Tala

open ICSharpCode.AvalonEdit.Highlighting
open ICSharpCode.AvalonEdit
open System.IO
open System.Resources
open System.Xml
open ICSharpCode.AvalonEdit.Document
open ICSharpCode.AvalonEdit.Folding

type HttpPayload() as this =
    inherit NotifyBase()
    let mutable highlighting = Resource.jsonHighlightingMode
    let mutable mode = "json"
    let mutable doc = TextDocument(Text="Foo")
    let mutable foldFunction = 
        let folding, _ = EditorOptions.get mode
        folding
    
    member this.Mode 
        with get() = mode
        and set v =
            let folding, highlighting2 = EditorOptions.get v
            this.Highlighting <- highlighting2
            foldFunction <- folding 
    member this.Highlighting 
        with get () = highlighting
        and set v = this.RaiseAndSetIfChanged(&highlighting, v, "Highlighting")
    
    member this.Doc 
        with get () = doc
        and set v = this.RaiseAndSetIfChanged(&doc, v, "Doc")

    member x.FoldFunction = foldFunction
        
    