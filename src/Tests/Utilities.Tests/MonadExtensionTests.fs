namespace FSharpPlus.Tests

module public MonadExtensionPropertyBasedTests =

    open FSharpPlus
    open FsCheck.NUnit

    [<Property>]
    let public ``Is Ok Returns True For Ok Results`` (any: obj) =
        let result = Ok any
        result.IsOk

    [<Property>]
    let public ``Is Ok Returns False For Error Results`` (any: obj) =
        let result = Error any
        not result.IsOk

    [<Property>]
    let public ``Is Error Returns True For Error Results`` (any: obj) =
        let result = Error any
        result.IsError

    [<Property>]
    let public ``Is Error Returns False For Ok Results`` (any: obj) =
        let result = Ok any
        not result.IsError

    [<Property>]
    let public ``To Option Converts Ok To Some`` (any: obj) =
        let result = Ok any
        result.ToOption = Some any

    [<Property>]
    let public ``To Option Converts Error To None`` (any: obj) =
        let result = Error any
        result.ToOption = None
