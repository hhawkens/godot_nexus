using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace App.Presentation.Utilities.Tests;

using ListChangedEventArgs = ListChangedEventArgs<string>;

[TestFixture]
public class ObservableListTests
{
	private ObservableList<string> sut = null!;
	private List<ListChangedEventArgs<string>> events = null!;

	[SetUp]
	public void SetUp()
	{
		sut = new(new[] { "1", "2", "3" });
		events = new();
		sut.ListChanged += (_, args) => { events.Add(args); };
	}

	[Test]
	public void Add_Events_Are_Fired_Correctly()
	{
		sut.Clear();
		events.Clear();

		sut.Add("11");
		sut.Add("22");
		sut.Add("33");
		sut.Insert(2, "44");

		Assert.That(sut, Is.EquivalentTo(new[] { "11", "22", "44", "33" }));
		Assert.That(events.Count, Is.EqualTo(4));
		Assert.That(events[0], Is.EqualTo(ListChangedEventArgs.Append("11", 0)));
		Assert.That(events[1], Is.EqualTo(ListChangedEventArgs.Append("22", 1)));
		Assert.That(events[2], Is.EqualTo(ListChangedEventArgs.Append("33", 2)));
		Assert.That(events[3], Is.EqualTo(ListChangedEventArgs.Insert("44", 2)));
	}

	[Test]
	public void Remove_Events_Are_Fired_Correctly()
	{
		sut.Remove("2");
		sut.RemoveAt(1);

		Assert.That(sut, Is.EquivalentTo(new[] { "1" }));
		Assert.That(events.Count, Is.EqualTo(2));
		Assert.That(events[0], Is.EqualTo(ListChangedEventArgs.Remove("2", 1)));
		Assert.That(events[1], Is.EqualTo(ListChangedEventArgs.Remove("3", 1)));
	}

	[Test]
	public void Move_Events_Are_Fired_Correctly()
	{
		sut.Move(2, 0);

		Assert.That(sut, Is.EquivalentTo(new[] { "3", "1", "2" }));
		Assert.That(events.Count, Is.EqualTo(1));
		Assert.That(events[0], Is.EqualTo(ListChangedEventArgs.Move("3", 2, 0)));
	}

	[Test]
	public void Replace_Events_Are_Fired_Correctly()
	{
		sut[1] = "100";

		Assert.That(sut, Is.EquivalentTo(new[] { "1", "100", "3" }));
		Assert.That(events.Count, Is.EqualTo(1));
		Assert.That(events[0], Is.EqualTo(ListChangedEventArgs.Replace("2", "100", 1)));
	}

	[Test]
	public void Reset_Events_Are_Fired_Correctly()
	{
		sut.Clear();

		Assert.That(sut, Is.EquivalentTo(Array.Empty<string>()));
		Assert.That(events.Count, Is.EqualTo(1));
		Assert.That(events[0], Is.EqualTo(ListChangedEventArgs.DoReset()));
	}
}
