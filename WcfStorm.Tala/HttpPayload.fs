namespace WcfStorm.Tala

open ICSharpCode.AvalonEdit.Highlighting
open ICSharpCode.AvalonEdit
open System.Text
open System.IO
open System.Resources
open System.Xml
open ICSharpCode.AvalonEdit.Document
open ICSharpCode.AvalonEdit.Folding
open Newtonsoft.Json
open System.Xml.Linq

/// <summary>
/// Selection of different payloads that goes with the HTTP request
/// </summary>
type PayloadSelection =
    {
        RawBody : int
        Parameters :int
        Cookies :int
    }

/// <summary>
/// Represents the HTTP Request payload
/// </summary>
type HttpPayload() =
    inherit NotifyBase()
    let mutable highlighting = Resource.jsonHighlightingMode
    let mutable mode = HttpContentType.Json("text/json")
    let mutable doc = TextDocument(Text="")
    let mutable foldFunction = 
        let folding, _ = EditorOptions.get mode
        folding

    let selection = { RawBody=0; Parameters = 1; Cookies = 2}
    let mutable selectedTabIndex = 0
    let mutable isParametersChecked = false
    let mutable isCookiesChecked = false
    let mutable isRawBodyChecked = true

    /// <summary>
    /// Gets or sets the HTTP ContentType
    /// </summary>
    member this.Mode 
        with get() = mode
        and set v =
            if not(mode = v) then
                let folding, highlighting2 =  EditorOptions.get v
                this.Highlighting <- highlighting2
                foldFunction <- folding 

    /// <summary>
    /// Gets or sets the highlighting mode (JSON or XML)
    /// </summary>
    member this.Highlighting 
        with get () = highlighting
        and set v = this.RaiseAndSetIfChanged(&highlighting, v, "Highlighting")
    
    /// <summary>
    /// Gets or sets the TextDocument instance used by AvalonEdit
    /// </summary>
    member this.Doc 
        with get () = doc
        and set v = this.RaiseAndSetIfChanged(&doc, v, "Doc")

    /// <summary>
    /// Gets or sets the function to fold the text in the avalon editor
    /// </summary>
    member x.FoldFunction = foldFunction
    
    /// <summary>
    /// Gets or sets the index of the tab depending on the selected payload (RawBody, Parameters, or Cookies)
    /// </summary>
    member this.SelectedTabIndex 
        with get () = selectedTabIndex
        and set v = this.RaiseAndSetIfChanged(&selectedTabIndex, v, "SelectedTabIndex")

    /// <summary>
    /// Gets or sets a value indicating whether the Parameters tab is selected
    /// </summary>
    member this.IsParametersChecked
        with get () = isParametersChecked
        and set v = 
            this.RaiseAndSetIfChanged(&isParametersChecked, v, "IsParametersChecked")
            if v then 
               this.SelectedTabIndex <- selection.Parameters

    /// <summary>
    /// Gets or sets a value indicating whether the RawBody tab is selected
    /// </summary>
    member this.IsRawBodyChecked
        with get () = isRawBodyChecked
        and set v = 
            this.RaiseAndSetIfChanged(&isRawBodyChecked, v, "IsRawBodyChecked")
            if v then 
                this.SelectedTabIndex <- selection.RawBody

    /// <summary>
    /// Gets or sets a value indicating whether the Cookies tab is selected
    /// </summary>
    member this.IsCookiesChecked
        with get () = isCookiesChecked
        and set v = 
            this.RaiseAndSetIfChanged(&isCookiesChecked, v, "IsCookiesChecked")
            if v then 
                this.SelectedTabIndex <- selection.Cookies

    /// <summary>
    /// Set the text for the payload
    /// </summary>
    member this.SetText(text, mode:HttpContentType) =
        match mode with
        | Json(_) -> this.Doc.Text <- EditorOptions.prettyPrintJson text
        | Xml(_) -> this.Doc.Text <- EditorOptions.prettyPrintXml text
        | _ -> this.Doc.Text <- text
            
