module MAB.SyntaxHighlighter.Languages

open System.IO
open System.Text
open System.Text.RegularExpressions

[<Literal>]
let IMPOSSIBLE_MATCH_REGEX = "(?!.*)_{37}(?<!.*)"

let span cls s =
    sprintf "<span class=\"%s\">%s</span>" cls s

// TODO: stop being naughty with mutable
let wrapComment s = 
    let sr = new StringReader(s)
            
    let sb = new StringBuilder()

    let mutable line = sr.ReadLine()

    while line <> null do
        if sb.Length > 0 then sb.Append("\n") |> ignore

        sb.Append(line |> span "c") |> ignore

        line <- sr.ReadLine()

    sb.ToString()

module Defaults = 
    [<Literal>]
    let COMMENT_GROUP = 1
    [<Literal>]
    let STRING_LITERAL_GROUP = 2
    [<Literal>]
    let PREPROCESSOR_KEYWORD_GROUP = 3
    [<Literal>]
    let KEYWORD_GROUP = 4
    [<Literal>]
    let OPERATOR_GROUP = 5
    [<Literal>]
    let NUMBER_GROUP = 6
    [<Literal>]
    let FUNCTION_GROUP = 7
    [<Literal>]
    let TYPE_GROUP = 8
    
    let numberMatcher = @"\b[+-]?\d+(?:\.\d+)?"

    let matchEvaluator (m: Match) =
        let matchGroups = [
            (m.Groups.[COMMENT_GROUP], wrapComment)
            (m.Groups.[STRING_LITERAL_GROUP], (span "s"))
            (m.Groups.[PREPROCESSOR_KEYWORD_GROUP], (span "pp"))
            (m.Groups.[KEYWORD_GROUP], (span "k"))
            (m.Groups.[OPERATOR_GROUP], (span "o"))
            (m.Groups.[NUMBER_GROUP], (span "n"))
            (m.Groups.[FUNCTION_GROUP], (span "f"))
            (m.Groups.[TYPE_GROUP], (span "t"))
        ]

        let suceededMatch = matchGroups |> Seq.tryFind (fun (grp, _) -> grp.Success)

        match suceededMatch with
        | None -> failwith "Match type is unknown"
        | Some (_, wrapf) -> wrapf (m.ToString())

module Html = 
    [<Literal>]
    let ATTRIBUTE_VALUE_GROUP = 1
    [<Literal>]
    let ATTRIBUTE_NAME_GROUP = 2

    [<Literal>]
    let EMBEDDED_JAVASCRIPT_GROUP = 1
    [<Literal>]
    let COMMENT_GROUP = 2
    [<Literal>]
    let TAG_DELIMITER_GROUP = 3
    [<Literal>]
    let TAG_NAME_GROUP = 4
    [<Literal>]
    let TAG_ATTRIBUTES_GROUP = 5
    [<Literal>]
    let ENTITY_GROUP = 6

    let attributeMatchEvaluator (m: Match) = 
        let matchGroups = [
            (m.Groups.[ATTRIBUTE_VALUE_GROUP], (span "s"))
            (m.Groups.[ATTRIBUTE_NAME_GROUP], (span "a"))
        ]

        let suceededMatch = matchGroups |> Seq.tryFind (fun (grp, _) -> grp.Success)

        match suceededMatch with
        | None -> failwith "Match type is unknown"
        | Some (_, wrapf) -> wrapf (m.ToString())

    let matchEvaluator (attributeMatcher: string) (m: Match) =
        let attributeRegex =
            Regex (attributeMatcher, RegexOptions.IgnoreCase)

        let replaceAttributes s =
            attributeRegex.Replace (s, attributeMatchEvaluator)

        let matchGroups = [
            (m.Groups.[EMBEDDED_JAVASCRIPT_GROUP], (span "s"))
            (m.Groups.[COMMENT_GROUP], wrapComment)
            (m.Groups.[TAG_DELIMITER_GROUP], (span "p"))
            (m.Groups.[TAG_NAME_GROUP], (span "t"))
            (m.Groups.[TAG_ATTRIBUTES_GROUP], replaceAttributes)
            (m.Groups.[ENTITY_GROUP], (span "k"))
        ]

        let suceededMatch = matchGroups |> Seq.tryFind (fun (grp, _) -> grp.Success)

        match suceededMatch with
        | None -> failwith "Match type is unknown"
        | Some (_, wrapf) -> wrapf (m.ToString())

