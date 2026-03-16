using System.Security.Cryptography;
using System.Text;

namespace simur_backend.Utilities
{
    public class StringUtilities
    {
        public static string ReplaceAccents(string text)
        {
            List<char> withAccents = ['ç', 'Ç', 'á', 'é', 'í', 'ó', 'ú', 'ý', 'Á', 'É', 'Í', 'Ó', 'Ú', 'Ý', 'à', 'è', 'ì', 'ò', 'ù', 'À', 'È', 'Ì', 'Ò', 'Ù', 'ã', 'õ', 'ñ', 'ä', 'ë', 'ï', 'ö', 'ü', 'ÿ', 'Ä', 'Ë', 'Ï', 'Ö', 'Ü', 'Ã', 'Õ', 'Ñ', 'â', 'ê', 'î', 'ô', 'û', 'Â', 'Ê', 'Î', 'Ô', 'Û', '&'];
            List<char> withoutAccents = ['c', 'C', 'a', 'e', 'i', 'o', 'u', 'y', 'A', 'E', 'I', 'O', 'U', 'Y', 'a', 'e', 'i', 'o', 'u', 'A', 'E', 'I', 'O', 'U', 'a', 'o', 'n', 'a', 'e', 'i', 'o', 'u', 'y', 'A', 'E', 'I', 'O', 'U', 'A', 'O', 'N', 'a', 'e', 'i', 'o', 'u', 'A', 'E', 'I', 'O', 'U', 'E'];

            for (int i = 0; i < withAccents.Count; i++)
            {
                if(text.Contains<char>(withAccents[i]))
                    text = text.Replace(withAccents[i], withoutAccents[i]);
            }
            return text;
        }

        public static string GetAlphanumericCode(int outputLentgh)
        {
            string Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            StringBuilder sb = new StringBuilder();
            Random random = new();

            for (int i = 0; i < outputLentgh; i++)
            {
                sb.Append(Chars[random.Next(outputLentgh-1)]);
            }
            return sb.ToString();
        }
    }
}
