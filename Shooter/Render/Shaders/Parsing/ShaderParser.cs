using System.Text;

namespace Shooter.Render.Shaders.Parsing;

/**
 * Basic shader parser for getting information like attribute names, types, and counts.
 * Probably not to spec, but I did my best :P
 */
public class ShaderParser(string source)
{
    private static readonly Dictionary<string, GlslTokenType> Keywords = new()
    {
        ["attribute"] = GlslTokenType.Attribute,
        ["const"] = GlslTokenType.Const,
        ["uniform"] = GlslTokenType.Uniform,
        ["varying"] = GlslTokenType.Varying,
        ["layout"] = GlslTokenType.Layout,
        ["centroid"] = GlslTokenType.Centroid,
        ["flat"] = GlslTokenType.Flat,
        ["smooth"] = GlslTokenType.Smooth,
        ["noperspective"] = GlslTokenType.Noperspective,
        ["break"] = GlslTokenType.Break,
        ["continue"] = GlslTokenType.Continue,
        ["do"] = GlslTokenType.Do,
        ["for"] = GlslTokenType.For,
        ["while"] = GlslTokenType.While,
        ["switch"] = GlslTokenType.Switch,
        ["case"] = GlslTokenType.Case,
        ["default"] = GlslTokenType.Default,
        ["if"] = GlslTokenType.If,
        ["else"] = GlslTokenType.Else,
        ["in"] = GlslTokenType.In,
        ["out"] = GlslTokenType.Out,
        ["inout"] = GlslTokenType.Inout,
        ["float"] = GlslTokenType.Float,
        ["int"] = GlslTokenType.Int,
        ["void"] = GlslTokenType.Void,
        ["bool"] = GlslTokenType.Bool,
        ["true"] = GlslTokenType.True,
        ["false"] = GlslTokenType.False,
        ["invariant"] = GlslTokenType.Invariant,
        ["discard"] = GlslTokenType.Discard,
        ["return"] = GlslTokenType.Return,
        ["mat2"] = GlslTokenType.Mat2,
        ["mat3"] = GlslTokenType.Mat3,
        ["mat4"] = GlslTokenType.Mat4,
        ["mat2x2"] = GlslTokenType.Mat2x2,
        ["mat2x3"] = GlslTokenType.Mat2x3,
        ["mat2x4"] = GlslTokenType.Mat2x4,
        ["mat3x2"] = GlslTokenType.Mat3x2,
        ["mat3x3"] = GlslTokenType.Mat3x3,
        ["mat3x4"] = GlslTokenType.Mat3x4,
        ["mat4x2"] = GlslTokenType.Mat4x2,
        ["mat4x3"] = GlslTokenType.Mat4x3,
        ["mat4x4"] = GlslTokenType.Mat4x4,
        ["vec2"] = GlslTokenType.Vec2,
        ["vec3"] = GlslTokenType.Vec3,
        ["vec4"] = GlslTokenType.Vec4,
        ["ivec2"] = GlslTokenType.Ivec2,
        ["ivec3"] = GlslTokenType.Ivec3,
        ["ivec4"] = GlslTokenType.Ivec4,
        ["bvec2"] = GlslTokenType.Bvec2,
        ["bvec3"] = GlslTokenType.Bvec3,
        ["bvec4"] = GlslTokenType.Bvec4,
        ["uint"] = GlslTokenType.Uint,
        ["uvec2"] = GlslTokenType.Uvec2,
        ["uvec3"] = GlslTokenType.Uvec3,
        ["uvec4"] = GlslTokenType.Uvec4,
        ["lowp"] = GlslTokenType.Lowp,
        ["mediump"] = GlslTokenType.Mediump,
        ["highp"] = GlslTokenType.Highp,
        ["precision"] = GlslTokenType.Precision,
        ["sampler1D"] = GlslTokenType.Sampler1D,
        ["sampler2D"] = GlslTokenType.Sampler2D,
        ["sampler3D"] = GlslTokenType.Sampler3D,
        ["samplerCube"] = GlslTokenType.SamplerCube,
        ["sampler1DShadow"] = GlslTokenType.Sampler1DShadow,
        ["sampler2DShadow"] = GlslTokenType.Sampler2DShadow,
        ["samplerCubeShadow"] = GlslTokenType.SamplerCubeShadow,
        ["sampler1DArray"] = GlslTokenType.Sampler1DArray,
        ["sampler2DArray"] = GlslTokenType.Sampler2DArray,
        ["sampler1DArrayShadow"] = GlslTokenType.Sampler1DArrayShadow,
        ["sampler2DArrayShadow"] = GlslTokenType.Sampler2DArrayShadow,
        ["isampler1D"] = GlslTokenType.Isampler1D,
        ["isampler2D"] = GlslTokenType.Isampler2D,
        ["isampler3D"] = GlslTokenType.Isampler3D,
        ["isamplerCube"] = GlslTokenType.IsamplerCube,
        ["isampler1DArray"] = GlslTokenType.Isampler1DArray,
        ["isampler2DArray"] = GlslTokenType.Isampler2DArray,
        ["usampler1D"] = GlslTokenType.Usampler1D,
        ["usampler2D"] = GlslTokenType.Usampler2D,
        ["usampler3D"] = GlslTokenType.Usampler3D,
        ["usamplerCube"] = GlslTokenType.UsamplerCube,
        ["usampler1DArray"] = GlslTokenType.Usampler1DArray,
        ["usampler2DArray"] = GlslTokenType.Usampler2DArray,
        ["sampler2DRect"] = GlslTokenType.Sampler2DRect,
        ["sampler2DRectShadow"] = GlslTokenType.Sampler2DRectShadow,
        ["isampler2DRect"] = GlslTokenType.Isampler2DRect,
        ["usampler2DRect"] = GlslTokenType.Usampler2DRect,
        ["samplerBuffer"] = GlslTokenType.SamplerBuffer,
        ["isamplerBuffer"] = GlslTokenType.IsamplerBuffer,
        ["usamplerBuffer"] = GlslTokenType.UsamplerBuffer,
        ["sampler2DMS"] = GlslTokenType.Sampler2DMS,
        ["isampler2DMS"] = GlslTokenType.Isampler2DMS,
        ["usampler2DMS"] = GlslTokenType.Usampler2DMS,
        ["sampler2DMSArray"] = GlslTokenType.Sampler2DMSArray,
        ["isampler2DMSArray"] = GlslTokenType.Isampler2DMSArray,
        ["usampler2DMSArray"] = GlslTokenType.Usampler2DMSArray,
        ["struct"] = GlslTokenType.Struct
    };

