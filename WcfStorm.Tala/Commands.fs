namespace WcfStorm.Tala

module Command =

    /// <summary>
    /// Creates a RelayCommand instance
    /// </summary>
    let create canRun onRun = RelayCommand(canRun, onRun)
     

