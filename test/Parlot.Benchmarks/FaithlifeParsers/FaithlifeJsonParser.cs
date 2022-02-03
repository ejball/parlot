using Parlot.Tests.Json;
using System.Collections.Generic;
using System.Linq;
using Faithlife.Parsing;

namespace Parlot.Benchmarks.FaithlifeParsers
{
    public static class FaithlifeJsonParser
    {
        private static readonly IParser<char> LBrace = Parser.Char('{');
        private static readonly IParser<char> RBrace = Parser.Char('}');
        private static readonly IParser<char> LBracket = Parser.Char('[');
        private static readonly IParser<char> RBracket = Parser.Char(']');
        private static readonly IParser<char> Quote = Parser.Char('"');
        private static readonly IParser<char> Colon = Parser.Char(':');
        private static readonly IParser<char> ColonWhitespace =
            Colon.Trim();
        private static readonly IParser<char> Comma = Parser.Char(',');

        private static readonly IParser<string> String =
            Parser.AnyCharExcept('"').Many().Capture()
                .Bracketed(Quote, Quote);
        private static readonly IParser<IJson> JsonString =
            String.Select(s => new JsonString(s));

        private static readonly IParser<IJson> Json =
            JsonString.Or(Parser.Ref(() => JsonArray)).Or(Parser.Ref(() => JsonObject));

        private static readonly IParser<IJson> JsonArray =
            Json.Trim()
                .Delimited(Comma)
                .Bracketed(LBracket, RBracket)
                .Select(els => new JsonArray(els.ToArray()));

        private static readonly IParser<KeyValuePair<string, IJson>> JsonMember =
            String
                .FollowedBy(ColonWhitespace)
                .Then(Json, (name, val) => new KeyValuePair<string, IJson>(name, val));

        private static readonly IParser<IJson> JsonObject =
            JsonMember.Trim()
                .Delimited(Comma)
                .Bracketed(LBrace, RBrace)
                .Select(kvps => new JsonObject(new Dictionary<string, IJson>(kvps)));

        public static IJson Parse(string input) => Json.Parse(input);
    }
}
