namespace WcfStorm.Tala

open System
open System.ComponentModel
open System.Windows
open System.Windows.Input


/// Base class of all viewmodels
type NotifyBase() =
    let propertyChangedEvent = new DelegateEvent<PropertyChangedEventHandler>()

    interface INotifyPropertyChanged with
        [<CLIEvent>]
        member x.PropertyChanged = propertyChangedEvent.Publish

    member x.OnPropertyChanged propertyName =
        propertyChangedEvent.Trigger([| x; new PropertyChangedEventArgs(propertyName) |])

    member x.RaiseAndSetIfChanged<'a when 'a : equality>((oldValue : 'a byref), newValue, propertyName) =
        if not (oldValue = newValue) then
            oldValue <- newValue
            x.OnPropertyChanged(propertyName)