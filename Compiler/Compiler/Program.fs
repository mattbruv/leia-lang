module Program

open Parser

let print x = printfn $"%A{x}"


run pint "-123Z" |> printResult
// -123

run pint "-Z123" |> printResult
// Line:0 Col:1 Error parsing integer
// -Z123
//  ^Unexpected 'Z'

run pfloat "-123.45Z" |> printResult
// -123.45

run pfloat "-123Z45" |> printResult
// Line:0 Col:4 Error parsing float
// -123Z45
//     ^Unexpected 'Z'
