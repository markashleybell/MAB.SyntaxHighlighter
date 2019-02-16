<Query Kind="FSharpProgram">
  <Reference Relative="..\MAB.SyntaxHighlighter\bin\Debug\net472\MAB.SyntaxHighlighter.dll">C:\Src\MAB.SyntaxHighlighter\MAB.SyntaxHighlighter\bin\Debug\net472\MAB.SyntaxHighlighter.dll</Reference>
  <NuGetReference>FSharp.Core</NuGetReference>
  <Namespace>MAB.SyntaxHighlighter</Namespace>
  <Namespace>System.IO</Namespace>
</Query>

Util.CurrentQueryPath
|> Path.GetDirectoryName
|> Directory.SetCurrentDirectory

let lang = "python"

let code = File.ReadAllText (sprintf "samples\%s.txt" lang)

let format =
    SyntaxHighlighter.formatCode SyntaxHighlighter.defaultLanguageMap

let (ok, rx, htmlOutput) = code |> format lang

htmlOutput |> Dump |> ignore

htmlOutput
|> (fun html -> ((File.ReadAllText "template.html"), html))
|> (fun (tmpl, html) -> Regex.Replace (tmpl, "{{CONTENT}}", html))
|> (fun output -> File.WriteAllText (@"output\index.html", output))
