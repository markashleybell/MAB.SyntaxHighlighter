namespace MAB.SyntaxHighlighter

open System.Text.RegularExpressions

type Language = {
    CaseSensitive: bool
    StringMatcher: string
    NumberMatcher: string
    CommentMatcher: string
    Operators: string
    Preprocessors: string
    Keywords: string
    MatchEvaluator: Match -> string
}
