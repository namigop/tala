namespace WcfStorm.Tala

open System
open System.Collections.ObjectModel
open System.Collections.Specialized

type HttpHeader() as this =
    inherit NotifyBase()
    let mutable key = ""
    let mutable value = ""

    member x.Key 
        with get () = key
        and set v = this.RaiseAndSetIfChanged(&key, v, "Key")
    member x.Value
        with get () = value
        and set v = this.RaiseAndSetIfChanged(&value, v, "Value")


type HttpHeaders() as this =
    inherit ObservableCollection<HttpHeader>()
    
//    override this.OnCollectionChanged(args) =
//        match args.Action with
//        | NotifyCollectionChangedAction.Add ->
//            base.OnCollectionChanged(args)
//            if (args.NewItems.Count = 1) then
//                let key = args.NewItems.[0] :?> HttpHeader
//                if not(String.IsNullOrWhiteSpace(key.Key)) then
//                    this.Add(HttpHeader(Key="", Value=""))
//        |_ ->  base.OnCollectionChanged(args)