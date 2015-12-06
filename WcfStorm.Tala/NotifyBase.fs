namespace WcfStorm.Tala

open System
open System.ComponentModel
open System.Windows
open System.Windows.Input


/// Base class of all viewmodels
type NotifyBase() =
    let propertyChangedEvent = new DelegateEvent<PropertyChangedEventHandler>()


    interface INotifyPropertyChanged with
       
        /// <summary>
        /// Implements the PropertyChangedEvent
        /// </summary>
        [<CLIEvent>]
        member x.PropertyChanged = propertyChangedEvent.Publish

    /// <summary>
    /// Manually raise a propertychanged event
    /// </summary>
    /// <param name="propertyName"></param>
    member x.OnPropertyChanged propertyName =
        propertyChangedEvent.Trigger([| x; new PropertyChangedEventArgs(propertyName) |])

    /// <summary>
    /// Assign a new value to a property and raise a property changed event if the new value is different from the old value
    /// </summary>
    member x.RaiseAndSetIfChanged<'a when 'a : equality>((oldValue : 'a byref), newValue, propertyName) =
        if not (oldValue = newValue) then
            oldValue <- newValue
            x.OnPropertyChanged(propertyName)