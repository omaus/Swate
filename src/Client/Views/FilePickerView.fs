module FilePickerView

open Fable.React
open Fable.React.Props
open Fable.Core.JsInterop
open Fulma
open Fulma.Extensions.Wikiki
open Fable.FontAwesome
open Thoth.Json
open Thoth.Elmish
open ExcelColors
open Api
open Model
open Messages
open Update
open Shared
open Browser.Types

let createFileList (model:Model) (dispatch: Msg -> unit) =
    if model.FilePickerState.FileNames.Length > 0 then
        model.FilePickerState.FileNames
        |> List.map (fun fileName ->
            tr [
                colorControl model.SiteStyleState.ColorMode
            ] [
                td [
                ] [
                    Delete.delete [
                        Delete.OnClick (fun _ -> fileName |> RemoveFileFromFileList |> FilePicker |> dispatch)
                    ][]
                ]
                td [] [
                    b [] [str fileName]
                ]

            ])
    else
        [
            tr [] [
                td [] [str "No Files selected."]
            ]
        ]

let filePickerComponent (model:Model) (dispatch:Msg -> unit) =
    Content.content [ Content.Props [colorControl model.SiteStyleState.ColorMode ]] [
        Label.label [Label.Size Size.IsLarge; Label.Props [Style [Color model.SiteStyleState.ColorMode.Accent]]][ str "File Picker"]
        File.file [] [
            File.label [] [
                File.input [
                    Props [
                        Multiple true
                        OnChange (fun ev ->
                            let files : FileList = ev.target?files
                            
                            let fileNames =
                                [ for i=0 to (files.length - 1) do yield files.item i ]
                                |> List.map (fun f -> f.name)

                            fileNames |> NewFilesLoaded |> FilePicker |> dispatch
                            )
                    ]
                ]
                File.cta [] [
                    File.icon [] [
                        Fa.i [
                            Fa.Solid.Upload
                        ] []
                    ]
                    File.name [] [
                        str "Chose one or multiple files"
                    ]
                ]
            ]
        ]
        Table.table [Table.IsFullWidth] [
            tbody [] (createFileList model dispatch)
        ]
        
    ]