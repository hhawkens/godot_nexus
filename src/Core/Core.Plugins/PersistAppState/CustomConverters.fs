namespace App.Core.Plugins

open App.Core.Domain
open App.Utilities
open Newtonsoft.Json

type internal DirectoryDataConverter () =
    inherit JsonConverter<DirectoryData>()

    override this.WriteJson (writer: JsonWriter, value: DirectoryData, _: JsonSerializer): unit =
        writer.WriteValue value.FullPath

    override this.ReadJson (reader, _, _, _, _) =
        let fullPath = reader.Value:?>string
        DirectoryData.TryCreate fullPath |> unwrap


type internal EngineConverter () =
    inherit JsonConverter<EngineOnline>()

    override this.WriteJson (writer: JsonWriter, value: EngineOnline, serializer: JsonSerializer): unit =
        let e = SerializableEngine(value.Data, value.Url, value.FileSize)
        serializer.Serialize(writer, e)

    override this.ReadJson (reader, _, _, _, serializer) =
        let e = serializer.Deserialize<SerializableEngine>(reader)
        EngineOnline.New e.Data e.Url e.FileSize


type internal EngineInstallConverter () =
    inherit JsonConverter<EngineInstall>()

    override this.WriteJson (writer: JsonWriter, value: EngineInstall, serializer: JsonSerializer): unit =
        let e = SerializableEngineInstall(value.Data, value.Path)
        serializer.Serialize(writer, e)

    override this.ReadJson (reader, _, _, _, serializer) =
        let e = serializer.Deserialize<SerializableEngineInstall>(reader)
        EngineInstall.New e.Data e.Path


type internal EngineInstallsConverter () =
    inherit JsonConverter<EngineInstalls>()

    override this.WriteJson (writer: JsonWriter, value: EngineInstalls, serializer: JsonSerializer): unit =
        let ei = SerializableEngineInstalls(value.Set, value.Active)
        serializer.Serialize(writer, ei)

    override this.ReadJson (reader, _, _, _, serializer) =
        let ei = serializer.Deserialize<SerializableEngineInstalls>(reader)
        match ei.Active with
        | Some active -> ActiveSet.createFrom ei.Set active |> unwrap
        | None -> ActiveSet.createEmpty ()
