<Query Kind="FSharpProgram">
  <Reference Relative="..\MAB.SyntaxHighlighter\bin\Debug\net472\MAB.SyntaxHighlighter.dll">C:\Src\MAB.SyntaxHighlighter\MAB.SyntaxHighlighter\bin\Debug\net472\MAB.SyntaxHighlighter.dll</Reference>
  <NuGetReference>FSharp.Core</NuGetReference>
  <Namespace>MAB.SyntaxHighlighter</Namespace>
  <Namespace>System.IO</Namespace>
</Query>

Util.CurrentQueryPath
|> Path.GetDirectoryName
|> Directory.SetCurrentDirectory

let code = @"

// This is a comment
var test = new DateTime(2019, 2, 10);

/*
Here's a multi-line comment
It's on multiple lines
*/
for (int i = 0; i < 10; i ++)
{
    Console.WriteLine(i);
}

string s = ""This is a string."";

"

let format =
    SyntaxHighlighter.formatCode Languages.defaultLanguageMap

let (ok, rx, html) = code |> format "cs"

// rx.Value.ToString() |> Dump |> ignore

html |> Dump |> ignore

html
|> (fun s -> ((File.ReadAllText "template.html"), html))
|> (fun (tmpl, html) -> Regex.Replace (tmpl, "{{CONTENT}}", html))
|> (fun output -> File.WriteAllText (@"output\index.html", output))
