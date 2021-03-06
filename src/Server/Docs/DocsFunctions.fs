module DocsFunctions

open System

open Shared

type ParameterType =
| ParamInteger
| ParamFloat
| ParamString
| ParamBoolean
| ParamUnit
| ParamDateTime
| ParamArray of ParameterType
| ParamOption of ParameterType
| ParamRecordType of Parameter []

    member this.toString =
        match this with
        | ParamInteger              -> "Integer"
        | ParamFloat                -> "Float"
        | ParamString               -> "String"
        | ParamBoolean              -> "Boolean"
        | ParamUnit                 -> "Unit"
        | ParamDateTime             -> "DateTime"
        | ParamArray param          -> sprintf "[ %s ]" param.toString
        | ParamOption param         -> sprintf "%s option" param.toString
        | ParamRecordType paramArr  -> Parameter.arrToString paramArr true

and Parameter = {
    Name : string
    Type : ParameterType
    Desc : string
}
    with
        /// isEnd defines if the single Parameter will be closed with a ',' or not. If isEnd = true then no comma, else comma.
        static member singleToString (param:Parameter) isEnd =
            sprintf
                "
                <div style=\"color:#153b57\">//%s</div>
                <div><span style=\"width: 200px; display: inline-block; color: #155724\">%s</span> : <span style=\"color: #571520\">%s</span>%s</div>
                "
                param.Desc
                param.Name
                param.Type.toString
                (if isEnd then "" else ",")

        member this.toString =
            Parameter.singleToString this true

        /// isRecordType defines if the Parameter array will be closed with a '[]' or '{}'. If isRecordType = true then '{}', else '[]'.
        static member arrToString (paramArr:Parameter []) isRecordType =
            let endInd = paramArr.Length-1
            let singleStrings =
                paramArr
                |> Array.mapi (fun i x ->
                    let isEnd = i = endInd
                    Parameter.singleToString x isEnd
                )
            String.concat "" singleStrings
            |> fun x ->
                if isRecordType then
                    sprintf
                        "
                        {<br>
                        <div style=\"margin-left: 1rem\">%s</div>
                        }"
                        x
                else
                    sprintf
                        "
                        [<br>
                        <div style=\"margin-left: 1rem\">%s</div>
                        ]"
                        x

        static member create name paramType desc =
            {
                Name = name
                Type = paramType
                Desc = desc
            }

module PredefinedParams =

    let dbDomainTerm =
        let dbdomaniTermParamArr = [|
            Parameter.create "ID" ParamInteger ""
            Parameter.create "OntologyId" ParamInteger ""
            Parameter.create "Accession" ParamString ""
            Parameter.create "Name" ParamString ""
            Parameter.create "Definition" ParamString ""
            Parameter.create "XRefValueType" (ParamOption ParamString) ""
            Parameter.create "IsObsolete" ParamBoolean ""
        |]
        dbdomaniTermParamArr
        |> ParamRecordType

    let dbDomainOntology =
        let dbdomaniOntologyParamArr = [|
            Parameter.create "ID" ParamInteger ""
            Parameter.create "Name" ParamString ""
            Parameter.create "CurrentVersion" ParamString ""
            Parameter.create "Definition" ParamString ""
            Parameter.create "DateCreated" ParamDateTime ""
            Parameter.create "UserID" ParamString ""
        |]
        dbdomaniOntologyParamArr
        |> ParamRecordType

    let unitOntology:DbDomain.Ontology = {
        ID = 1L
        Name = "uo"
        CurrentVersion = "releases/2020-03-10"
        Definition = "Unit Ontology"
        DateCreated = System.DateTime(2014,9,4) //"2014-09-04 00:00:00.000000"
        UserID = "gkoutos"
    }

    let test = System.DateTime(2020,11,17)


let createDocumentationDescription functionDesc usageDesc (paramArr:Parameter [] option) resultDesc (resultType:Parameter) =
    let prepParams =
        if paramArr.IsSome then Parameter.arrToString paramArr.Value false else "No parameters are passed."
        
    let prepResultParam =
        resultType.toString
    sprintf
        "
            <div><b>Function</b></div>
            <div>%s</div>
            <br>
            <div><b>Usage</b></div>
            <div>%s</div>
            <br>
            <div><b>Parameters</b></div>
            <div>%s</div>
            <br>
            <div><b>Result</b></div>
            <div>%s</div>
            <br>
            <div><b>ResultType</b></div>
            <div>%s</div>
        "
        functionDesc
        usageDesc
        prepParams
        resultDesc
        prepResultParam