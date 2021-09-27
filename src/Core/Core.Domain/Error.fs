namespace App.Core.Domain

/// Domain the error message belongs to.
[<Struct>]
type public ErrorType = | General


/// Wrapper for an error message and an error domain.
type public Error = {
    Type: ErrorType
    Message: ErrorMessage
} with

    static member public General msg = {Type = General; Message = msg}


/// Error message alias type.
and public ErrorMessage = string
