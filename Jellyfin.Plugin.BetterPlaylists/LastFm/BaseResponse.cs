namespace Jellyfin.Plugin.BetterPlaylists.LastFm;

using System.Runtime.Serialization;

[DataContract]
public class BaseResponse
{
    [DataMember(Name = "message")] public string Message { get; set; }

    [DataMember(Name = "error")] public int ErrorCode { get; set; }

    public bool IsError()
    {
        return ErrorCode > 0;
    }
}