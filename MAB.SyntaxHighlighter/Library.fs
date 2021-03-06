﻿(*

This code was inspired by (and heavily borrows from) the CSharpFormat code
by Jean-Claude Manoli. The original code was previously (but is no longer)
available at: http://www.manoli.net/csharpformat/

A reference version is available here:
https://github.com/fsprojects/FSharp.Formatting/tree/master/src/CSharpFormat

Copyright © 2001-2003 Jean-Claude Manoli [jc@manoli.net]

This software is provided 'as-is', without any express or implied warranty.
In no event will the author(s) be held liable for any damages arising from
the use of this software.

Permission is granted to anyone to use this software for any purpose,
including commercial applications, and to alter it and redistribute it
freely, subject to the following restrictions:

  1. The origin of this software must not be misrepresented; you must not
     claim that you wrote the original software. If you use this software
     in a product, an acknowledgment in the product documentation would be
     appreciated but is not required.

  2. Altered source versions must be plainly marked as such, and must not
     be misrepresented as being the original software.

  3. This notice may not be removed or altered from any source distribution.

*)

module MAB.SyntaxHighlighter.SyntaxHighlighter

open Utils
open Languages
open System.Text
open System.Text.RegularExpressions

let defaultLanguageMap = Map.ofList [
    ("cs", (csharp, ""))
    ("csharp", (csharp, ""))
    ("fs", (fsharp, ""))
    ("fsharp", (fsharp, ""))
    ("js", (javascript, ""))
    ("javascript", (javascript, ""))
    ("json", (javascript, ""))
    ("python", (python, ""))
    ("html", (html, ""))
    ("css", (css, ""))
]

let htmlReplacements = [|("&", "&amp;"); ("<", "&lt;"); (">", "&gt;")|]

let regexReplacements = 
    [|'&'; '?'; '*'; '.'; '<'; '>'; '['; ']'; '^'; '|'; '('; ')'; '#'; '+'; '$'|] 
    |> Array.map (fun c -> (c.ToString(), c |> sprintf @"\%c"))

let htmlEncode str =
    str |> escape htmlReplacements

let sanitiseRegex sb = 
    sb |> escape' regexReplacements

let buildRegex (separated: string) =
    match separated |> isEmpty with
    | true -> 
        IMPOSSIBLE_MATCH_REGEX
    | false -> 
        // We escape HTML chars in the list of matches before creating the regex
        // This way, they match correctly on HTML-escaped code
        let sb = new StringBuilder(separated |> htmlEncode) |> sanitiseRegex
        sb.Replace(" ", @"(?=\W|$)|(?<=^|\W)") |> ignore
        sb.ToString() |> sprintf @"(?<=^|\W)%s(?=\W|$)"

// This function relies on the fact that regexLists are defined with the regexes in the same order
// as the group name constants, e.g. comment string preprocessor keyword operators number
let concatenateRegex rxList =
    rxList |> List.map (sprintf "(%s)") |> String.concat "|"

let formatCode (languages: Map<string, (Language * string)>) languageId (code: string) = 
    let found =  languages.TryFind languageId

    // Note that we escape HTML chars at this point
    let code' = code |> trim |> htmlEncode

    match found with
    | None -> 
        (false, None, code')
    | Some (langType, types) -> 
        match langType with
        | CLikeLanguage lang ->
            let regexList = [
                lang.CommentMatcher
                lang.StringMatcher
                (lang.Preprocessors |> buildRegex)
                (lang.Keywords |> buildRegex)
                (lang.Operators |> buildRegex)
                lang.NumberMatcher
                lang.FunctionMatcher
                (types |> buildRegex)
            ]

            let matcher = new Regex((regexList |> concatenateRegex), RegexOptions.Singleline)

            (true, Some matcher, matcher.Replace(code', new MatchEvaluator(lang.MatchEvaluator)))
        | CLikeCaseInsensitiveLanguage lang ->
            let regexList = [
                lang.CommentMatcher
                lang.StringMatcher
                (lang.Preprocessors |> buildRegex)
                (lang.Keywords |> buildRegex)
                (lang.Operators |> buildRegex)
                lang.NumberMatcher
                lang.FunctionMatcher
                (types |> buildRegex)
            ]

            let matcher = new Regex((regexList |> concatenateRegex), RegexOptions.Singleline ||| RegexOptions.IgnoreCase)

            (true, Some matcher, matcher.Replace(code', new MatchEvaluator(lang.MatchEvaluator)))
        | SignificantWhiteSpaceLanguage lang ->
            let regexList = [
                lang.CommentMatcher
                lang.StringMatcher
                (lang.Preprocessors |> buildRegex)
                (lang.Keywords |> buildRegex)
                (lang.Operators |> buildRegex)
                lang.NumberMatcher
                lang.FunctionMatcher
            ]

            let matcher = new Regex((regexList |> concatenateRegex), RegexOptions.Singleline)

            (true, Some matcher, matcher.Replace(code', new MatchEvaluator(lang.MatchEvaluator)))
        | XmlLanguage lang ->
            let regexList = [
                lang.EmbeddedJavascriptMatcher
                lang.CommentMatcher
                lang.TagDelimiterMatcher
                lang.TagNameMatcher
                lang.TagAttributesMatcher
                lang.EntityMatcher
            ]

            let matcher = new Regex((regexList |> concatenateRegex), RegexOptions.IgnoreCase ||| RegexOptions.Singleline)

            (true, Some matcher, matcher.Replace(code', new MatchEvaluator(lang.MatchEvaluator)))
        | QueryLanguage lang -> 
            (false, None, code')
        | StyleLanguage lang -> 
            let regexList = [
                lang.CommentMatcher
                lang.ValueMatcher
                lang.PropertyMatcher
                (types |> buildRegex)
            ]

            let matcher = new Regex((regexList |> concatenateRegex), RegexOptions.Singleline)

            (true, Some matcher, matcher.Replace(code', new MatchEvaluator(lang.MatchEvaluator)))

