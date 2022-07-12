# STATUS UPDATE

The shoelace spec has changed a little bit and this generator won't work anymore, there is a more standards compatible metadata format which is here
https://github.com/webcomponents/custom-elements-manifest if you want to make this a reality ping me on twitter @angel_d_munoz, if we can generate an F# AST from that metadata then Fantomas can write the code for us rather than us using strings


This project for the moment will be put to rest in the meantime


# Sutil.Generator

This is a [Shoelace](https://github.com/shoelace-style/shoelace) and [Fast](https://fast.design) wrapper generator for [Sutil](https://github.com/davedawkins/Sutil) heavily inspider in [react-generator](https://github.com/shoelace-style/react-generator)

# Generate `Sutil.Shoelace` or `Sutil.Fast` project
To generate the `Sutil.Shoelace` or `Sutil.Fast` project you will need to have node installed in your machine.
- We download the `@shoelace-style/shoelace` package which contains a `metadata.json` file that allows us to automate the generation of the F# source code.
    > In the case of Fast we read each component's metadata file to do the file generation 
- Once the package is downloaded the project proceeds to generate one file for each component listed in the metadata file

To kick off these events run
```sh
./build.ps1 fast
```
Normally you would do this to either

1. Draft a new release
2. Use Sutil.Shoelace or Sutil.Fast to improve the docs website


# Future Ideas

- [x] ~Propose a json schema to different libraries~ There's already a [Custom Elements Manifest](https://github.com/open-wc/custom-elements-manifest)
- [ ] Support Custom Elements Manifest to generate libraries agnostically
- [ ] Decouple Shoelace to the generator
- [ ] Decouple Sutil from the generation phase (e.g allow this generator to create Feliz components or other DSL Flavor / Library)
