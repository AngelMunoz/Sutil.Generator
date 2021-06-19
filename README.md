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
