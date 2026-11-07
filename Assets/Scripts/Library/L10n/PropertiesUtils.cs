using System;
using System.Collections.Generic;
using System.IO;
using JetBrains.Annotations;

namespace Library.L10n
{
    public static class PropertiesUtils
    {
        private const int None = 0, Slash = 1, Unicode = 2, Continue = 3, KeyDone = 4, Ignore = 5;

        private const string LineSeparator = "\n";

        /** Adds to the specified {@code ObjectMap} the key/value pairs loaded from the Stream in a simple line-oriented format
        * compatible with <code>java.util.Properties</code>.
        * <p>
        * The input stream remains open after this method returns.
        *
        * @param properties the map to be filled.
        * @param reader the input character stream reader.
        * @throws IOException if an error occurred when reading from the input stream.
        * @throws ArgumentException if a malformed Unicode escape appears in the input. */
        public static void Load([NotNull] Dictionary<string, string> properties, [NotNull] Stream stream)
        {
            int mode = None, unicode = 0, count = 0;

            char nextChar;
            var buf = new char[40];
            int offset = 0, keyLength = -1, intVal;
            var firstChar = true;

            var br = new StreamReader(stream);

            while (true)
            {
                intVal = br.Read();

                if (intVal == -1) break;
                nextChar = (char) intVal;

                if (offset == buf.Length)
                {
                    var newBuf = new char[buf.Length * 2];
                    Array.Copy(buf, 0, newBuf, 0, offset);
                    buf = newBuf;
                }

                if (mode == Unicode)
                {
                    var digit = nextChar - '0';

                    if (digit >= 0)
                    {
                        unicode = (unicode << 4) + digit;

                        if (++count < 4) continue;
                    }
                    else if (count <= 4)
                    {
                        throw new ArgumentException("Invalid Unicode sequence: illegal character");
                    }

                    mode = None;
                    buf[offset++] = (char) unicode;

                    if (nextChar != '\n') continue;
                }

                if (mode == Slash)
                {
                    mode = None;

                    switch (nextChar)
                    {
                        case '\r':
                            mode = Continue; // Look for a following \n
                            continue;
                        case '\n':
                            mode = Ignore; // Ignore whitespace on the next line
                            continue;
                        case 'b':
                            nextChar = '\b';
                            break;
                        case 'f':
                            nextChar = '\f';
                            break;
                        case 'n':
                            nextChar = '\n';
                            break;
                        case 'r':
                            nextChar = '\r';
                            break;
                        case 't':
                            nextChar = '\t';
                            break;
                        case 'u':
                            mode = Unicode;
                            unicode = count = 0;
                            continue;
                    }
                }
                else
                {
                    switch (nextChar)
                    {
                        case '#':
                        case '!':

                            if (firstChar)
                            {
                                while (true)
                                {
                                    intVal = br.Read();

                                    if (intVal == -1) break;
                                    nextChar = (char) intVal;

                                    if (nextChar == '\r' || nextChar == '\n') break;
                                }

                                continue;
                            }

                            break;
                        case '\n':

                            if (mode == Continue)
                            {
                                // Part of a \r\n sequence
                                mode = Ignore; // Ignore whitespace on the next line
                                continue;
                            }

                            mode = None;
                            firstChar = true;

                            if (offset > 0 || offset == 0 && keyLength == 0)
                            {
                                if (keyLength == -1) keyLength = offset;
                                var temp = new string(buf, 0, offset);
                                properties.Add(temp.Substring(0, keyLength), temp.Substring(keyLength));
                            }

                            keyLength = -1;
                            offset = 0;
                            continue;
                        case '\r':
                            mode = None;
                            firstChar = true;

                            if (offset > 0 || offset == 0 && keyLength == 0)
                            {
                                if (keyLength == -1) keyLength = offset;
                                var temp = new string(buf, 0, offset);
                                properties.Add(temp.Substring(0, keyLength), temp.Substring(keyLength));
                            }

                            keyLength = -1;
                            offset = 0;
                            continue;
                        case '\\':

                            if (mode == KeyDone) keyLength = offset;
                            mode = Slash;
                            continue;
                        case ':':
                        case '=':

                            if (keyLength == -1)
                            {
                                // if parsing the key
                                mode = None;
                                keyLength = offset;
                                continue;
                            }

                            break;
                    }

                    // if (Character.isWhitespace(nextChar)) { <-- not supported by GWT; replaced with isSpace.
                    if (char.IsWhiteSpace(nextChar))
                    {
                        if (mode == Continue) mode = Ignore;

                        // if key length == 0 or value length == 0
                        if (offset == 0 || offset == keyLength || mode == Ignore) continue;

                        if (keyLength == -1)
                        {
                            // if parsing the key
                            mode = KeyDone;
                            continue;
                        }
                    }

                    if (mode == Ignore || mode == Continue) mode = None;
                }

                firstChar = false;

                if (mode == KeyDone)
                {
                    keyLength = offset;
                    mode = None;
                }

                buf[offset++] = nextChar;
            }

            if (mode == Unicode && count <= 4)
                throw new ArgumentException("Invalid Unicode sequence: expected format \\uxxxx");

            if (keyLength == -1 && offset > 0) keyLength = offset;

            if (keyLength >= 0)
            {
                var temp = new string(buf, 0, offset);
                var key = temp.Substring(0, keyLength);
                var value = temp.Substring(keyLength);

                if (mode == Slash) value += "\u0000";
                properties.Add(key, value);
            }
        }
    }
}