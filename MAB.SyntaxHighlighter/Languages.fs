module MAB.SyntaxHighlighter.Languages

let span cls s =
    sprintf "<span class=\"%s\">%s</span>" cls s

[<Literal>]
let IMPOSSIBLE_MATCH_REGEX = "(?!.*)_{37}(?<!.*)"

module Defaults = 
    open System.IO
    open System.Text
    open System.Text.RegularExpressions

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
    
    let numberMatcher = @"\b[+-]?\d+(?:\.\d+)?"

    let commentMatcher = @"/\*.*?\*/|//.*?(?=\r|\n)"

    let stringMatcher = @"@?""""|@?"".*?(?!\\).""|''|'[^\s]*?(?!\\).'"

    let matchEvaluator (m: Match) =
        let wrapComment s = 
            let sr = new StringReader(s)
            
            let sb = new StringBuilder()

            let mutable line = sr.ReadLine()

            while line <> null do
                if sb.Length > 0 then sb.Append("\n") |> ignore

                sb.Append(line |> span "c") |> ignore

                line <- sr.ReadLine()

            sb.ToString()

        let matchGroups = [
            (m.Groups.[COMMENT_GROUP], wrapComment)
            (m.Groups.[STRING_LITERAL_GROUP], (span "s"))
            (m.Groups.[PREPROCESSOR_KEYWORD_GROUP], (span "pp"))
            (m.Groups.[KEYWORD_GROUP], (span "k"))
            (m.Groups.[OPERATOR_GROUP], (span "o"))
            (m.Groups.[NUMBER_GROUP], (span "n"))
        ]

        let suceededMatch = matchGroups |> Seq.tryFind (fun (grp, _) -> grp.Success)

        match suceededMatch with
        | None -> failwith "Match type is unknown"
        | Some (_, wrapf) -> wrapf (m.ToString())

let csharp = CLikeLanguage {
    StringMatcher = Defaults.stringMatcher
    CommentMatcher = Defaults.commentMatcher
    NumberMatcher = Defaults.numberMatcher

    Operators = ". : + - * / % & | ^ ! ~ = < > ?"

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

let fsharp = SignificantWhiteSpaceLanguage {
    StringMatcher = @"@?""""|@?"".*?(?!\\).""|''|'[^\s]*?(?!\\)'"
    CommentMatcher = @"\(\*.*?\*\)|//.*?(?=\r|\n)"
    NumberMatcher = Defaults.numberMatcher

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

    Operators = "+ - * / % <> != == < >"

    Preprocessors = ""

    Keywords = "False None True and as assert async await break class continue def del elif else except finally for from "
             + "global if import in is lambda nonlocal not or pass raise return try while with yield"

    MatchEvaluator = Defaults.matchEvaluator
}

let html = MarkupLanguage {
    StringMatcher = @"r?"""""".*?(?!\\).""""""|r?""""|r?"".*?(?!\\).""|r?''|r?'[^\s]*?(?!\\)'"
    CommentMatcher = @"#.*?(?=\r|\n)"
    NumberMatcher = Defaults.numberMatcher

    Operators = "+ - * / % <> != == < >"

    Preprocessors = ""

    Keywords = "False None True and as assert async await break class continue def del elif else except finally for from "
             + "global if import in is lambda nonlocal not or pass raise return try while with yield"

    MatchEvaluator = Defaults.matchEvaluator
}
