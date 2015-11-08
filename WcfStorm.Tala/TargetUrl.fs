namespace WcfStorm.Tala

type TargetUrl() =
    inherit NotifyBase()
    let mutable url = ""
    let mutable isCallInProgress = false
    let mutable isSelected = false
    let mutable isNotSelected = true

    member this.Url 
        with get () = url
        and set v = this.RaiseAndSetIfChanged(&url, v, "Url")
    member this.IsCallInProgress
        with get () = isCallInProgress
        and set v = this.RaiseAndSetIfChanged(&isCallInProgress, v, "IsCallInProgress")
    member this.IsSelected
        with get () = isSelected
        and set v = 
            this.RaiseAndSetIfChanged(&isSelected, v, "IsSelected")
            this.IsNotSelected <- not v

    member this.IsNotSelected
        with get () = isNotSelected
        and set v = this.RaiseAndSetIfChanged(&isNotSelected, v, "IsNotSelected")

    