    private int _idx = 0;
    private readonly char[] _source = source.ToCharArray();

    public List<GlslToken> Parse()
    {
        List<GlslToken> tokens = this.Tokenize();
        return tokens;
    }

    private List<GlslToken> Tokenize()
    {
        List<GlslToken> tokens = [];
        while (this._idx < this._source.Length)
        {
            this.SkipWhitespace();
            char currentChar = this._source[this._idx];

            switch (currentChar)
            {
                case >= 'a' and <= 'z' or >= 'A' and <= 'Z' or '_':
                    tokens.Add(this.Identifier());
                    break;
                case >= '0' and <= '9':
                    tokens.Add(this.Number());
                    break;
                case '.':
                    if (Char.IsDigit(this.Peek()))
                    {
                        tokens.Add(this.Number());
                    }
                    else
                    {
                        tokens.Add(new(GlslTokenType.Dot, "."));
                    }

                    break;
                case '+':
                {
                    tokens.Add(
                        this.WithOptionalPostfix(
                            '+',
                            GlslTokenType.Plus,
                            ('+', GlslTokenType.PlusPlus),
                            ('=', GlslTokenType.PlusEquals)
                        )
                    );
                }
                    break;
                case '-':
                {
                    tokens.Add(
                        this.WithOptionalPostfix(
                            '-',
                            GlslTokenType.Minus,
                            ('-', GlslTokenType.MinusMinus),
                            ('=', GlslTokenType.MinusEquals)
                        )
                    );
                }
                    break;
                case '/':
                    // Special handling because comments
                    tokens.Add(
                        this.Peek(1) switch
                        {
                            '*' =>
                                (this.MultilineComment()),
                            '/' =>
                                (this.Comment()),
                            '=' =>
                                new(GlslTokenType.SlashEquals, "/="),
                            _ =>
                                new(GlslTokenType.Slash, "/")
                        }
                    );

                    break;
                case '*':
                    tokens.Add(
                        this.WithOptionalPostfix(
                            '*',
                            GlslTokenType.Star,
                            ('=', GlslTokenType.StarEquals)
                        )
                    );
                    break;
                case '%':
                    tokens.Add(
                        this.WithOptionalPostfix(
                            '%',
                            GlslTokenType.Percent,
                            ('=', GlslTokenType.PercentEquals)
                        )
                    );
                    break;
                case '<':
                    // special handling because of <<=
                    tokens.Add(
                        this.Peek(1) switch
                        {
                            '=' =>
                                new(GlslTokenType.LessEquals, "<="),
                            '<' when this.Peek(1) is '=' =>
                                new(GlslTokenType.DoubleLessEquals, "<<="),
                            '<' =>
                                new(GlslTokenType.DoubleLess, "<<"),
                            _ =>
                                new(GlslTokenType.Less, "<")
                        }
                    );

                    break;
                case '>':
                    // special handling because of >>=
                    tokens.Add(
                        this.Peek(1) switch
                        {
                            '=' => new(GlslTokenType.GreaterEquals, ">="),
                            '>' when this.Peek(1) is '=' => new(GlslTokenType.DoubleGreaterEquals, ">>="),
                            '>' => new(GlslTokenType.DoubleGreater, ">>"),
                            _ => new(GlslTokenType.Greater, ">")
                        }
                    );
                    break;
                case '[':
                    tokens.Add(new(GlslTokenType.OpenBracket, "["));
                    break;
                case ']':
                    tokens.Add(new(GlslTokenType.CloseBracket, "]"));
                    break;
                case '(':
                    tokens.Add(new(GlslTokenType.OpenParen, "("));
                    break;
                case ')':
                    tokens.Add(new(GlslTokenType.CloseParen, ")"));
                    break;
                case '{':
                    tokens.Add(new(GlslTokenType.OpenCurly, "{"));
                    break;
                case '}':
                    tokens.Add(new(GlslTokenType.CloseCurly, "}"));
                    break;
                case '^':
                    tokens.Add(
                        this.WithOptionalPostfix(
                            '^',
                            GlslTokenType.Caret,
                            ('^', GlslTokenType.DoubleCaret),
                            ('=', GlslTokenType.CaretEquals)
                        )
                    );
                    break;
                case '|':
                    tokens.Add(
                        this.WithOptionalPostfix(
                            '|',
                            GlslTokenType.Pipe,
                            ('|', GlslTokenType.DoublePipe),
                            ('=', GlslTokenType.PipeEquals)
                        )
                    );
                    break;
                case '&':
                    tokens.Add(
                        this.WithOptionalPostfix(
                            '&',
                            GlslTokenType.Ampersand,
                            ('&', GlslTokenType.DoubleAmpersand),
                            ('=', GlslTokenType.AmpersandEquals)
                        )
                    );
                    break;
                case '~':
                    tokens.Add(new(GlslTokenType.Tilde, "~"));
                    break;
                case '=':
                    tokens.Add(
                        this.WithOptionalPostfix(
                            '=',
                            GlslTokenType.Equals,
                            ('=', GlslTokenType.DoubleEquals)
                        )
                    );
                    break;
                case '!':
                    tokens.Add(
                        this.WithOptionalPostfix(
                            '!',
                            GlslTokenType.Exclamation,
                            ('=', GlslTokenType.ExclamationEquals)
                        )
                    );
                    break;
                case ':':
                    tokens.Add(new(GlslTokenType.Colon, ":"));
                    break;
                case ';':
                    tokens.Add(new(GlslTokenType.Semicolon, ";"));
                    break;
                case ',':
                    tokens.Add(new(GlslTokenType.Comma, ","));
                    break;
                case '?':
                    tokens.Add(new(GlslTokenType.Question, "?"));
                    break;
                case ' ':
                    break;
                case '#':
                    tokens.Add(new(GlslTokenType.Hash, "#"));
                    break;
                default:
                    throw new("Invalid Source Character `" + currentChar + "`");
            }

            this._idx++;
        }

        return tokens;
    }