module Css = 
    [<Literal>]
    let COMMENT_GROUP = 1
    [<Literal>]
    let VALUE_GROUP = 2
    [<Literal>]
    let PROPERTY_GROUP = 3
    
    let valueMatcher = @"(?<=: ).*?(?=;)"

    let propertyMatcher = @"(?<=\s)[a-z\-]+(?=:)"

    let matchEvaluator (m: Match) =
        let matchGroups = [
            (m.Groups.[COMMENT_GROUP], wrapComment)
            (m.Groups.[VALUE_GROUP], (span "n"))
            (m.Groups.[PROPERTY_GROUP], (span "k"))
        ]

        let suceededMatch = matchGroups |> Seq.tryFind (fun (grp, _) -> grp.Success)

        match suceededMatch with
        | None -> failwith "Match type is unknown"
        | Some (_, wrapf) -> wrapf (m.ToString())

let csharp = CLikeLanguage {
    StringMatcher = @"@?""""|@?"".*?(?!\\).""|''|'[^\s]*?(?!\\).'"
    CommentMatcher = @"/\*.*?\*/|//.*?(?=\r|\n)"
    NumberMatcher = Defaults.numberMatcher
    FunctionMatcher = @"(?<=class )[A-Za-z0-9_]+|(?<!new )(?<= )[A-Za-z0-9_]+(?=\()|(?<=\s)[A-Z]+[A-Za-z0-9_]+(?=\.[A-Za-z0-9\.]+\()"

    Operators = ". : += -= + - * / % & | ^ ! ~ == <= >= < (?<!\=)> ?"

    Preprocessors = "#if #else #elif #endif #define #undef #warning "
                  + "#error #line #region #endregion #pragma"

    Keywords = "abstract as base bool break byte case catch char "
             + "checked class const continue decimal default delegate do double else "
             + "enum event explicit extern false finally fixed float for foreach goto "
             + "if implicit in int interface internal is lock long namespace new null "
             + "object operator out override partial params private protected public readonly "
             + "ref return sbyte sealed short sizeof stackalloc static string struct "
             + "switch this throw true try typeof uint ulong unchecked unsafe ushort "
             + "using value virtual void volatile where while yield "
             + "var from select where orderby descending join on equals let ascending"
             + "into group by await async dynamic"

    MatchEvaluator = Defaults.matchEvaluator
}

let javascript = CLikeLanguage {
    StringMatcher = @"""""|"".*?(?!\\).""|''|'[^\s]*?(?!\\).'|``|`.*?(?!\\).`"
    CommentMatcher = @"/\*.*?\*/|//.*?(?=\r|\n)"
    NumberMatcher = Defaults.numberMatcher
    FunctionMatcher = @"(?<=(function) )[^(]+|(?<=\s)[A-Z]+.*?(?=\b)"

    Operators = ". + - * / % & | ^ ! ~ === !== <= >= < (?<!\=)>"

    Preprocessors = @"@\w*"

    Keywords = "var function abstract as base bool break byte case catch char "
             + "checked class const continue decimal default delegate do double else "
             + "enum event explicit extern false finally fixed float for foreach goto "
             + "if implicit in int interface internal is lock long namespace new null "
             + "object operator out override params private protected public readonly "
             + "ref return sbyte sealed short sizeof stackalloc static string struct "
             + "switch this throw true try typeof uint ulong unchecked unsafe ushort "
             + "using virtual void while module export import declare extends implements "
             + "constructor let async await from"

    MatchEvaluator = Defaults.matchEvaluator
}

