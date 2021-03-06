﻿namespace MAB.SyntaxHighlighter

open System.Text.RegularExpressions

type CLikeLanguage = {
    StringMatcher: string
    NumberMatcher: string
    CommentMatcher: string
    FunctionMatcher: string
    Operators: string
    Preprocessors: string
    Keywords: string
    MatchEvaluator: Match -> string
}

type CLikeCaseInsensitiveLanguage = {
    StringMatcher: string
    NumberMatcher: string
    CommentMatcher: string
    FunctionMatcher: string
    Operators: string
    Preprocessors: string
    Keywords: string
    MatchEvaluator: Match -> string
}

type SignificantWhiteSpaceLanguage = {
    StringMatcher: string
    NumberMatcher: string
    CommentMatcher: string
    FunctionMatcher: string
    Operators: string
    Preprocessors: string
    Keywords: string
    MatchEvaluator: Match -> string
}

type XmlLanguage = {
    EmbeddedJavascriptMatcher: string
    CommentMatcher: string
    TagDelimiterMatcher: string
    TagNameMatcher: string
    TagAttributesMatcher: string
    EntityMatcher: string
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

type StyleLanguage = {
    PropertyMatcher: string
    ValueMatcher: string
    CommentMatcher: string
    MatchEvaluator: Match -> string
}

type Language =
    | CLikeLanguage of CLikeLanguage
    | CLikeCaseInsensitiveLanguage of CLikeCaseInsensitiveLanguage
    | SignificantWhiteSpaceLanguage of SignificantWhiteSpaceLanguage
    | XmlLanguage of XmlLanguage
    | QueryLanguage of QueryLanguage
    | StyleLanguage of StyleLanguage
