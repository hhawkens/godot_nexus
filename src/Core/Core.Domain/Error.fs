namespace App.Core.Domain

[<Struct>]
type public ErrorType =
    | General

type public Error = {
    Type: ErrorType
    Message: ErrorMessage
}

and public ErrorMessage = string

module public Error =

    let public general msg = {Type = General; Message = msg}
