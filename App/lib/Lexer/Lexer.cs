﻿using System.Text.RegularExpressions;

namespace hulk;

public class Lexer
{

    private string code;
    private List<string> diagnostics = new();    

    private readonly List<Tuple<Regex, TokenType>> regexToTokenType = new()
    {
        Tuple.Create(new Regex(@"\("), TokenType.LParen),
        Tuple.Create(new Regex(@"\)"), TokenType.RParen),
        Tuple.Create(new Regex(@"\d+"), TokenType.Number),
        Tuple.Create(new Regex("true|false"), TokenType.Boolean),
        Tuple.Create(new Regex("="), TokenType.Asignation),
        Tuple.Create(new Regex(@"\+"), TokenType.Plus),
        Tuple.Create(new Regex(@"\-"), TokenType.Minus),
        Tuple.Create(new Regex(@"\*"), TokenType.Mult),
        Tuple.Create(new Regex(@"\/"), TokenType.Div),
        Tuple.Create(new Regex("print"), TokenType.Keyword),
        Tuple.Create(new Regex("let"), TokenType.Keyword),
        Tuple.Create(new Regex("in"), TokenType.Keyword),
        Tuple.Create(new Regex("\"(.*?)\""), TokenType.String),
        Tuple.Create(new Regex(@"\b[a-zA-Z_]\w*\b"), TokenType.Identificator),
        Tuple.Create(new Regex(";"), TokenType.EOL)
    };

    public Lexer(string code)
    {
        this.code = code;
    }

    public IEnumerable<string> Diagnostics => diagnostics;

    public List<Token> Tokenize()
    {
        List<Token> tokens = new();
        int currentIndex = 0;

        while (currentIndex < code.Length)
        {
            bool matchFound = false;
            int len = 0;

            foreach (var regexToken in regexToTokenType)
            {
                var match = regexToken.Item1.Match(code, currentIndex);
                if (match.Success && match.Index == currentIndex)
                {
                    if (!int.TryParse(match.Value, out int value) && regexToken.Item2 == TokenType.Number)
                    {
                        diagnostics.Add($"The number {match.Value} isn't valid Int32");
                    }

                    tokens.Add(new Token(regexToken.Item2,match.Index,code.Substring(match.Index,match.Length), value));
                    len = match.Length;
                    currentIndex += len;
                    matchFound = true;
                    break;
                }
            }

            if (currentIndex < code.Length && code[currentIndex] == ' ')
            {
                currentIndex++;
            }

            if (!matchFound){
                diagnostics.Add($"ERROR: bad character input: {code.Substring(currentIndex-1, len)}");

                tokens.Add(new Token(TokenType.Error, currentIndex++, code.Substring(currentIndex-1, len),null));
            }
        }

        if (tokens[tokens.Count-1].Type != TokenType.EOL)
        {
            tokens.Add(new Token(TokenType.Error, currentIndex, null, null));
        }

        return tokens;
    }
}