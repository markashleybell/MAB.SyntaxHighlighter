module MAB.SyntaxHighlighter.Utils

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
