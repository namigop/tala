namespace WcfStorm.Tala

type MainWindowViewModel() =
    let requestPayload = HttpPayload()
    let responsePayload = HttpPayload()

    let headers = 
        let temp = HttpHeaders()
        temp.Add(HttpHeader(Key="wer", Value="werwer"))
        temp

    member this.Headers 
        with get() = headers

    member this.Request = requestPayload
    member this.Response = responsePayload


