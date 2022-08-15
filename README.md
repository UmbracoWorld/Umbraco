## Umbraco

This is the 'entry point' for the Umbraco World ecosystem. Umbraco serves as an api gateway and as our presentation layer for the entire system.

Each microservice is it's own repo and is referenced through utilising git submodules.

### installation

```
mkdir UmbracoWorld
cd UmbracoWorld
git clone https://github.com/UmbracoWorld/Umbraco.git .

// let's get all the submodules
git submodule update --init --recursive
```