let fsharp = SignificantWhiteSpaceLanguage {
    StringMatcher = @"@?""""|@?"".*?(?!\\).""|''|'[^\s]*?(?!\\)'"
    CommentMatcher = @"\(\*.*?\*\)|//.*?(?=\r|\n)"
    NumberMatcher = Defaults.numberMatcher
    FunctionMatcher = @"(?<=(class|module|new) )[^\ ]+|(?<=\s)[A-Z]+.*?(?=\.)"

    Operators = "+ - _ -> ->> <- [< >] [| |] [ ] <@@ @@> <@| |@> <@. .@> <@ @> |> < > |"

    Preprocessors = "#light"

    Keywords = "abstract and as assert asr begin class default delegate do! do done downcast downto else "
             + "end enum exception extern false finally for fun function if in inherit interface land lazy "
             + "use! use let! let lor lsl lsr lxor match member mod module mutable namespace new null of open or override "
             + "rec return! return sig static struct then to true try type val when inline upcast while with void yield! yield"

    MatchEvaluator = Defaults.matchEvaluator
}

let python = SignificantWhiteSpaceLanguage {
    StringMatcher = @"r?"""""".*?(?!\\).""""""|r?""""|r?"".*?(?!\\).""|r?''|r?'[^\s]*?(?!\\)'"
    CommentMatcher = @"#.*?(?=\r|\n)"
    NumberMatcher = Defaults.numberMatcher
    FunctionMatcher = @"(?<=(class|def) )[^\(]+"

    Operators = "+ - * / % <> != == < >"

    Preprocessors = ""

    Keywords = "False None True and as assert async await break class continue def del elif else except finally for from "
             + "global if import in is lambda nonlocal not or pass raise return try while with yield"

    MatchEvaluator = Defaults.matchEvaluator
}

let html = XmlLanguage {
    EmbeddedJavascriptMatcher = @"(?<=&lt;script(?:\s.*?)?&gt;).+?(?=&lt;/script&gt;)"
    CommentMatcher = @"&lt;!--.*?--&gt;"
    TagDelimiterMatcher = @"(?:&lt;/?!?\??(?!%)|(?<!%)/?&gt;)+"
    TagNameMatcher = @"(?<=&lt;/?!?\??(?!%))[\w\.:-]+(?=.*&gt;)"
    TagAttributesMatcher = @"(?<=&lt;(?!%)/?!?\??[\w:-]+).*?(?=(?<!%)/?&gt;)"
    EntityMatcher = @"&amp;\w+;"

    MatchEvaluator = (Html.matchEvaluator @"("".*?""|'.*?')|([\w:-]+)")
}

let css = StyleLanguage {
    CommentMatcher = @"/\*.*?\*/|//.*?(?=\r|\n)"
    PropertyMatcher = Css.propertyMatcher
    ValueMatcher = Css.valueMatcher

    MatchEvaluator = Css.matchEvaluator
}

let powershell = CLikeCaseInsensitiveLanguage {
    StringMatcher = @"''|'[^']*?(?!\\).'|""""|""[^""]*?(?!\\)."""
    CommentMatcher = @"\<\#.*?\#\>|#.*?(?=\r|\n)"
    NumberMatcher = Defaults.numberMatcher
    FunctionMatcher = @"(?<=[\s\(])[A-Z]+.*?(?=[^\w\-])"

    Operators = ". + - * / % & | ^ ! ~ === !== <= >= < (?<!\=)>"

    Preprocessors = @"@\w*"

    Keywords = "begin break catch continue data do dynamicparam else elseif end exit \bfilter finally "
             + "for foreach\b from function if in param process return switch throw trap try until while "
             + "local private where $true $false string int parameter"

    MatchEvaluator = Defaults.matchEvaluator
}