    private GlslToken WithOptionalPostfix(char str, GlslTokenType baseToken, params (char, GlslTokenType)[] postfixes)
    {
        char peek = this.Peek(1);

        foreach ((char postfix, GlslTokenType type) in postfixes)
        {
            if (peek == postfix)
            {
                return new(type, new(str, postfix));
            }
        }


        return new(baseToken, str.ToString());
    }

    private GlslToken Number()
    {
        if (this.Peek() == '0')
        {
            this.Advance();
            if (this._idx < this._source.Length && (this.Peek() == 'x' || this.Peek() == 'X'))
                return this.ParseHexadecimal();
            return this.ParseOctal();
        }

        if (Char.IsDigit(this.Peek()))
        {
            return this.ParseDecimalOrFloat();
        }

        if (this.Peek() == '.')
        {
            return this.ParseFloat();
        }

        throw new ArgumentException("Invalid number format.");
    }

    private GlslToken ParseDecimalOrFloat()
    {
        string number = this.ConsumeDigits();
        if (this._idx < this._source.Length && (this.Peek() == '.' || this.Peek() == 'e' || this.Peek() == 'E'))
        {
            return this.ParseFloat(number);
        }

        if (this._idx < this._source.Length && (this.Peek() == 'u' || this.Peek() == 'U'))
        {
            this.Advance();
        }

        return new(GlslTokenType.IntegerConstant, number);
    }

