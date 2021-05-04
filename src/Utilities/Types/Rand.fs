module public App.Utilities.Rand

open System

[<Literal>]
let private HalfWord = 32

let private random = Random(0xABCDE)

/// Returns a seeded random int32 value.
let public NextI32 () = random.Next()

/// Returns a seeded random uint64 value.
let public NextU64 () =
    let ul1, ul2 = uint64 (random.Next()), uint64 (random.Next())
    ul1 <<< HalfWord ||| ul2
