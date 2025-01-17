﻿
open System

open wasm.read_basic
open wasm.read
open wasm.write
open wasm.cecil

[<EntryPoint>]
let main argv =
    let m =
        let filename = "..\\miniray.wasm"
        printfn "Reading %s" filename
        let br = BinaryWasmStream(System.IO.File.ReadAllBytes(filename))
        let timer = System.Diagnostics.Stopwatch.StartNew()
        let m = read_module br
        timer.Stop()
        //printfn "%A milliseconds" timer.ElapsedMilliseconds
        m

    //printfn "%A" m

    let assembly = System.Reflection.Assembly.GetAssembly(typeof<wasi_unstable>)

    let destname = "..\\miniray.dll"
    printfn "Generating assembly %s" destname
    let ba = 
        use ms = new System.IO.MemoryStream()
        let id = "miniray"
        let ns = id
        let classname = "foo"
        let ver = new System.Version(1, 0, 0, 0)
        let settings = {
            memory = MemorySetting.AlwaysImportPairFrom "wasi_unstable"
            profile = ProfileSetting.No
            trace = TraceSetting.No
            env = Some assembly
            }
        gen_assembly settings m id ns classname ver ms
        ms.ToArray()
    System.IO.File.WriteAllBytes(destname, ba)

    0 // return an integer exit code

