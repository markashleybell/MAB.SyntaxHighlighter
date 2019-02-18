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

namespace MAB.SyntaxHighlighter

module SyntaxHighlighter =
    open Utils
    open Languages
    open System.Text
    open System.Text.RegularExpressions

    let defaultLanguageMap = Map.ofList [
        ("cs", csharp)
        ("csharp", csharp)
        ("fs", fsharp)
        ("fsharp", fsharp)
        ("python", python)
    ]

    let htmlReplacements = [|("&", "&amp;"); ("<", "&lt;"); (">", "&gt;")|]

    let regexReplacements = 
        [|'&'; '?'; '*'; '.'; '<'; '>'; '['; ']'; '^'; '|'; '('; ')'; '#'; '+'|] 
        |> Array.map (fun c -> (c.ToString(), c |> sprintf @"\%c"))

    let escapeHtml str =
        str |> escape htmlReplacements

    let sanitiseRegex sb = 
        sb |> escape' regexReplacements

    let buildRegex (separated: string) =
        match separated |> isEmpty with
        | true -> 
            IMPOSSIBLE_MATCH_REGEX
        | false -> 
            let sb = new StringBuilder(separated) |> sanitiseRegex
            sb.Replace(" ", @"(?=\W|$)|(?<=^|\W)") |> ignore
            sb.ToString() |> sprintf @"(?<=^|\W)%s(?=\W|$)"

    let concatenateRegex lang preprocessorRx keywordRx operatorsRx =
        Defaults.concatenateRegex lang.CommentMatcher lang.StringMatcher preprocessorRx keywordRx operatorsRx lang.NumberMatcher 

    let formatCode (languages: Map<string, Language>) languageId (code: string) = 
        let language =  languages.TryFind languageId

        let code' = code |> trim

        match language with
        | None -> 
            (false, None, code' |> escapeHtml)
        | Some lang -> 
            let keywordMatcher = lang.Keywords |> buildRegex
            let preprocessorMatcher = lang.Preprocessors |> buildRegex
            let operatorMatcher = lang.Operators |> buildRegex

            let allMatcher = concatenateRegex lang preprocessorMatcher keywordMatcher operatorMatcher

            let rxOptions = 
                if lang.CaseSensitive 
                then RegexOptions.Singleline 
                else RegexOptions.Singleline ||| RegexOptions.IgnoreCase
            
            let matcher = new Regex(allMatcher, rxOptions)

            (true, Some matcher, matcher.Replace(code', new MatchEvaluator(Defaults.matchEvaluator)))

