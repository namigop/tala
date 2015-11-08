namespace WcfStorm.Tala

module Command =
    let create canRun onRun = RelayCommand(canRun, onRun)
     

