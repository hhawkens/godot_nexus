namespace App.Core.Domain

[<Struct>]
type public IdPrefixSuper = IdPrefixSuper of byte
[<Struct>]
type public IdPrefix = IdPrefix of byte
[<Struct>]
type public IdPrefixSub = IdPrefixSub of byte
[<Struct>]
type public IdVal = IdVal of int32

/// Process wide unique identifier for any object or entity.
[<Struct>]
type public Id = private {
    prefixSuper: IdPrefixSuper
    prefix: IdPrefix
    prefixSub: IdPrefixSub
    id: IdVal
} with

/// Super type of the id prefix, if there is one.
member public this.PrefixSuper = this.prefixSuper

/// Id prefix.
member public this.Prefix = this.prefix

/// Sub type of the id prefix, if there is one.
member public this.PrefixSub = this.prefixSub

/// Actual id value, regardless of prefix.
member public this.IdVal = this.id

/// Creates a new Id without any prefixes.
static member public New id = {
    prefixSuper = IdPrefixSuper 0uy
    prefix = IdPrefix 0uy
    prefixSub = IdPrefixSub 0uy
    id = id
}

/// Creates a new Id with a single level prefix.
static member public WithPrefix prefix id = {
    prefixSuper = IdPrefixSuper 0uy
    prefix = prefix
    prefixSub = IdPrefixSub 0uy
    id = id
}

/// Creates a new Id with a prefix and sub-prefix.
static member public WithPrefixSub prefix prefixSub id = {
    prefixSuper = IdPrefixSuper 0uy
    prefix = prefix
    prefixSub = prefixSub
    id = id
}

/// Text representation.
override this.ToString() =
    let (IdPrefixSuper prefixSuper) = this.prefixSuper
    let (IdPrefix prefix) = this.prefix
    let (IdPrefixSub prefixSub) = this.prefixSub
    let (IdVal id) = this.id
    $"{prefixSuper:D3}-{prefix:D3}-{prefixSub:D3}-{{{id}}}"
