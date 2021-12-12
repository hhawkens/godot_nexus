namespace App.Presentation.Utilities;

/// Marks all types that contain information about the changed state of an IObservableList.
public abstract record ListChangedEventArgs<T>
{
	public static Appended Append(T appendedItem, int newFinalIndex) => new Appended(appendedItem, newFinalIndex);
	public static Inserted Insert(T insertedItem, int atIndex) => new Inserted(insertedItem, atIndex);
	public static Removed Remove(T removedItem, int fromIndex) => new Removed(removedItem, fromIndex);
	public static Moved Move(T movedItem, int oldIndex, int newIndex) => new Moved(movedItem, oldIndex, newIndex);
	public static Replaced Replace(T oldItem, T newItem, int atIndex) => new Replaced(oldItem, newItem, atIndex);
	public static Reset DoReset() => ResetSingleton;

	/// Describes an object being appended to the end of a list.
	/// Contains the new final index as a convenience.
	public sealed record Appended(T AppendedItem, int NewFinalIndex) : ListChangedEventArgs<T>;

	/// Describes an object being added to a list, anywhere in the middle (includes everything except the last index).
	public sealed record Inserted(T InsertedItem, int AtIndex) : ListChangedEventArgs<T>;

	/// Describes an object being removed from a list.
	public sealed record Removed(T RemovedItem, int FromIndex) : ListChangedEventArgs<T>;

	/// Describes an object being moved within a list.
	public sealed record Moved(T MovedItem, int OldIndex, int NewIndex) : ListChangedEventArgs<T>;

	/// Describes an object being replaced in-place within a list.
	public sealed record Replaced(T OldItem, T NewItem, int AtIndex) : ListChangedEventArgs<T>;

	/// Describes a list being changed considerably, where tracking changes is no longer feasible.
	public sealed record Reset : ListChangedEventArgs<T>;

	private static readonly Reset ResetSingleton = new();
}
