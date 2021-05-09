module internal App.Core.Plugins.AppStateSerializer

open System.IO
open App.Core.Domain
open App.Utilities
open Newtonsoft.Json
open Newtonsoft.Json.Bson

let private defaultAppState = {
    Engines = Set.empty
    EngineInstalls = ActiveSet.createEmpty ()
    Projects = Set.empty
}
let private serializer =
    let jSer = JsonSerializer()
    jSer.ReferenceLoopHandling <- ReferenceLoopHandling.Ignore
    jSer.Converters.Add (DirectoryDataConverter())
    jSer.Converters.Add (EngineConverter())
    jSer.Converters.Add (EngineInstallConverter())
    jSer.Converters.Add (EngineInstallsConverter())
    jSer

let internal saveUnsafe file appState =
    use fileStream = new FileStream(file, FileMode.Truncate)
    use bsonWriter = new BsonDataWriter(fileStream)
    serializer.Serialize(bsonWriter, appState)

let internal loadUnsafe file =
    if not <| File.Exists file || FileInfo(file).Length = 0L then
        defaultAppState
    else
        use fileStream = new FileStream(file, FileMode.Open)
        use bsonReader = new BsonDataReader(fileStream)
        serializer.Deserialize<AppState>(bsonReader)
