module MAB.SyntaxHighlighter.Languages

let defaultNumberMatcher = @"\b[+-]?\d+(?:\.\d+)?"

module CLike =
    let commentMatcher = @"/\*.*?\*/|//.*?(?=\r|\n)"

    let stringMatcher = @"@?""""|@?"".*?(?!\\).""|''|'[^\s]*?(?!\\).'"

let csharp = {
    CaseSensitive = true

    StringMatcher = CLike.stringMatcher
    CommentMatcher = CLike.commentMatcher
    NumberMatcher = defaultNumberMatcher

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
}

let fsharp = {
    CaseSensitive = true

    StringMatcher = @"@?""""|@?"".*?(?!\\).""|''|'[^\s]*?(?!\\)'"
    CommentMatcher = @"\(\*.*?\*\)|//.*?(?=\r|\n)"
    NumberMatcher = defaultNumberMatcher

    Operators = "+ - _ -> ->> <- [< >] [| |] [ ] <@@ @@> <@| |@> <@. .@> <@ @> |> < > |"

    Preprocessors = "#light"

    Keywords = "abstract and as assert asr begin class default delegate do! do done downcast downto else "
             + "end enum exception extern false finally for fun function if in inherit interface land lazy "
             + "use! use let! let lor lsl lsr lxor match member mod module mutable namespace new null of open or override "
             + "rec return! return sig static struct then to true try type val when inline upcast while with void yield! yield"
}

let python = {
    CaseSensitive = true

    StringMatcher = @"r?"""""".*?(?!\\).""""""|r?""""|r?"".*?(?!\\).""|r?''|r?'[^\s]*?(?!\\)'"
    CommentMatcher = @"#.*?(?=\r|\n)"
    NumberMatcher = defaultNumberMatcher

    Operators = "+ - * / % <> != == < >"

    Preprocessors = ""

    Keywords = "False None True and as assert async await break class continue def del elif else except finally for from "
             + "global if import in is lambda nonlocal not or pass raise return try while with yield"
}
