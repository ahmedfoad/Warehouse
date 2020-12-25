using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace Warehouse.GlobalClass
{
    public class ConvertToEasternArabicNumerals
    {
        public string ConvertAR(string input)
        {
            try
            {
                System.Text.UTF8Encoding utf8Encoder = new UTF8Encoding();
                System.Text.Decoder utf8Decoder = utf8Encoder.GetDecoder();
                System.Text.StringBuilder convertedChars = new System.Text.StringBuilder();
                char[] convertedChar = new char[1];
                byte[] bytes = new byte[] { 217, 160 };
                char[] inputCharArray = input.ToCharArray();
               // var EnglishCharactersCounter = Regex.Matches(input, @"[a-zA-Z]").Count;

                bool onlyEnglishCharacters = Regex.IsMatch(input, @"[a-zA-Z]");
                if (onlyEnglishCharacters)
                {
                    return input;
                }
                foreach (char c in inputCharArray)
                {
                    if (char.IsDigit(c))
                    {
                        bytes[1] = Convert.ToByte(160 + char.GetNumericValue(c));
                        utf8Decoder.GetChars(bytes, 0, 2, convertedChar, 0);
                        convertedChars.Append(convertedChar[0]);
                    }
                    else
                    {
                        convertedChars.Append(c);
                    }
                }
                return convertedChars.ToString();
            }
            catch (Exception)
            {
                return "";
            }

        }

        public string GetDecimal_only(decimal number)
        {
            string input_decimal_number = number.ToString();
            var regex = new System.Text.RegularExpressions.Regex("(?<=[\\.])[0-9]+");
            if (regex.IsMatch(input_decimal_number))
            {
                string decimal_places = regex.Match(input_decimal_number).Value;
                return ConvertAR(decimal_places.ToString());
            }
            return ConvertAR("0");
        }
    }
}