namespace App.Core.Domain

/// Configuration data container.
type public ConfigData<'a when 'a: equality> = {
    Description: string
    DefaultValue: 'a
    CurrentValue: 'a
} with

    /// Checks if the currently set value is the default value.
    member this.IsDefault = (this.CurrentValue = this.DefaultValue)
