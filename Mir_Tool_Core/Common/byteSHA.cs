using System.Security.Cryptography;
using System.Text;

namespace Mir_Utilities.Common;

public class byteSHA
{
    public static string SHA256Byte(byte[] data)
    {
        using (SHA256 sha256 = SHA256.Create())
        {
            byte[] hash = sha256.ComputeHash(data);
            return Convert.ToBase64String(hash);
        }
    }}