using System.Collections.Generic;

namespace LaTeXTableGenerator.LaTeX
{
    public class LaTeXTextEscape
    {
        public string Escape(string text)
        {
            if (text == null) return string.Empty;

            var dictionary = new Dictionary<string, string>()
            {
                {@"\", @"\textbackslash" },
                {"{", @"\{" },
                {"}", @"\}" },
                {@"\textbackslash", @"\textbackslash{}" }, // Ugly workaround for preventing double replacement of { and }
                {"%", @"\%" },
                {"$", @"\$" },
                {"_", @"\_" },
                {"#", @"\#" },
                {"&", @"\&" },
                {"§", @"\S{}" },
                {"\n", @"\newline{}" },
                {"<", @"\textless{} " },
                {">", @"\textgreater{} " },
                {"|", @"\textbar{} " },
            };

            text = text.Replace("\r", "");

            foreach (var escapePair in dictionary)
            {
                text = text.Replace(escapePair.Key, escapePair.Value);
            }

            return text;
        }
    }
}
