﻿module MAB.SyntaxHighlighter.Languages

open MAB.SyntaxHighlighter

let defaultNumberMatcher = @"[+-]?\d+(?:\.\d+)?"

let cLikeCommentMatcher = @"/\*.*?\*/|//.*?(?=\r|\n)"
let cLikeStringMatcher = @"@?""""|@?"".*?(?!\\).""|''|'[^\s]*?(?!\\).'"

let csharp = {
    Name = "C#"

    CaseSensitive = true

    StringMatcher = cLikeStringMatcher
    CommentMatcher = cLikeCommentMatcher
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

let defaultLanguageMap = Map.ofList [
    ("cs", csharp)
    ("csharp", csharp)
]
