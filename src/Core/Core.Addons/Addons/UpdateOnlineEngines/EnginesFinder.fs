module internal App.Core.Addons.EnginesFinder

open AngleSharp.Html.Dom
open FSharpPlus
open App.Core.Domain
open App.Utilities

[<Literal>]
let internal ErrorMsg = "Could not find Godot engines online"

let private findGodotVersionLinks (godotVersionQuery: IGodotVersionQuery) url = async {
    let! anchors = findAnchors url
    return
        anchors
        |> filter (fun a -> godotVersionQuery.IsVersion a.InnerHtml)
        |> map (fun a -> a.Href)
}

let private findGodotEnginesBy archiveChecker dotnet url = async {
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
        | sizeData: IHtmlTableDataCellElement::_ ->
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

let private findGodotEngines (godotVersionQuery: IGodotVersionQuery) url =
    findGodotEnginesBy godotVersionQuery.GetArchiveVersion NoSupport url

let private findGodotMonoEngines (godotVersionQuery: IGodotVersionQuery) url =
    findGodotEnginesBy godotVersionQuery.GetMonoArchiveVersion Mono url

let internal find godotVersionQuery url = async {
    let! webConnection = checkWebConnection ()
    if not webConnection then
        return Error $"{ErrorMsg}, no internet connection!"
    else
        let! godotVersionLinks = findGodotVersionLinks godotVersionQuery url

        let! enginesAsync = godotVersionLinks |> Seq.map (findGodotEngines godotVersionQuery) |> startParallel
        let! monoEnginesAsync =
            godotVersionLinks
            |> Seq.map (fun gv -> findGodotMonoEngines godotVersionQuery $"{gv}mono/") |> startParallel

        let! enginesNested = enginesAsync
        let! monoEnginesNested = monoEnginesAsync

        let engines = enginesNested |> flatten
        let monoEngines = monoEnginesNested |> flatten
        let allEngines = Seq.append engines monoEngines |> List.ofSeq

        if allEngines.Length > 0 then return Ok allEngines else return Error $"{ErrorMsg}!"
}
