namespace Sutil.Generator

open Argu

module Types =

    [<RequireQualifiedAccess>]
    type ComponentSystem =
        | Fast
        | Shoelace

    type Args =
        | [<Mandatory; AltCommandLine("-c"); MainCommand; ExactlyOnce; Last>] Component_System of ComponentSystem
        interface IArgParserTemplate with
            member this.Usage =
                match this with
                | Component_System _ -> "You need to specify the component System"

    type SlProp =
        { name: string
          description: string
          ``type``: string
          attribute: string option
          defaultValue: string
          values: string array option }

    type SlMethod =
        { name: string
          description: string
          ``params``: {| name: string
                         ``type``: string
                         isOptional: bool option |} array }

    type SlEvents =
        { name: string
          description: string
          details: string }

    type SlSlots = { name: string; description: string }
    type SlCssCustomProperties = { name: string; description: string }
    type SlParts = { name: string; description: string }
    type SlAnimation = { name: string; description: string }

    type SlComponent =
        { className: string
          tag: string
          file: string
          since: string
          status: string
          props: SlProp array
          methods: SlMethod array
          events: SlEvents array
          slots: SlSlots array
          cssCustomProperties: SlCssCustomProperties array
          parts: SlParts array
          dependencies: string array
          animations: SlAnimation array }

    type ShoelaceMetadata =
        { name: string
          description: string
          version: string
          author: string
          homepage: string
          license: string
          components: SlComponent array }

    type AttributeVscodeDefinition =
        { name: string
          title: string
          ``type``: string
          description: string option
          ``default``: obj option
          required: bool option
          values: seq<{| name: string |}> option
          value: obj option }

    type SlotVsCodeDefinition =
        { name: string
          title: string
          description: string option }

    type TagVsCodeDefinition =
        { name: string
          title: string
          description: string
          attributes: seq<AttributeVscodeDefinition>
          slots: seq<SlotVsCodeDefinition> }


    type HtmlCustomDataVSC =
        { version: float
          tags: seq<TagVsCodeDefinition> }
