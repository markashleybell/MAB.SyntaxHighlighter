<Query Kind="FSharpProgram">
  <Reference Relative="..\MAB.SyntaxHighlighter\bin\Debug\net472\MAB.SyntaxHighlighter.dll">C:\Src\MAB.SyntaxHighlighter\MAB.SyntaxHighlighter\bin\Debug\net472\MAB.SyntaxHighlighter.dll</Reference>
  <NuGetReference>FSharp.Core</NuGetReference>
  <Namespace>MAB.SyntaxHighlighter</Namespace>
  <Namespace>System.IO</Namespace>
</Query>

Util.CurrentQueryPath
|> Path.GetDirectoryName
|> Directory.SetCurrentDirectory

let readFile lang = 
    File.ReadAllText (sprintf "samples\%s.txt" lang)

// let languages = ["html"] 
// let languages = ["python"] 
let languages = ["html"; "csharp"; "fsharp"; "python"]

let sources = 
    languages |> List.mapi (fun ord lang -> ((ord, lang), lang |> readFile)) |> Map.ofList

let format =
    SyntaxHighlighter.formatCode SyntaxHighlighter.defaultLanguageMap

let results = 
    sources |> Map.map (fun (_, lang) code -> code |> format lang)

results |> Dump |> ignore

let codeBlock lang code = 
    sprintf "<h2>%s</h2><pre class=\"code\"><code>%s</code></pre>" lang code

let htmlOutput =
    results 
    |> Map.map (fun (_, _) (_, _, code) -> code)
    |> Map.toList
    |> List.sortByDescending (fun ((ord, _), _)-> ord)
    |> List.fold (fun out ((_, lang), code) -> (codeBlock lang code) :: out) []
    |> String.concat ""

htmlOutput |> Dump |> ignore

htmlOutput
|> (fun html -> ((File.ReadAllText "template.html"), html))
|> (fun (tmpl, html) -> Regex.Replace (tmpl, "{{CONTENT}}", html))
|> (fun output -> File.WriteAllText (@"output\index.html", output))