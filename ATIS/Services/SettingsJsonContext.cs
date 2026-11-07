using System.Text.Json.Serialization;

namespace ATIS.Services;

[JsonSerializable(typeof(DbServerSettings.Data))]
[JsonSerializable(typeof(bool))]
internal partial class SettingsJsonContext : JsonSerializerContext
{
}
