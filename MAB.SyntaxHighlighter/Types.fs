﻿namespace MAB.SyntaxHighlighter

type Language = {
    Name: string
    CaseSensitive: bool
    StringMatcher: string
    NumberMatcher: string
    CommentMatcher: string
    Operators: string
    Preprocessors: string
    Keywords: string
}
