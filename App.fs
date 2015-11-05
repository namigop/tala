namespace Nico

open System
open FsXaml

type App = XAML<"App.xaml">

module main =
    open System.Windows

    [<STAThread>]
    [<EntryPoint>]
    let main argv =    
        let app = App()  
//        app.Root.Activated
//            |> Observable.add (fun arg -> 
//                let vm = app.Root.MainWindow.DataContext :?> MainWindowViewModel
//                app.Root.MainWindow.Closed |> Observable.add(fun w -> vm.Close() )
//                
//                ) |> ignore


        Application.Current.DispatcherUnhandledException 
        |> Observable.add(fun f -> MessageBox.Show(f.Exception.ToString()) |> ignore )
        app.Root.Run()