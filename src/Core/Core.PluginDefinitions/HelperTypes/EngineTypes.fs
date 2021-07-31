namespace App.Core.PluginDefinitions

open App.Core.Domain

/// Describes the possible outcomes of trying to remove an installed engine.
type public EngineRemovalResult =
    | SuccessfulRemoval
    | EngineNotFound
    | RemovalFailed of ErrorMessage
