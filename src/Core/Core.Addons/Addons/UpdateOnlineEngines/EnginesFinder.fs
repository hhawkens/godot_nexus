module internal App.Core.Addons.EnginesFinder

open System
open AngleSharp.Html.Dom
open FSharpPlus
open App.Core.Domain
open App.Utilities

let private findGodotVersionLinks url = async {
    let! anchors = findAnchors url
    return
        anchors
        |> filter (fun a -> TextHelpers.isGodotVersion a.InnerHtml)
        |> map (fun a -> a.Href)
}

let private findGodotEnginesBy (archiveChecker: string -> Version option) dotnet url = async {
    let tryGetLink (row: IHtmlTableRowElement) =
        match row.FindAnchors() with
        | [ a ] ->
            match archiveChecker a.Href with
            | Some version -> Some (row, version, a.Href)
            | None -> None
        | _ -> None

    let tryGetArchiveSize (row: IHtmlTableRowElement, x, y) =
        let sizeCells = row.GetElementsByClassName("s") |> choose trycast<IHtmlTableDataCellElement> |> List.ofSeq
        match sizeCells with
        | (sizeData: IHtmlTableDataCellElement)::_ ->
            match FileSize.FromText(sizeData.TextContent) with
            | Some fileSize -> (fileSize, x, y) |> Some
            | None -> None
        | _ -> None

    let! rows = findTableRows url
    let validRowInfos = rows |> choose tryGetLink |> choose tryGetArchiveSize

    return
        validRowInfos
        |> map (fun (size, ver, url) ->  EngineOnline.New {Version = ver; DotNetSupport = dotnet} url size)
}

let private findGodotEngines url =
    findGodotEnginesBy TextHelpers.getGodotArchiveVersion NoSupport url

let private findGodotMonoEngines url =
    findGodotEnginesBy TextHelpers.getGodotMonoArchiveVersion Mono url

let internal find url = async {
    let! webConnection = checkWebConnection ()
    if not webConnection then
        return Error $"{TextHelpers.ErrorMsg}, no internet connection!"
    else
        let! godotVersionLinks = findGodotVersionLinks url

        let! enginesAsync = godotVersionLinks |> Seq.map findGodotEngines |> startParallel
        let! monoEnginesAsync =
            godotVersionLinks |> Seq.map (fun gv -> findGodotMonoEngines $"{gv}mono/") |> startParallel

        let! enginesNested = enginesAsync
        let! monoEnginesNested = monoEnginesAsync

        let engines = enginesNested |> flatten
        let monoEngines = monoEnginesNested |> flatten
        let allEngines = Seq.append engines monoEngines |> List.ofSeq

        if allEngines.Length > 0 then return Ok allEngines else return Error $"{TextHelpers.ErrorMsg}!"
}
