namespace WcfStorm.Tala

open ICSharpCode.AvalonEdit.Document
open ICSharpCode.AvalonEdit.Folding
open ICSharpCode.AvalonEdit.Highlighting
open System
open System.Collections.Generic

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

    let get mode =
        match mode with
        | "json" -> braceFolding.UpdateFoldings, Resource.jsonHighlightingMode
        | _ -> xmlFolding.UpdateFoldings, HighlightingManager.Instance.GetDefinition("XML")
