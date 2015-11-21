namespace Nico

open System
open FsXaml

type App = XAML< "App.xaml" >

module main =
    open System.Windows
    open WcfStorm.Tala
    open ICSharpCode.AvalonEdit
    open System.ComponentModel
    open ICSharpCode.AvalonEdit.Folding
    open System.Windows.Threading

    [<STAThread>]
    [<EntryPoint>]
    let main argv =
        let app = App()
        let isSetup = ref false

        let getVm() =
            let window = app.Root.MainWindow
            window.DataContext :?> MainWindowViewModel
        app.Root.Activated 
            |> Observable.add (fun arg ->
                if not (!isSetup) then
                    isSetup := true
                    let vm = getVm()
                    let textEditorRequest =
                        app.Root.MainWindow.FindName("textEditorRequest") :?> TextEditor
                    let textEditorResponse =
                        app.Root.MainWindow.FindName("textEditorResponse") :?> TextEditor
                    let reqFoldingManager = FoldingManager.Install(textEditorRequest.TextArea)
                    let respFoldingManager = FoldingManager.Install(textEditorResponse.TextArea)

                    let timer =
                        let temp = new DispatcherTimer(Interval = TimeSpan.FromMilliseconds(1000.0))
                        temp.Tag <- true
                        temp.Tick
                        |> Observable.add
                                (fun arg ->
                                if (temp.Tag :?> bool) then
                                    if (vm.Request.Doc.Text.Length > 0) then
                                        vm.Request.FoldFunction(reqFoldingManager, vm.Request.Doc)
                                    temp.Tag <- false
                                else
                                    temp.Tag <- true
                                    if (vm.Response.Doc.Text.Length > 0) then
                                        vm.Response.FoldFunction(respFoldingManager, vm.Response.Doc))
                        temp
                    timer.Start())
        Application.Current.DispatcherUnhandledException 
            |> Observable.add (fun f ->
                    f.Handled <- true
                    //            let getVm() =
                    //                let window = app.Root.MainWindow
                    //                window.DataContext :?> MainWindowViewModel
                    let vm = getVm()
                    vm.IsCallInProgress <- false
                    MessageBox.Show(f.Exception.ToString()) |> ignore)
        app.Root.Run()