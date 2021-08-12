using System.Collections;
using System.Text;
using System.Collections.Generic;
using UnityEngine;


// Meant to parse a textasset into a list of lists of strings from a CSV!
public static class ParseCSVUtil
{
    public static List<List<string>> ParseData(TextAsset file)
    {
        string[] lines = file.text.Split("\n"[0]);

        List<List<string>> parsedData = new List<List<string>>();

        // Go through each line
        foreach(string line in lines)
        {
            List<string> lineStrings = new List<string>();

            bool insideQuote = false;
            StringBuilder sb = new StringBuilder();
            int charIndex = 0;
            // For each line, go through all the characters, keeping an eye out for commas and quotes.
            while (charIndex < line.Length)
            {
                char c = line[charIndex];
                // If there's a quote, it's either a quoted section because the text itself has commas or quotes, or there's two quotes in a row to add in a quote.
                if (c == '"')
                {
                    if (insideQuote)
                    {
                        if (charIndex < line.Length - 1)
                        {
                            char nextChar = line[charIndex + 1];
                            if (nextChar == '"')
                            {
                                sb.Append('"');
                                charIndex++;
                            }
                            else
                            {
                                insideQuote = false;
                            }
                        }
                        else
                        {
                            insideQuote = false;
                        }
                    }
                    else
                    {
                        insideQuote = true;
                    }
                    charIndex++;
                    continue;
                }
                // If we find a comma and we're not inside a quote, it's time for a new string!
                else if (c == ',' && !insideQuote)
                {
                    lineStrings.Add(sb.ToString());
                    sb.Length = 0;
                    charIndex++;
                    continue;
                }
                sb.Append(c);
                charIndex++;
            }
            lineStrings.Add(sb.ToString());

            parsedData.Add(lineStrings);
        }

        return parsedData;
    }
}
