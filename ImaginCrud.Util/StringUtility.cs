using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ImaginCrud.Util
{
    public static class StringUtility
    {
        public static string GetStringInBetween(this string strSource, string strBegin, string strEnd, bool includeBegin, bool includeEnd)
        {
            string[] result = { string.Empty, string.Empty };
            int iIndexOfBegin = strSource.IndexOf(strBegin);

            if (iIndexOfBegin != -1)
            {
                // include the Begin string if desired 
                if (includeBegin)
                    iIndexOfBegin -= strBegin.Length;

                strSource = strSource.Substring(iIndexOfBegin + strBegin.Length);

                int iEnd = strSource.IndexOf(strEnd);
                if (iEnd != -1)
                {
                    // include the End string if desired 
                    if (includeEnd)
                        iEnd += strEnd.Length;
                    result[0] = strSource.Substring(0, iEnd);
                    // advance beyond this segment 
                    if (iEnd + strEnd.Length < strSource.Length)
                        result[1] = strSource.Substring(iEnd + strEnd.Length);
                }
            }
            else
                // stay where we are 
                result[1] = strSource;
            return result[0];
        }

        public static string CustomNormalize(this string text)
        {
            string normalizedText = text.Normalize(NormalizationForm.FormD);
            //coincide todo lo que no sean letras y números ascii o espacio
            //y lo reemplazamos por una cadena vacía.
            Regex reg = new Regex("[^a-zA-Z0-9 ]");
            return reg.Replace(normalizedText, "").Replace(" ","");
        }
    }
}
