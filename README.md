# Shoelace.Generator

This is a [Shoelace](https://github.com/shoelace-style/shoelace) wrapper generator for [Sutil](https://github.com/davedawkins/Sutil) heavily inspider in [react-generator](https://github.com/shoelace-style/react-generator)

# Generate `Sutil.Shoelace` project
To generate the Sutil.Shoelace project you will need to have node installed in your machine.
- We download the `@shoelace-style/shoelace` package which contains a `metadata.json` file that allows us to automate the generation of the F# source code.
- Once the package is downloaded the project proceeds to generate one file for each component listed in the metadata file

To kick off these events run
```sh
dotnet tool restore

dotnet run
# at this point the Sutil.Shoelace project has been generated
# under a directory with the same name in the root of the project
dotnet fable --cwd Sutil.Shoelace
```
Normally you would do this to either

1. Draft a new release
2. Use Sutil.Shoelace to improve the docs website


# Future Ideas

- Propose a json schema to different libraries
- Decouple Shoelace to the generator
- Decouple Sutil from the generation phase (e.g allow this generator to create Feliz components or other DSL Flavor / Library)