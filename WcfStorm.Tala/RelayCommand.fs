namespace WcfStorm.Tala

open System
open System.ComponentModel
open System.Windows
open System.Windows.Input

/// ICommand that relays the call to the passed-in function
type RelayCommand(canExecute : obj -> bool, action : obj -> unit) =
    let event = new DelegateEvent<EventHandler>()
    interface ICommand with

        [<CLIEvent>]
        member x.CanExecuteChanged = event.Publish
        member x.CanExecute arg = canExecute (arg)
        member x.Execute arg = action (arg)

    member x.RaiseCanExecuteChanged() =
        let com = x :> ICommand
        event.Trigger([| x; EventArgs() |])