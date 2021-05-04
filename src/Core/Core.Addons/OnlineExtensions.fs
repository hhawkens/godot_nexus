[<AutoOpen>]
module internal App.Core.Addons.OnlineExtensions

open AngleSharp
open AngleSharp.Html.Dom
open FSharpPlus
open App.Utilities

let internal checkWebConnection () = Web.pingAsync "1.1.1.1"

let internal getWebContent (url: string) =
    let ctx = BrowsingContext.New(Configuration.Default.WithDefaultLoader())
    ctx.OpenAsync url |> Async.AwaitTask

let private getSelectors<'a when 'a: not struct> selector url = async {
    let! docs = getWebContent url
    return
        docs.QuerySelectorAll(selector)
        |> choose trycast<'a>
}

let internal findAnchors url = getSelectors<IHtmlAnchorElement> "a" url
let internal findTableRows url = getSelectors<IHtmlTableRowElement> "tr" url


type public AngleSharp.Dom.IParentNode with

    member public this.FindAnchors() =
        let anchors = this.QuerySelectorAll("a") |> choose trycast<IHtmlAnchorElement>
        anchors |> List.ofSeq
