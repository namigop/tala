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
open System.Collections.Generic
open System.Text
open System.Collections

///
/// This is a direct port to F# of the code https://github.com/icsharpcode/SharpDevelop/blob/master/samples/AvalonEdit.Sample/BraceFoldingStrategy.cs
///
type BraceFoldingStrategy() =
    let openingBrace = '{'
    let closingBrace = '}'
    let comparer =
        { new IComparer<NewFolding> with
              member x.Compare(a, b) = a.StartOffset.CompareTo(b.StartOffset) }

    let createNewFoldings(document : ITextSource) =
        let newFoldings = new List<NewFolding>()
        let startOffsets = new Stack<int>()
        let mutable lastNewLineOffset = 0
        for i in [ 0..document.TextLength-1 ] do
            let c = document.GetCharAt i
            if c = openingBrace then startOffsets.Push(i)
            elif (c = closingBrace && startOffsets.Count > 0) then
                let startOffset = startOffsets.Pop()
                if (startOffset < lastNewLineOffset) then newFoldings.Add(new NewFolding(startOffset, i + 1))
            elif (c = '\n' || c = '\r') then lastNewLineOffset <- i + 1
            else ()
        newFoldings.Sort(comparer)
        newFoldings

    let createNewFoldings2 (document : TextDocument) =
        let firstErrorOffset = -1
        firstErrorOffset, createNewFoldings(document)

  
    member this.UpdateFoldings (manager : FoldingManager, document : TextDocument) =
        let firstErrorOffset, newFoldings = createNewFoldings2 document
        manager.UpdateFoldings(newFoldings, firstErrorOffset)

   
module EditorOptions =
    let braceFolding = BraceFoldingStrategy()
    let xmlFolding = new XmlFoldingStrategy()
   
    let get (mode:HttpContentType) =
        match mode with
        | Json(raw) -> braceFolding.UpdateFoldings, Resource.jsonHighlightingMode
        | _ -> xmlFolding.UpdateFoldings, HighlightingManager.Instance.GetDefinition("XML")

    let prettyPrintJson(json) =
        try
            use stringReader = new StringReader(json)
            use stringWriter = new StringWriter()
            use jsonReader = new JsonTextReader(stringReader)
            use jsonWriter = new JsonTextWriter(stringWriter, Formatting = Newtonsoft.Json.Formatting.Indented)
            jsonWriter.WriteToken(jsonReader)
            stringWriter.ToString()
        with
        | _ -> json
        
    let prettyPrintXml(xml) =
        try
            XDocument.Parse(xml).ToString()
        with
        | _ -> xml