module AdvancedSearch

open Fable.React
open Fable.React.Props
open Fulma
open Fulma.Extensions.Wikiki
open Fable.FontAwesome
open ExcelColors
open Model
open Messages
open Shared
open CustomComponents


let isValidAdancedSearchOptions (opt:AdvancedTermSearchOptions) =
    ((
        opt.SearchTermName.Length
        + opt.SearchTermDefinition.Length
        + opt.MustContainName.Length
        + opt.MustContainDefinition.Length
            ) > 0)
            || opt.Ontology.IsSome

let createOntologyDropdownItem (model:Model) (dispatch:Msg -> unit) (ont: DbDomain.Ontology)  =
    Dropdown.Item.a [
        Dropdown.Item.Props [
            TabIndex 0
            OnClick (fun _ -> ont |> OntologySuggestionUsed |> AdvancedSearch |> dispatch)
            OnKeyDown (fun k -> if k.key = "Enter" then ont |> OntologySuggestionUsed |> AdvancedSearch |> dispatch)
            colorControl model.SiteStyleState.ColorMode
        ]

    ][
        Text.span [
            CustomClass (Tooltip.ClassName + " " + Tooltip.IsTooltipRight + " " + Tooltip.IsMultiline)
            Props [
                Tooltip.dataTooltip (ont.Definition)
                Style [PaddingRight "10px"]
            ]
        ] [
            Fa.i [Fa.Solid.InfoCircle] []
        ]
        
        Text.span [] [ont.Name |> str]
    ]

let createAdvancedTermSearchResultRows (model:Model) (dispatch: Msg -> unit) (suggestionUsedHandler: DbDomain.Term -> Msg) =
    if model.AdvancedSearchState.AdvancedSearchTermResults |> Array.isEmpty |> not then
        model.AdvancedSearchState.AdvancedSearchTermResults
        |> Array.map (fun sugg ->
            tr [
                OnClick (fun _ -> sugg |> suggestionUsedHandler |> dispatch)
                colorControl model.SiteStyleState.ColorMode
                Class "suggestion"
            ] [
                td [Class (Tooltip.ClassName + " " + Tooltip.IsTooltipRight + " " + Tooltip.IsMultiline);Tooltip.dataTooltip sugg.Definition] [
                    Fa.i [Fa.Solid.InfoCircle] []
                ]
                td [] [
                    b [] [str sugg.Name]
                ]
                td [Style [Color "red"]] [if sugg.IsObsolete then str "obsolete"]
                td [Style [FontWeight "light"]] [small [] [str sugg.Accession]]
            ])
    else
        [|
            tr [] [
                td [] [str "No terms found matching your input."]
            ]
        |]

