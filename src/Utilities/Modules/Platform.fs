module public FSharpPlus.Platform

open System

[<Struct; Flags>]
/// Operating system flags known to this app.
type public OS =
    | Unknown = 0b0000_0000uy
    | Unix = 0b0000_0001uy
    | Linux = 0b0000_0011uy
    | BSD = 0b0000_0101uy
    | Mac = 0b0000_1001uy
    | Windows = 0b0001_0000uy


/// Operating system related functions and checks.
module public OS =

    /// Returns the operating system that is running this application.
    let public current =
        if OperatingSystem.IsLinux () then OS.Linux
        else if OperatingSystem.IsFreeBSD () then OS.BSD
        else if OperatingSystem.IsMacOS () then OS.Mac
        else if OperatingSystem.IsWindows () then OS.Windows
        else OS.Unknown

    /// Check if the OS running this program is 64 bit.
    let public is64Bit = Environment.Is64BitOperatingSystem

    /// Check if the current OS is not known to this app.
    let public isUnknown = (current = OS.Unknown)

    /// Check if the current OS is a unix system.
    let public isUnix = current.HasFlag OS.Unix

    /// Check if the current OS is a linux system.
    let public isLinux = current.HasFlag OS.Linux

    /// Check if the current OS is a (Free-)BSD system.
    let public isBsd = current.HasFlag OS.BSD

    /// Check if the current OS is a mac system.
    let public isMac = current.HasFlag OS.Mac

    /// Check if the current OS is a windows system.
    let public isWindows = current.HasFlag OS.Windows
