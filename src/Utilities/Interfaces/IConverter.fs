namespace FSharpPlus

/// Describes a type that can convert an object of one type to an object
/// of another type.
type public IConverter<'a, 'b> =
    abstract Convert: 'a -> 'b

/// Describes a type that can convert an object of one type to an object
/// of another type and vice versa.
type public IDualConverter<'a, 'b> =
    abstract Convert: 'a -> 'b
    abstract Convert: 'b -> 'a
