namespace WcfStorm.Tala

/// <summary>
/// Url targetted by the the HTTP call
/// </summary>
type TargetUrl() =
    inherit NotifyBase()
    let mutable url = ""
    let mutable isCallInProgress = false
    let mutable isSelected = false
    let mutable isNotSelected = true

    /// <summary>
    /// Http-based Url
    /// </summary>
    member this.Url 
        with get () = url
        and set v = this.RaiseAndSetIfChanged(&url, v, "Url")
   
    /// <summary>
    /// True if an Http call is in progress
    /// </summary>
    member this.IsCallInProgress
        with get () = isCallInProgress
        and set v = this.RaiseAndSetIfChanged(&isCallInProgress, v, "IsCallInProgress")
   
    /// <summary>
    /// True if this url is selected by the user (through the combo box)
    /// </summary>
    member this.IsSelected
        with get () = isSelected
        and set v = 
            this.RaiseAndSetIfChanged(&isSelected, v, "IsSelected")

