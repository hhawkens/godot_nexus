namespace FSharpPlus

open System.Text.RegularExpressions

[<AutoOpen>]
module private FileSize =

    [<Literal>]
    let internal Kilo = 1_000UL
    [<Literal>]
    let internal Mega = 1_000_000UL
    [<Literal>]
    let internal Giga = 1_000_000_000UL
    [<Literal>]
    let internal Tera = 1_000_000_000_000UL
    [<Literal>]
    let internal Peta = 1_000_000_000_000_000UL

    let internal parsingRgx =
        let num = @"\b(\d[.\d]*)\s*"
        let unit = @"([Bb][Yy][Tt][Ee][Ss]|[Kk][Ii][Ll][Oo]|[Mm][Ee][Gg][Aa]"
        let unit1 = @"|[Gg][Ii][Gg][Aa]|[Tt][Ee][Rr][Aa]|[Pp][Ee][Tt][Aa]"
        let unit2 = @"|[BbKkMmGgTtPp])"
        let bytes = @"(?:$|[bB]{1}|[^\w\d]|[^\s\S])"
        Regex(num + unit + unit1 + unit2 + bytes)

/// Represents the size of a file in KB, MB, GB etc.
[<Struct>]
type public FileSize = {
    bytes: uint64
} with

    member this.Kilobytes = (float this.bytes) / (float Kilo)
    member this.Megabytes = (float this.bytes) / (float Mega)
    member this.Gigabytes = (float this.bytes) / (float Giga)
    member this.Terabytes = (float this.bytes) / (float Tera)
    member this.Petabytes = (float this.bytes) / (float Peta)

    static member public FromKilobytes kb = {bytes = uint64 (float Kilo * kb)}
    static member public FromMegabytes mb = {bytes = uint64 (float Mega * mb)}
    static member public FromGigabytes gb = {bytes = uint64 (float Giga * gb)}
    static member public FromTerabytes tb = {bytes = uint64 (float Tera * tb)}
    static member public FromPetabytes pb = {bytes = uint64 (float Peta * pb)}

    static member public FromText txt =
        let rgxGroups = parsingRgx.Match(txt).Groups |> List.ofSeq
        match rgxGroups with
        | _::numGroup::[unitGroup] ->
            let num = numGroup.Value |> float
            let factor =
                match unitGroup.Value.ToLower() with
                | "b" | "bytes" -> 1UL
                | "k" | "kilo" -> Kilo
                | "m" | "mega" -> Mega
                | "g" | "giga" -> Giga
                | "t" | "tera" -> Tera
                | "p" | "peta" -> Peta
                | u -> failwith $"Unknown file size unit {u}"
                |> float
            let bytes = num * factor |> uint64
            Some {bytes = bytes}
        | _ -> None

    override this.ToString() =
        match this.bytes with
        | b when b > Peta -> $"{this.Petabytes:F1} pb"
        | b when b > Tera -> $"{this.Terabytes:F1} tb"
        | b when b > Giga -> $"{this.Gigabytes:F1} gb"
        | b when b > Mega -> $"{this.Megabytes:F1} mb"
        | b when b > Kilo -> $"{this.Kilobytes:F1} kb"
        | b -> $"{b} bytes"
