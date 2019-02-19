namespace MAB.SyntaxHighlighter

open System.Text.RegularExpressions

type CLikeLanguage = {
    StringMatcher: string
    NumberMatcher: string
    CommentMatcher: string
    Operators: string
    Preprocessors: string
    Keywords: string
    MatchEvaluator: Match -> string
}

type SignificantWhiteSpaceLanguage = {
    StringMatcher: string
    NumberMatcher: string
    CommentMatcher: string
    Operators: string
    Preprocessors: string
    Keywords: string
    MatchEvaluator: Match -> string
}

type MarkupLanguage = {
    StringMatcher: string
    NumberMatcher: string
    CommentMatcher: string
    Operators: string
    Preprocessors: string
    Keywords: string
    MatchEvaluator: Match -> string
}

type QueryLanguage = {
    StringMatcher: string
    NumberMatcher: string
    CommentMatcher: string
    Operators: string
    Preprocessors: string
    Keywords: string
    MatchEvaluator: Match -> string
}

type Language =
    | CLikeLanguage of CLikeLanguage
    | SignificantWhiteSpaceLanguage of SignificantWhiteSpaceLanguage
    | MarkupLanguage of MarkupLanguage
    | QueryLanguage of QueryLanguage