let advancedTermSearchComponent (model:Model) (dispatch: Msg -> unit) =
    form [
        OnSubmit    (fun e -> e.preventDefault())
        OnKeyDown   (fun k -> if k.key = "Enter" then k.preventDefault())
    ] [
        Field.div [] [
            Label.label [Label.Props [Style [Color model.SiteStyleState.ColorMode.Accent]]] [ str "Ontology"]
            Help.help [] [str "Only search terms in the selected ontology"]
            Field.div [] [
                Dropdown.dropdown [
                    Dropdown.IsActive model.AdvancedSearchState.HasOntologyDropdownVisible
                ] [
                    Dropdown.trigger [] [
                        Button.button [
                            Button.OnClick (fun _ -> ToggleOntologyDropdown |> AdvancedSearch |> dispatch);
                            Button.Size Size.IsSmall;
                            Button.Props [Style [MarginTop "0.5rem"]]
                        ] [
                            span [] [
                                match model.AdvancedSearchState.AdvancedSearchOptions.Ontology with
                                | None -> "select ontology" |> str
                                | Some ont -> ont.Name |> str
                            ]
                            Fa.i [Fa.Solid.AngleDown] []
                        ]
                    ]
                    Dropdown.menu [Props[colorControl model.SiteStyleState.ColorMode];] [
                        Dropdown.content [] (
                            model.PersistentStorageState.SearchableOntologies
                            |> Array.map snd
                            |> Array.toList
                            |> List.map (createOntologyDropdownItem model dispatch))
                    ]
                ]
            ]
        ]
        Field.div [] [
            Label.label [Label.Props [Style [Color model.SiteStyleState.ColorMode.Accent]]] [ str "Term name keywords:"]
            Help.help [] [str "Search the term name for the following words."]
            Field.div [] [
                Control.div [] [
                    Input.input [
                        Input.Placeholder "... search key words"
                        Input.Size IsSmall
                        Input.Props [ExcelColors.colorControl model.SiteStyleState.ColorMode]
                        Input.OnChange (fun e ->
                            {model.AdvancedSearchState.AdvancedSearchOptions
                                with SearchTermName = e.Value
                            }
                            |> UpdateAdvancedTermSearchOptions
                            |> AdvancedSearch
                            |> dispatch)
                        Input.Value model.AdvancedSearchState.AdvancedSearchOptions.SearchTermName
                    ] 
                ]
            ]
        ]
        Field.div [] [
            Label.label [Label.Props [Style [Color model.SiteStyleState.ColorMode.Accent]]] [ str "Name must contain:"]
            Help.help [] [str "Only suggest Terms, which name includes the following text part."]
            Field.div [] [
                Control.div [] [
                    Input.input [
                        Input.Placeholder "... must exist in name"
                        Input.Size IsSmall
                        Input.Props [ExcelColors.colorControl model.SiteStyleState.ColorMode]
                        Input.OnChange (fun e ->
                            {model.AdvancedSearchState.AdvancedSearchOptions
                                with MustContainName = e.Value
                            }
                            |> UpdateAdvancedTermSearchOptions
                            |> AdvancedSearch
                            |> dispatch)
                        Input.Value model.AdvancedSearchState.AdvancedSearchOptions.MustContainName
                    ] 
                ]
            ]
        ]
        Field.div [] [
            Label.label [Label.Props [Style [Color model.SiteStyleState.ColorMode.Accent]]] [ str "Term definition keywords:"]
            Help.help [] [str "Search the term definition for the following words."]
            Field.div [] [
                Control.div [] [
                    Input.input [
                        Input.Placeholder "... search key words"
                        Input.Size IsSmall
                        Input.Props [ExcelColors.colorControl model.SiteStyleState.ColorMode]
                        Input.OnChange (fun e ->
                            {model.AdvancedSearchState.AdvancedSearchOptions
                                with SearchTermDefinition = e.Value
                            }
                            |> UpdateAdvancedTermSearchOptions
                            |> AdvancedSearch
                            |> dispatch)
                        Input.Value model.AdvancedSearchState.AdvancedSearchOptions.SearchTermDefinition
                    ] 
                ]
            ] 
        ]
        Field.div [] [
            Label.label [Label.Props [Style [Color model.SiteStyleState.ColorMode.Accent]]] [ str "Definition must contain:"]
            Help.help [] [str "The definition of the term must contain any of these space-separated words (at any position)"]
            Field.body [] [
                Field.div [] [
                    Control.div [] [
                        Input.input [
                            Input.Placeholder "... must exist in definition"
                            Input.Size IsSmall
                            Input.Props [ExcelColors.colorControl model.SiteStyleState.ColorMode]
                            Input.OnChange (fun e ->
                                {model.AdvancedSearchState.AdvancedSearchOptions
                                    with MustContainDefinition = e.Value
                                }
                                |> UpdateAdvancedTermSearchOptions
                                |> AdvancedSearch
                                |> dispatch)
                            Input.Value model.AdvancedSearchState.AdvancedSearchOptions.MustContainDefinition
                        ] 
                    ]
                ]
            ]
        ]
    ]

let advancedSearchResultTable (model:Model) (dispatch: Msg -> unit) =
    Field.div [Field.Props [] ] [
        div [
            Style [Margin "1rem"]
        ][
            Button.buttonComponent model.SiteStyleState.ColorMode true "Change search options" (fun _ -> UpdateAdvancedTermSearchSubpage AdvancedTermSearchSubpages.InputForm |> AdvancedSearch |> dispatch)
        ]
        Label.label [] [str "Results:"]
        if model.AdvancedSearchState.AdvancedTermSearchSubpage = AdvancedTermSearchSubpages.Results then
            if model.AdvancedSearchState.HasAdvancedSearchResultsLoading then
                div [
                    Style [Width "100%"; Display DisplayOptions.Flex; JustifyContent "center"]
                ][
                    Loading.loadingComponent
                ]
            else
                PaginatedTable.paginatedTableComponent
                    model
                    dispatch
                    (createAdvancedTermSearchResultRows
                        model
                        dispatch
                        /// The following line defines which message is executed onClick on one of the terms in the result table.
                        ((fun t -> UpdateAdvancedTermSearchSubpage <| AdvancedTermSearchSubpages.SelectedResult t) >> AdvancedSearch)
                    )
    ]

