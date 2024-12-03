using System.Buffers.Text;

namespace Mir_Utilities;

public class SoundApiSchema
{
    public struct GetSoundByGuidSnapshot
    {
        public String Name;
        public String Guid;
        public String Sound;
        public TimeSpan Length;
        public String Note;
        public int Volume;
        public String OwnerId;

    }

    public struct GetSoundsSnapshot
    {
        public String Guid;
        public String Name;
    }
}