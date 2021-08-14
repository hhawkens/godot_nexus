module public App.Utilities.Tests.FileDataTests

open System.IO
open App.Utilities
open NUnit.Framework

[<Literal>]
let private FilePath = "TestData/Test.txt"

[<Literal>]
let private NewFile = "TestData/New.txt"

let private RemoveNewFile() =
    match File.Exists NewFile with
        | true -> File.Delete NewFile
        | false -> ()

[<SetUp>]
let public SetUp() = RemoveNewFile()

[<TearDown>]
let public TearDown() = RemoveNewFile()

[<Test>]
let public ``TryFind Will Not Find File That Does Not Exist`` () =
    match FileData.tryFind "ImaginaryFile.txt" with
    | Some _ -> Assert.Fail("Was not supposed to find this file")
    | None -> Assert.Pass()

[<Test>]
let public ``TryFind Will Find Existing File`` () =
    match FileData.tryFind FilePath with
    | Some _ -> Assert.Pass()
    | None -> Assert.Fail("Could not find test file.")

[<TestCase("")>]
let public ``TryCreate Fails With Invalid Input`` input =
    match FileData.tryCreate input with
    | Ok _ -> Assert.Fail("Was not supposed to be able to create file")
    | Error _ -> Assert.Pass()

[<Test>]
let public ``TryCreate With Valid Path Creates New File`` () =
    let newFile = Path.Combine("NewFolder", "NewFile.test")
    Assert.That(File.Exists newFile, Is.False)
    match FileData.tryCreate newFile with
    | Ok _ -> Assert.That(File.Exists newFile, Is.True)
    | Error _ -> Assert.Fail("Was not able to create file")
    (DirectoryData.tryFind "NewFolder" |> unwrap).TryDelete() |> unwrap

[<Test>]
let public ``TryCreate With Existing File Preserves Content`` () =
    Assert.That(File.Exists FilePath, Is.True)
    match FileData.tryCreate FilePath with
    | Ok file ->
        Assert.That(File.ReadAllText(file.FullPath), Is.EqualTo("Test Text Here!"))
    | Error _ -> Assert.Fail("Was not able to create file")

[<Test>]
let public ``File Properties Are Correct`` () =
    match FileData.tryFind FilePath with
    | Some file ->
        Assert.That(file.FullPath, Does.Contain("/TestData/Test.txt"))
        Assert.That(file.Name, Is.EqualTo("Test.txt"))
        Assert.That(file.Extension, Is.EqualTo(".txt"))
    | None -> Assert.Fail("Could not find test file.")

[<Test>]
let public ``Still Exists Shows Correct File State`` () =
    match FileData.tryCreate NewFile with
    | Ok file ->
        Assert.That(file.StillExists)
        RemoveNewFile()
        Assert.That(file.StillExists, Is.False)
    | Error _ -> Assert.Fail("Was not able to create file")

[<Test>]
let public ``Trying To Delete Existing File Succeeds`` () =
    let file = "hello.bin"
    let sut = FileData.tryCreate file |> unwrap
    Assert.That(File.Exists(file))

    match sut.TryDelete() with
    | Ok _ -> Assert.That(File.Exists(file), Is.False)
    | Error err -> Assert.Fail(err)

    File.Delete file

[<Test>]
let public ``Trying To Delete Not Existing File Fails`` () =
    let file = "hello.bin"
    let sut = FileData.tryCreate file |> unwrap
    File.Delete file
    Assert.That(File.Exists(file), Is.False)

    match sut.TryDelete() with
    | Ok _ -> Assert.Fail("Deleting was not supposed to work!")
    | Error _ -> Assert.Pass()

[<Test>]
let public ``Empty File Data Has Empty Properties`` () =
    let sut = FileData.Empty
    Assert.That(sut.FullPath, Is.EqualTo(""))
    Assert.That(sut.Name, Is.EqualTo(""))
    Assert.That(sut.Extension, Is.EqualTo(""))
    Assert.That(sut.StillExists, Is.False)
