using System.Buffers.Text;

namespace Mir_Utilities.MirApi;

public class SoundApiSchema
{
    public struct GetSoundByGuidSnapshot
    {
        public string Name;
        public string Guid;
        public string Sound;
        public TimeSpan Length;
        public string Note;
        public int Volume;
        public string OwnerId;

    }

    public struct GetSoundsSnapshot
    {
        public string Guid;
        public string Name;
    }
}