    private GlslToken ParseOctal()
    {
        string number = "0" + this.ConsumeWhile(c => c is >= '0' and <= '7');
        if (this._idx < this._source.Length && (this.Peek() == 'u' || this.Peek() == 'U'))
        {
            this.Advance();
        }

        return new(GlslTokenType.IntegerConstant, number);
    }

    private GlslToken ParseHexadecimal()
    {
        this.Advance(); // Skip 'x' or 'X'
        string number =
            "0x" + this.ConsumeWhile(c => Char.IsDigit(c) || c is >= 'a' and <= 'f' || c is >= 'A' and <= 'F');
        if (this._idx < this._source.Length && (this.Peek() == 'u' || this.Peek() == 'U'))
        {
            this.Advance();
            number += "U";
        }

        return new(GlslTokenType.IntegerConstant, number);
    }

    private GlslToken ParseFloat(string prefix = "")
    {
        string number = prefix;
        if (this.Peek() == '.')
        {
            this.Advance();
            number += "." + this.ConsumeDigits();
        }

        if (this._idx < this._source.Length && (this.Peek() == 'e' || this.Peek() == 'E'))
        {
            number += this.ConsumeExponent();
        }

        if (this._idx < this._source.Length && (this.Peek() == 'f' || this.Peek() == 'F'))
        {
            this.Advance();
            number += "F";
        }

        return new(GlslTokenType.FloatingConstant, number);
    }

    private string ConsumeExponent()
    {
        string exponent = $"{this.Advance()}";
        if (this._idx < this._source.Length && (this.Peek() == '+' || this.Peek() == '-'))
        {
            exponent += this.Advance();
        }

        exponent += this.ConsumeDigits();
        return exponent;
    }

    private string ConsumeDigits()
    {
        return this.ConsumeWhile(Char.IsDigit);
    }

    private string ConsumeWhile(Func<char, bool> condition)
    {
        string result = "";
        while (this._idx < this._source.Length && condition(this.Peek()))
        {
            result += this.Advance();
        }

        return result;
    }

    private GlslToken Identifier()
    {
        StringBuilder word = new();

        do
        {
            word.Append(this._source[this._idx++]);
        } while (ShaderParser.IsValidIdentifierChar(this._source[this._idx]));

        if (ShaderParser.Keywords.TryGetValue(word.ToString(), out GlslTokenType value))
        {
            return new(value, word.ToString());
        }

        return new(GlslTokenType.Identifier, word.ToString());
    }

    private GlslToken Comment()
    {
        StringBuilder word = new();

        do
        {
            word.Append(this.Advance());
        } while (this.Peek() is not '\r' and '\n');

        return new(GlslTokenType.Comment, word.ToString());
    }

    private GlslToken MultilineComment()
    {
        StringBuilder word = new();

        do
        {
            word.Append(this.Advance());
        } while (this.Peek() is not '*' && this.Peek(1) != '/');

        return new(GlslTokenType.Comment, word.ToString());
    }

    private static bool IsValidIdentifierChar(char c)
    {
        return c is >= 'a' and <= 'z' or >= 'A' and <= 'Z' or >= '0' and <= '9' or '_';
    }

    private void SkipWhitespace()
    {
        while (this._idx < this._source.Length && Char.IsWhiteSpace(this.Peek()))
        {
            this.Advance();
        }
    }

    private char Peek(int distance = 0)
    {
        return this._source[this._idx + distance];
    }

    private char Advance()
    {
        return this._source[this._idx++];
    }
}