let advancedSearchSelectedResultDisplay (model:Model) (result:DbDomain.Term) =
    Container.container [] [
        Heading.h4 [] [str "Selected Result:"]
        Table.table [Table.IsFullWidth] [
            thead [] []
            tbody [] [
                tr [
                colorControl model.SiteStyleState.ColorMode
                Class "suggestion"
                ] [
                    td [Class (Tooltip.ClassName + " " + Tooltip.IsTooltipRight + " " + Tooltip.IsMultiline);Tooltip.dataTooltip result.Definition] [
                        Fa.i [Fa.Solid.InfoCircle] []
                    ]
                    td [] [
                        b [] [str result.Name]
                    ]
                    td [Style [Color "red"]] [if result.IsObsolete then str "obsolete"]
                    td [Style [FontWeight "light"]] [small [] [str result.Accession]]
                ]
            ]
        ]
    ]

let advancedSearchModal (model:Model) (id:string) (dispatch: Msg -> unit) (resultHandler: DbDomain.Term -> Msg) =
    Modal.modal [
        Modal.IsActive (
            model.AdvancedSearchState.HasModalVisible
            && model.AdvancedSearchState.ModalId = id
        )
        Modal.Props [
            colorControl model.SiteStyleState.ColorMode
            Id id
        ]
    ] [
        Modal.background [] []
        Modal.Card.card [Props [colorControl model.SiteStyleState.ColorMode]] [
            Modal.Card.head [Props [colorControl model.SiteStyleState.ColorMode]] [
                Modal.close [Modal.Close.Size IsLarge; Modal.Close.OnClick (fun _ -> ToggleModal id |> AdvancedSearch |> dispatch)] []
                Heading.h4 [Heading.Props [Style [Color model.SiteStyleState.ColorMode.Accent]]] [
                    str "Advanced Search"
                ]
            ]
            Modal.Card.body [Props [colorControl model.SiteStyleState.ColorMode]] [
                match model.AdvancedSearchState.AdvancedTermSearchSubpage with
                | AdvancedTermSearchSubpages.InputForm ->
                    advancedTermSearchComponent model dispatch
                | AdvancedTermSearchSubpages.Results ->
                    advancedSearchResultTable model dispatch
                | AdvancedTermSearchSubpages.SelectedResult r ->
                    advancedSearchSelectedResultDisplay model r
                //else
                //    match model.AdvancedSearchState.SelectedResult with
                //    |None   -> advancedSearchResultTable model dispatch 
                //    |Some r -> advancedSearchSelectedResultDisplay model r
            ]
            Modal.Card.foot [] [
                form [
                    OnSubmit    (fun e -> e.preventDefault())
                    OnKeyDown   (fun k -> if k.key = "Enter" then k.preventDefault())
                    Style [Width "100%"]
                ] [
                    Level.level [][
                        Level.item [][
                            Level.level [Level.Level.Props [Style [Width "100%"]]][
                                Level.item [][
                                    Control.p [] [
                                        Button.button   [   
                                                            Button.CustomClass "is-danger"
                                                            Button.IsFullWidth
                                                            Button.OnClick (fun _ -> ResetAdvancedSearchOptions |> AdvancedSearch |> dispatch)

                                                        ] [
                                            str "Reset"
                                        ]
                                    ]
                                ]
                                Level.item [][
                                    Control.p [Control.IsExpanded] [
                                        Button.button   [   
                                                            Button.CustomClass "is-danger"
                                                            Button.IsFullWidth
                                                            Button.OnClick (fun _ -> ResetAdvancedSearchState |> AdvancedSearch |> dispatch)

                                                        ] [
                                            str "Cancel"
                                        ]
                                    ]
                                ]
                            ]
                        ]
                        Level.item [][
                            Control.p [] [
                                if (model.AdvancedSearchState.AdvancedTermSearchSubpage = AdvancedTermSearchSubpages.Results |> not) then
                                    Button.button   [
                                        let isValid = isValidAdancedSearchOptions model.AdvancedSearchState.AdvancedSearchOptions
                                        if isValid then
                                            Button.CustomClass "is-success"
                                            Button.IsActive true
                                        else
                                            Button.CustomClass "is-danger"
                                            Button.Props [Disabled (not isValid)]
                                        Button.IsFullWidth
                                        Button.OnClick (fun _ -> StartAdvancedSearch |> AdvancedSearch |> dispatch)
                                    ] [ str "Start advanced search"]
                                else
                                    Button.button   [
                                        let hasText = model.AdvancedSearchState.SelectedResult.IsSome
                                        if hasText then
                                            Button.CustomClass "is-success"
                                            Button.IsActive true
                                        else
                                            Button.CustomClass "is-danger"
                                            Button.Props [Disabled true]
                                        Button.IsFullWidth
                                        Button.OnClick (fun _ ->
                                            ResetAdvancedSearchState |> AdvancedSearch |> dispatch;
                                            model.AdvancedSearchState.SelectedResult.Value |> resultHandler |> dispatch)
                                    ] [
                                        str "Confirm"
                                
                                    ]
                        ]
                        ]
                    ]
                ]
            ]
        ]
    ]