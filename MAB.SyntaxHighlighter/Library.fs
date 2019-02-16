(*

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

module Utils = 
    open System
    open System.Text

    let isEmpty (s: string) = String.IsNullOrWhiteSpace s

    let trim (s: string) = s.Trim()

    let escape' (substitutions: (string * string) array) (sb: StringBuilder) =
        substitutions |> Seq.iter (fun (m, r) -> sb.Replace(m, r) |> ignore)
        sb

    let escape (substitutions: (string * string) array) (input: string) =
        new StringBuilder(input) 
        |> escape' substitutions 
        |> (fun sb -> sb.ToString())

module Constants = 
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
    let IMPOSSIBLE_MATCH_REGEX = "(?!.*)_{37}(?<!.*)"

module SyntaxHighlighter =
    open Constants
    open Utils
    open Languages
    open System.IO
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

    // Build a master regex with capturing groups
    // Note that the group numbers must match with the constants COMMENT_GROUP, OPERATOR_GROUP...
    let concatenateRegex' commentRx stringRx preprocessorRx keywordRx operatorsRx numberRx =
        sprintf "(%s)|(%s)|(%s)|(%s)|(%s)|(%s)" 
            commentRx stringRx preprocessorRx keywordRx operatorsRx numberRx

    let concatenateRegex lang preprocessorRx keywordRx operatorsRx =
        concatenateRegex' lang.CommentMatcher lang.StringMatcher preprocessorRx keywordRx operatorsRx lang.NumberMatcher 

    let span cls s =
        sprintf "<span class=\"%s\">%s</span>" cls s

    let matchEval (m: Match) =
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

            (true, Some matcher, matcher.Replace(code', new MatchEvaluator(matchEval)))

