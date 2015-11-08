namespace WcfStorm.Tala

open System
open System.Collections.Generic
open ICSharpCode.AvalonEdit.Document
open ICSharpCode.AvalonEdit.Folding

type JsonBraceFolder() as this =
    inherit  ICSharpCode.AvalonEdit.Folding.AbstractFoldingStrategy()
    
    let openingBrace = "{"
    let closingBrace = "}"
    /// <summary>

    override  this.CreateNewFoldings(document:TextDocument) =
        firstErrorOffset = -1;
        CreateNewFoldings(document);
