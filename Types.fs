namespace Shoelace.Generator.Types

type SlProp =
    { name: string
      description: string
      ``type``: string
      defaultValue: string
      values: string array option }

type SlMethod =
    { name: string
      description: string
      ``params``: {| name: string; ``type``: string |} array }

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
